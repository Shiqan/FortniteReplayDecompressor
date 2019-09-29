using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private Dictionary<uint, string> Channels = new Dictionary<uint, string>(); // TODO: UChannel
        private Dictionary<uint, uint> IgnoringChannels = new Dictionary<uint, uint>();
        private Dictionary<uint, string> NetGuidCache = new Dictionary<uint, string>();
        private Dictionary<uint, bool> ChannelActors = new Dictionary<uint, bool>();
        private Dictionary<string, NetFieldExportGroup> NetFieldExportGroupMap = new Dictionary<string, NetFieldExportGroup>();
        private Dictionary<uint, NetFieldExportGroup> NetFieldExportGroupIndexToGroup = new Dictionary<uint, NetFieldExportGroup>();

        // DemoNetDriver -> SeenLevelStatuses TODO: dont think we need it...

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

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L282
        /// </summary>
        /// <param name="archive"></param>
        /// <returns></returns>
        public virtual void ReadCheckpoint(FArchive archive)
        {
            var info = new CheckpointInfo
            {
                Id = archive.ReadFString(),
                Group = archive.ReadFString(),
                Metadata = archive.ReadFString(),
                StartTime = archive.ReadUInt32(),
                EndTime = archive.ReadUInt32(),
                SizeInBytes = archive.ReadInt32()
            };

            using (var binaryArchive = Decompress(archive))
            {
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
                        Debug($"checkpoint-{checkpointIndex}-packet-{packetIndex}", "packet", packet.Data);
                        packetIndex++;
                        ReceivedRawPacket(packet);
                    }
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

            using (var binaryArchive = Decompress(archive))
            {
                while (!binaryArchive.AtEnd())
                {
                    var startPos = binaryArchive.BaseStream.Position;
                    var playbackPackets = ReadDemoFrameIntoPlaybackPackets(binaryArchive);

                    // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L3338
                    foreach (var packet in playbackPackets)
                    {
                        if (packet.State == PacketState.Success)
                        {
                            //ReceivedRawPacket(packet);
                        }
                    }
                }
            }
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
                archive.Branch = header.Branch;
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

                Debug($"externaldata-{externalDataIndex}", "externaldata", externalData);
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
            for (var i = 0; i < numGuids; i++)
            {
                var size = archive.ReadInt32();
                InternalLoadObject(archive); // TODO: only burning data?
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1497
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
                    group = NetFieldExportGroupIndexToGroup[pathNameIndex];
                }

                var netField = ReadNetFieldExport(archive);

                // preserve compatibility flag
                netField.Incompatible = group.NetFieldExports.Where(i => i.Name.Equals(netField.Name))?.FirstOrDefault()?.Incompatible ?? netField.Incompatible;
                group.NetFieldExports = group.NetFieldExports.Replace(i => i.Name.Equals(netField.Name), netField).ToList(); // TODO MonkaS
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
            _logger?.LogInformation($"ReadDemoFrameIntoPlaybackPackets at  {timeSeconds}");

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
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L804
        /// </summary>
        public virtual NetworkGUID InternalLoadObject(FArchive archive)
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

            var flags = archive.ReadByteAsEnum<ExportFlags>();

            // outerguid
            if (flags == ExportFlags.bHasPath || flags == ExportFlags.bHasPathAndNetWorkChecksum || flags == ExportFlags.All)
            {
                var outerGuid = InternalLoadObject(archive);

                var pathName = archive.ReadFString();

                if (!NetGuidCache.ContainsKey(netGuid.Value))
                {
                    NetGuidCache.Add(netGuid.Value, pathName);
                }

                if (flags >= ExportFlags.bHasNetworkChecksum)
                {
                    var networkChecksum = archive.ReadUInt32();
                }

                return netGuid;
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
                InternalLoadObject(bitArchive);
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

                            PartialBunch.Archive.Mark();
                            var alignpartial = PartialBunch.Archive.GetBitsLeft() % 8;
                            if (alignpartial != 0)
                            {
                                var append = new bool[alignpartial];
                                for (var i = 0; i < alignpartial; i++)
                                {
                                    append[i] = false;
                                }
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

            if (bunch.bClose)
            {
                // We have fully received the bunch, so process it.
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
                // Initialize client if first time through.

                // SerializeNewActor
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L257
                var netGuid = InternalLoadObject(bunch.Archive);

                if (bunch.Archive.AtEnd() && netGuid.IsDynamic())
                {
                    return;
                }

                if (netGuid.IsDynamic())
                {
                    var archetypeNetGUID = InternalLoadObject(bunch.Archive);

                    // if (Ar.IsSaving() || (Connection && (Connection->EngineNetworkProtocolVersion >= EEngineNetworkVersionHistory::HISTORY_NEW_ACTOR_OVERRIDE_LEVEL)))
                    //if (bunch.Archive.EngineNetworkVersion >= EngineNetworkVersionHistory.HISTORY_NEW_ACTOR_OVERRIDE_LEVEL)
                    //{
                    //    InternalLoadObject(bunch.Archive);
                    //}

                    // FVector_NetQuantize10
                    // bSerializeLocation
                    if (bunch.Archive.ReadBit())
                    {
                        // Location.NetSerialize(Ar, this, SerSuccess);
                        var location = bunch.Archive.ReadPackedVector(10, 24);
                    }

                    // bSerializeRotation
                    if (bunch.Archive.ReadBit())
                    {
                        // Rotation.NetSerialize(Ar, this, SerSuccess);
                        var rotation = bunch.Archive.ReadPackedVector(10, 24);
                    }

                    // bSerializeScale
                    if (bunch.Archive.ReadBit())
                    {
                        // Scale.NetSerialize(Ar, this, SerSuccess);
                        var scale = bunch.Archive.ReadPackedVector(10, 24);
                    }

                    // bSerializeVelocity
                    if (bunch.Archive.ReadBit())
                    {
                        // Velocity.NetSerialize(Ar, this, SerSuccess);
                        var velocity = bunch.Archive.ReadPackedVector(10, 24);
                    }
                }

                //SetChannelActor(NewChannelActor);

                //NotifyActorChannelOpen(Actor, Bunch);

                //RepFlags.bNetInitial = true;

                ChannelActors.Add(bunch.ChIndex, true);
            }

            //RepFlags.bIgnoreRPCs = Bunch.bIgnoreRPCs;
            //RepFlags.bSkipRoleSwap = bSkipRoleSwap;

            //  Read chunks of actor content
            while (!bunch.Archive.AtEnd())
            {
                //FNetBitReader Reader(Bunch.PackageMap, 0 );
                //bool bHasRepLayout = false;
                var bHasRepLayout = ReadContentBlockPayload(bunch);

                if (bunch.Archive.AtEnd())
                {
                    // Nothing else in this block, continue on (should have been a delete or create block)
                    continue;
                }

                // if ( !Replicator->ReceivedBunch( Reader, RepFlags, bHasRepLayout, bHasUnmapped ) )
                ReceivedReplicatorBunch(bunch, bHasRepLayout);
            }
            // PostReceivedBunch, not interesting?
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/DataReplication.cpp#L896
        /// </summary>
        /// <param name="bunch"></param>
        public virtual void ReceivedReplicatorBunch(DataBunch bunch, bool bHasRepLayout)
        {
            // Handle replayout properties
            if (bHasRepLayout)
            {
                // TODO track bHasReplicatedProperties per channel?
                //if (!bHasReplicatedProperties)
                //{
                //    bHasReplicatedProperties = true;        // Persistent, not reset until PostNetReceive is called
                //    PreNetReceive();
                //}

                //if (Connection->Driver->ShouldReceiveRepNotifiesForObject(Object))
                //{
                //    ReceivePropFlags |= EReceivePropertiesFlags::RepNotifies;
                //}

                //if (RepFlags.bSkipRoleSwap)
                //{
                //    ReceivePropFlags |= EReceivePropertiesFlags::SkipRoleSwap;
                //}

                // if ( !RepLayout->ReceiveProperties(OwningChannel, ObjectClass, RepState.Get(), ( void* )Object, Bunch, bLocalHasUnmapped, bGuidsChanged, ReceivePropFlags ) )
                // RepLayout.cpp
                // FRepLayout::ReceiveProperties(
                //      ReceiveProperties_BackwardsCompatible
                //          ReceiveProperties_BackwardsCompatible_r
            }

            //FNetFieldExportGroup* NetFieldExportGroup = OwningChannel->GetNetFieldExportGroupForClassNetCache(ObjectClass);

            // Read fields from stream
            // const FFieldNetCache* FieldCache = nullptr;

            // TODO figure out where NetFieldExportGroup is coming from
            while (ReadFieldHeaderAndPayload(bunch, null))
            {
                //if (FieldCache == nullptr)
                //{
                //    UE_LOG(LogNet, Warning, TEXT("ReceivedBunch: FieldCache == nullptr: %s"), *Object->GetFullName());
                //    continue;
                //}

                //if (FieldCache->bIncompatible)
                //{
                //    // We've already warned about this property once, so no need to continue to do so
                //    UE_LOG(LogNet, Verbose, TEXT("ReceivedBunch: FieldCache->bIncompatible == true. Object: %s, Field: %s"), *Object->GetFullName(), *FieldCache->Field->GetFName().ToString());
                //    continue;
                //}


                // Handle property
                // if (UProperty * ReplicatedProp = Cast<UProperty>(FieldCache->Field))
                // {
                // We should only be receiving custom delta properties (since RepLayout handles the rest)
                //if (!Retirement[ReplicatedProp->RepIndex].CustomDelta)

                //// Call PreNetReceive if we haven't yet
                //if (!bHasReplicatedProperties)
                //{
                //    bHasReplicatedProperties = true;        // Persistent, not reset until PostNetReceive is called
                //    PreNetReceive();
                //}

                // // Receive array index (static sized array, i.e. MemberVariable[4])
                // bunch.Archive.ReadIntPacked();

                // Call the custom delta serialize function to handle it
                //CppStructOps->NetDeltaSerialize(Parms, Data);

                // Successfully received it.
                // }
                //else
                //{
                // Handle function call
                //Cast<UFunction>(FieldCache->Field)
                //}
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L3579
        /// </summary>
        /// <param name="bitReader"></param>
        /// <param name="bunch"></param>
        /// <returns></returns>
        public virtual bool ReadFieldHeaderAndPayload(DataBunch bunch, NetFieldExportGroup group)
        {
            if (bunch.Archive.AtEnd())
            {
                return false;
            }
            // const int32 NetFieldExportHandle = Bunch.ReadInt(FMath::Max(NetFieldExportGroup->NetFieldExports.Num(), 2));
            var netFieldExportHandle = bunch.Archive.ReadSerializedInt(Math.Max((int) group.NetFieldExportsLength, 2));

            // const FNetFieldExport& NetFieldExport = NetFieldExportGroup->NetFieldExports[NetFieldExportHandle];
            // var netfieldexport = group.NetFieldExports[(int) netFieldExportHandle];

            // *OutField = ClassCache->GetFromChecksum( NetFieldExport.CompatibleChecksum );
            var numPayloadBits = bunch.Archive.ReadIntPacked();
            // OutPayload.SetData( Bunch, NumPayloadBits );

            return true;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L3391
        /// </summary>
        public virtual bool ReadContentBlockPayload(DataBunch bunch)
        {
            //bool bObjectDeleted = false;
            //// Read the content block header and payload
            //UObject* RepObj = ReadContentBlockPayload(Bunch, Reader, bHasRepLayout);
            // sets bObjectDeleted and bOutHasRepLayout

            var bOutHasRepLayout = ReadContentBlockHeader(bunch);

            //if (bObjectDeleted)
            //{
            //    OutPayload.SetData(Bunch, 0);

            //    // Nothing else in this block, continue on
            //    return nullptr;
            //}

            var numPayloadBits = bunch.Archive.ReadIntPacked();
            //OutPayload.SetData(Bunch, NumPayloadBits);
            //return RepObj;

            return bOutHasRepLayout;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L3175
        /// </summary>
        public virtual bool ReadContentBlockHeader(DataBunch bunch)
        {
            //  bool& bObjectDeleted, bool& bOutHasRepLayout 
            //var bObjectDeleted = false;
            var bOutHasRepLayout = bunch.Archive.ReadBit();
            var bIsActor = bunch.Archive.ReadBit();
            if (bIsActor)
            {
                // If this is for the actor on the channel, we don't need to read anything else
                return bOutHasRepLayout;
            }

            // We need to handle a sub-object
            // Manually serialize the object so that we can get the NetGUID (in order to assign it if we spawn the object here)
            var netGuid = InternalLoadObject(bunch.Archive);

            var bStablyNamed = bunch.Archive.ReadBit();
            if (bStablyNamed)
            {
                // If this is a stably named sub-object, we shouldn't need to create it. Don't raise a bunch error though because this may happen while a level is streaming out.
                return bOutHasRepLayout;
            }

            // Serialize the class in case we have to spawn it.
            var classNetGUID = InternalLoadObject(bunch.Archive);

            //if (!classNetGUID.IsValid())
            //{
            //    bObjectDeleted = true;
            //}

            return bOutHasRepLayout;
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
                    _logger?.LogError(ex, $"failed ReceivedPacket");
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

            var rejectedChannels = new Dictionary<uint, uint>();
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

                    if (InReliable.ContainsKey(bunch.ChIndex))
                    {
                        bunch.ChSequence = InReliable[bunch.ChIndex] + 1;
                    }
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
                bunch.Archive.Mark();
                var align = bunch.Archive.GetBitsLeft() % 8;
                if (align != 0)
                {
                    var append = new bool[align];
                    for (var i = 0; i < align; i++)
                    {
                        append[i] = false;
                    }
                    bunch.Archive.AppendDataFromChecked(append);
                }
                Debug($"bunch-{bunchIndex}-{bunch.ChIndex}-{bunch.ChName}", "bunches", bunch.Archive.ReadBytes(bunch.Archive.GetBitsLeft() / 8));
                bunch.Archive.Pop();

                if (bunch.bHasPackageMapExports)
                {
                    // Driver->NetGUIDInBytes += (BunchDataBits + (HeaderPos - IncomingStartPos)) >> 3 ??
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
                //if (Channel)
                //{
                //    var bNewlyOpenedActorChannel = bunch.bOpen && (chName == ChannelName.Actor) && (!bunch.bPartial || bunch.bPartialInitial);
                //    if (bNewlyOpenedActorChannel)
                //    {
                //          // GetActorGUIDFromOpenBunch(Bunch);
                //        if (bunch.bHasMustBeMappedGUIDs)
                //        {
                //            var numMustBeMappedGUIDs = bunchReader.ReadUInt16();
                //            for (var i = 0; i < numMustBeMappedGUIDs; i++)
                //            {
                //                // FNetworkGUID NetGUID
                //                var guid = bunch.Archive.ReadIntPacked();
                //            }
                //        }

                //        //FNetworkGUID ActorGUID;
                //        var actorGuid = bunchReader.ReadIntPacked();
                //        IgnoringChannels.Add(bunch.ChIndex, actorGuid);
                //    }

                //    if (IgnoringChannels.ContainsKey(bunch.ChIndex))
                //    {
                //        if (bunch.bClose && (!bunch.bPartial || bunch.bPartialFinal))
                //        {
                //            //FNetworkGUID ActorGUID = IgnoringChannels.FindAndRemoveChecked(Bunch.ChIndex);
                //            IgnoringChannels.Remove(bunch.ChIndex, out var actorguid);
                //        }
                //        continue;
                //    }
                //}

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
                    if (rejectedChannels.ContainsKey(bunch.ChIndex))
                    {
                        _logger?.LogDebug($"Ignoring Bunch for ChIndex {bunch.ChIndex}, as the channel was already rejected while processing this packet.");
                        continue;
                    }

                    //if (!Driver->IsKnownChannelName(Bunch.ChName))
                    //{
                    //    CLOSE_CONNECTION_DUE_TO_SECURITY_VIOLATION
                    //}

                    // Reliable (either open or later), so create new channel.
                    // Channel = CreateChannelByName(Bunch.ChName, EChannelCreateFlags::None, Bunch.ChIndex);
                    Channels.Add(bunch.ChIndex, bunch.ChName.ToString());
                    // Notify the server of the new channel.
                    // if( !Driver->Notify->NotifyAcceptingChannel( Channel ) ) { continue; }
                }

                // Dispatch the raw, unsequenced bunch to the channel
                // Channel->ReceivedRawBunch( Bunch, bLocalSkipAck ); //warning: May destroy channel.
                ReceivedRawBunch(bunch);
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
        private Core.BinaryReader Decompress(FArchive archive)
        {
            if (!Replay.Info.IsCompressed)
            {
                return archive as Core.BinaryReader;
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
