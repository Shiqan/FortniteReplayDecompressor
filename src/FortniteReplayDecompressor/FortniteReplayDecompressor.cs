using FortniteReplayReader;
using FortniteReplayReader.Core.Models;
using FortniteReplayReader.Core.Models.Enums;
using FortniteReplayReaderDecompressor.Core.Models;
using System;
using System.IO;

namespace FortniteReplayReaderDecompressor
{
    public class FortniteBinaryDecompressor : FortniteBinaryReader
    {
        public FortniteBinaryDecompressor(Stream input) : base(input)
        {
        }

        public FortniteBinaryDecompressor(Stream input, int offset) : base(input)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/CoreUObject/Private/UObject/CoreNet.cpp#L277
        /// </summary>
        public void StaticParseName()
        {
            var isHardcoded = ReadBoolean();
            if (isHardcoded)
            {
                uint nameIndex;
                if (Replay.Header.EngineNetworkVersionHistory < EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES)
                {
                    nameIndex = ReadUInt32();
                }
                else
                {
                    nameIndex = ReadIntPacked();
                }

                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.h#L31
                // hard coded names in "UnrealNames.inl"
            }
            else
            {
                string inString = ReadString();
                int inNumber = ReadInt32();
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/PackageMapClient.h#L64
        /// </summary>
        public virtual void ReadNetFieldExport()
        {
            var isExported = ReadBoolean();
            if (isExported)
            {
                var handle = ReadIntPacked();
                var checksum = ReadUInt32();


                // we dont know the header on checkpoint0 ...?
                // guess we can skip isLoading parts
                if (EngineNetworkVersionHistory.HISTORY_NEW_ACTOR_OVERRIDE_LEVEL < EngineNetworkVersionHistory.HISTORY_NETEXPORT_SERIALIZATION)
                {
                    string name = ReadString();
                    string type = ReadString();
                }
                else if (EngineNetworkVersionHistory.HISTORY_NEW_ACTOR_OVERRIDE_LEVEL < EngineNetworkVersionHistory.HISTORY_NETEXPORT_SERIALIZE_FIX)
                {
                    // fname
                    string exportname = ReadFString();
                }
                else
                {
                    StaticParseName();
                }
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/PackageMapClient.h#L133
        /// </summary>
        public virtual void ReadNetFieldExportGroupMap()
        {
            var pathName = ReadFString();
            var pathNameIndex = ReadIntPacked();
            var numNetFieldExports = ReadUInt32();
            for (int i = 0; i < numNetFieldExports; i++)
            {
                ReadNetFieldExport();
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1667
        /// </summary>
        public override void ParseCheckPoint()
        {
            string id = ReadFString();
            string group = ReadFString();
            string metadata = ReadFString();
            uint time1 = ReadUInt32();
            uint time2 = ReadUInt32();
            int eventSizeInBytes = ReadInt32();

            var offset = BaseStream.Position;
            using (var reader = Decompress())
            {
                // SerializeDeletedStartupActors
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1916
                var packetOffset = reader.ReadInt64();
                var levelForCheckpoint = reader.ReadInt32();
                var deletedNetStartupActors = reader.ReadArray(reader.ReadFString);

                // SerializeGuidCache
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1591
                var count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    uint guid = reader.ReadIntPacked();
                    uint outerGuid = reader.ReadIntPacked();
                    string path = reader.ReadFString();
                    uint checksum = reader.ReadUInt32();
                    byte flags = reader.ReadByte();
                }

                // SerializeNetFieldExportGroupMap 
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1289
                var numNetFieldExportGroups = reader.ReadUInt32();
                for (int i = 0; i < numNetFieldExportGroups; i++)
                {
                    //reader.ReadNetFieldExportGroupMap();
                }

                var remainingBytes = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
                var output = reader.ReadBytes(remainingBytes);
                File.WriteAllBytes($"{id}.dump", output);

                // SerializeDemoFrameFromQueuedDemoPackets
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1978
            }
        }

        // fname
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Serialization/NameAsStringProxyArchive.h#L11

        // ReceiveNetFieldExports
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1497

        // ReadDemoFrameIntoPlaybackPackets
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L2848

        // SaveExternalData
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L2071

        // TickDemoRecordFrame
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L2324

        // ReadDemoFrameIntoPlaybackPackets
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L2848

        // WritePacket
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L3450

        // Serialize - might be useful...
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L5775


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L318
        /// </summary>
        public override void ParseReplayData()
        {
            if (Replay.Metadata.FileVersion >= ReplayVersionHistory.StreamChunkTimes)
            {
                var start = ReadUInt32();
                var end = ReadUInt32();
                var length = ReadUInt32();
            }

            var offset = BaseStream.Position;
            using (var reader = Decompress())
            {
                // now what?
                var remainingBytes = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
                var output = reader.ReadBytes(remainingBytes);
                File.WriteAllBytes($"replaydata-{offset}.dump", output);
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Plugins/Runtime/PacketHandlers/CompressionComponents/Oodle/Source/OodleHandlerComponent/Private/OodleArchives.cpp#L21
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private FortniteBinaryDecompressor Decompress()
        {
            var decompressedSize = ReadInt32();
            var compressedSize = ReadInt32();
            var compressedBuffer = ReadBytes(compressedSize);
            var output = Oodle.DecompressReplayData(compressedBuffer, compressedBuffer.Length, decompressedSize);
            var reader = new FortniteBinaryDecompressor(new MemoryStream(output));
            reader.BaseStream.Position = 0;
            return reader;
        }
    }
}
