using FortniteReplayReader;
using FortniteReplayReader.Core.Models;
using FortniteReplayReader.Core.Models.Enums;
using FortniteReplayReaderDecompressor.Core.Models;
using FortniteReplayReaderDecompressor.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace FortniteReplayReaderDecompressor
{
    public class FortniteBinaryDecompressor : FortniteBinaryReader
    {
        private int index = 0;
        private int externalDataIndex = 0;

        public FortniteBinaryDecompressor(Stream input) : base(input)
        {
        }

        public FortniteBinaryDecompressor(Stream input, Replay replay) : base(input)
        {
            this.Replay = replay;
        }

        public FortniteBinaryDecompressor(Stream input, int offset) : base(input)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/CoreUObject/Private/UObject/CoreNet.cpp#L277
        /// </summary>
        public virtual string StaticParseName()
        {
            throw new NotSupportedException("This function should be using a BitReader...");
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/CoreUObject/Private/UObject/CoreNet.cpp#L277
        /// </summary>
        public virtual string StaticParseName(BitReader bitReader)
        {
            var isHardcoded = bitReader.ReadBit();
            if (isHardcoded)
            {
                uint nameIndex;
                if (Replay.Header.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES)
                {
                    nameIndex = bitReader.ReadUInt32();
                }
                else
                {
                    nameIndex = bitReader.ReadIntPacked();
                }

                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.h#L31
                // hard coded names in "UnrealNames.inl"
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.inl
                return nameIndex.ToString();
            }

            var inString = bitReader.ReadFString();
            var inNumber = bitReader.ReadInt32();
            return inString;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/PackageMapClient.h#L64
        /// </summary>
        public virtual NetFieldExport ReadNetFieldExport(BitReader bitReader)
        {
            var isExported = bitReader.ReadUInt32() == 1u;
            if (isExported)
            {
                var fieldExport = new NetFieldExport()
                {
                    Handle = bitReader.ReadIntPacked(),
                    CompatibleChecksum = bitReader.ReadUInt32()
                };

                if (Replay.Header.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_NETEXPORT_SERIALIZATION)
                {
                    fieldExport.Name = bitReader.ReadFString();
                    fieldExport.Type = bitReader.ReadFString();
                }
                else if (Replay.Header.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_NETEXPORT_SERIALIZE_FIX)
                {
                    // FName
                    fieldExport.Name = bitReader.ReadFString();
                }
                else
                {
                    fieldExport.Name = StaticParseName(bitReader);
                }

                return fieldExport;
            }

            return null;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/PackageMapClient.h#L64
        /// </summary>
        public virtual NetFieldExport ReadNetFieldExport()
        {
            var isExported = ReadBoolean();
            if (isExported)
            {
                var fieldExport = new NetFieldExport()
                {
                    Handle = ReadIntPacked(),
                    CompatibleChecksum = ReadUInt32()
                };

                if (Replay.Header.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_NETEXPORT_SERIALIZATION)
                {
                    fieldExport.Name = ReadFString();
                    fieldExport.Type = ReadFString();
                }
                else if (Replay.Header.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_NETEXPORT_SERIALIZE_FIX)
                {
                    // FName
                    fieldExport.Name = ReadFString();
                }
                else
                {
                    fieldExport.Name = StaticParseName();
                }

                return fieldExport;
            }

            return null;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/PackageMapClient.h#L133
        /// </summary>
        public virtual NetFieldExportGroup ReadNetFieldExportGroupMap()
        {
            var group = new NetFieldExportGroup()
            {
                PathName = ReadFString(),
                PathNameIndex = ReadIntPacked(),
                NetFieldExportsLenght = ReadIntPacked(),
                NetFieldExports = new List<NetFieldExport>()
            };

            for (var i = 0; i < group.NetFieldExportsLenght; i++)
            {
                var netFieldExport = ReadNetFieldExport();
                if (netFieldExport != null)
                {
                    group.NetFieldExports.Add(netFieldExport);
                }
            }

            return group;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L2848
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<PlaybackPacket> ReadDemoFrameIntoPlaybackPackets()
        {
            Console.WriteLine("ReadDemoFrameIntoPlaybackPackets...");
            if (Replay.Header.NetworkVersion >= NetworkVersionHistory.HISTORY_MULTIPLE_LEVELS)
            {
                var currentLevelIndex = ReadInt32();
            }
            var timeSeconds = ReadSingle();
            Console.WriteLine($"Time: {timeSeconds}...");

            if (Replay.Header.NetworkVersion >= NetworkVersionHistory.HISTORY_LEVEL_STREAMING_FIXES)
            {
                ReadExportData();
            }

            if (Replay.Header.HasLevelStreamingFixes())
            {
                var numStreamingLevels = ReadIntPacked();
                for (var i = 0; i < numStreamingLevels; i++)
                {
                    var levelName = ReadFString();
                }
            }
            else
            {
                var numStreamingLevels = ReadIntPacked();
                for (var i = 0; i < numStreamingLevels; i++)
                {
                    var packageName = ReadFString();
                    var packageNameToLoad = ReadFString();
                    // FTransform
                    //var levelTransform = reader.ReadFString();
                    // filter duplicates
                }
            }

            if (Replay.Header.HasLevelStreamingFixes())
            {
                var externalOffset = ReadUInt64();
            }

            // if (!bForLevelFastForward)
            ReadExternalData();
            // else skip externalOffset

            var playbackPackets = new List<PlaybackPacket>();
            var @continue = true;
            while (@continue)
            {
                if (Replay.Header.HasLevelStreamingFixes())
                {
                    var seenLevelIndex = ReadIntPacked();
                }

                var packet = ReadPacket();
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

        public void Debug(string name)
        {
            var remainingBytes = (int)(BaseStream.Length - BaseStream.Position);
            var output = ReadBytes(remainingBytes);
            File.WriteAllBytes($"{name}.dump", output);
            BaseStream.Position -= remainingBytes;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1667
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L4503
        /// </summary>
        public override void ParseCheckPoint()
        {
            Console.WriteLine("ParseCheckPoint...");
            var id = ReadFString();
            var group = ReadFString();
            var metadata = ReadFString();
            var time1 = ReadUInt32();
            var time2 = ReadUInt32();
            var eventSizeInBytes = ReadInt32();

            var offset = BaseStream.Position;
            using (var reader = Decompress())
            {
                // SerializeDeletedStartupActors
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1916

                if (Replay.Header.HasLevelStreamingFixes())
                {
                    var packetOffset = reader.ReadInt64();
                }

                if (Replay.Header.NetworkVersion >= NetworkVersionHistory.HISTORY_MULTIPLE_LEVELS)
                {
                    var levelForCheckpoint = reader.ReadInt32();
                }

                if (Replay.Header.NetworkVersion >= NetworkVersionHistory.HISTORY_DELETED_STARTUP_ACTORS)
                {
                    var deletedNetStartupActors = reader.ReadArray(reader.ReadFString);
                }

                // SerializeGuidCache
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1591
                var count = reader.ReadInt32();
                for (var i = 0; i < count; i++)
                {
                    var guid = reader.ReadIntPacked();
                    var outerGuid = reader.ReadIntPacked();
                    var path = reader.ReadFString();
                    var checksum = reader.ReadUInt32();
                    var flags = reader.ReadByte();
                }

                // SerializeNetFieldExportGroupMap 
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1289
                var numNetFieldExportGroups = reader.ReadUInt32();
                for (var i = 0; i < numNetFieldExportGroups; i++)
                {
                    reader.ReadNetFieldExportGroupMap();
                }

                // SerializeDemoFrameFromQueuedDemoPackets
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1978
                reader.ReadDemoFrameIntoPlaybackPackets();
            }
        }

        // fname
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Serialization/NameAsStringProxyArchive.h#L11

        // ReceiveNetFieldExports
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1497

        // ReadDemoFrameIntoPlaybackPackets
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L2848

        // TickDemoRecord
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L2255

        // TickDemoRecordFrame
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L2324

        // WritePacket
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L3450

        // Serialize - might be useful...
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L5775

        // SerializeHitResult - might be useful one day
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Developer/CollisionAnalyzer/Private/CollisionAnalyzer.cpp#L19

        //  UnrealMath - FVector etc
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Math/UnrealMath.cpp#L57

        // PackedVector
        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/NetSerialization.h#L1210


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L2106
        /// </summary>
        public virtual void ReadExternalData()
        {
            while (true)
            {
                var externalDataNumBits = ReadIntPacked();
                if (externalDataNumBits == 0)
                {
                    return;
                }

                // Read net guid this payload belongs to
                var netGuid = ReadIntPacked();
                var externalDataNumBytes = (int)(externalDataNumBits + 7) >> 3;
                var externalData = ReadBytes(externalDataNumBytes);

                File.WriteAllBytes($"external/externaldata-{netGuid}-{externalDataIndex}.dump", externalData);
                externalDataIndex++;
            }

            // Using an indirect array here since FReplayExternalData stores an FBitReader, and it's not safe to store an FArchive directly in a TArray.
            // typedef TIndirectArray<FReplayExternalData> FReplayExternalDataArray;
            // class DemoNetDriver.h -> BitReader and TimeSeconds (float)

            // ScriptCore.cpp UObject::ProcessEvent( UFunction* Function, void* Parms )
        }


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1348
        /// </summary>
        public virtual void ReadExportData()
        {
            ReadNetFieldExports();
            ReadNetExportGuids();
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1564
        /// </summary>
        public virtual void ReadNetExportGuids()
        {
            var numGuids = ReadIntPacked();
            for (var i = 0; i < numGuids; i++)
            {
                var size = ReadInt32();
                var guid = ReadBytes(size);
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1497
        /// </summary>
        public virtual void ReadNetFieldExports()
        {
            var numLayoutCmdExports = ReadIntPacked();
            for (var i = 0; i < numLayoutCmdExports; i++)
            {
                var pathNameIndex = ReadIntPacked();
                var isExported = ReadIntPacked() == 1;

                if (isExported)
                {
                    var pathName = ReadFString();
                    var numExports = ReadIntPacked();
                }

                ReadNetFieldExport();
            }
        }


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L3220
        /// </summary>
        public virtual PlaybackPacket ReadPacket()
        {
            var packet = new PlaybackPacket();

            var bufferSize = ReadInt32();
            if (bufferSize == 0)
            {
                packet.State = PacketState.End;
                return packet;
            }

            packet.Data = ReadBytes(bufferSize);
            packet.State = PacketState.Success;
            return packet;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L1452
        /// </summary>
        public virtual void ReadPacketInfo(BitReader reader)
        {
            var bHasServerFrameTime = reader.ReadBit();

            if (bHasServerFrameTime)
            {
                var frameTimeByte = reader.ReadByte();
            }
            var remoteInKBytesPerSecondByte = reader.ReadByte();
        }


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/Net/NetPacketNotify.cpp#L105
        /// </summary>
        /// <param name="packed"></param>
        public virtual uint GetHistoryWordCount(uint packed)
        {
            var historyWordCountBits = 4;
            var historyWordCountMask = (1 << historyWordCountBits) - 1;

            return (uint)(packed & historyWordCountMask);
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/Net/NetPacketNotify.cpp#L143
        /// </summary>
        public virtual void ReadPacketHeader(BitReader reader)
        {
            var packedHeader = reader.ReadUInt32(); // do we need it unpacked?

            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Public/Net/NetPacketNotify.h#L139
            // var historyWordCount = GetHistoryWordCount(packedHeader) + 1;

            // sequencehistory.h
            //NumWords = FPlatformMath::Min(NumWords, WordCount);
            //for (SIZE_T CurrentWordIt = 0; CurrentWordIt < NumWords; ++CurrentWordIt)
            //{
            //    Reader << Storage[CurrentWordIt];
            //}
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1409
        /// </summary>
        public virtual void ReceiveNetFieldExportsCompat(BitReader bitReader)
        {
            var numLayoutCmdExports = bitReader.ReadInt32();
            for (var i = 0; i < numLayoutCmdExports; i++)
            {
                var pathNameIndex = bitReader.ReadIntPacked();
                if (bitReader.ReadBit())
                {
                    var pathName = bitReader.ReadFString();
                    var maxExports = bitReader.ReadInt32();
                }

                var netFieldExport = ReadNetFieldExport(bitReader);
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L804
        /// </summary>
        /// <param name="bitReader"></param>
        public virtual void InternalLoadObject(BitReader bitReader)
        {
            var netGuid = bitReader.ReadUInt32();
            // exportFlags ? 
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1203
        /// </summary>
        public virtual void ReceiveNetGUIDBunch(BitReader bitReader)
        {
            var bHasRepLayoutExport = bitReader.ReadBit();

            if (bHasRepLayoutExport)
            {
                // We need to keep this around to ensure we don't break backwards compatability.
                ReceiveNetFieldExportsCompat(bitReader);
                return;
            }

            var numGUIDsInBunch = bitReader.ReadInt32();
            //if (NumGUIDsInBunch > MAX_GUID_COUNT)

            var numGUIDsRead = 0;
            while (numGUIDsRead < numGUIDsInBunch)
            {
                //InternalLoadObject(InBunch, Obj, 0);
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L804
                InternalLoadObject(bitReader);

                numGUIDsRead++;
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L384
        /// </summary>
        /// <param name="bitReader"></param>
        public virtual void ReceivedRawBunch(BitReader bitReader)
        {

        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L1346
        /// </summary>
        /// <param name="bitReader"></param>
        public virtual void ReceivedBunch(BitReader bitReader)
        {
            var messageType = bitReader.ReadByte();

        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L1525
        /// </summary>
        /// <param name="packet"><see cref="PlaybackPacket"/></param>
        public virtual void ProcessPacket(PlaybackPacket packet)
        {
            // serializebits...
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Serialization/BitReader.h#L36

            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L1080
            //var count = packet.Data.Length;
            //var bitSize = count;
            //var lastByte = packet.Data[count - 1];
            //if (lastByte != 0)
            //{
            //    bitSize = (count * 8) - 1;
            //    while (!((lastByte & 0x80) > 0))
            //    {
            //        lastByte *= 2;
            //        bitSize--;
            //    }
            //}

            //const ProcessedPacket UnProcessedPacket = Handler->Incoming(Data, Count);
            var bitReader = new BitReader(packet.Data);

            // var DEFAULT_MAX_CHANNEL_SIZE = 32767;

            // InternalAck is always true of demo?
            //ReadPacketHeader(bitReader);
            //ReadPacketInfo(bitReader);

            while (!bitReader.AtEnd())
            {
                // For demo backwards compatibility, old replays still have this bit
                if (Replay.Header.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_ACKS_INCLUDED_IN_HEADER)
                {
                    var isAckDummy = bitReader.ReadBit();
                }

                // FInBunch
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataBunch.cpp#L18
                
                var bControl = bitReader.ReadBit();
                var bOpen = bControl ? bitReader.ReadBit() : false;
                var bClose = bControl ? bitReader.ReadBit() : false;

                if (Replay.Header.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_CLOSE_REASON)
                {
                    var bDormant = bClose ? bitReader.ReadBit() : false;
                    var closeReason = bDormant ? ChannelCloseReason.Dormancy : ChannelCloseReason.Destroyed;
                }
                else
                {
                    var closeReason = bClose ? (ChannelCloseReason)bitReader.ReadInt((int)ChannelCloseReason.MAX) : ChannelCloseReason.Destroyed;
                    var bDormant = closeReason == ChannelCloseReason.Dormancy;
                }

                var bIsReplicationPaused = bitReader.ReadBit();
                var bReliable = bitReader.ReadBit();

                if (Replay.Header.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_CLOSE_REASON)
                {
                    var OLD_MAX_ACTOR_CHANNELS = 10240;
                    var chIndex = bitReader.ReadInt(OLD_MAX_ACTOR_CHANNELS);
                }
                else
                {
                    var chIndex = bitReader.ReadIntPacked();
                }

                var bHasPackageMapExports = bitReader.ReadBit();
                var bHasMustBeMappedGUIDs = bitReader.ReadBit();
                var bPartial = bitReader.ReadBit();

                if (bReliable)
                {
                    // We can derive the sequence for 100% reliable connections
                    // var ChSequence = InReliable[Bunch.ChIndex] + 1;
                }
                else if (bPartial)
                {
                    // If this is an unreliable partial bunch, we simply use packet sequence since we already have it
                    // var ChSequence = InPacketId;
                }
                else
                {
                    var chSequence = 0;
                }

                var bPartialInitial = bPartial ? bitReader.ReadBit() : false;
                var bPartialFinal = bPartial ? bitReader.ReadBit() : false;

                var chType = ChannelType.None;
                var chName = ChannelName.None;

                if (Replay.Header.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES)
                {
                    chType = (bReliable || bOpen) ? (ChannelType)bitReader.ReadInt((int)ChannelType.MAX) : ChannelType.None;
                }
                else
                {
                    if (bReliable || bOpen)
                    {
                        //chName = UPackageMap::StaticSerializeName(Reader, Bunch.ChName);
                        Enum.TryParse(StaticParseName(bitReader), out chName);

                        if (chName == ChannelName.Control)
                        {
                            chType = ChannelType.Control;
                        }
                        else if (chName == ChannelName.Voice)
                        {
                            chType = ChannelType.Voice;
                        }
                        else if (chName == ChannelName.Actor)
                        {
                            chType = ChannelType.Actor;
                        }
                    }
                }

                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L83
                var maxPacket = 1024 * 2;
                var bunchDataBits = bitReader.ReadInt(maxPacket * 8);
                var headerPos = bitReader.Position;
                // Bunch.SetData( Reader, BunchDataBits );
                if (bHasPackageMapExports)
                {
                    ReceiveNetGUIDBunch(bitReader);
                }

                var bNewlyOpenedActorChannel = bOpen && (chName == ChannelName.Actor) && (!bPartial || bPartialInitial);
                if (bNewlyOpenedActorChannel)
                {
                    if (bHasMustBeMappedGUIDs)
                    {
                        var numMustBeMappedGUIDs = bitReader.ReadUInt16();
                        for (var i = 0; i < numMustBeMappedGUIDs; i++)
                        {
                            bitReader.ReadUInt32();
                            //bitReader.ReadIntPacked();
                        }
                    }

                    //FNetworkGUID ActorGUID;
                    var actorGuid = bitReader.ReadUInt32();
                }

                ReceivedBunch(bitReader);
            }

            // FBitWriterMark() ?
            // FChannelRecordImpl::PushPacketId(ChannelRecord, OutPacketId); ?

            // termination bit?
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L1170
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L318
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L2848
        /// </summary>
        public override void ParseReplayData()
        {
            Console.WriteLine("ParseReplayData...");
            if (Replay.Metadata.FileVersion >= ReplayVersionHistory.StreamChunkTimes)
            {
                var start = ReadUInt32();
                var end = ReadUInt32();
                var length = ReadUInt32();
            }
            else
            {
                var length = ReadUInt32();
            }

            using (var reader = Decompress())
            {
                var replayDataIndex = index;
                var remainingBytes = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
                var output = reader.ReadBytes(remainingBytes);
                File.WriteAllBytes($"replaydata-{replayDataIndex}.dump", output);
                reader.BaseStream.Position -= remainingBytes;

                while (reader.BaseStream.Length > reader.BaseStream.Position)
                {
                    var startPos = reader.BaseStream.Position;
                    var playbackPackets = reader.ReadDemoFrameIntoPlaybackPackets();

                    // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L3338
                    foreach (var packet in playbackPackets)
                    {
                        if (packet.State == PacketState.Success)
                        {
                            File.WriteAllBytes($"packets/packet-{index}-{replayDataIndex}-{startPos}-{reader.BaseStream.Position}.dump", packet.Data);
                            index++;
                            ProcessPacket(packet);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Serialization/CompressedChunkInfo.cpp#L9
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
            var reader = new FortniteBinaryDecompressor(new MemoryStream(output), this.Replay);
            reader.BaseStream.Position = 0;
            return reader;
        }
    }
}
