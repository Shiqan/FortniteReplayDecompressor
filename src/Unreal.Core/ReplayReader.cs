using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unreal.Core.Exceptions;
using Unreal.Core.Extensions;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;
using Unreal.Encryption;

namespace Unreal.Core
{
    public abstract class ReplayReader<T> where T : Replay
    {
        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L59
        /// </summary>
        public const uint FileMagic = 0x1CA2E27F;

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/811c1ce579564fa92ecc22d9b70cbe9c8a8e4b9a/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L107
        /// </summary>
        public const uint NetworkMagic = 0x2CF5A13D;

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/811c1ce579564fa92ecc22d9b70cbe9c8a8e4b9a/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L111
        /// </summary>
        public const uint MetadataMagic = 0x3D06B24E;

        protected ILogger _logger;
        protected T Replay { get; set; }

        private int replayDataIndex = 0;
        private int checkpointIndex = 0;
        private int packetIndex = 0;
        private int externalDataIndex = 0;
        private int bunchIndex = 0;

        private int InPacketId;
        private DataBunch PartialBunch;
        // const int32 UNetConnection::DEFAULT_MAX_CHANNEL_SIZE = 32767; netconnection.cpp 84
        private Dictionary<uint, int> InReliable = new Dictionary<uint, int>(); // TODO: array in unreal
        private Dictionary<uint, UChannel> Channels = new Dictionary<uint, UChannel>();
        private Dictionary<uint, uint> ChannelNetGuids = new Dictionary<uint, uint>();
        private Dictionary<uint, string> NetGuidCache = new Dictionary<uint, string>();
        private Dictionary<uint, uint> OuterNetGuidCache = new Dictionary<uint, uint>();
        private Dictionary<uint, string> ArchetypeToNetFieldGroup = new Dictionary<uint, string>();
        private Dictionary<uint, bool> ChannelActors = new Dictionary<uint, bool>();
        private Dictionary<string, NetFieldExportGroup> NetFieldExportGroupMap = new Dictionary<string, NetFieldExportGroup>();
        private Dictionary<uint, NetFieldExportGroup> NetFieldExportGroupIndexToGroup = new Dictionary<uint, NetFieldExportGroup>();

        private Dictionary<uint, List<AthenaPlayerState>> ActorStates = new Dictionary<uint, List<AthenaPlayerState>>();
        private Dictionary<uint, List<AthenaPlayerPawn>> PlayerPawns = new Dictionary<uint, List<AthenaPlayerPawn>>();
        //private List<string> UnknownFields = new List<string>();

        /// <summary>
        /// Tracks channels that we should ignore when handling special demo data.
        /// </summary>
        private Dictionary<uint, uint> IgnoringChannels = new Dictionary<uint, uint>(); // channel index, actorguid

        public virtual T ReadReplay(FArchive archive)
        {
            ReadReplayInfo(archive);
            ReadReplayChunks(archive);
            return Replay;
        }

        public virtual void Debug(string filename, string directory, byte[] data)
        {
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes($"{directory}/{filename}.dump", data);
        }

        public void Debug(string filename, string line)
        {
            File.AppendAllLines($"{filename}.txt", new string[1] { line });
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L4892
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L282
        /// </summary>
        /// <param name="archive"></param>
        /// <returns></returns>
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

            using var binaryArchive = Decompress(archive, info.SizeInBytes);

            // SerializeDeletedStartupActors
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1916

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
                var deletedNetStartupActors = binaryArchive.ReadArray(binaryArchive.ReadFString);
            }

            // SerializeGuidCache
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1591
            var count = binaryArchive.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var guid = binaryArchive.ReadIntPacked();
                var outerGuid = binaryArchive.ReadIntPacked();
                var path = binaryArchive.ReadFString();
                var checksum = binaryArchive.ReadUInt32();
                var flags = binaryArchive.ReadByte();

                // TODO DemoNetDriver 5319
                // GuidCache->ObjectLookup.Add(Guid, CacheObject);
            }

            // SerializeNetFieldExportGroupMap 
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1289

            // Clear all of our mappings, since we're starting over
            NetFieldExportGroupMap.Clear();
            NetFieldExportGroupIndexToGroup.Clear();

            var numNetFieldExportGroups = binaryArchive.ReadUInt32();
            for (var i = 0; i < numNetFieldExportGroups; i++)
            {
                var group = ReadNetFieldExportGroupMap(binaryArchive);

                // Add the export group to the map
                NetFieldExportGroupMap.Add(group.PathName, group);
                NetFieldExportGroupIndexToGroup.Add(group.PathNameIndex, group);
            }

            // SerializeDemoFrameFromQueuedDemoPackets
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1978
            var playbackPackets = ReadDemoFrameIntoPlaybackPackets(binaryArchive);
            foreach (var packet in playbackPackets)
            {
                if (packet.State == PacketState.Success)
                {
                    Debug($"checkpoint-{checkpointIndex}-packet-{packetIndex}", "checkpoint-packets", packet.Data);
                    packetIndex++;
                    //ReceivedRawPacket(packet);
                }
            }
            checkpointIndex++;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L363
        /// </summary>
        /// <param name="archive"></param>
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

