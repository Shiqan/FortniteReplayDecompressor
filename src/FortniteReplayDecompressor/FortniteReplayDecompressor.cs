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
        private int externalDataIndex = 0;

        private int InPacketId;
        private bool Actor; // TODO: per channel (i think)
        private bool Control;
        private bool Channel;
        private DataBunch PartialBunch;
        // const int32 UNetConnection::DEFAULT_MAX_CHANNEL_SIZE = 32767; netconnection.cpp 84
        private Dictionary<uint, int> InReliable = new Dictionary<uint, int>(); // TODO: array in unreal
        private Dictionary<uint, string> Channels = new Dictionary<uint, string>(); // TODO: UChannel
        private Dictionary<uint, uint> IgnoringChannels = new Dictionary<uint, uint>();
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
            var isExported = bitReader.ReadByte() == 1;
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


                // merge netguid cache like this for debugging purposes...
                foreach (var key in reader.NetGuidCache.Keys)
                {
                    if (!this.NetGuidCache.ContainsKey(key))
                    {
                        this.NetGuidCache[key] = reader.NetGuidCache[key];
                    }
                    else
                    {
                        if (this.NetGuidCache[key] != reader.NetGuidCache[key])
                        {
                            Console.WriteLine($"{this.NetGuidCache[key]}, {reader.NetGuidCache[key]}");
                        }
                    }
                }
            }
        }

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

                Debug($"external/externalData-{externalDataIndex}-{netGuid}", externalData);

                // this is a bitreader, probably some compressed string property?
                //var bitReader = new BitReader(externalData);
                //var unknownString = bitReader.ReadExternalData();

                //if (!NetGuidCache.ContainsKey(netGuid))
                //{
                //    NetGuidCache.Add(netGuid, unknownString);
                //}

                externalDataIndex++;
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
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1409
        /// </summary>
        public virtual void ReceiveNetFieldExportsCompat(BitReader bitReader)
        {
            var numLayoutCmdExports = bitReader.ReadUInt32();
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
            // TODO: INTERNAL_LOAD_OBJECT_RECURSION_LIMIT  = 16
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

            var flags = ExportFlags.None;
            if (netGuid.IsDefault())
            {
                flags = (ExportFlags)bitReader.ReadByte();
            }

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
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/Net/NetPacketNotify.cpp#L143
        /// see https://github.com/EpicGames/UnrealEngine/blob/27e7396206a1b3778d357cd67b93ec718ffb0dad/Engine/Source/Runtime/Engine/Public/Net/Util/SequenceHistory.h#L119
        /// </summary>
        public virtual void ReadPacketHeader(BitReader reader)
        {
            var packedHeader = reader.ReadUInt32();
            var historyWordCount = FPackedHeader.GetHistoryWordCount(packedHeader) + 1;

            var numWords = Math.Min(historyWordCount, FPackedHeader.MaxSequenceHistoryLength);
            for (var i = 0; i < numWords; i++)
            {
                reader.ReadBit(); // I think?
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
            // Immediately consume the NetGUID portion of this bunch, regardless if it is partial or reliable.
            if (bunch.bHasPackageMapExports)
            {
                ReceiveNetGUIDBunch(bitReader);
            }

            // bDeleted =
            ReceivedNextBunch(bitReader, bunch);

            // if (bDeleted) return;
            // else { We shouldn't hit this path on 100% reliable connections }
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
                    // new bunch
                    // !Bunch.bHasPackageMapExports && Bunch.GetBitsLeft() > 0
                    // InPartialBunch->AppendDataFromChecked(Bunch.GetDataPosChecked(), Bunch.GetBitsLeft());
                }
                else
                {
                    if (PartialBunch != null)
                    {

                    }
                    else
                    {
                        // skip
                    }

                    //if (!Bunch.bHasPackageMapExports && Bunch.GetBitsLeft() > 0)
                    //{
                    //    InPartialBunch->AppendDataFromChecked(Bunch.GetDataPosChecked(), Bunch.GetBitsLeft());
                    //}


                    // Advance the sequence of the current partial bunch so we know what to expect next
                    //InPartialBunch->ChSequence = Bunch.ChSequence;

                    if (bunch.bPartialFinal)
                    {
                        //if (Bunch.bHasPackageMapExports)
                        //{
                        //    // Shouldn't have these, they only go in initial partial export bunches
                        //    UE_LOG(LogNetPartialBunch, Warning, TEXT("Corrupt partial bunch. Final partial bunch has package map exports. %s"), *Describe());
                        //    Bunch.SetError();
                        //    return false;
                        //}

                        //    HandleBunch = InPartialBunch;

                        //    InPartialBunch->bPartialFinal = true;
                        //    InPartialBunch->bClose = Bunch.bClose;
                        //    PRAGMA_DISABLE_DEPRECATION_WARNINGS

                        //InPartialBunch->bDormant = Bunch.bDormant;
                        //    PRAGMA_ENABLE_DEPRECATION_WARNINGS

                        //InPartialBunch->CloseReason = Bunch.CloseReason;
                        //    InPartialBunch->bIsReplicationPaused = Bunch.bIsReplicationPaused;
                        //    InPartialBunch->bHasMustBeMappedGUIDs = Bunch.bHasMustBeMappedGUIDs;
                    }
                }
            }

            // Receive it in sequence.
            ReceivedSequencedBunch(bitReader, bunch);
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L348
        /// </summary>
        /// <param name="bitReader"></param>
        /// <param name="bunch"></param>
        public virtual void ReceivedSequencedBunch(BitReader bitReader, DataBunch bunch)
        {
            // if ( !Closing ) {
            switch (bunch.ChName)
            {
                case ChannelName.Actor:
                    ReceivedActorBunch(bitReader, bunch);
                    break;
                case ChannelName.Control:
                    ReceivedControlBunch(bitReader, bunch);
                    break;
                default:
                    throw new Exception();
            };
            // }

            if (bunch.bClose)
            {
                // We have fully received the bunch, so process it.
                // cleanup
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L1346
        /// </summary>
        /// <param name="bitReader"></param>
        /// <param name="bunch"></param>
        public virtual void ReceivedControlBunch(BitReader bitReader, DataBunch bunch)
        {
            // control channel
            while (!bitReader.AtEnd())
            {
                var messageType = bitReader.ReadByte();
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataChannel.cpp#L2298
        /// </summary>
        /// <param name="bitReader"></param>
        /// <param name="bunch"></param>
        public virtual void ReceivedActorBunch(BitReader bitReader, DataBunch bunch)
        {
            if (bunch.bHasMustBeMappedGUIDs)
            {
                var numMustBeMappedGUIDs = bitReader.ReadUInt16();
                for (var i = 0; i < numMustBeMappedGUIDs; i++)
                {
                    var guid = bitReader.ReadIntPacked();
                }
            }

            // if actor == null
            if (!Actor && bunch.bOpen)
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
            if (!Actor)
            {
                // Initialize client if first time through.

                // SerializeNewActor
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L257
                var netGuid = new NetworkGUID();
                InternalLoadObject(bitReader); // TODO: out netGuid

                if (bitReader.AtEnd() && netGuid.IsDynamic())
                {
                    return;
                }

                if (netGuid.IsDynamic())
                {
                    var ArchetypeNetGUID = new NetworkGUID();
                    InternalLoadObject(bitReader); // out ArchetypeNetGUID

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

                Actor = true;
            }



            while (!bitReader.AtEnd())
            {
                // FNetBitReader Reader( Bunch.PackageMap, 0 );

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
            // A bit reader that serializes FNames and UObject* through a network packagemap.
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
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L1007
        /// </summary>
        /// <param name="packet"></param>
        public virtual void ReceivedRawPacket(PlaybackPacket packet)
        {
            var lastByte = packet.Data[^1];

            if (lastByte != 0)
            {
                var bitReader = new BitReader(packet.Data);
                try
                {
                    ReceivedPacket(bitReader, packet);
                }
                catch
                {
                    Console.WriteLine("failed ReceivedPacket");
                }
            }
            else
            {
                throw new Exception("Malformed packet: Received packet with 0's in last byte of packet");
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L3352
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L1525
        /// </summary>
        /// <param name="bitReader"><see cref="BitReader"/></param>
        /// <param name="packet"><see cref="PlaybackPacket"/></param>
        public virtual void ReceivedPacket(BitReader bitReader, PlaybackPacket packet)
        {
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L5101
            // InternalAck always true!

            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L1669
            const int OLD_MAX_ACTOR_CHANNELS = 10240;

            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#1549
            InPacketId++;

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
                    var closeReason = bitReader.ReadInt((int)ChannelCloseReason.MAX);
                    bunch.CloseReason = bunch.bClose ? (ChannelCloseReason)closeReason : ChannelCloseReason.Destroyed;
                    bunch.bDormant = bunch.CloseReason == ChannelCloseReason.Dormancy;
                }

                bunch.bIsReplicationPaused = bitReader.ReadBit();
                bunch.bReliable = bitReader.ReadBit();

                if (Replay.Header.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_MAX_ACTOR_CHANNELS_CUSTOMIZATION)
                {
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
                    if (InReliable.ContainsKey(bunch.ChIndex))
                    {
                        bunch.ChSequence = InReliable[bunch.ChIndex] + 1;
                    }
                    else
                    {
                        bunch.ChSequence = 0;
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

                // If there's an existing channel and the bunch specified it's channel type, make sure they match.

                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L83
                var maxPacket = 1024 * 2;
                var bunchDataBits = bitReader.ReadInt(maxPacket * 8);
                // Bunch.SetData( Reader, BunchDataBits );
                var bunchReader = new BitReader(bitReader.ReadBits(bunchDataBits));

                if (bunch.bHasPackageMapExports)
                {
                    ReceiveNetGUIDBunch(bunchReader);
                }

                // Can't handle other channels until control channel exists.
                if (!Channels.ContainsKey(bunch.ChIndex) && (bunch.ChIndex != 0 || bunch.ChName != ChannelName.Control))
                {
                    if (!Channels.ContainsKey(0))
                    {
                        return;
                    }
                }

                // ignore control channel close if it hasn't been opened yet
                if (bunch.ChIndex == 0 && !Channels.ContainsKey(0) && bunch.bClose && bunch.ChName == ChannelName.Control)
                {
                    return;
                }

                // We're on a 100% reliable connection and we are rolling back some data.
                // In that case, we can generally ignore these bunches.
                // if (InternalAck && Channel && bIgnoreAlreadyOpenedChannels)
                // bIgnoreAlreadyOpenedChannels always true?  https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L4393
                if (Channel)
                {
                    var bNewlyOpenedActorChannel = bunch.bOpen && (chName == ChannelName.Actor) && (!bunch.bPartial || bunch.bPartialInitial);
                    if (bNewlyOpenedActorChannel)
                    {
                        if (bunch.bHasMustBeMappedGUIDs)
                        {
                            var numMustBeMappedGUIDs = bunchReader.ReadUInt16();
                            for (var i = 0; i < numMustBeMappedGUIDs; i++)
                            {
                                // FNetworkGUID NetGUID
                                var guid = bunchReader.ReadIntPacked();
                            }
                        }

                        //FNetworkGUID ActorGUID;
                        var actorGuid = bunchReader.ReadIntPacked();
                        IgnoringChannels.Add(bunch.ChIndex, actorGuid);
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
                if (!Channel && !bunch.bReliable)
                {
                    if (bunch.bOpen && (bunch.bClose || bunch.bPartial))
                    {
                        continue;
                    }
                }

                // Create channel if necessary
                if (!Channel)
                {
                    // if (RejectedChans.Contains(Bunch.ChIndex))
                    // if ( !Driver->IsKnownChannelName( Bunch.ChName ) )

                    // if( !Driver->Notify->NotifyAcceptingChannel( Channel ) )
                    // Channel = CreateChannelByName(Bunch.ChName, EChannelCreateFlags::None, Bunch.ChIndex);
                    Channel = true;
                    Channels.Add(bunch.ChIndex, bunch.ChName.ToString());
                    continue;
                }

                // Dispatch the raw, unsequenced bunch to the channel
                ReceivedRawBunch(bunchReader, bunch);
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
                            ReceivedRawPacket(packet);
                            packetIndex++;
                        }
                    }

                    // merge netguid cache like this for debugging purposes...
                    foreach (var key in reader.NetGuidCache.Keys)
                    {
                        if (!this.NetGuidCache.ContainsKey(key))
                        {
                            this.NetGuidCache[key] = reader.NetGuidCache[key];
                        }
                        else
                        {
                            if (this.NetGuidCache[key] != reader.NetGuidCache[key])
                            {
                                Console.WriteLine($"{this.NetGuidCache[key]}, {reader.NetGuidCache[key]}");
                            }
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
