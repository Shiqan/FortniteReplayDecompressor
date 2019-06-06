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
        private int replayDataIndex = 0;
        private int packetIndex = 0;

        // TODO make sure only one netguidcache is used ...
        private Dictionary<uint, string> NetGuidCache = new Dictionary<uint, string>();

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
            var isHardcoded = ReadBoolean();
            if (isHardcoded)
            {
                uint nameIndex;
                if (Replay.Header.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES)
                {
                    nameIndex = ReadUInt32();
                }
                else
                {
                    nameIndex = ReadIntPacked();
                }

                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.h#L31
                // hard coded names in "UnrealNames.inl"
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.inl
                return ((UnrealNames)nameIndex).ToString();
            }

            var inString = ReadFString();
            var inNumber = ReadInt32();
            return inString;
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
                return ((UnrealNames)nameIndex).ToString();
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

                    if (!NetGuidCache.ContainsKey(guid))
                    {
                        NetGuidCache.Add(guid, path);
                    }
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

                // this is a bitreader, probably some compressed string property?
                var bitReader = new BitReader(externalData);
                var unknownString = bitReader.ReadExternalData();

                if (!NetGuidCache.ContainsKey(netGuid))
                {
                    NetGuidCache.Add(netGuid, unknownString);
                }
            }

            // FRepChangedPropertyTracker

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
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1579
        /// </summary>
        public virtual void ReadNetExportGuids()
        {
            var numGuids = ReadIntPacked();
            for (var i = 0; i < numGuids; i++)
            {
                var size = ReadInt32();
                //var guid = ReadBytes(size);
                InternalLoadObject();
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
        public virtual void InternalLoadObject()
        {
            // INTERNAL_LOAD_OBJECT_RECURSION_LIMIT  = 16
            var netGuid = new NetworkGUID()
            {
                Value = ReadIntPacked()
            };

            if (!netGuid.IsValid())
            {
                return;
            }

            var flags = ReadByteAsEnum<ExportFlags>();

            // outerguid
            if (flags == ExportFlags.bHasPath || flags == ExportFlags.bHasPathAndNetWorkChecksum || flags == ExportFlags.All)
            {
                InternalLoadObject();

                var pathName = ReadFString();
                if (!NetGuidCache.ContainsKey(netGuid.Value))
                {
                    NetGuidCache.Add(netGuid.Value, pathName);
                }
                if (flags >= ExportFlags.bHasNetworkChecksum)
                {
                    var networkChecksum = ReadUInt32();
                }
            }
        }

        /// <summary> 
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L804
        /// </summary>
        /// <param name="bitReader"></param>
        public virtual void InternalLoadObject(BitReader bitReader)
        {
            // INTERNAL_LOAD_OBJECT_RECURSION_LIMIT  = 16
            var netGuid = new NetworkGUID()
            {
                Value = bitReader.ReadIntPacked()
            };

            if (!netGuid.IsValid())
            {
                return;
            }

            var flags = (ExportFlags)bitReader.ReadByte();

            // outerguid
            if (flags == ExportFlags.bHasPath || flags == ExportFlags.bHasPathAndNetWorkChecksum || flags == ExportFlags.All)
            {
                InternalLoadObject(bitReader);

                var pathName = bitReader.ReadFString();
                if (!NetGuidCache.ContainsKey(netGuid.Value))
                {
                    NetGuidCache.Add(netGuid.Value, pathName);
                }
                if (flags >= ExportFlags.bHasNetworkChecksum)
                {
                    var networkChecksum = bitReader.ReadUInt32();
                }
            }
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
                InternalLoadObject(bitReader);
                numGUIDsRead++;
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L384
        /// </summary>
        /// <param name="bitReader"></param>
        /// <param name="bunch"></param>
        public virtual void ReceivedRawBunch(BitReader bitReader, DataBunch bunch)
        {
            if (bunch.bHasPackageMapExports)
            {
                ReceiveNetGUIDBunch(bitReader);
            }

            if (bunch.bReliable)
            {
                ReceivedNextBunch(bitReader, bunch);
            }
            else
            {
                // "burn" bunches until ?
                // ReceivedNextBunch(bitReader, bunch);
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L517
        /// </summary>
        /// <param name="bitReader"></param>
        /// <param name="bunch"></param>
        public virtual void ReceivedNextBunch(BitReader bitReader, DataBunch bunch)
        {
            // We received the next bunch. Basically at this point:
            //	-We know this is in order if reliable
            //	-We dont know if this is partial or not
            // If its not a partial bunch, of it completes a partial bunch, we can call ReceivedSequencedBunch to actually handle it

            if (bunch.bPartial)
            {
                // merge
            }

            ReceivedSequencedBunch(bitReader, bunch);
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L348
        /// </summary>
        /// <param name="bitReader"></param>
        /// <param name="bunch"></param>
        public virtual void ReceivedSequencedBunch(BitReader bitReader, DataBunch bunch)
        {
            ReceivedBunch(bitReader, bunch); // based on Closing flag...

            if (bunch.bClose)
            {
                // We have fully received the bunch, so process it.
                // cleanup
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L1346
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L2298
        /// </summary>
        /// <param name="bitReader"></param>
        /// <param name="bunch"></param>
        public virtual void ReceivedBunch(BitReader bitReader, DataBunch bunch)
        {
            // control channel
            // var messageType = bitReader.ReadByte();


            // actor channel
            if (bunch.bHasMustBeMappedGUIDs)
            {
                var numMustBeMappedGUIDs = bitReader.ReadUInt16();
                for (var i = 0; i < numMustBeMappedGUIDs; i++)
                {
                    var guid = bitReader.ReadIntPacked();
                }
            }

            if (bunch.bOpen)
            {
                var actorGuid = bitReader.ReadIntPacked();
            }

            ProcessBunch(bitReader, bunch);
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L2411
        /// </summary>
        /// <param name="bitReader"></param>
        /// <param name="bunch"></param>
        public virtual void ProcessBunch(BitReader bitReader, DataBunch bunch)
        {
            // Initialize client if first time through.

            // SerializeNewActor
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L257
            InternalLoadObject(bitReader);
            var netGuid = new NetworkGUID();
            if (netGuid.IsDynamic())
            {
                var ArchetypeNetGUID = new NetworkGUID();
                InternalLoadObject(bitReader);

                // Only in Saving ??
                //if (Replay.Header.EngineNetworkVersion >= EngineNetworkVersionHistory.HISTORY_NEW_ACTOR_OVERRIDE_LEVEL)
                //{
                //    InternalLoadObject(bitReader);
                //}

                // bSerializeLocation
                if (bitReader.ReadBit())
                {
                    // Location.NetSerialize(Ar, this, SerSuccess);
                    bitReader.ReadPackedVector(10, 24);
                }

                // bSerializeRotation
                if (bitReader.ReadBit())
                {
                    // Rotation.NetSerialize(Ar, this, SerSuccess);
                    bitReader.ReadPackedVector(10, 24);
                }

                // bSerializeScale
                if (bitReader.ReadBit())
                {
                    // Scale.NetSerialize(Ar, this, SerSuccess);
                    bitReader.ReadPackedVector(10, 24);
                }

                // bSerializeVelocity
                if (bitReader.ReadBit())
                {
                    // Velocity.NetSerialize(Ar, this, SerSuccess);
                    bitReader.ReadPackedVector(10, 24);
                }
            }

            while (!bitReader.AtEnd())
            {
                ReadContentBlockPayload(bitReader, bunch);
                // if (empty) continue

                while (ReadFieldHeaderAndPayload(bitReader, bunch))
                {
                    bitReader.ReadIntPacked();
                    // not sure...
                }
            }
            // PostReceivedBunch, not interesting?

        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataReplication.cpp#L610
        /// </summary>
        /// <param name="bitReader"></param>
        /// <param name="bunch"></param>
        /// <returns></returns>
        public virtual bool ReadFieldHeaderAndPayload(BitReader bitReader, DataBunch bunch)
        {
            if (bitReader.AtEnd())
            {
                return false;
            }

            // NetFieldExportGroup.Num ?
            var netFieldExportHandle = bitReader.ReadInt(2);
            var numPayloadBits = bitReader.ReadIntPacked();
            return true;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L3391
        /// </summary>
        public virtual void ReadContentBlockPayload(BitReader bitReader, DataBunch bunch)
        {
            ReadContentBlockHeader(bitReader, bunch);
            var numPayloadBits = bitReader.ReadIntPacked();
            // FNetBitReader?
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L3175
        /// </summary>
        public virtual void ReadContentBlockHeader(BitReader bitReader, DataBunch bunch)
        {
            var bOutHasRepLayout = bitReader.ReadBit();
            var bIsActor = bitReader.ReadBit();
            if (bIsActor)
            {
                // If this is for the actor on the channel, we don't need to read anything else
                return;
            }

            // We need to handle a sub-object
            // Manually serialize the object so that we can get the NetGUID (in order to assign it if we spawn the object here)
            InternalLoadObject(bitReader);

            var bStablyNamed = bitReader.ReadBit();
            if (bStablyNamed)
            {
                // If this is a stably named sub-object, we shouldn't need to create it. Don't raise a bunch error though because this may happen while a level is streaming out.
                return;
            }

            // Serialize the class in case we have to spawn it.
            InternalLoadObject(bitReader);
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


            //const ProcessedPacket UnProcessedPacket = Handler->Incoming(Data, Count);
            var bitReader = new BitReader(packet.Data);

            // var DEFAULT_MAX_CHANNEL_SIZE = 32767;

            // InternalAck is always true for demo?
            //ReadPacketHeader(bitReader);
            //ReadPacketInfo(bitReader);
            return;
            while (!bitReader.AtEnd())
            {
                // For demo backwards compatibility, old replays still have this bit
                if (Replay.Header.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_ACKS_INCLUDED_IN_HEADER)
                {
                    var isAckDummy = bitReader.ReadBit();
                }

                // FInBunch
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataBunch.cpp#L18
                var bunch = new DataBunch();

                var bControl = bitReader.ReadBit();
                bunch.bOpen = bControl ? bitReader.ReadBit() : false;
                bunch.bClose = bControl ? bitReader.ReadBit() : false;

                if (Replay.Header.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_CLOSE_REASON)
                {
                    bunch.bDormant = bunch.bClose ? bitReader.ReadBit() : false;
                    bunch.CloseReason = bunch.bDormant ? ChannelCloseReason.Dormancy : ChannelCloseReason.Destroyed;
                }
                else
                {
                    bunch.CloseReason = bunch.bClose ? (ChannelCloseReason)bitReader.ReadInt((int)ChannelCloseReason.MAX) : ChannelCloseReason.Destroyed;
                    bunch.bDormant = bunch.CloseReason == ChannelCloseReason.Dormancy;
                }

                bunch.bIsReplicationPaused = bitReader.ReadBit();
                bunch.bReliable = bitReader.ReadBit();

                if (Replay.Header.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_CLOSE_REASON)
                {
                    var OLD_MAX_ACTOR_CHANNELS = 10240;
                    bunch.ChIndex = bitReader.ReadInt(OLD_MAX_ACTOR_CHANNELS);
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
                    //if (InteralAck)
                    //{
                    //    // We can derive the sequence for 100% reliable connections
                    //    var ChSequence = InReliable[Bunch.ChIndex] + 1;
                    //}
                    //else
                    //{
                    //    Bunch.ChSequence = MakeRelative(Reader.ReadInt(MAX_CHSEQUENCE), InReliable[Bunch.ChIndex], MAX_CHSEQUENCE);
                    //}
                }
                else if (bunch.bPartial)
                {
                    // If this is an unreliable partial bunch, we simply use packet sequence since we already have it
                    // var ChSequence = InPacketId;
                }
                else
                {
                    bunch.ChSequence = 0;
                }

                bunch.bPartialInitial = bunch.bPartial ? bitReader.ReadBit() : false;
                bunch.bPartialFinal = bunch.bPartial ? bitReader.ReadBit() : false;

                var chType = ChannelType.None;
                var chName = ChannelName.None;

                if (Replay.Header.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES)
                {
                    var type = bitReader.ReadInt((int)ChannelType.MAX);
                    chType = (bunch.bReliable || bunch.bOpen) ? (ChannelType)type : ChannelType.None;

                    if (chType == ChannelType.Control)
                    {
                        chName = ChannelName.Control;
                    }
                    else if (chType == ChannelType.Voice)
                    {
                        chName = ChannelName.Voice;
                    }
                    else if (chType == ChannelType.Actor)
                    {
                        chName = ChannelName.Actor;
                    }
                }
                else
                {
                    if (bunch.bReliable || bunch.bOpen)
                    {
                        //chName = UPackageMap::StaticSerializeName(Reader, Bunch.ChName);
                        var name = StaticParseName(bitReader);
                        Enum.TryParse(name, out chName);

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
                bunch.ChType = chType;
                bunch.ChName = chName;

                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L83
                var maxPacket = 1024 * 2;
                var bunchDataBits = bitReader.ReadInt(maxPacket * 8);
                var headerPos = bitReader.Position;
                // Bunch.SetData( Reader, BunchDataBits );
                if (bunch.bHasPackageMapExports)
                {
                    ReceiveNetGUIDBunch(bitReader);
                }

                var bNewlyOpenedActorChannel = bunch.bOpen && (chName == ChannelName.Actor) && (!bunch.bPartial || bunch.bPartialInitial);
                if (bNewlyOpenedActorChannel)
                {
                    if (bunch.bHasMustBeMappedGUIDs)
                    {
                        var numMustBeMappedGUIDs = bitReader.ReadUInt16();
                        for (var i = 0; i < numMustBeMappedGUIDs; i++)
                        {
                            // FNetworkGUID NetGUID
                            bitReader.ReadIntPacked();
                        }
                    }

                    //FNetworkGUID ActorGUID;
                    var actorGuid = bitReader.ReadUInt32();
                }

                // Channel->ReceivedRawBunch(Bunch, bLocalSkipAck);
                // ReceivedRawBunch(bitReader, bunch);
            }

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
                            File.WriteAllBytes($"packets/packet-{packetIndex}-{replayDataIndex}-{startPos}-{reader.BaseStream.Position}.dump", packet.Data);
                            packetIndex++;
                            ProcessPacket(packet);
                        }
                    }
                }
                replayDataIndex++;
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
