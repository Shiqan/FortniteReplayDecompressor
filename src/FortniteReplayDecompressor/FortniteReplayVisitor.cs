using FortniteReplayReaderDecompressor.Core;
using FortniteReplayReaderDecompressor.Core.Contracts;
using FortniteReplayReaderDecompressor.Core.Exceptions;
using FortniteReplayReaderDecompressor.Core.Models;
using FortniteReplayReaderDecompressor.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace FortniteReplayReaderDecompressor
{
    public class FortniteReplayVisitor : ReplayVisitor
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

        public Replay Replay { get; private set; }

        // TODO: seperate visitor
        public Dictionary<string, dynamic> eventTypes = new Dictionary<string, dynamic>()
        {
            { ReplayEventTypes.PLAYER_ELIMINATION, new ReplayHeader() },
        };

        // TODO: split Unreal part
        public Dictionary<ReplayChunkType, dynamic> types = new Dictionary<ReplayChunkType, dynamic>()
        {
            { ReplayChunkType.Header, new ReplayHeader() },
            { ReplayChunkType.Checkpoint, new CheckpointInfo() },
            { ReplayChunkType.Event, new EventInfo() },
            { ReplayChunkType.ReplayData, new ReplayDataInfo() }
        };

        public override void ReadReplay(FArchive archive)
        {
            ReadReplayInfo(archive);
            ReadReplayChunks(archive);
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L282
        /// </summary>
        /// <param name="archive"></param>
        /// <returns></returns>
        public override CheckpointInfo ReadCheckpoint(FArchive archive)
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

            //using (var binaryArchive = Decompress(archive))
            //{
            //    // SerializeDeletedStartupActors
            //    // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1916

            //    if (binaryArchive.HasLevelStreamingFixes())
            //    {
            //        var packetOffset = binaryArchive.ReadInt64();
            //    }

            //    if (binaryArchive.NetworkVersion >= NetworkVersionHistory.HISTORY_MULTIPLE_LEVELS)
            //    {
            //        var levelForCheckpoint = binaryArchive.ReadInt32();
            //    }

            //    if (binaryArchive.NetworkVersion >= NetworkVersionHistory.HISTORY_DELETED_STARTUP_ACTORS)
            //    {
            //        var deletedNetStartupActors = binaryArchive.ReadArray(binaryArchive.ReadFString);
            //    }

            //    // SerializeGuidCache
            //    // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1591
            //    var count = binaryArchive.ReadInt32();
            //    for (var i = 0; i < count; i++)
            //    {
            //        var guid = binaryArchive.ReadIntPacked();
            //        var outerGuid = binaryArchive.ReadIntPacked();
            //        var path = binaryArchive.ReadFString();
            //        var checksum = binaryArchive.ReadUInt32();
            //        var flags = binaryArchive.ReadByte();
            //    }

            //    // SerializeNetFieldExportGroupMap 
            //    // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1289
            //    var numNetFieldExportGroups = binaryArchive.ReadUInt32();
            //    for (var i = 0; i < numNetFieldExportGroups; i++)
            //    {
            //        ReadNetFieldExportGroupMap(binaryArchive);
            //    }

            //    // SerializeDemoFrameFromQueuedDemoPackets
            //    // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1978
            //    var playbackPackets = ReadDemoFrameIntoPlaybackPackets(binaryArchive);
            //    foreach (var packet in playbackPackets)
            //    {
            //        if (packet.State == PacketState.Success)
            //        {
            //            ReceivedRawPacket(packet);
            //        }
            //    }
            //}

            return info;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L363
        /// </summary>
        /// <param name="archive"></param>
        /// <returns></returns>
        public override void ReadEvent(FArchive archive)
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

            //eventTypes[info.Metadata].Accept(this, archive);
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L243
        /// </summary>
        /// <param name="archive"></param>
        public override void ReadReplayChunks(FArchive archive)
        {
            while (!archive.AtEnd())
            {
                var chunkType = archive.ReadUInt32AsEnum<ReplayChunkType>();
                var chunkSize = archive.ReadInt32();
                var offset = archive.Position;

                types[chunkType].Accept(this, archive);

                if (archive.Position != offset + chunkSize)
                {
                    // log
                    archive.Seek(offset + chunkSize, SeekOrigin.Begin);
                }
            }
        }


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L318
        /// </summary> 
        /// <param name="archive"></param>
        public override ReplayDataInfo ReadReplayData(FArchive archive)
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

            //using (var binaryArchive = Decompress(archive))
            //{
            //    while (!binaryArchive.AtEnd())
            //    {
            //        var startPos = binaryArchive.BaseStream.Position;
            //        var playbackPackets = ReadDemoFrameIntoPlaybackPackets(binaryArchive);

            //        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L3338
            //        foreach (var packet in playbackPackets)
            //        {
            //            if (packet.State == PacketState.Success)
            //            {
            //                ReceivedRawPacket(packet);
            //            }
            //        }
            //    }
            //}

            return info;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/811c1ce579564fa92ecc22d9b70cbe9c8a8e4b9a/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L191
        /// </summary>
        /// <param name="archive"></param>
        /// <returns>ReplayHeader</returns>
        public override ReplayHeader ReadReplayHeader(FArchive archive)
        {
            var magic = archive.ReadUInt32();

            if (magic != NetworkMagic)
            {
                throw new InvalidReplayException($"Header.Magic != NETWORK_DEMO_MAGIC. Header.Magic: {magic}, NETWORK_DEMO_MAGIC: {NetworkMagic}");
            }

            var header = new ReplayHeader
            {
                NetworkVersion = archive.ReadUInt32AsEnum<NetworkVersionHistory>()
            };

            if (header.NetworkVersion <= NetworkVersionHistory.HISTORY_EXTRA_VERSION)
            {
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
            }

            header.GameSpecificData = archive.ReadArray(archive.ReadFString);

            return header;
        }


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L183
        /// </summary>
        /// <param name="archive"></param>
        /// <returns>ReplayInfo</returns>
        public override ReplayInfo ReadReplayInfo(FArchive archive)
        {
            var magicNumber = archive.ReadUInt32();

            if (magicNumber != FileMagic)
            {
                throw new InvalidReplayException("Invalid replay file");
            }

            var fileVersion = archive.ReadUInt32AsEnum<ReplayVersionHistory>();

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

            return info;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Serialization/CompressedChunkInfo.cpp#L9
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Plugins/Runtime/PacketHandlers/CompressionComponents/Oodle/Source/OodleHandlerComponent/Private/OodleArchives.cpp#L21
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private Core.BinaryReader Decompress(FArchive archive)
        {
            var decompressedSize = archive.ReadInt32();
            var compressedSize = archive.ReadInt32();
            var compressedBuffer = archive.ReadBytes(compressedSize);
            var output = Oodle.DecompressReplayData(compressedBuffer, compressedBuffer.Length, decompressedSize);
            var decompressed = new Core.BinaryReader(new MemoryStream(output))
            {
                EngineNetworkVersion = Replay.Header.EngineNetworkVersion,
                NetworkVersion = Replay.Header.NetworkVersion,
                ReplayHeaderFlags = Replay.Header.Flags,
                ReplayVersion = Replay.Info.FileVersion
            };
            return decompressed;
        }
    }
}