            throw new UnknownEventException();
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L243
        /// </summary>
        /// <param name="archive"></param>
        public virtual void ReadReplayChunks(FArchive archive)
        {
            while (!archive.AtEnd())
            {
                var chunkType = archive.ReadUInt32AsEnum<ReplayChunkType>();
                var chunkSize = archive.ReadInt32();
                var offset = archive.Position;

                if (chunkType == ReplayChunkType.Checkpoint)
                {
                    ReadCheckpoint(archive);
                }

                else if (chunkType == ReplayChunkType.Event)
                {
                    ReadEvent(archive);
                }

                else if (chunkType == ReplayChunkType.ReplayData)
                {
                    ReadReplayData(archive);
                }

                else if (chunkType == ReplayChunkType.Header)
                {
                    ReadReplayHeader(archive);
                }

                if (archive.Position != offset + chunkSize)
                {
                    _logger?.LogWarning($"Chunk ({chunkType}) at offset {offset} not fully read...");
                    archive.Seek(offset + chunkSize, SeekOrigin.Begin);
                }
            }
        }


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L318
        /// </summary> 
        /// <param name="archive"></param>
        public virtual void ReadReplayData(FArchive archive)
        {
            var info = new ReplayDataInfo();
            if (archive.ReplayVersion >= ReplayVersionHistory.StreamChunkTimes)
            {
                info.Start = archive.ReadUInt32();
                info.End = archive.ReadUInt32();
                info.Length = archive.ReadUInt32();
            }
            else
            {
                info.Length = archive.ReadUInt32();
            }

            using var binaryArchive = Decompress(archive, (int)info.Length);
            Debug($"replaydata-{replayDataIndex}", "replaydata", binaryArchive.ReadBytes((int)binaryArchive.BaseStream.Length));
            binaryArchive.Seek(0);
            while (!binaryArchive.AtEnd())
            {
                var playbackPackets = ReadDemoFrameIntoPlaybackPackets(binaryArchive);

                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L3338
                foreach (var packet in playbackPackets)
                {
                    if (packet.State == PacketState.Success)
                    {
                        Debug($"replaydata-{replayDataIndex}-packet-{packetIndex}", "replay-packets", packet.Data);
                        packetIndex++;
                        ReceivedRawPacket(packet);
                    }
                }
            }
            replayDataIndex++;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/811c1ce579564fa92ecc22d9b70cbe9c8a8e4b9a/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L191
        /// </summary>
        /// <param name="archive"></param>
        /// <returns>ReplayHeader</returns>
        public virtual void ReadReplayHeader(FArchive archive)
        {
            var magic = archive.ReadUInt32();

            if (magic != NetworkMagic)
            {
                _logger?.LogError($"Header.Magic != NETWORK_DEMO_MAGIC. Header.Magic: {magic}, NETWORK_DEMO_MAGIC: {NetworkMagic}");
                throw new InvalidReplayException($"Header.Magic != NETWORK_DEMO_MAGIC. Header.Magic: {magic}, NETWORK_DEMO_MAGIC: {NetworkMagic}");
            }

            var header = new ReplayHeader
            {
                NetworkVersion = archive.ReadUInt32AsEnum<NetworkVersionHistory>()
            };

            if (header.NetworkVersion <= NetworkVersionHistory.HISTORY_EXTRA_VERSION)
            {
                _logger.LogError($"Header.Version < MIN_NETWORK_DEMO_VERSION. Header.Version: {header.NetworkVersion}, MIN_NETWORK_DEMO_VERSION: {NetworkVersionHistory.HISTORY_EXTRA_VERSION}");
                throw new InvalidReplayException($"Header.Version < MIN_NETWORK_DEMO_VERSION. Header.Version: {header.NetworkVersion}, MIN_NETWORK_DEMO_VERSION: {NetworkVersionHistory.HISTORY_EXTRA_VERSION}");
            }

            header.NetworkChecksum = archive.ReadUInt32();
            header.EngineNetworkVersion = archive.ReadUInt32AsEnum<EngineNetworkVersionHistory>();
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

            if (header.NetworkVersion <= NetworkVersionHistory.HISTORY_MULTIPLE_LEVELS)
            {
                throw new NotImplementedException();
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

            archive.EngineNetworkVersion = header.EngineNetworkVersion;
            archive.NetworkVersion = header.NetworkVersion;

            Replay.Header = header;
        }


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L183
        /// </summary>
        /// <param name="archive"></param>
        /// <returns>ReplayInfo</returns>
        public virtual void ReadReplayInfo(FArchive archive)
        {
            var magicNumber = archive.ReadUInt32();

            if (magicNumber != FileMagic)
            {
                _logger?.LogError("Invalid replay file");
                throw new InvalidReplayException("Invalid replay file");
            }

            var fileVersion = archive.ReadUInt32AsEnum<ReplayVersionHistory>();
            archive.ReplayVersion = fileVersion;

            var info = new ReplayInfo()
            {
                FileVersion = fileVersion,
                LengthInMs = archive.ReadUInt32(),
                NetworkVersion = archive.ReadUInt32(),
                Changelist = archive.ReadUInt32(),
                FriendlyName = archive.ReadFString(),
                IsLive = archive.ReadUInt32AsBoolean()
            };

            if (fileVersion >= ReplayVersionHistory.RecordedTimestamp)
            {
                info.Timestamp = DateTime.FromBinary(archive.ReadInt64());
            }

            if (fileVersion >= ReplayVersionHistory.Compression)
            {
                info.IsCompressed = archive.ReadUInt32AsBoolean();
            }

            Replay.Info = info;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L3220
        /// </summary>
        public virtual PlaybackPacket ReadPacket(FArchive archive)
        {
            var packet = new PlaybackPacket();

            var bufferSize = archive.ReadInt32();
            if (bufferSize == 0)
            {
                packet.State = PacketState.End;
                return packet;
            }

            packet.Data = archive.ReadBytes(bufferSize);
            packet.State = PacketState.Success;
            return packet;
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

                var externalDataNumBytes = (int)(externalDataNumBits + 7) >> 3;
                var externalData = archive.ReadBytes(externalDataNumBytes);

                // replayout setexternaldata
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Public/Net/RepLayout.h#L122
                // FMemory::Memcpy(ExternalData.GetData(), Src, NumBytes);

                // this is a bitreader...
                //var bitReader = new BitReader(externalData);
                //bitReader.ReadBytes(3); // always 19 FB 01 ?
                //var size = bitReader.ReadUInt32();

                // FCharacterSample
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/Components/CharacterMovementComponent.cpp#L7074
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/GameFramework/CharacterMovementComponent.h#L2656
                //var location = bitReader.ReadPackedVector(10, 24);
                //var velocity = bitReader.ReadPackedVector(10, 24);
                //var acceleration = bitReader.ReadPackedVector(10, 24);
                //var rotation = bitReader.ReadSerializeCompressed();
                //var remoteViewPitch = bitReader.ReadByte();
                //if (!bitReader.AtEnd())
                //{
                //    var time = bitReader.ReadSingle();
                //}

                Debug($"externaldata-{externalDataIndex}-{netGuid}", "externaldata", externalData);
                externalDataIndex++;
            }
        }


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/CoreUObject/Private/UObject/CoreNet.cpp#L277
        /// </summary>
        public virtual string StaticParseName(FArchive archive)
        {
            var isHardcoded = archive.ReadBoolean();
            if (isHardcoded)
            {
                uint nameIndex;
                if (archive.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES)
                {
                    nameIndex = archive.ReadUInt32();
                }
                else
                {
                    nameIndex = archive.ReadIntPacked();
                }
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.h#L31
                // hard coded names in "UnrealNames.inl"
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.inl

                // https://github.com/EpicGames/UnrealEngine/blob/375ba9730e72bf85b383c07a5e4a7ba98774bcb9/Engine/Source/Runtime/Core/Public/UObject/NameTypes.h#L599
                // https://github.com/EpicGames/UnrealEngine/blob/375ba9730e72bf85b383c07a5e4a7ba98774bcb9/Engine/Source/Runtime/Core/Private/UObject/UnrealNames.cpp#L283
                // TODO: Combine with Fortnite SDK dump
                return ((UnrealNames)nameIndex).ToString();
            }

            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.h#L17
            // MAX_NETWORKED_HARDCODED_NAME = 410

            // https://github.com/EpicGames/UnrealEngine/blob/375ba9730e72bf85b383c07a5e4a7ba98774bcb9/Engine/Source/Runtime/Core/Public/UObject/NameTypes.h#L34
            // NAME_SIZE = 1024

            // InName.GetComparisonIndex() <= MAX_NETWORKED_HARDCODED_NAME;
            // InName.GetPlainNameString();
            // InName.GetNumber();

            var inString = archive.ReadFString();
            var inNumber = archive.ReadInt32();
            return inString;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/PackageMapClient.h#L64
        /// </summary>
        public virtual NetFieldExport ReadNetFieldExport(FArchive archive)
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
                else if (Replay.Header.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_NETEXPORT_SERIALIZE_FIX)
                {
                    // FName
                    fieldExport.Name = archive.ReadFString();
                }
                else
                {
                    fieldExport.Name = StaticParseName(archive);
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
                NetFieldExports = new List<NetFieldExport>()
            };

            for (var i = 0; i < group.NetFieldExportsLength; i++)
            {
                var netFieldExport = ReadNetFieldExport(archive);
                if (netFieldExport != null)
                {
                    // TODO fix null fields
                    group.NetFieldExports.Add(netFieldExport);
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
                // TODO seperate reader?
                var size = archive.ReadInt32();
                InternalLoadObject(archive, true);
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
                NetFieldExportGroup group;

                if (isExported)
                {
                    var pathName = archive.ReadFString();
                    var numExports = archive.ReadIntPacked();

                    if (!NetFieldExportGroupMap.TryGetValue(pathName, out group))
                    {
                        group = new NetFieldExportGroup
                        {
                            PathName = pathName,
                            PathNameIndex = pathNameIndex,
                            NetFieldExportsLength = numExports
                        };

                        // TODO: 0 is reserved !?
                        group.NetFieldExports = new List<NetFieldExport>((int)numExports);

                        NetFieldExportGroupMap.Add(pathName, group);
                        NetFieldExportGroupIndexToGroup.Add(pathNameIndex, group); //TODO outside if statement!?
                    }
                    //GuidCache->NetFieldExportGroupPathToIndex.Add(PathName, PathNameIndex);
                    //GuidCache->NetFieldExportGroupIndexToGroup.Add(PathNameIndex, NetFieldExportGroup);
                }
                else
                {
                    NetFieldExportGroupIndexToGroup.TryGetValue(pathNameIndex, out group);
                }

                var netField = ReadNetFieldExport(archive);

                if (group != null)
                {
                    group.NetFieldExports.Add(netField);
                    // TODO preserve compatibility flag ?
                }
                else
                {
                    _logger.LogInformation("ReceiveNetFieldExports: Unable to find NetFieldExportGroup for export.");
                }
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L2848
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<PlaybackPacket> ReadDemoFrameIntoPlaybackPackets(FArchive archive)
        {
            if (archive.NetworkVersion >= NetworkVersionHistory.HISTORY_MULTIPLE_LEVELS)
            {
                var currentLevelIndex = archive.ReadInt32();
            }
            var timeSeconds = archive.ReadSingle();
            _logger?.LogInformation($"ReadDemoFrameIntoPlaybackPackets at {timeSeconds}");

            if (archive.NetworkVersion >= NetworkVersionHistory.HISTORY_LEVEL_STREAMING_FIXES)
            {
                ReadExportData(archive);
            }

            if (archive.HasLevelStreamingFixes())
            {
                var numStreamingLevels = archive.ReadIntPacked();
                for (var i = 0; i < numStreamingLevels; i++)
                {
                    var levelName = archive.ReadFString();
                }
            }
            else
            {
                var numStreamingLevels = archive.ReadIntPacked();
                for (var i = 0; i < numStreamingLevels; i++)
                {
                    var packageName = archive.ReadFString();
                    var packageNameToLoad = archive.ReadFString();
                    // FTransform
                    //var levelTransform = reader.ReadFString();
                    // filter duplicates
                }
            }

            if (archive.HasLevelStreamingFixes())
            {
                var externalOffset = archive.ReadUInt64();
            }

            // if (!bForLevelFastForward)
            ReadExternalData(archive);

            if (archive.HasGameSpecificFrameData())
            {
                var skipExternalOffset = archive.ReadUInt64();

                if (skipExternalOffset > 0)
                {
                    // ignore it for now
                    var bytes = archive.ReadBytes((int)skipExternalOffset);
                }
            }
            // else skip externalOffset

            var playbackPackets = new List<PlaybackPacket>();
            var @continue = true;
            while (@continue)
            {
                if (archive.HasLevelStreamingFixes())
                {
                    var seenLevelIndex = archive.ReadIntPacked();
                }

                var packet = ReadPacket(archive);
                playbackPackets.Add(packet);

                @continue = packet.State switch
                {
                    PacketState.End => false,
                    PacketState.Error => false,
                    PacketState.Success => true,
                    _ => false
                };
            }

            foreach (var packet in playbackPackets.Where(p => p.State == PacketState.Success))
            {
                Debug("packetsizes", $"size: {packet.Data.Length}, time: {timeSeconds}");
            }
            return playbackPackets;
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
                NetFieldExportGroup group;

                if (bitArchive.ReadBit())
                {
                    var pathName = bitArchive.ReadFString();
                    var numExports = bitArchive.ReadUInt32();

                    if (!NetFieldExportGroupMap.TryGetValue(pathName, out group))
                    {
                        group = new NetFieldExportGroup
                        {
                            PathName = pathName,
                            PathNameIndex = pathNameIndex,
                            NetFieldExportsLength = numExports
                        };
                        NetFieldExportGroupMap.Add(pathName, group);
                    }

                    NetFieldExportGroupIndexToGroup.Add(pathNameIndex, group);
                }
                else
                {
                    group = NetFieldExportGroupIndexToGroup[pathNameIndex];
                }

                var netField = ReadNetFieldExport(bitArchive);

                if (group.IsValidIndex(netField.Handle))
                {
                    //netField.Incompatible = group.NetFieldExports[(int)netField.Handle].Incompatible;
                    group.NetFieldExports[(int)netField.Handle] = netField;
                }
                else
                {
                    // ReceiveNetFieldExports: Invalid NetFieldExport Handle
                    // InBunch.SetError();
                }
            }
        }

        /// <summary>
        /// Loads a UObject from an FArchive stream. Reads object path if there, and tries to load object if its not already loaded
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L804
        /// </summary>
        public virtual NetworkGUID InternalLoadObject(FArchive archive, bool isExportingNetGUIDBunch)
        {
            // TODO: INTERNAL_LOAD_OBJECT_RECURSION_LIMIT  = 16
            var netGuid = new NetworkGUID()
            {
                Value = archive.ReadIntPacked()
            };

            if (!netGuid.IsValid())
            {
                return null;
            }

            if (netGuid.IsDefault() || isExportingNetGUIDBunch)
            {
                var flags = archive.ReadByteAsEnum<ExportFlags>();

                // outerguid
                if (flags == ExportFlags.bHasPath || flags == ExportFlags.bHasPathAndNetWorkChecksum || flags == ExportFlags.All)
                {
                    var outerGuid = InternalLoadObject(archive, true);

                    var pathName = archive.ReadFString();

                    if (!NetGuidCache.ContainsKey(netGuid.Value))
                    {
                        NetGuidCache.Add(netGuid.Value, pathName);
                    }

                    if (outerGuid != null && !OuterNetGuidCache.ContainsKey(netGuid.Value))
                    {
                        OuterNetGuidCache.Add(netGuid.Value, outerGuid.Value);
                    }

                    if (flags >= ExportFlags.bHasNetworkChecksum)
                    {
                        var networkChecksum = archive.ReadUInt32();
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
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1027
            const int MAX_GUID_COUNT = 2048;
            if (numGUIDsInBunch > MAX_GUID_COUNT)
            {
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
        /// <param name="bitReader"></param>
        /// <param name="bunch"></param>
        public virtual void ReceivedRawBunch(DataBunch bunch)
        {
            // bDeleted =
            ReceivedNextBunch(bunch);

            // if (bDeleted) return;
            // else { We shouldn't hit this path on 100% reliable connections }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L517
        /// </summary>
        /// <param name="bitReader"></param>
        /// <param name="bunch"></param>
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
                InReliable[bunch.ChIndex] = bunch.ChSequence;
            }

            // merge
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
                            _logger?.LogWarning($"Corrupt partial bunch. Initial partial bunches are expected to be byte-aligned. BitsLeft = {bitsLeft % 8}.");
                            return;
                        }

                        PartialBunch.Archive.AppendDataFromChecked(bunch.Archive.ReadBits(bitsLeft));
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
                        _logger?.LogDebug($"Merging Partial Bunch: {bitsLeft} Bytes");
                        if (!bunch.bHasPackageMapExports && bitsLeft > 0)
                        {
                            PartialBunch.Archive.AppendDataFromChecked(bunch.Archive.ReadBits(bitsLeft));
                            // InPartialBunch->AppendDataFromChecked( Bunch.GetDataPosChecked(), Bunch.GetBitsLeft() );
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

                            // debugging
                            PartialBunch.Archive.Mark();
                            var alignpartial = PartialBunch.Archive.GetBitsLeft() % 8;
                            if (alignpartial != 0)
                            {
                                var append = new bool[alignpartial];
                                PartialBunch.Archive.AppendDataFromChecked(append);
                            }
                            Debug($"partialbunch-{PartialBunch.ChIndex}-{PartialBunch.ChName}", "partialbunches", PartialBunch.Archive.ReadBytes(PartialBunch.Archive.GetBitsLeft() / 8));
                            PartialBunch.Archive.Pop();

                            ReceivedSequencedBunch(PartialBunch);
                            return;
                        }
                        return;
                    }
                    else
                    {
                        // Merge problem - delete InPartialBunch. This is mainly so that in the unlikely chance that ChSequence wraps around, we wont merge two completely separate partial bunches.
                        // We shouldn't hit this path on 100% reliable connections
                        _logger?.LogError("Merge problem:  We shouldn't hit this path on 100% reliable connections");
                        return;
                    }
                }
                // bunch size check...
            }

            // something with opening channels...

            // Receive it in sequence.
            ReceivedSequencedBunch(bunch);
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L348
        /// </summary>
        /// <param name="bitReader"></param>
        /// <param name="bunch"></param>
        public virtual bool ReceivedSequencedBunch(DataBunch bunch)
        {
            // if ( !Closing ) {
            switch (bunch.ChName)
            {
                case "Control":
                    ReceivedControlBunch(bunch);
                    break;
                default:
                    ReceivedActorBunch(bunch);
                    break;
            };
            // }

            // We have fully received the bunch, so process it.
            if (bunch.bClose)
            {
                //ConditionalCleanUp(false, Bunch.CloseReason);
                // lots of stuff is happening here, but maybe this is enough... :)
                ChannelActors[bunch.ChIndex] = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L1346
        /// </summary>
        /// <param name="bitReader"></param>
        /// <param name="bunch"></param>
        public virtual void ReceivedControlBunch(DataBunch bunch)
        {
            // control channel
            while (!bunch.Archive.AtEnd())
            {
                var messageType = bunch.Archive.ReadByte();
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L2298
        /// </summary>
        /// <param name="bitReader"></param>
        /// <param name="bunch"></param>
        public virtual void ReceivedActorBunch(DataBunch bunch)
        {
            if (bunch.bHasMustBeMappedGUIDs)
            {
                var numMustBeMappedGUIDs = bunch.Archive.ReadUInt16();
                for (var i = 0; i < numMustBeMappedGUIDs; i++)
                {
                    var guid = bunch.Archive.ReadIntPacked();
                }
            }

            // if actor == null
            var actor = ChannelActors.ContainsKey(bunch.ChIndex) ? ChannelActors[bunch.ChIndex] : false;
            if (!actor && bunch.bOpen)
            {
                // FBitReaderMark (how does this even work??)
                // Take a sneak peak at the actor guid so we have a copy of it now
                bunch.Archive.Mark();
                var actorGuid = bunch.Archive.ReadIntPacked();
                bunch.Archive.Pop();

                // TODO set channel actor here??
                // we can now map guid to channel, even if all the bunches get queued
                //if (Connection->InternalAck)
                //{
                //    Connection->NotifyActorNetGUID(this);
                //}
            }

            ProcessBunch(bunch);
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L2411
        /// </summary>
        /// <param name="bitReader"></param>
        /// <param name="bunch"></param>
        public virtual void ProcessBunch(DataBunch bunch)
        {
            var actor = ChannelActors.ContainsKey(bunch.ChIndex) ? ChannelActors[bunch.ChIndex] : false;
            if (!actor)
            {
                if (!bunch.bOpen)
                {
                    _logger?.LogError("New actor channel received non-open packet.");
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

                    // if (Ar.IsSaving() || (Connection && (Connection->EngineNetworkProtocolVersion >= EEngineNetworkVersionHistory::HISTORY_NEW_ACTOR_OVERRIDE_LEVEL)))
                    if (bunch.Archive.EngineNetworkVersion >= EngineNetworkVersionHistory.HISTORY_NEW_ACTOR_OVERRIDE_LEVEL)
                    {
                        inActor.Level = InternalLoadObject(bunch.Archive, false);
                    }

                    // bSerializeLocation
                    if (bunch.Archive.ReadBit())
                    {
                        // Location.NetSerialize(Ar, this, SerSuccess);
                        inActor.Location = bunch.Archive.ReadPackedVector(10, 24);
                    }

                    // bSerializeRotation
                    if (bunch.Archive.ReadBit())
                    {
                        // Rotation.NetSerialize(Ar, this, SerSuccess);
                        inActor.Rotation = bunch.Archive.ReadRotationShort();
                    }

                    // bSerializeScale
                    if (bunch.Archive.ReadBit())
                    {
                        // Scale.NetSerialize(Ar, this, SerSuccess);
                        inActor.Scale = bunch.Archive.ReadPackedVector(10, 24);
                    }

                    // bSerializeVelocity
                    if (bunch.Archive.ReadBit())
                    {
                        // Velocity.NetSerialize(Ar, this, SerSuccess);
                        inActor.Velocity = bunch.Archive.ReadPackedVector(10, 24);
                    }
                }
                Channels[bunch.ChIndex].Actor = inActor;
                //SetChannelActor(NewChannelActor);

                //NotifyActorChannelOpen(Actor, Bunch);
                // OnActorChannelOpen
                // Attempt to match the player controller to a local viewport (client side)
                // shootergame debugging
                //if (bunch.ChIndex == 6)
                //{
                //    var netPlayerIndex = bunch.Archive.ReadByte();
                //}

                //RepFlags.bNetInitial = true;

                ChannelActors[bunch.ChIndex] = true;
                ChannelNetGuids[bunch.ChIndex] = inActor.ActorNetGUID.Value;
            }

            // RepFlags.bNetOwner = true; // ActorConnection == Connection is always true??

            //RepFlags.bIgnoreRPCs = Bunch.bIgnoreRPCs;
            //RepFlags.bSkipRoleSwap = bSkipRoleSwap;

            //  Read chunks of actor content
            while (!bunch.Archive.AtEnd())
            {
                //FNetBitReader Reader(Bunch.PackageMap, 0 );
                var repObject = ReadContentBlockPayload(bunch, out var bHasRepLayout, out var reader);

                if (bunch.Archive.IsError)
                {
                    _logger?.LogError($"UActorChannel::ReceivedBunch: ReadContentBlockPayload FAILED. bunch: {bunchIndex}");
                    break;
                }

                if (repObject == 0 || reader == null || reader.AtEnd())
                {
                    // Nothing else in this block, continue on (should have been a delete or create block)
                    continue;
                }

                if (!ReceivedReplicatorBunch(bunch, reader, repObject, bHasRepLayout))
                {
                    // Don't consider this catastrophic in replays
                    _logger?.LogWarning("UActorChannel::ProcessBunch: Replicator.ReceivedBunch returned false");
                    continue;
                }
            }
            // PostReceivedBunch, not interesting?
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/DataReplication.cpp#L896
        /// </summary>
        /// <param name="archive"></param>
        public virtual bool ReceivedReplicatorBunch(DataBunch bunch, FBitArchive archive, uint repObject, bool bHasRepLayout)
        {
            // TODO fix this shit

            // outer is used to get path name
            // coreredirects.cpp ...
            NetFieldExportGroup netFieldExportGroup = null;

            if (!ArchetypeToNetFieldGroup.ContainsKey(repObject))
            {
                var path = NetGuidCache[Channels[bunch.ChIndex].Actor.Archetype.Value];
                path = RemoveAllPathPrefixes(path);
                foreach (var groupPath in NetFieldExportGroupMap.Keys)
                {
                    var groupPathFixed = RemoveAllPathPrefixes(groupPath); // TODO, do this earlier so we dont have to work with strings when this loops because that's SLOW AF
                    if (groupPathFixed.Contains(path))
                    {
                        netFieldExportGroup = NetFieldExportGroupMap[groupPath];
                        ArchetypeToNetFieldGroup.Add(repObject, groupPath);
                        break;
                    }
                }

                if (netFieldExportGroup == null)
                {
                    Debug("failedgroups", $"actor guid: {NetGuidCache[repObject]}");
                    return false;
                }
            }
            else
            {
                netFieldExportGroup = NetFieldExportGroupMap[ArchetypeToNetFieldGroup[repObject]];
            }

            // Handle replayout properties
            if (bHasRepLayout)
            {
                // TODO bool
                //if (!ReceiveProperties())
                //{
                //    _logger?.LogError("RepLayout->ReceiveProperties FAILED");
                //    return false;
                //}
                ReceiveProperties(archive, netFieldExportGroup, bunch.ChIndex);
            }

            // moved from ReadFieldHeaderAndPayload to here to save a search for ClassNetCache if not needed
            if (archive.AtEnd())
            {
                return true;
            }

            //FNetFieldExportGroup* NetFieldExportGroup = OwningChannel->GetNetFieldExportGroupForClassNetCache(ObjectClass);

            //static FORCEINLINE FString GenerateClassNetCacheNetFieldExportGroupName( const UClass* ObjectClass )
            //{
            //    return ObjectClass->GetName() + FString(TEXT("_ClassNetCache"));
            //}
            var classNetCachePath = $"{RemoveAllPathPrefixes(netFieldExportGroup.PathName)}_ClassNetCache";
            NetFieldExportGroup classNetCache;
            if (!NetFieldExportGroupMap.TryGetValue(classNetCachePath, out classNetCache))
            {
                _logger?.LogError($"Couldnt find ClassNetCache for {netFieldExportGroup.PathName}");
                return false;
            }

            // Read fields from stream
            NetFieldExport fieldCache; // TODO FFieldNetCache ?

            FBitArchive reader;
            while (ReadFieldHeaderAndPayload(archive, classNetCache, out fieldCache, out reader))
            {
                _logger?.LogDebug($"RPCs to read for group {classNetCache.PathName}, field {fieldCache.Name} and numbits: {reader.GetBitsLeft()}");
                if (fieldCache == null)
                {
                    _logger?.LogError($"ReceivedBunch: FieldCache == nullptr: {classNetCache.PathName}");
                    continue;
                }

                if (fieldCache.Incompatible)
                {
                    // We've already warned about this property once, so no need to continue to do so
                    _logger?.LogWarning($"ReceivedBunch: FieldCache->bIncompatible == true: {fieldCache.Name}");
                    continue;
                }

                reader.Mark();
                var bits = reader.ReadBits(reader.GetBitsLeft());
                byte[] ret = new byte[(int)Math.Ceiling(bits.Length / 8.0)];
                for (int i = 0; i < bits.Length; i += 8)
                {
                    int value = 0;
                    for (int j = 0; j < 8; j++)
                    {
                        if (i + j < bits.Length)
                        {
                            if (bits[i + j])
                            {
                                value += 1 << (7 - j);
                            }
                        }
                    }
                    ret[i / 8] = (byte)value;
                }
                Debug($"rpc-{fieldCache.Name}-{bits.Length}", "rpc", ret);
                reader.Pop();

                // Handle property
                // TODO get this from fieldCache type property somehow...
                var property = false;
                var function = true;
                if (property)
                {
                    if (!ReceiveCustomDeltaProperty(reader))
                    {
                        //FieldCache->bIncompatible = true;
                        continue;
                    }
                    // Successfully received it.
                }
                else if (function)
                {
                    // TODO yeah yeah I know :(
                    NetFieldExportGroup functionGroup = null;
                    foreach (var groupPath in NetFieldExportGroupMap.Keys)
                    {
                        var groupPathFixed = RemoveAllPathPrefixes(groupPath);
                        if (groupPathFixed.Contains(fieldCache.Name))
                        {
                            functionGroup = NetFieldExportGroupMap[groupPath];
                            break;
                        }
                    }

                    if (!ReceivedRPC(reader, functionGroup, bunch.ChIndex))
                    {
                        return false;
                    }

                    //if (!bSuccess)
                    //{
                    //    return false;
                    //}
                    //else if (bDelayFunction)
                    //{
                    //}
                    //else if (Object == nullptr || Object->IsPendingKill())
                    //{
                    //    // replicated function destroyed Object
                    //    return true;
                    //}
                }
                else
                {
                    _logger?.LogError($"ReceivedBunch: Invalid replicated field {fieldCache.Name}. BunchIndex: {bunchIndex}, packetId: {bunch.PacketId}");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// DataReplication.cpp 1158
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public virtual bool ReceivedRPC(FBitArchive reader, NetFieldExportGroup netFieldExportGroup, uint channelIndex)
        {
            ReceiveProperties(reader, netFieldExportGroup, channelIndex);

            if (reader.IsError)
            {
                _logger?.LogError("ReceivedRPC: ReceivePropertiesForRPC - Reader.IsError() == true");
                return false;
            }

            if (!reader.AtEnd())
            {
                _logger?.LogError("ReceivedRPC: ReceivePropertiesForRPC - Mismatch read.");
                return false;
            }

            return true;
        }

        public virtual bool ReceiveCustomDeltaProperty(FBitArchive reader)
        {
            if (reader.EngineNetworkVersion >= EngineNetworkVersionHistory.HISTORY_FAST_ARRAY_DELTA_STRUCT)
            {
                // bSupportsFastArrayDeltaStructSerialization
                reader.ReadBit();
            }

            // Receive array index (static sized array, i.e. MemberVariable[4])
            //if (Property->ArrayDim != 1)

            // We should only be receiving custom delta properties (since RepLayout handles the rest)
            //if (!EnumHasAnyFlags(Parent.Flags, ERepParentFlags::IsCustomDelta))

            if (NetDeltaSerialize(reader))
            {
                //if (UNLIKELY(Params.Reader->IsError()))
                //{
                //    UE_LOG(LogNet, Error, TEXT("FRepLayout::ReceiveCustomDeltaProperty: NetDeltaSerialize - Reader.IsError() == true. Property: %s, Object: %s"), *Params.DebugName, *Params.Object->GetFullName());
                //    return false;
                //}
                //if (UNLIKELY(Params.Reader->GetBitsLeft() != 0))
                //{
                //    UE_LOG(LogNet, Error, TEXT("FRepLayout::ReceiveCustomDeltaProperty: NetDeltaSerialize - Mismatch read. Property: %s, Object: %s"), *Params.DebugName, *Params.Object->GetFullName());
                //    return false;
                //}

                // Successfully received it.
                return true;
            }

            return false;
        }

        public virtual bool NetDeltaSerialize(FBitArchive reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/Engine/Private/RepLayout.cpp#L3141
        /// </summary>
        public IList<uint> ReceivePropertiesArray(FBitArchive archive, NetFieldExportGroup group, uint channelIndex)
        {
            IList<uint> result = new List<uint>();

            var stateValue = new FortItemEntryStateValue();

            var arrayNum = archive.ReadIntPacked();
            while (true)
            {
                var index = archive.ReadIntPacked();
                if (index == 0)
                {
                    // At this point, the 0 either signifies:
                    //	An array terminator, at which point we're done.
                    //	An array element terminator, which could happen if the array had tailing elements removed.
                    if (archive.GetBitsLeft() == 8)
                    {
                        // We have bits left over, so see if its the Array Terminator.
                        // This should be 0
                        var terminator = archive.ReadIntPacked();

                        if (terminator != 0)
                        {
                            _logger?.LogError("Invalid array terminator");
                            break;
                            //UE_LOG(LogRep, Warning, TEXT("ReceiveProperties_BackwardsCompatible_r: Invalid array terminator. Owner: %s, Name: %s, NetFieldExportHandle: %i, Terminator: %d"), *Owner->GetName(), *NetFieldExportGroup->NetFieldExports[NetFieldExportHandle].ExportName.ToString(), NetFieldExportHandle, Terminator);
                            //return false;
                        }
                    }

                    // We're done
                    break;
                }

                // Shift all indexes down since 0 represents null handle
                index--;

                if (index > arrayNum)
                {
                    _logger?.LogError("Array index out of bounds.");
                    //UE_LOG(LogRep, Warning, TEXT("ReceiveProperties_BackwardsCompatible_r: Array index out of bounds. Index: %i, ArrayNum: %i, Owner: %s, Name: %s, NetFieldExportHandle: %i, Checksum: %u"), Index, ArrayNum, *Owner->GetName(), *NetFieldExportGroup->NetFieldExports[NetFieldExportHandle].ExportName.ToString(), NetFieldExportHandle, Checksum);
                    //return false;
                }

                // TODO remove duplicate code
                while (true)
                {
                    var handle = archive.ReadIntPacked();

                    if (handle == 0)
                    {
                        // We're done
                        break;
                        // return true;
                    }

                    // We purposely add 1 on save, so we can reserve 0 for "done"
                    handle--;

                    // TODO remove loop...
                    var export = group.NetFieldExports.FirstOrDefault(i => i.Handle == handle);
                    //var export = netFieldExportGroup.NetFieldExports[(int)handle];

                    var numBits = archive.ReadIntPacked();

                    if (export == null)
                    {
                        _logger?.LogError($"Couldnt find handle {handle}, numbits is {numBits}");
                        Debug("missinghandles", $"\n{group.PathName}\t{handle}\t{numBits}");
                        archive.ReadBits(numBits);
                        continue;
                    }

                    if (export.Incompatible)
                    {
                        _logger?.LogInformation("Incompatible export");
                        archive.ReadBits(numBits);
                        // We've already warned that this property doesn't load anymore
                        continue;
                    }

                    archive.Mark();
                    Debug($"cmdarray-{export.Name}-{numBits}", "cmds", archive.ReadBytes(Math.Max((int)Math.Ceiling(numBits / 8.0), 1)));
                    archive.Pop();

                    var cmdReader = new NetBitReader(archive.ReadBits(numBits));

                    if (group.PathName == "/Script/FortniteGame.FortPickupAthena")
                    {
                        switch (export.Name)
                        {
                            // FFortItemEntryStateValue
                            case "StateType":
                                stateValue.StateType = cmdReader.SerializePropertyEnum(11); // EFortItemEntryState
                                break;
                            case "IntValue":
                                stateValue.IntValue = cmdReader.SerializePropertyInt();
                                break;
                            case "NameValue":
                                stateValue.NameValue = StaticParseName(cmdReader);
                                break;
                        }
                    }
                    else
                    {
                        switch (export.Name)
                        {
                            case "Dances":
                                result.Add(cmdReader.SerializePropertyObject());
                                break;
                            case "ItemWraps":
                                result.Add(cmdReader.SerializePropertyObject());
                                break;
                        }
                    }

                }
                if (group.PathName == "/Script/FortniteGame.FortPickupAthena")
                {
                    _logger.LogDebug(stateValue.ToString());
                }

                if (archive.IsError)
                {
                    _logger?.LogError($"ReceiveProperties_BackwardsCompatible_r: Error reading array index element payload. Name: {group.PathName}, Channel: {channelIndex}");
                    break;
                }

            }

            // TODO wrong location-ish...
            if (!archive.AtEnd())
            {
                _logger?.LogError($"ReceiveProperties_BackwardsCompatible_r: Array didn't read proper number of bits. Name: {group.PathName}, Channel: {channelIndex}");
            }
            return result;
        }

        /// <summary>
        ///  https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/RepLayout.cpp#L2895
        ///  https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/RepLayout.cpp#L2971
        ///  https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/RepLayout.cpp#L3022
        /// </summary>
        /// <param name="archive"></param>
        public virtual void ReceiveProperties(FBitArchive archive, NetFieldExportGroup group, uint channelIndex)
        {
            // if ENABLE_PROPERTY_CHECKSUMS
            var doChecksum = archive.ReadBit();

            Debug("types", $"\n{group.PathName}");

            AthenaPlayerState playerState = new AthenaPlayerState();
            if (group.PathName == "/Script/FortniteGame.FortPlayerStateAthena")
            {
                if (!ActorStates.ContainsKey(channelIndex))
                {
                    ActorStates[channelIndex] = new List<AthenaPlayerState>();
                }
            }

            AthenaPlayerPawn playerPawn = new AthenaPlayerPawn();
            if (group.PathName == "/Game/Athena/PlayerPawn_Athena.PlayerPawn_Athena_C")
            {
                if (!PlayerPawns.ContainsKey(channelIndex))
                {
                    PlayerPawns[channelIndex] = new List<AthenaPlayerPawn>();
                }
            }

            while (true)
            {
                var handle = archive.ReadIntPacked();

                if (handle == 0)
                {
                    // We're done
                    break;
                    // return true;
                }

                // We purposely add 1 on save, so we can reserve 0 for "done"
                handle--;

                // TODO remove loop...
                var export = group.NetFieldExports.FirstOrDefault(i => i.Handle == handle);
                //var export = netFieldExportGroup.NetFieldExports[(int)handle];

                var numBits = archive.ReadIntPacked();

                if (export == null)
                {
                    _logger?.LogError($"Couldnt find handle {handle}, numbits is {numBits}");
                    Debug("missinghandles", $"\n{group.PathName}\t{handle}\t{numBits}");
                    archive.ReadBits(numBits);
                    continue;
                }

                Debug("types", $"{ export.Name}\t{export.Type}\t{numBits}");

                if (export.Incompatible)
                {
                    _logger?.LogInformation("Incompatible export");
                    archive.ReadBits(numBits);
                    // We've already warned that this property doesn't load anymore
                    continue;
                }

                archive.Mark();
                Debug($"cmd-{export.Name}-{numBits}", "cmds", archive.ReadBytes(Math.Max((int)Math.Ceiling(numBits / 8.0), 1)));
                archive.Pop();

                var cmdReader = new NetBitReader(archive.ReadBits(numBits));

                if (group.PathName == "/Script/FortniteGame.FortPlayerStateAthena")
                {
                    switch (export.Name)
                    {
                        case "PlayerID":
                            playerState.PlayerId = cmdReader.ReadInt32();
                            Debug("playerIDs", $"{playerState.PlayerId}");
                            break;
                        case "StartTime":
                            playerState.StartTime = cmdReader.ReadInt32();
                            break;
                        case "PlatformUniqueNetId":
                            playerState.PlatformId = cmdReader.SerializePropertyNetId();
                            break;
                        case "UniqueId":
                            playerState.UniqueId = cmdReader.SerializePropertyNetId();
                            break;
                        case "ColorId":
                            playerState.ColorId = cmdReader.ReadFString();
                            break;
                        case "IconId":
                            playerState.IconId = cmdReader.ReadFString();
                            break;
                        case "StreamerModeName":
                            var unknown = cmdReader.ReadByte();
                            cmdReader.SkipBytes(8);
                            playerState.PlayerNameId = cmdReader.ReadFString();
                            playerState.SkinName = cmdReader.ReadFString();
                            Debug("playernames", $"[StreamerModeName] {unknown} {playerState.PlayerNameId} {playerState.SkinName}");
                            break;
                        case "PlayerNamePrivate":
                            playerState.PlayerNamePrivate = cmdReader.ReadFString();
                            Debug("playernames", playerState.PlayerNamePrivate);
                            break;
                        case "PartyOwnerUniqueId":
                            playerState.PartyOwnerUniqueId = cmdReader.SerializePropertyNetId();
                            break;
                        case "WorldPlayerId":
                            playerState.WorldPlayerId = cmdReader.ReadInt32();

                            Debug("WorldPlayerIds", $"{playerState.WorldPlayerId}");
                            break;
                        case "Platform":
                            playerState.Platform = cmdReader.ReadFString();
                            break;
                        case "PlayerTeamPrivate":
                            playerState.PlayerTeamPrivate = cmdReader.SerializePropertyByte();
                            break;
                        case "Team":
                            playerState.Team = cmdReader.SerializePropertyEnum(104);
                            break;
                        case "TeamIndex":
                            playerState.TeamIndex = cmdReader.SerializePropertyByte();
                            break;
                        case "SquadListUpdateValue":
                            playerState.SquadListUpdateValue = cmdReader.ReadInt32();
                            break;
                        case "SquadId":
                            playerState.SquadId = cmdReader.ReadByte();
                            break;
                        case "Level":
                            playerState.Level = cmdReader.ReadUInt32();
                            break;
                        case "bInAircraft":
                            playerState.bInAircraft = cmdReader.SerializePropertyBool();
                            break;
                        case "bHasFinishedLoading":
                            playerState.bHasFinishedLoading = cmdReader.SerializePropertyBool();
                            break;
                        case "bHasStartedPlaying":
                            playerState.bHasStartedPlaying = cmdReader.SerializePropertyBool();
                            break;
                        case "HeroType":
                            playerState.HeroType = cmdReader.SerializePropertyObject();
                            break;
                        case "CharacterGender":
                            playerState.CharacterGender = cmdReader.SerializePropertyEnum(4);
                            break;
                        case "CharacterBodyType":
                            playerState.CharacterBodyType = cmdReader.SerializePropertyEnum(8);
                            break;
                        case "WasReplicatedFlags":
                            playerState.WasReplicatedFlags = cmdReader.SerializePropertyByte();
                            break;
                        case "MapIndicatorPos":
                            playerState.MapIndicatorPos = cmdReader.SerializeVector2D();
                            break;
                        case "Owner":
                            playerState.Owner = cmdReader.SerializePropertyUInt32();
                            break;
                        case "Parts":
                            playerState.Parts = cmdReader.SerializePropertyObject();
                            break;
                        case "bUsingStreamerMode":
                            playerState.bUsingStreamerMode = cmdReader.SerializePropertyBool();
                            break;
                        case "bThankedBusDriver":
                            playerState.bThankedBusDriver = cmdReader.SerializePropertyBool();
                            break;
                        case "bOnlySpectator":
                            playerState.bOnlySpectator = cmdReader.SerializePropertyBool();
                            break;
                        case "bIsDisconnected":
                            playerState.bIsDisconnected = cmdReader.SerializePropertyBool();
                            break;
                        case "FinisherOrDowner":
                            playerState.FinisherOrDowner = cmdReader.SerializePropertyObject();
                            break;
                        case "bInitialized":
                            playerState.bInitialized = cmdReader.SerializePropertyBool();
                            break;
                        case "bUsingAnonymousMode":
                            playerState.bUsingAnonymousMode = cmdReader.SerializePropertyBool();
                            break;
                        case "ResurrectionExpirationTime":
                            playerState.ResurrectionExpirationTime = cmdReader.SerializePropertyFloat();
                            break;
                        case "Ping":
                            playerState.Ping = cmdReader.SerializePropertyUInt32();
                            break;
                        case "Role":
                        case "RemoteRole":
                        case "PlayerRole":
                        case "DeathTags":
                            break;
                        default:
                            Debug("playerstate-fields", $"{export.Name}\t{numBits}");
                            _logger.LogDebug($"unknown field {export.Name} in player state");
                            break;
                    }
                }

                else if (group.PathName == "/Script/FortniteGame.FortPickupAthena")
                {
                    switch (export.Name)
                    {
                        case "bReplicateMovement":
                            cmdReader.SerializePropertyBool();
                            break;
                        case "ReplicatedMovement":
                            cmdReader.SerializeRepMovement();
                            break;
                        case "bRandomRotation":
                            cmdReader.SerializePropertyBool();
                            break;
                        // UFortItemDefinition
                        case "ItemDefinition":
                            cmdReader.SerializePropertyObject();
                            break;
                        case "Durability":
                            cmdReader.SerializePropertyInt();
                            break;
                        case "Level":
                            cmdReader.SerializePropertyInt();
                            break;
                        case "A":
                            cmdReader.SerializePropertyInt();
                            break;
                        case "B":
                            cmdReader.SerializePropertyInt();
                            break;
                        case "C":
                            cmdReader.SerializePropertyInt();
                            break;
                        case "D":
                            cmdReader.SerializePropertyInt();
                            break;
                        case "bIsDirty":
                            cmdReader.SerializePropertyBool();
                            break;
                        case "bTossedFromContainer":
                            cmdReader.SerializePropertyBool();
                            break;
                        case "bServerStoppedSimulation":
                            cmdReader.SerializePropertyBool();
                            break;
                        case "ServerImpactSoundFlash":
                            cmdReader.SerializePropertyByte();
                            break;
                        case "PickupTarget":
                            cmdReader.SerializePropertyObject();
                            break;
                        case "ItemOwner":
                            cmdReader.SerializePropertyObject();
                            break;
                        case "FlyTime":
                            cmdReader.SerializePropertyFloat();
                            break;
                        case "FinalTossRestLocation":
                            cmdReader.SerializePropertyVector10();
                            break;
                        case "TossState":
                            cmdReader.SerializePropertyEnum(3);
                            break;
                        case "bPickedUp":
                            cmdReader.SerializePropertyBool();
                            break;
                        case "Count":
                            cmdReader.SerializePropertyInt();
                            break;
                        case "LoadedAmmo":
                            cmdReader.SerializePropertyInt();
                            break;
                        case "PawnWhoDroppedPickup":
                            cmdReader.SerializePropertyObject();
                            break;
                        case "StartDirection":
                            cmdReader.SerializePropertyVectorNormal();
                            break;
                        case "LootInitialPosition":
                            cmdReader.SerializePropertyVector10();
                            break;
                        case "LootFinalPosition":
                            cmdReader.SerializePropertyVector10();
                            break;
                        // TArray<struct FFortItemEntryStateValue>
                        case "StateValues":
                            ReceivePropertiesArray(cmdReader, group, channelIndex);
                            break;
                        case "Role":
                        case "RemoteRole":
                            break;
                        default:
                            Debug("pickup-fields", $"{export.Name}\t{numBits}");
                            _logger.LogDebug($"unknown field {export.Name} in pickup");
                            break;
                    }
                }

                else if (group.PathName == "/Game/Athena/Aircraft/AthenaAircraft.AthenaAircraft_C")
                {
                    switch (export.Name)
                    {
                        case "FlightStartLocation":
                            var startLocation = cmdReader.SerializePropertyVector100();
                            break;
                        case "FlightStartRotation":
                            var startRotation = cmdReader.SerializePropertyRotator();
                            break;
                        case "FlightSpeed":
                            var speed = cmdReader.ReadSingle();
                            break;
                        case "TimeTillFlightEnd":
                            var flightEnd = cmdReader.ReadSingle();
                            break;
                        case "TimeTillDropStart":
                            var dropStart = cmdReader.ReadSingle();
                            break;
                        case "TimeTillDropEnd":
                            var dropEnd = cmdReader.ReadSingle();
                            break;
                    }
                }

                else if (group.PathName == "/Game/Athena/SupplyDrops/Llama/AthenaSupplyDrop_Llama.AthenaSupplyDrop_Llama_C")
                {
                    switch (export.Name)
                    {
                        case "ReplicatedMovement":
                            cmdReader.SerializeRepMovement();
                            break;
                        case "bEditorPlaced":
                            cmdReader.SerializePropertyBool();
                            break;
                    }
                }

                else if (group.PathName == "/Game/Athena/PlayerPawn_Athena.PlayerPawn_Athena_C")
                {
                    switch (export.Name)
                    {
                        case "PawnUniqueID":
                            playerPawn.PawnUniqueID = cmdReader.SerializePropertyInt();
                            break;
                        case "ReplicatedMovement":
                            playerPawn.RepMovement = cmdReader.SerializeRepMovement();
                            break;
                        case "ReplicatedMovementMode":
                            playerPawn.ReplicatedMovementMode = cmdReader.SerializePropertyByte();
                            break;
                        case "ReplayLastTransformUpdateTimeStamp":
                            playerPawn.ReplayLastTransformUpdateTimeStamp = cmdReader.SerializePropertyFloat();
                            break;
                        case "AccelerationPack":
                            playerPawn.AccelerationPack = cmdReader.SerializePropertyUInt16();
                            break;
                        case "AccelerationZPack":
                            playerPawn.AccelerationZPack = cmdReader.SerializePropertyByte();
                            break;
                        case "RemoteViewData32":
                            playerPawn.RemoteViewData32 = cmdReader.SerializePropertyUInt32();
                            break;
                        case "bCanBeDamaged":
                            playerPawn.bCanBeDamaged = cmdReader.SerializePropertyBool();
                            break;
                        case "Instigator":
                            playerPawn.Instigator = cmdReader.SerializePropertyObject();
                            break;
                        case "PlayerState":
                            playerPawn.PlayerState = cmdReader.SerializePropertyObject();
                            break;
                        case "VocalChords":
                            break;
                        case "CapsuleRadiusAthena":
                            playerPawn.CapsuleRadiusAthena = cmdReader.SerializePropertyFloat();
                            break;
                        case "CapsuleHalfHeightAthena":
                            playerPawn.CapsuleHalfHeightAthena = cmdReader.SerializePropertyFloat();
                            break;
                        case "WalkSpeed":
                            playerPawn.WalkSpeed = cmdReader.SerializePropertyFloat();
                            break;
                        case "RunSpeed":
                            playerPawn.RunSpeed = cmdReader.SerializePropertyFloat();
                            break;
                        case "SprintSpeed":
                            playerPawn.SprintSpeed = cmdReader.SerializePropertyFloat();
                            break;
                        case "CrouchedRunSpeed":
                            playerPawn.CrouchedRunSpeed = cmdReader.SerializePropertyFloat();
                            break;
                        case "CrouchedSprintSpeed":
                            playerPawn.CrouchedSprintSpeed = cmdReader.SerializePropertyFloat();
                            break;
                        case "FlySpeed":
                            playerPawn.FlySpeed = cmdReader.SerializePropertyFloat();
                            break;
                        case "BannerIconId":
                            playerPawn.BannerIconId = cmdReader.SerializePropertyString();
                            break;
                        case "BannerColorId":
                            playerPawn.BannerColorId = cmdReader.SerializePropertyString();
                            break;
                        case "SkyDiveContrail":
                            playerPawn.SkyDiveContrail = cmdReader.SerializePropertyObject();
                            break;
                        case "Glider":
                            playerPawn.Glider = cmdReader.SerializePropertyObject();
                            break;
                        case "Pickaxe":
                            playerPawn.Pickaxe = cmdReader.SerializePropertyObject();
                            break;
                        case "Character":
                            playerPawn.Character = cmdReader.SerializePropertyObject();
                            break;
                        case "Backpack":
                            playerPawn.Backpack = cmdReader.SerializePropertyObject();
                            break;
                        case "LoadingScreen":
                            playerPawn.LoadingScreen = cmdReader.SerializePropertyObject();
                            break;
                        case "MusicPack":
                            playerPawn.MusicPack = cmdReader.SerializePropertyObject();
                            break;
                        case "EncryptedPawnReplayData":
                            break;
                        case "MovementBase":
                            playerPawn.AnimMontage = cmdReader.SerializePropertyObject();
                            break;
                        case "bServerHasBaseComponent":
                            playerPawn.bServerHasBaseComponent = cmdReader.SerializePropertyBool();
                            break;
                        case "CharacterVariantChannels":
                            break;
                        case "JumpFlashCount":
                            playerPawn.JumpFlashCount = cmdReader.SerializePropertyByte();
                            break;
                        case "CurrentWeapon":
                            playerPawn.CurrentWeapon = cmdReader.SerializePropertyObject();
                            break;
                        case "AnimMontage":
                            playerPawn.AnimMontage = cmdReader.SerializePropertyObject();
                            break;
                        case "PlayRate":
                            playerPawn.PlayRate = cmdReader.SerializePropertyFloat();
                            break;
                        case "BlendTime":
                            playerPawn.BlendTime = cmdReader.SerializePropertyFloat();
                            break;
                        case "ForcePlayBit":
                            playerPawn.ForcePlayBit = cmdReader.SerializePropertyBool();
                            break;
                        case "IsStopped":
                            playerPawn.ForcePlayBit = cmdReader.SerializePropertyBool();
                            break;
                        case "RepAnimMontageStartSection":
                            playerPawn.RepAnimMontageStartSection = cmdReader.SerializePropertyInt();
                            break;
                        case "bIsProxySimulationTimedOut":
                            playerPawn.bIsProxySimulationTimedOut = cmdReader.SerializePropertyBool();
                            break;
                        case "bIsDefaultCharacter":
                            playerPawn.bIsDefaultCharacter = cmdReader.SerializePropertyBool();
                            break;
                        case "PetState":
                            playerPawn.PetState = cmdReader.SerializePropertyObject();
                            break;
                        case "PetSkin":
                            playerPawn.PetSkin = cmdReader.SerializePropertyObject();
                            break;
                        case "bProxyIsJumpForceApplied":
                            playerPawn.bProxyIsJumpForceApplied = cmdReader.SerializePropertyBool();
                            break;
                        case "BuildingState":
                            playerPawn.BuildingState = cmdReader.SerializePropertyEnum(3);
                            break;
                        case "CurrentMovementStyle":
                            playerPawn.CurrentMovementStyle = cmdReader.SerializePropertyEnum(5);
                            break;
                        case "bIsCrouched":
                            playerPawn.bIsCrouched = cmdReader.SerializePropertyBool();
                            break;
                        case "bWeaponHolstered":
                            playerPawn.bWeaponHolstered = cmdReader.SerializePropertyBool();
                            break;
                        case "PawnMontage":
                            playerPawn.PawnMontage = cmdReader.SerializePropertyObject();
                            break;
                        case "bPlayBit":
                            playerPawn.bPlayBit = cmdReader.SerializePropertyBool();
                            break;
                        case "WeaponActivated":
                            playerPawn.WeaponActivated = cmdReader.SerializePropertyBool();
                            break;
                        case "bIsTargeting":
                            playerPawn.bIsTargeting = cmdReader.SerializePropertyBool();
                            break;
                        case "PackedReplicatedSlopeAngles":
                            break;
                        case "bIsSlopeSliding":
                            playerPawn.bIsSlopeSliding = cmdReader.SerializePropertyBool();
                            break;
                        case "bIsInsideSafeZone":
                            playerPawn.bIsInsideSafeZone = cmdReader.SerializePropertyBool();
                            break;
                        case "bCanQue":
                            playerPawn.bCanQue = cmdReader.SerializePropertyBool();
                            break;
                        case "bCanBeInterrupted":
                            playerPawn.bCanBeInterrupted = cmdReader.SerializePropertyBool();
                            break;
                        case "bInterruptCurrentLine":
                            playerPawn.bInterruptCurrentLine = cmdReader.SerializePropertyBool();
                            break;
                        case "bIsPlayingEmote":
                            playerPawn.bIsPlayingEmote = cmdReader.SerializePropertyBool();
                            break;
                        case "LastReplicatedEmoteExecuted":
                            playerPawn.LastReplicatedEmoteExecuted = cmdReader.SerializePropertyObject();
                            break;
                        case "SkipPositionCorrection":
                            playerPawn.SkipPositionCorrection = cmdReader.SerializePropertyBool();
                            break;
                        case "Location":
                            playerPawn.Location = cmdReader.SerializeRepMovement();
                            break;
                        case "Position":
                            playerPawn.Position = cmdReader.ReadUInt32();
                            break;
                        case "NextSectionID":
                            cmdReader.SerializePropertyByte();
                            break;
                        case "JumpFlashCountPacked":
                            cmdReader.SerializePropertyByte();
                            break;
                        case "LandingFlashCountPacked":
                            cmdReader.SerializePropertyByte();
                            break;
                        case "FastReplicationMinimalReplicationTags":
                            break;
                        case "Dances":
                            playerPawn.Dances = ReceivePropertiesArray(cmdReader, group, channelIndex);
                            break;
                        case "ItemWraps":
                            playerPawn.ItemWraps = ReceivePropertiesArray(cmdReader, group, channelIndex);
                            break;
                        case "Role":
                        case "RemoteRole":
                            break;
                        default:
                            Debug("playerpawn-fields", $"{export.Name}\t{numBits}");
                            _logger.LogDebug($"unknown field {export.Name} in playerpawn");
                            break;
                    }
                }


                // /Script/FortniteGame.FortTeamPrivateInfo
                // /Script/FortniteGame.GameMemberInfo

                // /Script/FortniteGame.ActiveGameplayModifier

                // NetworkGameplayTagNodeIndex
                // /Script/FortniteGame.FortPoiManager

                // /Game/Athena/Playlists/GameplayMutators/Mutator_SpecialEvent.Mutator_SpecialEvent_C
                // /Game/Athena/Playlists/Respawn/SifMutator_HealthAndShieldSet.SifMutator_HealthAndShieldSet_C


                // /Game/Weapons/FORT_Sniper/Blueprints/B_Prj_Bullet_Sniper_Heavy.B_Prj_Bullet_Sniper_Heavy_C
                // /Game/Weapons/FORT_Pistols/Blueprints/B_Pistol_Light_PDW_Athena.B_Pistol_Light_PDW_Athena_C
                // /Game/Weapons/FORT_Rifles/Blueprints/Assault/B_Assault_Auto_Athena.B_Assault_Auto_Athena_C
                // /Game/Weapons/FORT_Shotguns/Blueprints/B_Shotgun_Standard_Athena.B_Shotgun_Standard_Athena_C


                // /Game/Building/ActorBlueprints/Player/Wood/L1/PBWA_W1_Floor.PBWA_W1_Floor_C
                // /Game/Building/ActorBlueprints/Player/Stone/L1/PBWA_S1_Floor.PBWA_S1_Floor_C

                // /Game/Building/ActorBlueprints/Player/Wood/L1/PBWA_W1_Solid.PBWA_W1_Solid_C

                // /Game/Building/ActorBlueprints/Player/Wood/L1/PBWA_W1_StairW.PBWA_W1_StairW_C
                // /Game/Building/ActorBlueprints/Player/Metal/L1/PBWA_M1_StairW.PBWA_M1_StairW_C
                //A int32   32
                //B int32   32
                //C int32   32
                //D int32   32
                //OwnerPersistentID int32   32
                //bEditorPlaced uint8   1
                //bPlayerPlaced uint8   1
                //Team TEnumAsByte<EFortTeam::Type > 7
                //BuildTime FQuantizedBuildingAttribute 16
                //RepairTime FQuantizedBuildingAttribute 16
                //Health int16   16
                //MaxHealth int16   16



                // /Game/Building/ActorBlueprints/Player/Metal/L1/PBWA_M1_DoorC.PBWA_M1_DoorC_C
                // /Game/Athena/BuildingActors/Prop/Athena_Soccerball.Athena_Soccerball_C

                else
                {
                    cmdReader.ReadBits(numBits);
                }

                if (cmdReader.IsError)
                {
                    _logger?.LogError($"Property {export.Name} caused error when reading (bits: {numBits}, group: {group.PathName})");
                    continue;
                }

                if (!cmdReader.AtEnd())
                {
                    // allow until we figured out how this works
                    _logger?.LogWarning($"Property {export.Name} didnt read proper number of bits: {cmdReader.GetBitsLeft()} out of {numBits}");
                    continue;

                    //_logger?.LogError("Property didn't read proper number of bits.");
                    //return;
                    //return false;
                }

                // RepLayout 3139
                // Find this property
                // const int32 CmdIndex = FindCompatibleProperty(CmdStart, CmdEnd, Checksum);
                // const FRepLayoutCmd& Cmd = Cmds[CmdIndex];
            }

            if (group.PathName == "/Game/Athena/PlayerPawn_Athena.PlayerPawn_Athena_C")
            {
                PlayerPawns[channelIndex].Add(playerPawn);
            }

            if (group.PathName == "/Script/FortniteGame.FortPlayerStateAthena")
            {
                ActorStates[channelIndex].Add(playerState);
            }
        }

        // see UObjectBaseUtility
        private string RemoveAllPathPrefixes(string path)
        {
            path = RemovePathPrefix(path, "Default__");

            if (path.Contains("."))
            {
                var index = path.IndexOf(".");
                path = path.Remove(0, index + 1);
            }
            return path;
        }

        private string RemovePathPrefix(string path, string toRemove)
        {
            if (path.Contains(toRemove))
            {
                var index = path.IndexOf(toRemove);
                path = path.Remove(index, toRemove.Length);
            }
            return path;
        }

        private string RemovePathSuffix(string path)
        {
            return Regex.Replace(path, @"(_?[0-9]+)+$", "");
        }

        private string RemovePathSuffix(string path, string toRemove)
        {
            return Regex.Replace(path, $@"{toRemove}$", "");
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L3579
        /// </summary>
        /// <param name="archive"></param>
        /// <returns></returns>
        public virtual bool ReadFieldHeaderAndPayload(FBitArchive bunch, NetFieldExportGroup group, out NetFieldExport outField, out FBitArchive reader)
        {
            if (bunch.AtEnd())
            {
                reader = null;
                outField = null;
                return false;
            }

            // const int32 NetFieldExportHandle = Bunch.ReadInt(FMath::Max(NetFieldExportGroup->NetFieldExports.Num(), 2));
            var netFieldExportHandle = bunch.ReadSerializedInt(Math.Max((int)group.NetFieldExportsLength, 2));
            if (bunch.IsError)
            {
                reader = null;
                outField = null;
                _logger?.LogError("ReadFieldHeaderAndPayload: Error reading NetFieldExportHandle.");
                return false;
            }

            // const FNetFieldExport& NetFieldExport = NetFieldExportGroup->NetFieldExports[NetFieldExportHandle];
            outField = group.NetFieldExports[(int)netFieldExportHandle];

            var numPayloadBits = bunch.ReadIntPacked();
            if (bunch.IsError)
            {
                reader = null;
                outField = null;
                _logger?.LogError("ReadFieldHeaderAndPayload: Error reading numbits.");
                return false;
            }

            reader = new BitReader(bunch.ReadBits(numPayloadBits));
            if (bunch.IsError)
            {
                _logger?.LogError($"ReadFieldHeaderAndPayload: Error reading payload. Bunch: {bunchIndex}, OutField: {netFieldExportHandle}");
                return false;
            }

            // More to read
            return true;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L3391
        /// </summary>
        public virtual uint ReadContentBlockPayload(DataBunch bunch, out bool bOutHasRepLayout, out FBitArchive reader)
        {
            // Read the content block header and payload
            var repObj = ReadContentBlockHeader(bunch, out bOutHasRepLayout, out bool bObjectDeleted);

            if (bObjectDeleted)
            {
                // Nothing else in this block, continue on
                reader = null;
                return 0;
            }

            var numPayloadBits = bunch.Archive.ReadIntPacked();
            if (numPayloadBits == 0)
            {
                _logger?.LogWarning($"ReadContentBlockPayload found payload of 0, bunch: {bunch.ChIndex}");
                reader = null;
                return 0;
            }

            reader = new BitReader(bunch.Archive.ReadBits(numPayloadBits));
            return repObj;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L3175
        /// </summary>
        public virtual uint ReadContentBlockHeader(DataBunch bunch, out bool bOutHasRepLayout, out bool bObjectDeleted)
        {
            bObjectDeleted = false;
            bOutHasRepLayout = bunch.Archive.ReadBit();
            var bIsActor = bunch.Archive.ReadBit();
            if (bIsActor)
            {
                // If this is for the actor on the channel, we don't need to read anything else
                // TODO fix this
                return Channels[bunch.ChIndex].Actor.Archetype?.Value ?? Channels[bunch.ChIndex].Actor.ActorNetGUID.Value;
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

            // Serialize the class in case we have to spawn it.
            var classNetGUID = InternalLoadObject(bunch.Archive, false);

            if (classNetGUID == null || !classNetGUID.IsValid())
            {
                // TODO not sure if we ever reach here...
                _logger?.LogDebug("[!!!!] classnetguid not valid");
                bObjectDeleted = true;
                return 0;
            }

            // TODO figure this out
            return classNetGUID.Value;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L1007
        /// </summary>
        /// <param name="packet"></param>
        public virtual void ReceivedRawPacket(PlaybackPacket packet)
        {
            var lastByte = packet.Data[^1];

            if (lastByte != 0)
            {

                var bitSize = (packet.Data.Length * 8) - 1;

                // Bit streaming, starts at the Least Significant Bit, and ends at the MSB.
                while (!((lastByte & 0x80) >= 1))
                {
                    lastByte *= 2;
                    bitSize--;
                }

                var bitArchive = new BitReader(packet.Data, bitSize)
                {
                    EngineNetworkVersion = Replay.Header.EngineNetworkVersion,
                    NetworkVersion = Replay.Header.NetworkVersion,
                    ReplayHeaderFlags = Replay.Header.Flags
                };
                try
                {
                    ReceivedPacket(bitArchive);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"failed ReceivedPacket, index: {packetIndex}");
                }
            }
            else
            {
                _logger?.LogError("Malformed packet: Received packet with 0's in last byte of packet");
                throw new MalformedPacketException("Malformed packet: Received packet with 0's in last byte of packet");
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L3352
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L1525
        /// </summary>
        /// <param name="bitReader"><see cref="Core.BitReader"/></param>
        /// <param name="packet"><see cref="PlaybackPacket"/></param>
        public virtual void ReceivedPacket(FBitArchive bitReader)
        {
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L5101
            // InternalAck always true!

            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L1669
            const int OLD_MAX_ACTOR_CHANNELS = 10240;

            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#1549
            InPacketId++;

            //var rejectedChannels = new Dictionary<uint, uint>();
            while (!bitReader.AtEnd())
            {
                // For demo backwards compatibility, old replays still have this bit
                if (bitReader.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_ACKS_INCLUDED_IN_HEADER)
                {
                    var isAckDummy = bitReader.ReadBit();
                }

                // FInBunch
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataBunch.cpp#L18
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Public/Net/DataBunch.h#L168
                var bunch = new DataBunch();

                var bControl = bitReader.ReadBit();
                bunch.PacketId = InPacketId;
                bunch.bOpen = bControl ? bitReader.ReadBit() : false;
                bunch.bClose = bControl ? bitReader.ReadBit() : false;

                if (bitReader.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_CLOSE_REASON)
                {
                    bunch.bDormant = bunch.bClose ? bitReader.ReadBit() : false;
                    bunch.CloseReason = bunch.bDormant ? ChannelCloseReason.Dormancy : ChannelCloseReason.Destroyed;
                }
                else
                {
                    bunch.CloseReason = bunch.bClose ? (ChannelCloseReason)bitReader.ReadSerializedInt((int)ChannelCloseReason.MAX) : ChannelCloseReason.Destroyed;
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
                    //Bunch.ChSequence = InReliable[Bunch.ChIndex] + 1;

                    if (!InReliable.ContainsKey(bunch.ChIndex))
                    {
                        InReliable.Add(bunch.ChIndex, 0);
                    }
                    bunch.ChSequence = InReliable[bunch.ChIndex] + 1;
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

                bunch.bPartialInitial = bunch.bPartial ? bitReader.ReadBit() : false;
                bunch.bPartialFinal = bunch.bPartial ? bitReader.ReadBit() : false;

                var chType = ChannelType.None;
                var chName = "";

                if (bitReader.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES)
                {
                    var type = bitReader.ReadSerializedInt((int)ChannelType.MAX);
                    chType = (bunch.bReliable || bunch.bOpen) ? (ChannelType)type : ChannelType.None;

                    if (chType == ChannelType.Control)
                    {
                        chName = ChannelName.Control.ToString();
                    }
                    else if (chType == ChannelType.Voice)
                    {
                        chName = ChannelName.Voice.ToString();
                    }
                    else if (chType == ChannelType.Actor)
                    {
                        chName = ChannelName.Actor.ToString();
                    }
                }
                else
                {
                    if (bunch.bReliable || bunch.bOpen)
                    {
                        //chName = UPackageMap::StaticSerializeName(Reader, Bunch.ChName);
                        try
                        {
                            chName = StaticParseName(bitReader);
                        }
                        catch
                        {
                            _logger.LogError("Channel name serialization failed.");
                            return;
                        }

                        if (chName.Equals(ChannelName.Control.ToString()))
                        {
                            chType = ChannelType.Control;
                        }
                        else if (chName.Equals(ChannelName.Voice.ToString()))
                        {
                            chType = ChannelType.Voice;
                        }
                        else if (chName.Equals(ChannelName.Actor.ToString()))
                        {
                            chType = ChannelType.Actor;
                        }
                    }
                }
                bunch.ChType = chType;
                bunch.ChName = chName;

                // UChannel* Channel = Channels[Bunch.ChIndex];
                var channel = Channels.ContainsKey(bunch.ChIndex);

                // If there's an existing channel and the bunch specified it's channel type, make sure they match.
                // Channel && (Bunch.ChName != NAME_None) && (Bunch.ChName != Channel->ChName)

                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L83
                var maxPacket = 1024 * 2;
                var bunchDataBits = bitReader.ReadSerializedInt(maxPacket * 8);
                // Bunch.SetData( Reader, BunchDataBits );
                bunch.Archive = new BitReader(bitReader.ReadBits(bunchDataBits))
                {
                    EngineNetworkVersion = bitReader.EngineNetworkVersion,
                    NetworkVersion = bitReader.NetworkVersion,
                    ReplayHeaderFlags = bitReader.ReplayHeaderFlags
                };
                bunchIndex++;

                // debugging
                Debug("bunchsizes", $"packetIndex: {packetIndex}, bunchIndex: {bunchIndex - 1}, size: {bunchDataBits}");
                bunch.Archive.Mark();
                var bits = bunch.Archive.ReadBits(bunch.Archive.GetBitsLeft());
                byte[] ret = new byte[(int)Math.Ceiling(bits.Length / 8.0)];
                for (int i = 0; i < bits.Length; i += 8)
                {
                    int value = 0;
                    for (int j = 0; j < 8; j++)
                    {
                        if (i + j < bits.Length)
                        {
                            if (bits[i + j])
                            {
                                value += 1 << (7 - j);
                            }
                        }
                    }
                    ret[i / 8] = (byte)value;
                }
                Debug($"bunch-{bunchIndex}-{bunch.ChIndex}-{bunch.ChName}", "bunches", ret);
                bunch.Archive.Pop();

                if (bunch.bHasPackageMapExports)
                {
                    // Driver->NetGUIDInBytes += (BunchDataBits + (HeaderPos - IncomingStartPos)) >> 3 ??
                    // Cast<UPackageMapClient>( PackageMap )->ReceiveNetGUIDBunch( Bunch );
                    ReceiveNetGUIDBunch(bunch.Archive);
                }

                // Can't handle other channels until control channel exists.
                //if (!Channels.ContainsKey(bunch.ChIndex) && (bunch.ChIndex != 0 || bunch.ChName != ChannelName.Control.ToString()))
                //{
                //    if (!Channels.ContainsKey(0))
                //    {
                //        return;
                //    }
                //}

                // ignore control channel close if it hasn't been opened yet
                //if (bunch.ChIndex == 0 && !Channels.ContainsKey(0) && bunch.bClose && bunch.ChName == ChannelName.Control)
                //{
                //    return;
                //}

                // We're on a 100% reliable connection and we are rolling back some data.
                // In that case, we can generally ignore these bunches.
                // if (InternalAck && Channel && bIgnoreAlreadyOpenedChannels)
                // bIgnoreAlreadyOpenedChannels always true?  https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L4393
                if (channel && false)
                {
                    var bNewlyOpenedActorChannel = bunch.bOpen && (bunch.ChName == ChannelName.Actor.ToString()) && (!bunch.bPartial || bunch.bPartialInitial);
                    if (bNewlyOpenedActorChannel)
                    {
                        // GetActorGUIDFromOpenBunch(Bunch);
                        if (bunch.bHasMustBeMappedGUIDs)
                        {
                            var numMustBeMappedGUIDs = bunch.Archive.ReadUInt16();
                            for (var i = 0; i < numMustBeMappedGUIDs; i++)
                            {
                                // FNetworkGUID NetGUID
                                var guid = bunch.Archive.ReadIntPacked();
                            }
                        }

                        //FNetworkGUID ActorGUID;
                        var actorGuid = bunch.Archive.ReadIntPacked();
                        IgnoringChannels.TryAdd(bunch.ChIndex, actorGuid);
                    }

                    if (IgnoringChannels.ContainsKey(bunch.ChIndex))
                    {
                        if (bunch.bClose && (!bunch.bPartial || bunch.bPartialFinal))
                        {
                            //FNetworkGUID ActorGUID = IgnoringChannels.FindAndRemoveChecked(Bunch.ChIndex);
                            IgnoringChannels.Remove(bunch.ChIndex, out var actorguid);
                        }
                        continue;
                    }
                }

                // Ignore if reliable packet has already been processed.
                if (bunch.bReliable && InReliable.ContainsKey(bunch.ChIndex) && bunch.ChSequence <= InReliable[bunch.ChIndex])
                {
                    continue;
                }

                // If opening the channel with an unreliable packet, check that it is "bNetTemporary", otherwise discard it
                //if (!Channel && !bunch.bReliable)
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
                    // Channel = CreateChannelByName(Bunch.ChName, EChannelCreateFlags::None, Bunch.ChIndex);

                    var newChannel = new UChannel()
                    {
                        ChannelName = bunch.ChName,
                        ChannelType = bunch.ChType,
                        ChannelIndex = bunch.ChIndex,
                    };

                    Channels.Add(bunch.ChIndex, newChannel);
                    // Notify the server of the new channel.
                    // if( !Driver->Notify->NotifyAcceptingChannel( Channel ) ) { continue; }
                }

                // Dispatch the raw, unsequenced bunch to the channel
                // Channel->ReceivedRawBunch( Bunch, bLocalSkipAck ); //warning: May destroy channel.
                try
                {
                    ReceivedRawBunch(bunch);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"failed ReceivedRawBunch, index: {bunchIndex}");
                }
            }

            if (!bitReader.AtEnd())
            {
                _logger?.LogWarning("Packet not fully read...");
            }

            // termination bit?
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L1170
        }


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Serialization/CompressedChunkInfo.cpp#L9
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Plugins/Runtime/PacketHandlers/CompressionComponents/Oodle/Source/OodleHandlerComponent/Private/OodleArchives.cpp#L21
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private Core.BinaryReader Decompress(FArchive archive, int size)
        {
            if (!Replay.Info.IsCompressed)
            {
                var uncompressed = new Core.BinaryReader(new MemoryStream(archive.ReadBytes(size)))
                {
                    EngineNetworkVersion = Replay.Header.EngineNetworkVersion,
                    NetworkVersion = Replay.Header.NetworkVersion,
                    ReplayHeaderFlags = Replay.Header.Flags,
                    ReplayVersion = Replay.Info.FileVersion
                };
                return uncompressed;
            }

            var decompressedSize = archive.ReadInt32();
            var compressedSize = archive.ReadInt32();
            var compressedBuffer = archive.ReadBytes(compressedSize);
            var output = Oodle.DecompressReplayData(compressedBuffer, compressedSize, decompressedSize);
            var decompressed = new Core.BinaryReader(new MemoryStream(output))
            {
                EngineNetworkVersion = Replay.Header.EngineNetworkVersion,
                NetworkVersion = Replay.Header.NetworkVersion,
                ReplayHeaderFlags = Replay.Header.Flags,
                ReplayVersion = Replay.Info.FileVersion
            };

            _logger?.LogInformation($"Decompressed archive from {compressedSize} to {decompressedSize}.");
            return decompressed;
        }
    }
}
