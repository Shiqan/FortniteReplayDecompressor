using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Unreal.Core.Contracts;
using Unreal.Core.Exceptions;
using Unreal.Core.Extensions;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace Unreal.Core;

/// <summary>
/// Abstract base class implementation of default UE replays.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ReplayReader<T> where T : Replay, new()
{
    /// <summary>
    /// const int32 UNetConnection::DEFAULT_MAX_CHANNEL_SIZE = 32767; 
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L84
    /// </summary>
    protected const int DefaultMaxChannelSize = 32767;

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L59
    /// </summary>
    protected const uint FileMagic = 0x1CA2E27F;

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/811c1ce579564fa92ecc22d9b70cbe9c8a8e4b9a/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L107
    /// </summary>
    protected const uint NetworkMagic = 0x2CF5A13D;

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/811c1ce579564fa92ecc22d9b70cbe9c8a8e4b9a/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L111
    /// </summary>
    protected const uint MetadataMagic = 0x3D06B24E;

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L83
    /// </summary>
    protected const int MaxPacketSizeInBits = 16384; // 2 * 1024 * 8

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L1669
    /// </summary>
    protected const int OLD_MAX_ACTOR_CHANNELS = 10240;

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1027
    /// </summary>
    protected const int MAX_GUID_COUNT = 2048;

    protected ILogger _logger;
    protected T Replay { get; set; }
    protected ParseMode _parseMode;
    protected bool IsDebugMode => _parseMode == ParseMode.Debug;

    protected NetGuidCache _netGuidCache;
    protected NetFieldParser _netFieldParser;

    private int replayDataIndex = 0;
    private int checkpointIndex = 0;
    private int packetIndex = 0;
    private int bunchIndex = 0;

    private int InPacketId;
    private DataBunch? PartialBunch;

    // Allocate these once
    private readonly DataBunch _currentBunch = new();
    private readonly NetBitReader _packetReader = new();
    private readonly NetBitReader _exportReader = new();
    private readonly NetBitReader _cmdReader = new();
    private readonly NetDeltaUpdate _deltaUpdate = new();

    /// <summary>
    /// Index of latest received bunch sequence.
    /// </summary>
    private int InReliable = 0;

    /// <summary>
    /// Received channels during replay parsing.
    /// </summary>
    protected UChannel?[] Channels = new UChannel[DefaultMaxChannelSize];

    /// <summary>
    /// Tracks channels that we should ignore when handling special demo data.
    /// </summary>
    private uint?[] IgnoringChannels = new uint?[DefaultMaxChannelSize]; // channel index, actorguid

    public ReplayReader(ILogger logger, ParseMode mode)
    {
        _logger = logger;
        _parseMode = mode;

        _netGuidCache = new NetGuidCache();
        _netFieldParser = new NetFieldParser(_netGuidCache, mode);
    }

    /// <summary>
    /// Parses the entire replay. 
    /// It first parses the info section, and then all chunks.
    /// </summary>
    public virtual T ReadReplay(FArchive archive)
    {
        Replay = new T();

        ReadReplayInfo(archive);
        ReadReplayChunks(archive);

        Cleanup();

        return Replay;
    }

    /// <summary>
    /// Reset everything back to initial values.
    /// Required to call after parsing a replay.
    /// </summary>
    protected virtual void Cleanup()
    {
        InReliable = 0;
        Channels = new UChannel[DefaultMaxChannelSize];
        IgnoringChannels = new uint?[DefaultMaxChannelSize];

        replayDataIndex = 0;
        checkpointIndex = 0;
        packetIndex = 0;
        bunchIndex = 0;
        InPacketId = 0;
        PartialBunch = null;

        _netGuidCache.Cleanup();
    }

#if DEBUG
    public virtual void Debug(string filename, string directory, ReadOnlySpan<byte> data)
    {
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllBytes($"{directory}/{filename}.dump", data.ToArray());
    }

    public void Debug(string filename, string line)
    {
        if (IsDebugMode)
        {
            File.AppendAllLines($"{filename}.txt", new string[1] { line });
        }
    }
#endif

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L4892
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L282
    /// </summary>
    public virtual void ReadCheckpoint(FArchive archive)
    {
        // TODO add support for bDeltaCheckpoint ??

        var info = new CheckpointInfo
        {
            Id = archive.ReadFString(),
            Group = archive.ReadFString(),
            Metadata = archive.ReadFString(),
            StartTime = archive.ReadUInt32(),
            EndTime = archive.ReadUInt32(),
            SizeInBytes = archive.ReadInt32()
        };

        using var decrypted = DecryptBuffer(archive, info.SizeInBytes);
        using var binaryArchive = Decompress(decrypted);

        // SerializeDeletedStartupActors
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1916

        if (binaryArchive.HasDeltaCheckpoints())
        {
            var checkPointSize = binaryArchive.ReadUInt32();
        }

        if (binaryArchive.HasLevelStreamingFixes())
        {
            var packetOffset = binaryArchive.ReadInt64();
        }

        if (binaryArchive.NetworkVersion >= NetworkVersionHistory.HISTORY_MULTIPLE_LEVELS)
        {
            var levelForCheckpoint = binaryArchive.ReadInt32();
        }

        if (binaryArchive.NetworkVersion >= NetworkVersionHistory.HISTORY_DELETED_STARTUP_ACTORS)
        {
            if (binaryArchive.HasDeltaCheckpoints())
            {
                throw new NotImplementedException("Delta checkpoints not supported currently");
            }

            var deletedNetStartupActors = binaryArchive.ReadArray(binaryArchive.ReadFString);
        }

        // SerializeGuidCache
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1591
        var count = binaryArchive.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var guid = binaryArchive.ReadIntPacked();

            var cacheObject = new NetGuidCacheObject
            {
                OuterGuid = new NetworkGUID
                {
                    Value = binaryArchive.ReadIntPacked()
                }
            };

            if (binaryArchive.NetworkVersion < NetworkVersionHistory.HISTORY_GUID_NAMETABLE)
            {
                cacheObject.PathName = binaryArchive.ReadFString();
            }
            else
            {
                var isExported = binaryArchive.ReadBoolean();

                if (isExported)
                {
                    cacheObject.PathName = binaryArchive.ReadFString();
                }
                else
                {
                    var pathNameIndex = binaryArchive.ReadIntPacked();
                }
            }

            if (binaryArchive.NetworkVersion < NetworkVersionHistory.HISTORY_GUIDCACHE_CHECKSUMS)
            {
                cacheObject.NetworkChecksum = binaryArchive.ReadUInt32();
            }

            cacheObject.Flags = binaryArchive.ReadByte();

            // DemoNetDriver 5319
            // GuidCache->ObjectLookup.Add(Guid, CacheObject);
        }

        if (binaryArchive.HasDeltaCheckpoints())
        {
            throw new NotImplementedException("Delta checkpoints not implemented");
        }
        else
        {
            // Clear all of our mappings, since we're starting over
            _netGuidCache.NetFieldExportGroupMap.Clear();
            _netGuidCache.NetFieldExportGroupIndexToGroup.Clear();

            // SerializeNetFieldExportGroupMap 
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1289
            var numNetFieldExportGroups = binaryArchive.ReadUInt32();
            for (var i = 0; i < numNetFieldExportGroups; i++)
            {
                var group = ReadNetFieldExportGroupMap(binaryArchive);

                // Add the export group to the map
                _netGuidCache.NetFieldExportGroupIndexToGroup[group.PathNameIndex] = group.PathName;
                _netGuidCache.AddToExportGroupMap(group.PathName, group);
            }
        }

        //Remove all actors
        foreach (var channel in Channels)
        {
            if (channel == null)
            {
                continue;
            }

            channel.Actor = null;
        }

        // SerializeDemoFrameFromQueuedDemoPackets
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1978
        ReadDemoFrameIntoPlaybackPackets(binaryArchive);
        checkpointIndex++;
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L363
    /// </summary>
    public virtual void ReadEvent(FArchive archive)
    {
        var info = new EventInfo
        {
            Id = archive.ReadFString(),
            Group = archive.ReadFString(),
            Metadata = archive.ReadFString(),
            StartTime = archive.ReadUInt32(),
            EndTime = archive.ReadUInt32(),
            SizeInBytes = archive.ReadInt32()
        };

        throw new UnknownEventException($"Unknown event with id {info.Id}, group {info.Group}, metadata {info.Metadata}, size {info.SizeInBytes}");
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L243
    /// </summary>
    public virtual void ReadReplayChunks(FArchive archive)
    {
        while (!archive.AtEnd())
        {
            var chunkType = archive.ReadUInt32AsEnum<ReplayChunkType>();
            var chunkSize = archive.ReadInt32();
            var offset = archive.Position;

            if (chunkSize <= 0 || (long) chunkSize + offset > int.MaxValue)
            {
                _logger?.LogError("Invalid chunk size ({chunkSize} for chunk {chunkType}) at offset {offset}. Stopping the parsing...", chunkSize, chunkType, archive.Position);
                archive.SetError();
                return;
            }

            if (chunkType == ReplayChunkType.ReplayData && _parseMode > ParseMode.EventsOnly)
            {
                ReadReplayData(archive, chunkSize);
            }

            else if (chunkType == ReplayChunkType.Checkpoint)
            {
                // Checkpoints are used for fast forwarding,
                // we dont need it unless we want to process small parts of replay files.
                //ReadCheckpoint(archive);
            }

            else if (chunkType == ReplayChunkType.Event)
            {
                ReadEvent(archive);
            }

            else if (chunkType == ReplayChunkType.Header)
            {
                ReadReplayHeader(archive);
            }

            if (archive.Position != offset + chunkSize)
            {
                _logger?.LogDebug("Chunk ({chunkType}) at offset {offset} not fully read...", chunkType, offset);
                archive.Seek(offset + chunkSize, SeekOrigin.Begin);
            }
        }
    }


    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L318
    /// </summary> 
    public virtual void ReadReplayData(FArchive archive, int fallbackChunkSize)
    {
        var info = new ReplayDataInfo();
        if (archive.ReplayVersion.HasFlag(ReplayVersionHistory.HISTORY_STREAM_CHUNK_TIMES))
        {
            info.Start = archive.ReadUInt32();
            info.End = archive.ReadUInt32();
            info.Length = (int) archive.ReadUInt32();
        }
        else
        {
            info.Length = fallbackChunkSize;
        }

        if (archive.ReplayVersion.HasFlag(ReplayVersionHistory.HISTORY_ENCRYPTION))
        {
            // memorySizeInBytes
            archive.ReadInt32();
        }

        using var decrypted = DecryptBuffer(archive, info.Length);
        using var binaryArchive = Decompress(decrypted);
        while (!binaryArchive.AtEnd())
        {
            ReadDemoFrameIntoPlaybackPackets(binaryArchive);
        }
        replayDataIndex++;
    }

    /// <summary>
    /// Parse the replay header and set it to the <see cref="Replay"/>.
    /// see https://github.com/EpicGames/UnrealEngine/blob/811c1ce579564fa92ecc22d9b70cbe9c8a8e4b9a/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L191
    /// </summary>
    public virtual void ReadReplayHeader(FArchive archive)
    {
        var magic = archive.ReadUInt32();

        if (magic != NetworkMagic)
        {
            _logger?.LogError("Header.Magic != NETWORK_DEMO_MAGIC. Header.Magic: {magic}, NETWORK_DEMO_MAGIC: {NetworkMagic}", magic, NetworkMagic);
            throw new InvalidReplayException($"Header.Magic != NETWORK_DEMO_MAGIC. Header.Magic: {magic}, NETWORK_DEMO_MAGIC: {NetworkMagic}");
        }

        var header = new ReplayHeader
        {
            NetworkVersion = archive.ReadUInt32AsEnum<NetworkVersionHistory>()
        };

        if (header.NetworkVersion > NetworkVersionHistory.LATEST)
        {
            _logger?.LogWarning("Found unexpected NetworkVersionHistory: {}", header.NetworkVersion);
        }

        if (header.NetworkVersion <= NetworkVersionHistory.HISTORY_EXTRA_VERSION)
        {
            _logger?.LogError("Header.Version < MIN_NETWORK_DEMO_VERSION. Header.Version: {}, MIN_NETWORK_DEMO_VERSION: {}", header.NetworkVersion, NetworkVersionHistory.HISTORY_EXTRA_VERSION);
            throw new InvalidReplayException($"Header.Version < MIN_NETWORK_DEMO_VERSION. Header.Version: {header.NetworkVersion}, MIN_NETWORK_DEMO_VERSION: {NetworkVersionHistory.HISTORY_EXTRA_VERSION}");
        }

        if (header.NetworkVersion >= NetworkVersionHistory.HISTORY_USE_CUSTOM_VERSION)
        {
            var customVersionCount = archive.ReadInt32();

            //var fguid = archive.ReadGUID();
            //var latestVersion = archive.ReadInt32();
            archive.SkipBytes(customVersionCount * 20);
        }

        header.NetworkChecksum = archive.ReadUInt32();
        header.EngineNetworkVersion = archive.ReadUInt32AsEnum<EngineNetworkVersionHistory>();

        if (header.EngineNetworkVersion > EngineNetworkVersionHistory.LATEST)
        {
            _logger?.LogWarning("Found unexpected EngineNetworkVersionHistory: {}", header.EngineNetworkVersion);
        }

        header.GameNetworkProtocolVersion = archive.ReadUInt32();

        if (header.NetworkVersion >= NetworkVersionHistory.HISTORY_HEADER_GUID)
        {
            header.Guid = archive.ReadGUID();
        }

        if (header.NetworkVersion >= NetworkVersionHistory.HISTORY_SAVE_FULL_ENGINE_VERSION)
        {
            header.Major = archive.ReadUInt16();
            header.Minor = archive.ReadUInt16();
            header.Patch = archive.ReadUInt16();
            header.Changelist = archive.ReadUInt32();
            header.Branch = archive.ReadFString();

            archive.NetworkReplayVersion = new NetworkReplayVersion()
            {
                Major = header.Major,
                Minor = header.Minor,
                Patch = header.Patch,
                Changelist = header.Changelist,
                Branch = header.Branch
            };
        }
        else
        {
            header.Changelist = archive.ReadUInt32();
        }

        if (header.NetworkVersion >= NetworkVersionHistory.HISTORY_RECORDING_METADATA)
        {
            header.UE4Version = archive.ReadUInt32();
            header.UE5Version = archive.ReadUInt32();
            header.PackageVersionLicenseeUE = archive.ReadUInt32();
        }

        if (header.NetworkVersion <= NetworkVersionHistory.HISTORY_MULTIPLE_LEVELS)
        {
            throw new NotImplementedException("HISTORY_MULTIPLE_LEVELS not supported yet.");
        }
        else
        {
            header.LevelNamesAndTimes = archive.ReadTupleArray(archive.ReadFString, archive.ReadUInt32);
        }

        if (header.NetworkVersion >= NetworkVersionHistory.HISTORY_HEADER_FLAGS)
        {
            header.Flags = archive.ReadUInt32AsEnum<ReplayHeaderFlags>();
            archive.ReplayHeaderFlags = header.Flags;
        }

        header.GameSpecificData = archive.ReadArray(archive.ReadFString);

        if (header.NetworkVersion >= NetworkVersionHistory.HISTORY_SAVE_PACKAGE_VERSION_UE)
        {
            var minRecordHz = archive.ReadSingle();
            var maxRecordHz = archive.ReadSingle();
            var frameLimitInMS = archive.ReadSingle();
            var checkpointLimitInMS = archive.ReadSingle();

            header.Platform = archive.ReadFString();
            var buildConfig = archive.ReadByte();
            header.BuildTargetType = archive.ReadByteAsEnum<BuildTargetType>();
        }

        archive.EngineNetworkVersion = header.EngineNetworkVersion;
        archive.NetworkVersion = header.NetworkVersion;

        Replay.Header = header;

        _packetReader.EngineNetworkVersion = header.EngineNetworkVersion;
        _packetReader.NetworkVersion = header.NetworkVersion;
        _packetReader.ReplayHeaderFlags = header.Flags;

        _exportReader.EngineNetworkVersion = header.EngineNetworkVersion;
        _exportReader.NetworkVersion = header.NetworkVersion;
        _exportReader.ReplayHeaderFlags = header.Flags;

        _cmdReader.EngineNetworkVersion = header.EngineNetworkVersion;
        _cmdReader.NetworkVersion = header.NetworkVersion;
        _cmdReader.ReplayHeaderFlags = header.Flags;
    }


    /// <summary>
    /// Parse the replay info and set it on the <see cref="Replay"/>.
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L183
    /// </summary>
    public virtual void ReadReplayInfo(FArchive archive)
    {
        var magicNumber = archive.ReadUInt32();

        if (magicNumber != FileMagic)
        {
            _logger?.LogError("Invalid replay file, expected file magic {}, found {}", FileMagic, magicNumber);
            throw new InvalidReplayException("Invalid replay file");
        }

        var fileVersion = archive.ReadUInt32AsEnum<ReplayVersionHistory>();
        archive.ReplayVersion = fileVersion;

        if (fileVersion > ReplayVersionHistory.LATEST)
        {
            _logger?.LogWarning("Found unexpected ReplayVersionHistory: {}", fileVersion);
        }
        if (archive.ReplayVersion >= ReplayVersionHistory.HISTORY_CUSTOM_VERSIONS)
        {
            var customVersionCount = archive.ReadInt32();

            // version guid -> 16 bytes
            // version -> 4 bytes
            archive.SkipBytes(customVersionCount * 20);
        }
        var info = new ReplayInfo()
        {
            FileVersion = fileVersion,
            LengthInMs = archive.ReadUInt32(),
            NetworkVersion = archive.ReadUInt32(),
            Changelist = archive.ReadUInt32(),
            FriendlyName = archive.ReadFString(),
            IsLive = archive.ReadUInt32AsBoolean()
        };

        if (fileVersion >= ReplayVersionHistory.HISTORY_RECORDED_TIMESTAMP)
        {
            info.Timestamp = DateTime.FromBinary(archive.ReadInt64());
        }

        if (fileVersion >= ReplayVersionHistory.HISTORY_COMPRESSION)
        {
            info.IsCompressed = archive.ReadUInt32AsBoolean();
        }

        if (fileVersion >= ReplayVersionHistory.HISTORY_ENCRYPTION)
        {
            info.IsEncrypted = archive.ReadUInt32AsBoolean();
            var size = archive.ReadUInt32();
            info.EncryptionKey = archive.ReadBytes(size).ToArray();
        }

        if (!info.IsLive && info.IsEncrypted && (info.EncryptionKey.Length == 0))
        {
            _logger?.LogError("ReadReplayInfo: Completed replay is marked encrypted but has no key!");
            throw new InvalidReplayException("Completed replay is marked encrypted but has no key!");
        }

        if (info.IsLive && info.IsEncrypted)
        {
            _logger?.LogError("ReadReplayInfo: Replay is marked encrypted but not yet marked as completed!");
            throw new InvalidReplayException("Replay is marked encrypted but not yet marked as completed!");
        }

        Replay.Info = info;
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L3220
    /// </summary>
    public virtual PacketState ReadPacket(FArchive archive)
    {
        var bufferSize = archive.ReadInt32();
        if (bufferSize == 0)
        {
            return PacketState.End;
        }
        else if (bufferSize > 2048)
        {
            _logger?.LogWarning("ReadPacket: OutBufferSize > 2048");
            return PacketState.Error;
        }
        else if (bufferSize < 0)
        {
            _logger?.LogWarning("ReadPacket: OutBufferSize < 0");
            return PacketState.Error;
        }

        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L3338
        ReceivedRawPacket(archive.ReadBytes(bufferSize));
        return PacketState.Success;
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L2106
    /// </summary>
    public virtual void ReadExternalData(FArchive archive)
    {
        while (true)
        {
            var externalDataNumBits = archive.ReadIntPacked();
            if (externalDataNumBits == 0)
            {
                return;
            }

            // Read net guid this payload belongs to
            var netGuid = archive.ReadIntPacked();

            _logger?.LogDebug("External data found for netguid {}, bits: {}", netGuid, externalDataNumBits);

            _netGuidCache.ExternalData[netGuid] = new ExternalData()
            {
                Archive = new BinaryReader(archive.ReadBytes((externalDataNumBits + 7) >> 3))
                {
                    NetworkReplayVersion = archive.NetworkReplayVersion,
                    EngineNetworkVersion = archive.EngineNetworkVersion,
                    ReplayHeaderFlags = archive.ReplayHeaderFlags,
                    ReplayVersion = archive.ReplayVersion,
                    NetworkVersion = archive.NetworkVersion
                },
                NetGUID = netGuid
            };
        }
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/PackageMapClient.h#L64
    /// </summary>
    public virtual NetFieldExport? ReadNetFieldExport(FArchive archive)
    {
        var isExported = archive.ReadBoolean();
        if (isExported)
        {
            var fieldExport = new NetFieldExport()
            {
                Handle = archive.ReadIntPacked(),
                CompatibleChecksum = archive.ReadUInt32()
            };

            if (archive.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_NETEXPORT_SERIALIZATION)
            {
                fieldExport.Name = archive.ReadFString();
                fieldExport.Type = archive.ReadFString();
            }
            else if (archive.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_NETEXPORT_SERIALIZE_FIX)
            {
                fieldExport.Name = archive.ReadFString();
            }
            else
            {
                fieldExport.Name = archive.ReadFName();
            }
            return fieldExport;
        }

        return null;
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/PackageMapClient.h#L133
    /// </summary>
    public virtual NetFieldExportGroup ReadNetFieldExportGroupMap(FArchive archive)
    {
        var group = new NetFieldExportGroup()
        {
            PathName = archive.ReadFString(),
            PathNameIndex = archive.ReadIntPacked(),
            NetFieldExportsLength = archive.ReadIntPacked(),
        };
        group.NetFieldExports = new NetFieldExport[group.NetFieldExportsLength];

        for (var i = 0; i < group.NetFieldExportsLength; i++)
        {
            var netFieldExport = ReadNetFieldExport(archive);
            if (netFieldExport != null)
            {
                group.NetFieldExports[netFieldExport.Handle] = netFieldExport;
            }
        }
        return group;
    }


    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1348
    /// </summary>
    public virtual void ReadExportData(FArchive archive)
    {
        ReadNetFieldExports(archive);
        ReadNetExportGuids(archive);
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1579
    /// </summary>
    public virtual void ReadNetExportGuids(FArchive archive)
    {
        var numGuids = archive.ReadIntPacked();
        // TODO bIgnoreReceivedExportGUIDs ?
        for (var i = 0; i < numGuids; i++)
        {
            var size = archive.ReadInt32();
            _exportReader.FillBuffer(archive.ReadBytes(size));
            InternalLoadObject(_exportReader, true);
        }
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1571
    /// </summary>
    public virtual void ReadNetFieldExports(FArchive archive)
    {
        var numLayoutCmdExports = archive.ReadIntPacked();
        for (var i = 0; i < numLayoutCmdExports; i++)
        {
            var pathNameIndex = archive.ReadIntPacked();
            var isExported = archive.ReadIntPacked() == 1;
            NetFieldExportGroup? group;

            if (isExported)
            {
                var pathName = archive.ReadFString();
                var numExports = archive.ReadIntPacked();

                if (!_netGuidCache.NetFieldExportGroupMap.TryGetValue(pathName, out group))
                {
                    group = new NetFieldExportGroup
                    {
                        PathName = pathName,
                        PathNameIndex = pathNameIndex,
                        NetFieldExportsLength = numExports,
                        NetFieldExports = new NetFieldExport[numExports]
                    };
                    _netGuidCache.AddToExportGroupMap(pathName, group);
                }
                else if (numExports > group.NetFieldExportsLength)
                {
                    // Allow our exports to dynamically grow as more are encountered
                    var oldExports = group.NetFieldExports;

                    group.NetFieldExports = new NetFieldExport[numExports];
                    group.NetFieldExportsLength = numExports;

                    Array.Copy(oldExports, group.NetFieldExports, oldExports.Length);
                }

                //GuidCache->NetFieldExportGroupPathToIndex.Add(PathName, PathNameIndex);
                //GuidCache->NetFieldExportGroupIndexToGroup.Add(PathNameIndex, NetFieldExportGroup);
            }
            else
            {
                group = _netGuidCache.GetNetFieldExportGroupFromIndex(pathNameIndex);
            }

            var netField = ReadNetFieldExport(archive);
            if (group != null && netField != null)
            {
                group.NetFieldExports[netField.Handle] = netField;
            }
            else
            {
                _logger?.LogInformation("ReceiveNetFieldExports: Unable to find NetFieldExportGroup for export.");
            }
        }
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L2848
    /// </summary>
    public virtual void ReadDemoFrameIntoPlaybackPackets(FArchive archive)
    {
        if (archive.NetworkVersion >= NetworkVersionHistory.HISTORY_MULTIPLE_LEVELS)
        {
            // currentLevelIndex
            archive.ReadInt32();
        }

        var timeSeconds = archive.ReadSingle();
        _logger?.LogDebug("ReadDemoFrameIntoPlaybackPackets at {}", timeSeconds);

        if (archive.NetworkVersion >= NetworkVersionHistory.HISTORY_LEVEL_STREAMING_FIXES)
        {
            ReadExportData(archive);
        }

        if (archive.HasLevelStreamingFixes())
        {
            var numStreamingLevels = archive.ReadIntPacked();
            for (var i = 0; i < numStreamingLevels; i++)
            {
                // levelName 
                archive.ReadFString();
            }
        }
        else
        {
            var numStreamingLevels = archive.ReadIntPacked();
            for (var i = 0; i < numStreamingLevels; i++)
            {
                // packageName
                archive.ReadFString();
                // packageNameToLoad
                archive.ReadFString();
                // levelTransform
                archive.ReadFTransfrom();
            }
        }

        if (archive.HasLevelStreamingFixes())
        {
            // externalOffset
            archive.ReadUInt64();
        }

        // if (!bForLevelFastForward)
        // Load any custom external data in this frame
        ReadExternalData(archive);
        // else skip externalOffset

        if (archive.HasGameSpecificFrameData())
        {
            var skipExternalOffset = archive.ReadUInt64();
            if (skipExternalOffset > 0)
            {
                // ignore it for now
                archive.SkipBytes((int) skipExternalOffset);
            }
            // else skip externalOffset
        }

        var @continue = true;
        while (@continue)
        {
            if (archive.HasLevelStreamingFixes())
            {
                // seenLevelIndex
                archive.ReadIntPacked();
            }

            @continue = ReadPacket(archive) switch
            {
                PacketState.End => false,
                PacketState.Error => false,
                PacketState.Success => true,
                _ => false
            };
        }
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1409
    /// </summary>
    public virtual void ReceiveNetFieldExportsCompat(FBitArchive bitArchive)
    {
        var numLayoutCmdExports = bitArchive.ReadUInt32();
        for (var i = 0; i < numLayoutCmdExports; i++)
        {
            var pathNameIndex = bitArchive.ReadIntPacked();
            NetFieldExportGroup? group;

            if (bitArchive.ReadBit())
            {
                var pathName = bitArchive.ReadFString();
                var numExports = bitArchive.ReadUInt32();

                if (!_netGuidCache.NetFieldExportGroupMap.TryGetValue(pathName, out group))
                {
                    group = new NetFieldExportGroup
                    {
                        PathName = pathName,
                        PathNameIndex = pathNameIndex,
                        NetFieldExportsLength = numExports,
                        NetFieldExports = new NetFieldExport[numExports]
                    };
                    _netGuidCache.AddToExportGroupMap(pathName, group);
                }
            }
            else
            {
                group = _netGuidCache.GetNetFieldExportGroupFromIndex(pathNameIndex);
            }

            var netField = ReadNetFieldExport(bitArchive);
            if (group != null && netField is not null && group.IsValidIndex(netField.Handle))
            {
                //netField.Incompatible = group.NetFieldExports[(int)netField.Handle].Incompatible;
                group.NetFieldExports[netField.Handle] = netField;
            }
            // ReceiveNetFieldExports: Invalid NetFieldExport Handle
            // InBunch.SetError();
        }
    }

    /// <summary>
    /// Loads a UObject from an FArchive stream. Reads object path if there, and tries to load object if its not already loaded
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L804
    /// </summary>
    public virtual NetworkGUID InternalLoadObject(FArchive archive, bool isExportingNetGUIDBunch, int internalLoadObjectRecursionCount = 0)
    {
        if (internalLoadObjectRecursionCount > 16)
        {
            _logger?.LogWarning("InternalLoadObject: Hit recursion limit.");
            return new NetworkGUID();
        }

        var netGuid = new NetworkGUID()
        {
            Value = archive.ReadIntPacked()
        };

        if (!netGuid.IsValid())
        {
            return netGuid;
        }

        if (netGuid.IsDefault() || isExportingNetGUIDBunch)
        {
            var flags = archive.ReadByteAsEnum<ExportFlags>();

            if (flags.HasFlag(ExportFlags.bHasPath))
            {
                // outerGuid
                InternalLoadObject(archive, true, internalLoadObjectRecursionCount + 1);
                var pathName = archive.ReadFString();

                if (flags.HasFlag(ExportFlags.bHasNetworkChecksum))
                {
                    // networkChecksum
                    archive.ReadUInt32();
                }

                if (isExportingNetGUIDBunch)
                {
                    _netGuidCache.NetGuidToPathName[netGuid.Value] = pathName.RemoveAllPathPrefixes();
                }

                return netGuid;
            }
        }

        return netGuid;
    }


    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1203
    /// </summary>
    public virtual void ReceiveNetGUIDBunch(FBitArchive bitArchive)
    {
        var bHasRepLayoutExport = bitArchive.ReadBit();

        if (bHasRepLayoutExport)
        {
            // We need to keep this around to ensure we don't break backwards compatability.
            ReceiveNetFieldExportsCompat(bitArchive);
            return;
        }

        var numGUIDsInBunch = bitArchive.ReadInt32();
        if (numGUIDsInBunch > MAX_GUID_COUNT)
        {
            _logger?.LogWarning("UPackageMapClient::ReceiveNetGUIDBunch: NumGUIDsInBunch > MAX_GUID_COUNT ({})", numGUIDsInBunch);
            return;
        }

        var numGUIDsRead = 0;
        while (numGUIDsRead < numGUIDsInBunch)
        {
            InternalLoadObject(bitArchive, true);
            numGUIDsRead++;
        }
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L384
    /// </summary>
    public virtual void ReceivedRawBunch(DataBunch bunch) =>
        // bDeleted =
        ReceivedNextBunch(bunch);// if (bDeleted) return;// else { We shouldn't hit this path on 100% reliable connections }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L517
    /// </summary>
    public virtual void ReceivedNextBunch(DataBunch bunch)
    {
        // We received the next bunch. Basically at this point:
        // -We know this is in order if reliable
        // -We dont know if this is partial or not
        // If its not a partial bunch, of it completes a partial bunch, we can call ReceivedSequencedBunch to actually handle it

        // Note this bunch's retirement.
        if (bunch.bReliable)
        {
            // Reliables should be ordered properly at this point
            //check(Bunch.ChSequence == Connection->InReliable[Bunch.ChIndex] + 1);
            InReliable = bunch.ChSequence;
        }

        if (bunch.bPartial)
        {
            if (bunch.bPartialInitial)
            {
                if (PartialBunch != null)
                {
                    if (!PartialBunch.bPartialFinal)
                    {
                        if (PartialBunch.bReliable)
                        {
                            if (bunch.bReliable)
                            {
                                _logger?.LogWarning("Reliable initial partial trying to destroy reliable initial partial");
                                return;
                            }
                            _logger?.LogWarning("Unreliable initial partial trying to destroy unreliable initial partial");
                            return;

                        }
                        // Incomplete partial bunch. 
                    }
                    PartialBunch = null;
                }

                // InPartialBunch = new FInBunch(Bunch, false);
                PartialBunch = new DataBunch(bunch);
                var bitsLeft = bunch.Archive.GetBitsLeft();
                if (!bunch.bHasPackageMapExports && bitsLeft > 0)
                {
                    if (bitsLeft % 8 != 0)
                    {
                        _logger?.LogWarning("Corrupt partial bunch. Initial partial bunches are expected to be byte-aligned. BitsLeft = {}.", bitsLeft % 8);
                        return;
                    }

                    //PartialBunch.Archive.AppendDataFromChecked(bunch.Archive.ReadBytes(bitsLeft / 8), bitsLeft);
                }
                else
                {
                    _logger?.LogInformation("Received New partial bunch. It only contained NetGUIDs.");
                }

                return;
            }
            else
            {
                // Merge in next partial bunch to InPartialBunch if:
                // -We have a valid InPartialBunch
                // -The current InPartialBunch wasn't already complete
                // -ChSequence is next in partial sequence
                // -Reliability flag matches
                var bSequenceMatches = false;

                if (PartialBunch != null)
                {
                    var bReliableSequencesMatches = bunch.ChSequence == PartialBunch.ChSequence + 1;
                    var bUnreliableSequenceMatches = bReliableSequencesMatches || (bunch.ChSequence == PartialBunch.ChSequence);

                    // Unreliable partial bunches use the packet sequence, and since we can merge multiple bunches into a single packet,
                    // it's perfectly legal for the ChSequence to match in this case.
                    // Reliable partial bunches must be in consecutive order though
                    bSequenceMatches = PartialBunch.bReliable ? bReliableSequencesMatches : bUnreliableSequenceMatches;
                }

                // if (InPartialBunch && !InPartialBunch->bPartialFinal && bSequenceMatches && InPartialBunch->bReliable == Bunch.bReliable)
                if (PartialBunch != null && !PartialBunch.bPartialFinal && bSequenceMatches && PartialBunch.bReliable == bunch.bReliable)
                {
                    var bitsLeft = bunch.Archive.GetBitsLeft();
                    _logger?.LogDebug("Merging Partial Bunch: {} bits", bitsLeft);
                    if (!bunch.bHasPackageMapExports && bitsLeft > 0)
                    {
                        PartialBunch.Archive.AppendDataFromChecked(bunch.Archive.ReadBits(bitsLeft), bitsLeft);
                    }

                    // Only the final partial bunch should ever be non byte aligned. This is enforced during partial bunch creation
                    // This is to ensure fast copies/appending of partial bunches. The final partial bunch may be non byte aligned.
                    if (!bunch.bHasPackageMapExports && !bunch.bPartialFinal && (bitsLeft % 8 != 0))
                    {
                        _logger?.LogWarning("Corrupt partial bunch. Non-final partial bunches are expected to be byte-aligned.");
                        return;
                    }

                    // Advance the sequence of the current partial bunch so we know what to expect next
                    PartialBunch.ChSequence = bunch.ChSequence;

                    if (bunch.bPartialFinal)
                    {
                        _logger?.LogDebug("Completed Partial Bunch.");

                        if (bunch.bHasPackageMapExports)
                        {
                            _logger?.LogWarning("Corrupt partial bunch. Final partial bunch has package map exports.");
                            return;
                        }

                        // HandleBunch = InPartialBunch;
                        PartialBunch.bPartialFinal = true;
                        PartialBunch.bClose = bunch.bClose;
                        PartialBunch.bDormant = bunch.bDormant;
                        PartialBunch.CloseReason = bunch.CloseReason;
                        PartialBunch.bIsReplicationPaused = bunch.bIsReplicationPaused;
                        PartialBunch.bHasMustBeMappedGUIDs = bunch.bHasMustBeMappedGUIDs;
                        ReceivedSequencedBunch(PartialBunch);
                        return;
                    }
                    return;
                }
                else
                {
                    // Merge problem - delete InPartialBunch. This is mainly so that in the unlikely chance that ChSequence wraps around, we wont merge two completely separate partial bunches.
                    // We shouldn't hit this path on 100% reliable connections
                    _logger?.LogWarning("Merge problem:  We shouldn't hit this path on 100% reliable connections");
                    return;
                }
            }
        }

        // Receive it in sequence.
        ReceivedSequencedBunch(bunch);
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L348
    /// </summary>
    public virtual bool ReceivedSequencedBunch(DataBunch bunch)
    {
        ReceivedActorBunch(bunch);

        // We have fully received the bunch, so process it.
        if (bunch.bClose)
        {
            //ConditionalCleanUp(false, Bunch.CloseReason);
            // lots of stuff is happening here, but maybe this is enough... :)
            Channels[bunch.ChIndex] = null;
            OnChannelClosed(bunch.ChIndex, Channels[bunch.ChIndex]?.Actor?.ActorNetGUID);
            return true;
        }

        return false;
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L2298
    /// </summary>
    public virtual void ReceivedActorBunch(DataBunch bunch)
    {
        if (bunch.bHasMustBeMappedGUIDs)
        {
            var numMustBeMappedGUIDs = bunch.Archive.ReadUInt16();
            for (var i = 0; i < numMustBeMappedGUIDs; i++)
            {
                // guid
                bunch.Archive.ReadIntPacked();
            }
        }

        ProcessBunch(bunch);
    }

    /// <summary>
    /// https://github.com/EpicGames/UnrealEngine/blob/8776a8e357afff792806b997fbbd8e715111a271/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L490
    /// </summary>
    public virtual FVector ConditionallySerializeQuantizedVector(FBitArchive archive, FVector defaultVector)
    {
        var bWasSerialized = archive.ReadBit();
        if (bWasSerialized)
        {
            var bShouldQuantize = (archive.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_OPTIONALLY_QUANTIZE_SPAWN_INFO) || archive.ReadBit();
            return bShouldQuantize ? archive.ReadPackedVector(10, 24) : archive.ReadFVector();
        }

        return defaultVector;
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L2411
    /// </summary>
    public virtual void ProcessBunch(DataBunch bunch)
    {
        var channel = Channels[bunch.ChIndex];

        if (channel != null && channel.Actor is null)
        {
            if (!bunch.bOpen)
            {
                _logger?.LogWarning("New actor channel received non-open packet.");
                return;
            }

            var inActor = new Actor
            {
                // Initialize client if first time through.

                // SerializeNewActor
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L257
                ActorNetGUID = InternalLoadObject(bunch.Archive, false)
            };

            if (bunch.Archive.AtEnd() && inActor.ActorNetGUID.IsDynamic())
            {
                return;
            }

            if (inActor.ActorNetGUID.IsDynamic())
            {
                inActor.Archetype = InternalLoadObject(bunch.Archive, false);

                if (bunch.Archive.EngineNetworkVersion >= EngineNetworkVersionHistory.HISTORY_NEW_ACTOR_OVERRIDE_LEVEL)
                {
                    inActor.Level = InternalLoadObject(bunch.Archive, false);
                }

                inActor.Location = ConditionallySerializeQuantizedVector(bunch.Archive, new FVector(0, 0, 0));

                if (bunch.Archive.ReadBit())
                {
                    inActor.Rotation = bunch.Archive.ReadRotationShort();
                }
                else
                {
                    inActor.Rotation = new FRotator(0, 0, 0);
                }

                inActor.Scale = ConditionallySerializeQuantizedVector(bunch.Archive, new FVector(1, 1, 1));
                inActor.Velocity = ConditionallySerializeQuantizedVector(bunch.Archive, new FVector(0, 0, 0));
            }

            channel.Actor = inActor;
            OnChannelOpened(channel.ChannelIndex, inActor.ActorNetGUID);

            // OnActorChannelOpen
            // see https://github.com/EpicGames/UnrealEngine/blob/6c20d9831a968ad3cb156442bebb41a883e62152/Engine/Source/Runtime/Engine/Private/PlayerController.cpp#L1338
            if (_netGuidCache.TryGetPathName(channel.ArchetypeId ?? 0, out var path))
            {
                if (_netFieldParser.PlayerControllerGroups.Contains(path))
                {
                    // netPlayerIndex
                    bunch.Archive.ReadByte();
                }
            }
        }

        //  Read chunks of actor content
        while (!bunch.Archive.AtEnd())
        {
            var repObject = ReadContentBlockPayload(bunch, out var bObjectDeleted, out var bHasRepLayout, out var payload);

            if (payload is null)
            {
                continue;
            }

            bunch.Archive.SetTempEnd((int) payload, FBitArchiveEndIndex.CONTENT_BLOCK_PAYLOAD);

            try
            {
                if (bObjectDeleted)
                {
                    continue;
                }

                if (bunch.Archive.IsError)
                {
                    _logger?.LogWarning("UActorChannel::ReceivedBunch: ReadContentBlockPayload FAILED. bunch: {}", bunchIndex);
                    break;
                }

                if (repObject is null || bunch.Archive.AtEnd())
                {
                    // Nothing else in this block, continue on (should have been a delete or create block)
                    continue;
                }

                if (!ReceivedReplicatorBunch(bunch, bunch.Archive, repObject, bHasRepLayout))
                {
                    _logger?.LogInformation("UActorChannel::ProcessBunch: Replicator.ReceivedBunch returned false");
                    continue;
                }
            }
            finally
            {
                bunch.Archive.RestoreTempEnd(FBitArchiveEndIndex.CONTENT_BLOCK_PAYLOAD);
            }
        }
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/DataReplication.cpp#L896
    /// </summary>
    public virtual bool ReceivedReplicatorBunch(DataBunch bunch, FBitArchive archive, uint? repObject, bool bHasRepLayout)
    {
        var netFieldExportGroup = _netGuidCache.GetNetFieldExportGroup(repObject);

        if (netFieldExportGroup is null)
        {
            //_logger?.LogWarning($"Failed group. {bunch.ChIndex}");
            return true;
        }

        // Handle replayout properties
        if (bHasRepLayout)
        {
            if (!ReceiveProperties(archive, netFieldExportGroup, bunch.ChIndex, out _))
            {
                return false;
            }

            ReceiveExternalData(netFieldExportGroup, bunch.ChIndex);
        }

        // moved from ReadFieldHeaderAndPayload to here to save a search for ClassNetCache if not needed
        if (archive.AtEnd())
        {
            return true;
        }

        if (!_netGuidCache.TryGetClassNetCache(netFieldExportGroup.PathName, out var classNetCache, bunch.Archive.EngineNetworkVersion >= EngineNetworkVersionHistory.HISTORY_CLASSNETCACHE_FULLNAME))
        {
            _logger?.LogDebug("Couldnt find ClassNetCache for {}", netFieldExportGroup.PathName);
            return false;
        }

        // Read fields from stream
        while (ReadFieldHeaderAndPayload(archive, classNetCache, out var fieldCache, out var payload))
        {
            if (payload is null)
            {
                continue;
            }

            archive.SetTempEnd((int) payload, FBitArchiveEndIndex.FIELD_HEADER_PAYLOAD);
            try
            {
                if (fieldCache == null)
                {
                    _logger?.LogInformation("ReceivedBunch: FieldCache == nullptr: {}", classNetCache.PathName);
                    continue;
                }

                if (fieldCache.Incompatible)
                {
                    // We've already warned about this property once, so no need to continue to do so
                    _logger?.LogDebug("ReceivedBunch: FieldCache->bIncompatible == true: {}", fieldCache.Name);
                    continue;
                }

                if (archive is null || archive.IsError || archive.AtEnd())
                {
                    _logger?.LogDebug("ReceivedBunch: reader == nullptr or IsError: {}", classNetCache.PathName);
                    continue;
                }

                if (!_netFieldParser.WillReadClassNetCache(classNetCache.PathName))
                {
                    continue;
                }

                if (_netFieldParser.TryGetClassNetCacheProperty(fieldCache.Name, classNetCache.PathName, out var classNetProperty))
                {
                    if (classNetProperty.IsFunction)
                    {
                        // Handle function call
                        var functionGroup = _netGuidCache.GetNetFieldExportGroup(classNetProperty.PathName);
                        if (!ReceivedRPC(archive, functionGroup, bunch.ChIndex))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // Handle property
                        if (classNetProperty.IsCustomStruct)
                        {
                            if (!ReceiveCustomProperty(archive, classNetCache, fieldCache, bunch.ChIndex))
                            {
                                _logger?.LogWarning("Failed to parse custom property {pathName} {name} (bunchIndex: {bunchIndex})", classNetCache.PathName, fieldCache.Name, bunchIndex);
                                continue;
                            }
                        }
                        else
                        {
                            var group = _netGuidCache.GetNetFieldExportGroup(classNetProperty.PathName);
                            if (group is null || !_netFieldParser.WillReadType(group.PathName))
                            {
                                continue;
                            }

                            if (!ReceiveCustomDeltaProperty(archive, group, bunch.ChIndex, classNetProperty.EnablePropertyChecksum))
                            {
                                //FieldCache->bIncompatible = true; TODO?
                                _logger?.LogWarning("Failed to parse custom delta property {name} (bunchIndex: {bunchIndex})", fieldCache.Name, bunchIndex);
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    _logger?.LogDebug("Skipping struct {name} from group {pathName}", fieldCache.Name, classNetCache.PathName);
                }
            }
            finally
            {
                if (payload is not null)
                {
                    archive.RestoreTempEnd(FBitArchiveEndIndex.FIELD_HEADER_PAYLOAD);
                }
            }
        }

        return true;
    }

    /// <summary>
    /// If there is any <see cref="ExternalData"> for this group and netguid, read it and pass it back.
    /// There is no info in UE on how to handle external data.
    /// </summary>
    public virtual bool ReceiveExternalData(NetFieldExportGroup group, uint channelIndex)
    {
        if (Channels[channelIndex] is null)
        {
            _logger?.LogError("No channel found for {}", channelIndex);
            return false;
        }

        if (Channels[channelIndex]!.IsIgnoringGroup(group.PathName))
        {
            _logger?.LogInformation("Ignoring channel for type {}", group.PathName);
            return false;
        }

        _logger?.LogDebug("ReceiveExternalData: group {}", group.PathName);
        if (_netGuidCache.TryGetExternalData(Channels[channelIndex]?.Actor?.ActorNetGUID?.Value, out var externalData))
        {
            OnExternalDataRead(channelIndex, externalData);
        }

        return true;
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/DataReplication.cpp#L1158
    /// see https://github.com/EpicGames/UnrealEngine/blob/8776a8e357afff792806b997fbbd8e715111a271/Engine/Source/Runtime/Engine/Private/RepLayout.cpp#L5801
    /// </summary>
    public virtual bool ReceivedRPC(FBitArchive reader, NetFieldExportGroup? netFieldExportGroup, uint channelIndex)
    {
        if (netFieldExportGroup is null)
        {
            _logger?.LogWarning("ReceivedRPC: ReceivePropertiesForRPC - netFieldExportGroup is null");
            return false;
        }

        ReceiveProperties(reader, netFieldExportGroup, channelIndex, out _);

        if (reader.IsError)
        {
            _logger?.LogWarning("ReceivedRPC: ReceivePropertiesForRPC - Reader.IsError() == true (bunch: {})", bunchIndex);
            return false;
        }

        if (!Channels[channelIndex]!.IsIgnoringGroup(netFieldExportGroup.PathName) && _netFieldParser.WillReadType(netFieldExportGroup.PathName) && !reader.AtEnd())
        {
            _logger?.LogWarning("ReceivedRPC: ReceivePropertiesForRPC - Mismatch read. (bunch: {})", bunchIndex);
            return false;
        }

        return true;
    }

    /// <summary>
    /// We didnt really know how certain properties were parsed, so we mark those with <see cref="NetFieldParser.ClassNetCachePropertyInfo.IsCustomStruct"/>
    /// and we deserialize and/or resolve them 'manually' here.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="fieldCache"></param>
    /// <param name="channelIndex"></param>
    /// <returns></returns>
    public virtual bool ReceiveCustomProperty(FBitArchive reader, NetFieldExportGroup classNetCache, NetFieldExport fieldCache, uint channelIndex)
    {
        var export = _netFieldParser.CreatePropertyType(classNetCache.PathName, fieldCache.Name);
        if (export != null)
        {
            var numBits = reader.GetBitsLeft();
            _cmdReader.FillBuffer(reader.ReadBits(numBits), numBits);
            export.Serialize(_cmdReader);

            if (_cmdReader.IsError)
            {
                _logger?.LogWarning("Custom Property {} caused error when reading (bits: {})", fieldCache.Name, _cmdReader.LastBit);
            }

            if (!_cmdReader.AtEnd())
            {
                _logger?.LogWarning("Custom Property {name} didnt read proper number of bits: {bitsLeft} out of {bits}", fieldCache.Name, (_cmdReader.LastBit - _cmdReader.GetBitsLeft()), _cmdReader.LastBit);
            }

            (export as IResolvable)?.Resolve(_netGuidCache);

            OnExportRead(channelIndex, export as INetFieldExportGroup);

            return true;
        }
        return false;
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/8776a8e357afff792806b997fbbd8e715111a271/Engine/Source/Runtime/Engine/Private/RepLayout.cpp#L3744
    /// </summary>
    public virtual bool ReceiveCustomDeltaProperty(FBitArchive reader, NetFieldExportGroup group, uint channelIndex, bool enablePropertyChecksum)
    {
        if (reader.EngineNetworkVersion >= EngineNetworkVersionHistory.HISTORY_FAST_ARRAY_DELTA_STRUCT)
        {
            // bSupportsFastArrayDeltaStructSerialization
            reader.ReadBit();
        }

        // TODO when to read this
        // Receive array index (static sized array, i.e. MemberVariable[4])
        //var staticArrayIndex = reader.ReadIntPacked();

        // We should only be receiving custom delta properties (since RepLayout handles the rest)
        if (NetDeltaSerialize(reader, group, channelIndex, enablePropertyChecksum))
        {
            // Successfully received it.
            return true;
        }

        return false;
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/8776a8e357afff792806b997fbbd8e715111a271/Engine/Source/Runtime/Engine/Classes/Engine/NetSerialization.h#L895
    /// </summary>
    public virtual FFastArraySerializerHeader NetDeltaSerializeHeader(FBitArchive reader) => new()
    {
        ArrayReplicationKey = reader.ReadInt32(),
        BaseReplicationKey = reader.ReadInt32(),
        NumDeletes = reader.ReadInt32(),
        NumChanged = reader.ReadInt32()
    };

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/8776a8e357afff792806b997fbbd8e715111a271/Engine/Source/Runtime/Engine/Classes/Engine/NetSerialization.h#L1064
    /// </summary>
    public virtual bool NetDeltaSerialize(FBitArchive reader, NetFieldExportGroup group, uint channelIndex, bool enablePropertyChecksum)
    {
        // https://github.com/EpicGames/UnrealEngine/blob/8776a8e357afff792806b997fbbd8e715111a271/Engine/Source/Runtime/Engine/Private/RepLayout.cpp#L6302
        // DeltaSerializeFastArrayProperty

        // Read header
        var header = NetDeltaSerializeHeader(reader);

        if (reader.IsError)
        {
            return false;
        }

        // Read deleted elements
        if (header.NumDeletes > 0)
        {
            for (var i = 0; i < header.NumDeletes; ++i)
            {
                var elementIndex = reader.ReadInt32();
                _deltaUpdate.Deleted = true;
                _deltaUpdate.ElementIndex = elementIndex;
                _deltaUpdate.Export = null;
                _deltaUpdate.ChannelIndex = channelIndex;

                OnNetDeltaRead(channelIndex, _deltaUpdate);
            }
        }

        // Read Changed/New elements
        for (var i = 0; i < header.NumChanged; ++i)
        {
            var elementIndex = reader.ReadInt32();

            // https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/DataReplication.cpp#L896
            ReceiveProperties(reader, group, channelIndex, out var export, !enablePropertyChecksum, netDeltaUpdate: true);

            _deltaUpdate.Deleted = true;
            _deltaUpdate.ElementIndex = elementIndex;
            _deltaUpdate.Export = export;
            _deltaUpdate.ChannelIndex = channelIndex;
            OnNetDeltaRead(channelIndex, _deltaUpdate);
        }

        return true;
    }

    /// <summary>
    ///  https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/RepLayout.cpp#L2895
    ///  https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/RepLayout.cpp#L2971
    ///  https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/RepLayout.cpp#L3022
    /// </summary>
    public virtual bool ReceiveProperties(FBitArchive archive, NetFieldExportGroup group, uint channelIndex, [NotNullWhen(returnValue: true)] out INetFieldExportGroup? exportGroup, bool enablePropertyChecksum = true, bool netDeltaUpdate = false)
    {
        exportGroup = default;

        if (Channels[channelIndex] is null)
        {
            _logger?.LogError("No channel found for {}", channelIndex);
            return false;
        }

        if (Channels[channelIndex]!.IsIgnoringGroup(group.PathName))
        {
            _logger?.LogInformation("Ignoring channel for type {}", group.PathName);
            return false;
        }

        if (!_netFieldParser.WillReadType(group.PathName))
        {
            _logger?.LogInformation("Not reading type {}", group.PathName);
            Channels[channelIndex]!.IgnoreGroup(group.PathName);

#if DEBUG
            Debug("not-reading-groups", group.PathName);
            foreach (var field in group.NetFieldExports)
            {
                if (field == null)
                {
                    continue;
                }

                Debug("not-reading-groups", $"\t\t{field.Name}");
            }
#endif

            return false;
        }

        if (enablePropertyChecksum)
        {
            var doChecksum = archive.ReadBit();
        }

        _logger?.LogDebug("ReceiveProperties: group {}", group.PathName);
        exportGroup = _netFieldParser.CreateType(group.PathName);

        if (exportGroup is null)
        {
            _logger?.LogWarning("Couldnt create export group of type {}", group.PathName);
            return false;
        }

        var hasdata = false;

        while (true)
        {
            var handle = archive.ReadIntPacked();

            if (handle == 0)
            {
                // We're done
                break;
            }

            // We purposely add 1 on save, so we can reserve 0 for "done"
            handle--;

            if (!group.IsValidIndex(handle))
            {
                _logger?.LogWarning("NetFieldExports length for group {pathname} ({length}) < handle ({handle})", group.PathName, group?.NetFieldExportsLength, handle);
                return false;
            }

            var export = group.NetFieldExports[handle];
            var numBits = archive.ReadIntPacked();

            if (numBits == 0)
            {
                continue;
            }

            if (export is null)
            {
                _logger?.LogWarning("Couldnt find handle {handle} for group {pathname}, numbits is {numBits}", handle, group.PathName, numBits);
                archive.SkipBits(numBits);
                continue;
            }

            if (export.Incompatible)
            {
                _logger?.LogDebug("Incompatible export {name} for group {pathName}, numbits is {numBits}", export.Name, group.PathName, numBits);
                archive.SkipBits(numBits);
                // We've already warned that this property doesn't load anymore
                continue;
            }

            hasdata = true;
            try
            {
                _cmdReader.FillBuffer(archive.ReadBits(numBits), (int) numBits);
                if (!_netFieldParser.ReadField(exportGroup, export, handle, group, _cmdReader))
                {
                    // Set field incompatible since we couldnt (or didnt want to) parse it.
                    export.Incompatible = true;
                }


                if (_cmdReader.IsError)
                {
                    _logger?.LogWarning("Property {name} (handle: {handle}, path: {pathName}) caused error when reading (bits: {numBits})", export.Name, handle, group.PathName, numBits);
                    export.Incompatible = true;
#if DEBUG
                    Debug("failed-properties", $"Property {export.Name} (handle: {handle}, path: {group.PathName}) caused error when reading (bits: {numBits}, group: {group.PathName})");
                    _cmdReader.Reset();
                    Debug($"cmd-{export.Name}-{numBits}", "cmds", _cmdReader.ReadBytes(Math.Max((int) Math.Ceiling(_cmdReader.GetBitsLeft() / 8.0), 1)));
#endif
                    continue;
                }

                if (!_cmdReader.AtEnd())
                {
                    _logger?.LogWarning("Property {name} (handle: {handle}, path: {pathName}) didnt read proper number of bits: {numBitsLeft} out of {numBits}", export.Name, handle, group.PathName, (_cmdReader.LastBit - _cmdReader.GetBitsLeft()), numBits);
                    export.Incompatible = true;

#if DEBUG
                    Debug("failed-properties", $"Property {export.Name} (handle: {handle}, path: {group.PathName}) didnt read proper number of bits: {(_cmdReader.LastBit - _cmdReader.GetBitsLeft())} out of {numBits}");
                    _cmdReader.Reset();
                    Debug($"cmd-{export.Name}-{numBits}", "cmds", _cmdReader.ReadBytes(Math.Max((int) Math.Ceiling(_cmdReader.GetBitsLeft() / 8.0), 1)));
#endif
                    continue;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "NetFieldParser exception for property: {}, path: {}", export.Name, group.PathName);
#if DEBUG
                Debug("failed-properties", $"Property {export.Name} (handle: {handle}, path: {group.PathName}, bits: {numBits}) threw exception {ex.Message}");
#endif
            }
        }

        if (!netDeltaUpdate && hasdata)
        {
            OnExportRead(channelIndex, exportGroup);
        }

        if (Channels[channelIndex].IsIgnoringGroup(group.PathName) && _parseMode != ParseMode.Debug)
        {
            Channels[channelIndex].IgnoreGroup(group.PathName);
        }

        return true;
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L3579
    /// </summary>
    public virtual bool ReadFieldHeaderAndPayload(FBitArchive archive, NetFieldExportGroup group, out NetFieldExport? outField, [NotNullWhen(returnValue: true)] out uint? payload)
    {
        if (archive.AtEnd())
        {
            payload = null;
            outField = null;
            return false;
        }

        // const int32 NetFieldExportHandle = Bunch.ReadInt(FMath::Max(NetFieldExportGroup->NetFieldExports.Num(), 2));
        var netFieldExportHandle = archive.ReadSerializedInt(Math.Max((int) group.NetFieldExportsLength, 2));
        if (archive.IsError)
        {
            payload = null;
            outField = null;
            _logger?.LogWarning("ReadFieldHeaderAndPayload: Error reading NetFieldExportHandle.");
            return false;
        }

        // const FNetFieldExport& NetFieldExport = NetFieldExportGroup->NetFieldExports[NetFieldExportHandle];
        outField = group.NetFieldExports[(int) netFieldExportHandle];

        payload = archive.ReadIntPacked();
        if (archive.IsError)
        {
            payload = null;
            outField = null;
            _logger?.LogWarning("ReadFieldHeaderAndPayload: Error reading numbits.");
            return false;
        }

        if (!archive.CanRead((int) payload))
        {
            payload = null;
            return false;
        }

        // More to read
        return true;
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L3391
    /// </summary>
    public virtual uint? ReadContentBlockPayload(DataBunch bunch, out bool bObjectDeleted, out bool bOutHasRepLayout, out uint? payload)
    {
        // Read the content block header and payload
        var repObject = ReadContentBlockHeader(bunch, out bOutHasRepLayout, out bObjectDeleted);
        payload = null;

        if (bObjectDeleted)
        {
            // Nothing else in this block, continue on
            return repObject;
        }

        payload = bunch.Archive.ReadIntPacked();
        return repObject;
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L3175
    /// </summary>
    public virtual uint? ReadContentBlockHeader(DataBunch bunch, out bool bOutHasRepLayout, out bool bObjectDeleted)
    {
        bObjectDeleted = false;
        bOutHasRepLayout = bunch.Archive.ReadBit();
        var bIsActor = bunch.Archive.ReadBit();
        if (bIsActor)
        {
            // If this is for the actor on the channel, we don't need to read anything else
            return Channels[bunch.ChIndex]?.ArchetypeId ?? Channels[bunch.ChIndex]?.ActorId;
        }

        // We need to handle a sub-object
        // Manually serialize the object so that we can get the NetGUID (in order to assign it if we spawn the object here)
        var netGuid = InternalLoadObject(bunch.Archive, false);

        var bStablyNamed = bunch.Archive.ReadBit();
        if (bStablyNamed)
        {
            // If this is a stably named sub-object, we shouldn't need to create it. Don't raise a bunch error though because this may happen while a level is streaming out.
            return netGuid.Value;
        }

        var bDeleteSubObject = false;
        var bSerializeClass = true;

        if (bunch.Archive.EngineNetworkVersion >= EngineNetworkVersionHistory.HISTORY_SUBOBJECT_DESTROY_FLAG)
        {
            var bIsDestroyMessage = bunch.Archive.ReadBit();
            if (bIsDestroyMessage)
            {
                bDeleteSubObject = true;
                bSerializeClass = false;

                var destroyFlags = bunch.Archive.ReadByte();
            }
        }
        //else
        //{
        //    bSerializeClass = true;
        //}

        var classNetGUID = new NetworkGUID();
        if (bSerializeClass)
        {
            // Serialize the class in case we have to spawn it.
            classNetGUID = InternalLoadObject(bunch.Archive, false);

            bDeleteSubObject = !classNetGUID.IsValid();
        }

        if (bDeleteSubObject)
        {
            bObjectDeleted = true;
            return null;
        }

        if (bunch.Archive.EngineNetworkVersion >= EngineNetworkVersionHistory.HISTORY_SUBOBJECT_OUTER_CHAIN)
        {
            var bActorIsOuter = bunch.Archive.AtEnd() || bunch.Archive.ReadBit();
            if (!bActorIsOuter)
            {
                // outerobject
                InternalLoadObject(bunch.Archive, false);
            }
        }

        return classNetGUID?.Value;
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L1007
    /// </summary>
    public virtual void ReceivedRawPacket(ReadOnlySpan<byte> packet)
    {
        var lastByte = packet[^1];

        if (lastByte != 0)
        {
            var bitSize = (packet.Length * 8) - 1;

            // Bit streaming, starts at the Least Significant Bit, and ends at the MSB.
            while (!((lastByte & 0x80) >= 1))
            {
                lastByte *= 2;
                bitSize--;
            }

            _packetReader.FillBuffer(packet, bitSize);
            try
            {
                ReceivedPacket(_packetReader);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "ReceivedPacket failed, index: {}", packetIndex);
            }
        }
        else
        {
            _logger?.LogError("Malformed packet: Received packet with 0's in last byte of packet, index: {}", packetIndex);
            throw new MalformedPacketException("Malformed packet: Received packet with 0's in last byte of packet");
        }
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L3352
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L1525
    /// </summary>
    public virtual void ReceivedPacket(FBitArchive bitReader)
    {
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L5101
        // InternalAck always true!

        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#1549
        InPacketId++;

        // We want to be able to read replays that do not support export extentions.
        bool bHasPartialCustomExportsFinalBit = !(bitReader.EngineNetworkVersion < EngineNetworkVersionHistory.CustomExports);

        //var rejectedChannels = new Dictionary<uint, uint>();
        while (!bitReader.AtEnd())
        {
            // For demo backwards compatibility, old replays still have this bit
            if (bitReader.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_ACKS_INCLUDED_IN_HEADER)
            {
                // isAckDummy
                bitReader.ReadBit();
            }

            // FInBunch
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataBunch.cpp#L18
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Public/Net/DataBunch.h#L168
            var bunch = new DataBunch();

            var bControl = bitReader.ReadBit();
            bunch.PacketId = InPacketId;
            bunch.bOpen = bControl && bitReader.ReadBit();
            bunch.bClose = bControl && bitReader.ReadBit();

            if (bitReader.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_CLOSE_REASON)
            {
                bunch.bDormant = bunch.bClose && bitReader.ReadBit();
                bunch.CloseReason = bunch.bDormant ? ChannelCloseReason.Dormancy : ChannelCloseReason.Destroyed;
            }
            else
            {
                bunch.CloseReason = bunch.bClose ? (ChannelCloseReason) bitReader.ReadSerializedInt((int) ChannelCloseReason.MAX) : ChannelCloseReason.Destroyed;
                bunch.bDormant = bunch.CloseReason == ChannelCloseReason.Dormancy;
            }

            bunch.bIsReplicationPaused = bitReader.ReadBit();
            bunch.bReliable = bitReader.ReadBit();

            if (bitReader.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_MAX_ACTOR_CHANNELS_CUSTOMIZATION)
            {
                bunch.ChIndex = bitReader.ReadSerializedInt(OLD_MAX_ACTOR_CHANNELS);
            }
            else
            {
                bunch.ChIndex = bitReader.ReadIntPacked();
            }

            bunch.bHasPackageMapExports = bitReader.ReadBit();
            bunch.bHasMustBeMappedGUIDs = bitReader.ReadBit();
            bunch.bPartial = bitReader.ReadBit();

            if (bunch.bReliable)
            {
                // We can derive the sequence for 100% reliable connections
                bunch.ChSequence = InReliable + 1;
            }
            else if (bunch.bPartial)
            {
                // If this is an unreliable partial bunch, we simply use packet sequence since we already have it
                bunch.ChSequence = InPacketId;
            }
            else
            {
                bunch.ChSequence = 0;
            }

            bunch.bPartialInitial = bunch.bPartial && bitReader.ReadBit();
            bunch.bHasPartialCustomExportsFinalBit = bunch.bPartial && bHasPartialCustomExportsFinalBit ? bitReader.ReadBit() : false;
            bunch.bPartialFinal = bunch.bPartial && bitReader.ReadBit();

            var chType = ChannelType.None;
            var chName = ChannelName.None;

            if (bitReader.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES)
            {
                var type = bitReader.ReadSerializedInt((int) ChannelType.MAX);
                //    chType = (bunch.bReliable || bunch.bOpen) ? (ChannelType)type : ChannelType.None;

                //    chName = chType switch
                //    {
                //        ChannelType.Actor => ChannelName.Actor,
                //        ChannelType.Control => ChannelName.Control,
                //        ChannelType.Voice => ChannelName.Voice,
                //        _ => ChannelName.None
                //    };
            }
            else
            {
                if (bunch.bReliable || bunch.bOpen)
                {
                    bitReader.ReadFName();
                    //Enum.TryParse(bitReader.ReadFName(), out chName);
                    //chType = chName switch
                    //{
                    //    ChannelName.Actor => ChannelType.Actor,
                    //    ChannelName.Control => ChannelType.Control,
                    //    ChannelName.Voice => ChannelType.Voice,
                    //    _ => ChannelType.None
                    //};
                }
            }
            bunch.ChType = chType;
            bunch.ChName = chName;

            // UChannel* Channel = Channels[Bunch.ChIndex];
            var channel = Channels[bunch.ChIndex] != null;

            // If there's an existing channel and the bunch specified it's channel type, make sure they match.
            // Channel && (Bunch.ChName != NAME_None) && (Bunch.ChName != Channel->ChName)

            var bunchDataBits = (int) bitReader.ReadSerializedInt(MaxPacketSizeInBits);

            if (bunch.bPartial)
            {
                bunch.Archive = new BitReader(bitReader.ReadBits(bunchDataBits), bunchDataBits)
                {
                    EngineNetworkVersion = bitReader.EngineNetworkVersion,
                    NetworkVersion = bitReader.NetworkVersion,
                    ReplayHeaderFlags = bitReader.ReplayHeaderFlags
                };
            }
            else
            {
                bitReader.SetTempEnd(bunchDataBits, FBitArchiveEndIndex.BUNCH);
                bunch.Archive = bitReader;
            }

            bunchIndex++;

            if (bunch.bHasPackageMapExports)
            {
                ReceiveNetGUIDBunch(bunch.Archive);
            }

            // We're on a 100% reliable connection and we are rolling back some data.
            // In that case, we can generally ignore these bunches.
            // if (InternalAck && Channel && bIgnoreAlreadyOpenedChannels)
            // bIgnoreAlreadyOpenedChannels always true?  https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L4393
            //if (channel && false)
            //{
            //    var bNewlyOpenedActorChannel = bunch.bOpen && (bunch.ChName == ChannelName.Actor) && (!bunch.bPartial || bunch.bPartialInitial);
            //    if (bNewlyOpenedActorChannel)
            //    {
            //        // GetActorGUIDFromOpenBunch(Bunch);
            //        if (bunch.bHasMustBeMappedGUIDs)
            //        {
            //            var numMustBeMappedGUIDs = bunch.Archive.ReadUInt16();
            //            for (var i = 0; i < numMustBeMappedGUIDs; i++)
            //            {
            //                // FNetworkGUID NetGUID
            //                var guid = bunch.Archive.ReadIntPacked();
            //            }
            //        }

            //        //FNetworkGUID ActorGUID;
            //        var actorGuid = bunch.Archive.ReadIntPacked();
            //        IgnoringChannels[bunch.ChIndex] = actorGuid;
            //    }

            //    if (IgnoringChannels[bunch.ChIndex] != null)
            //    {
            //        if (bunch.bClose && (!bunch.bPartial || bunch.bPartialFinal))
            //        {
            //            //FNetworkGUID ActorGUID = IgnoringChannels.FindAndRemoveChecked(Bunch.ChIndex);
            //            IgnoringChannels[bunch.ChIndex] = null;
            //        }

            //        continue;
            //    }
            //}

            //// Ignore if reliable packet has already been processed.
            //if (bunch.bReliable && bunch.ChSequence <= InReliable)
            //{
            //    continue;
            //}

            //// If opening the channel with an unreliable packet, check that it is "bNetTemporary", otherwise discard it
            //if (!channel && !bunch.bReliable)
            //{
            //    if (!(bunch.bOpen && (bunch.bClose || bunch.bPartial)))
            //    {
            //        continue;
            //    }
            //}

            // Create channel if necessary
            if (!channel)
            {
                //if (rejectedChannels.ContainsKey(bunch.ChIndex))
                //{
                //    _logger?.LogDebug($"Ignoring Bunch for ChIndex {bunch.ChIndex}, as the channel was already rejected while processing this packet.");
                //    continue;
                //}

                //if (!Driver->IsKnownChannelName(Bunch.ChName))
                //{
                //    CLOSE_CONNECTION_DUE_TO_SECURITY_VIOLATION
                //}

                // Reliable (either open or later), so create new channel.
                var newChannel = new UChannel()
                {
                    //ChannelName = bunch.ChName,
                    //ChannelType = bunch.ChType,
                    ChannelIndex = bunch.ChIndex,
                };

                Channels[bunch.ChIndex] = newChannel;

                // Notify the server of the new channel.
                // if( !Driver->Notify->NotifyAcceptingChannel( Channel ) ) { continue; }
            }

            try
            {
                // Dispatch the raw, unsequenced bunch to the channel
                ReceivedRawBunch(bunch);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "failed ReceivedRawBunch, index: {}", bunchIndex);
            }
            finally
            {
                if (!bunch.bPartial)
                {
                    bitReader.RestoreTempEnd(FBitArchiveEndIndex.BUNCH);
                }
            }
        }

        if (!bitReader.AtEnd())
        {
            _logger?.LogWarning("Packet {} not fully read...", packetIndex);
        }
    }

    /// <summary>
    /// Receive an <see cref="INetFieldExportGroup"/> after receiving properties.
    /// </summary>
    protected abstract void OnExportRead(uint channelIndex, INetFieldExportGroup? exportGroup);

    /// <summary>
    /// Receive an <see cref="IExternalData"/> group.
    /// </summary>
    protected abstract void OnExternalDataRead(uint channelIndex, IExternalData? exportGroup);


    /// <summary>
    /// Receive a <see cref="NetDeltaUpdate"/>.
    /// </summary>
    protected abstract void OnNetDeltaRead(uint channelIndex, NetDeltaUpdate update);

    /// <summary>
    /// After receiving propteries for this <paramref name="exportGroup"/> indicate wheter it can be ignored for the rest of the replay (on the given channel)
    /// or if it can be ignored for the rest of the replay. 
    /// Useful to minimize the parsing, e.g. if you're only interested in the initial values and not subsequent updates. 
    /// </summary>
    protected virtual bool IgnoreGroupOnChannel(uint channelIndex, INetFieldExportGroup exportGroup) => true;

    /// <summary>
    /// Notifies when a new actor channel is created.
    /// </summary>
    protected virtual void OnChannelOpened(uint channelIndex, NetworkGUID? actor)
    {

    }

    /// <summary>
    /// Notifies when a channel is closed.
    /// </summary>
    protected virtual void OnChannelClosed(uint channelIndex, NetworkGUID? actor)
    {

    }

    /// <summary>
    /// Chunks can be encrypted with the <see cref="ReplayInfo.EncryptionKey"/>. If the replay is encrypted this method needs to be implemented.
    /// see https://github.com/EpicGames/UnrealEngine/blob/12dbd9877379223a839e59ceb92131a7e400aae5/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Public/LocalFileNetworkReplayStreaming.h#L475
    /// </summary>
    protected virtual FArchive DecryptBuffer(FArchive archive, int size)
    {
        if (!Replay.Info.IsEncrypted)
        {
            return archive;
        }

        _logger?.LogError("Replay is marked as encrypted. Make sure to implement this method to decrypt the chunks.");
        throw new NotImplementedException("Replay is marked as encrypted. Make sure to implement this method to decrypt the chunks.");
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Serialization/CompressedChunkInfo.cpp#L9
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Plugins/Runtime/PacketHandlers/CompressionComponents/Oodle/Source/OodleHandlerComponent/Private/OodleArchives.cpp#L21
    /// </summary>
    protected virtual FArchive Decompress(FArchive archive)
    {
        if (!Replay.Info.IsCompressed)
        {
            return archive;
        }

        var decompressedSize = archive.ReadInt32();
        var compressedSize = archive.ReadInt32();
        var compressedBuffer = archive.ReadBytes(compressedSize);

        _logger?.LogDebug("Decompressing archive from {compressedSize} to {decompressedSize}.", compressedSize, decompressedSize);
        throw new NotImplementedException("Replay is marked as compressed. Make sure to implement this method to decompress the chunks");
    }
}
