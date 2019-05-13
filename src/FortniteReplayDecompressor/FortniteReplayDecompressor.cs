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
            // todo: should be bits...
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
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.inl
                return "";
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
                if (Replay.Header.EngineNetworkVersionHistory < EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES)
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
                return "";
            }

            var inString = bitReader.ReadFString();
            var inNumber = bitReader.ReadInt32();
            return inString;
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

                if (Replay.Header.EngineNetworkVersionHistory < EngineNetworkVersionHistory.HISTORY_NETEXPORT_SERIALIZATION)
                {
                    fieldExport.Name = ReadFString();
                    fieldExport.Type = ReadFString();
                }
                else if (Replay.Header.EngineNetworkVersionHistory < EngineNetworkVersionHistory.HISTORY_NETEXPORT_SERIALIZE_FIX)
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
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1667
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
                var packetOffset = reader.ReadInt64();
                var levelForCheckpoint = reader.ReadInt32();
                var deletedNetStartupActors = reader.ReadArray(reader.ReadFString);

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

                var remainingBytes = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
                var output = reader.ReadBytes(remainingBytes);
                File.WriteAllBytes($"{id}.dump", output);
                reader.BaseStream.Position -= remainingBytes;

                // SerializeDemoFrameFromQueuedDemoPackets
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1978
                // WriteDemoFrameFromQueuedDemoPackets
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L3374

                var currentLevelIndex = reader.ReadUInt32();
                var frameTime = reader.ReadSingle();

                reader.ReadExportData();

                if (Replay.Header.HasLevelStreamingFixes())
                {
                    var numLevelsAddedThisFrame = reader.ReadIntPacked();
                    for (var i = 0; i < numLevelsAddedThisFrame; i++)
                    {
                        var levelName = reader.ReadFString();
                    }
                }
                // else todo?

                // SaveExternalData
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L2071

                // FReplayExternalOutData
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L916

                // FRepChangedPropertyTracker
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Public/Net/RepLayout.h#L83

                remainingBytes = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
                try
                {
                    reader.ReadExternalData();

                    var playbackPackets = new List<PlaybackPacket>();
                    var @continue = true;
                    while (@continue)
                    {
                        if (Replay.Header.HasLevelStreamingFixes())
                        {
                            var seenLevelIndex = reader.ReadIntPacked();
                        }

                        var packet = reader.ReadPacket();
                        playbackPackets.Add(packet);

                        @continue = packet.State switch
                        {
                            PacketState.End => false,
                            PacketState.Error => false,
                            PacketState.Success => true,
                            _ => false
                        };
                    }

                    if (Replay.Header.HasLevelStreamingFixes())
                    {
                        reader.ReadIntPacked();
                    }
                    // Write a count of 0 to signal the end of the frame
                    reader.ReadInt32();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                reader.BaseStream.Position -= remainingBytes;
                output = reader.ReadBytes(remainingBytes);
                File.WriteAllBytes($"{id}-end.dump", output);
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

                var netGuid = ReadIntPacked();
                var externalDataNumBytes = (int)(externalDataNumBits + 7) >> 3;
                var unknown = ReadBytes(externalDataNumBytes);
            }
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
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1356
        /// </summary>
        public virtual void ReadNetFieldExports()
        {
            var netFieldCount = ReadIntPacked();
            for (var i = 0; i < netFieldCount; i++)
            {
                var pathNameIndex = ReadIntPacked();
                var needsExport = ReadIntPacked() == 1;

                if (needsExport)
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
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/PackageMapClient.cpp#L1409
        /// </summary>
        public virtual void ReceiveNetFieldExportsCompat(BitReader bitReader)
        {
            throw new NotImplementedException();
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

                numGUIDsRead++;
            }

        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L1525
        /// </summary>
        /// <param name="packet"><see cref="PlaybackPacket"/></param>
        public virtual void ProcessPacket(PlaybackPacket packet)
        {
            // serializebits...
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Serialization/BitReader.h#L36

            var bitReader = new BitReader(packet.Data);

            // var DEFAULT_MAX_CHANNEL_SIZE = 32767;
            // ReadPacketInfo(bitrReader);
            // while( !bitReader.AtEnd() )

            // For demo backwards compatibility, old replays still have this bit
            if (Replay.Header.EngineNetworkVersionHistory < EngineNetworkVersionHistory.HISTORY_ACKS_INCLUDED_IN_HEADER)
            {
                var isAckDummy = bitReader.ReadBit();
            }

            var startPos = bitReader.Position;
            var bControl = bitReader.ReadBit();
            var bOpen = bControl ? bitReader.ReadBit() : false;
            var bClose = bControl ? bitReader.ReadBit() : false;

            // FInBunch
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DataBunch.cpp#L18

            if (Replay.Header.EngineNetworkVersionHistory < EngineNetworkVersionHistory.HISTORY_CHANNEL_CLOSE_REASON)
            {
                var bDormant = bClose ? bitReader.ReadBit() : false;
                //CloseReason = Bunch.bDormant ? EChannelCloseReason::Dormancy : EChannelCloseReason::Destroyed;
            }
            else
            {
                var closeReason = bClose ? bitReader.ReadUInt32() : 0;
                //Bunch.CloseReason = Bunch.bClose ? (EChannelCloseReason)Reader.ReadInt((uint32)EChannelCloseReason::MAX) : EChannelCloseReason::Destroyed;
                //Bunch.bDormant = (Bunch.CloseReason == EChannelCloseReason::Dormancy);
            }

            var bIsReplicationPaused = bitReader.ReadBit();
            var bReliable = bitReader.ReadBit();

            if (Replay.Header.EngineNetworkVersionHistory < EngineNetworkVersionHistory.HISTORY_CHANNEL_CLOSE_REASON)
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
                var ChSequence = 0;
            }

            var bPartialInitial = bPartial ? bitReader.ReadBit() : false;
            var bPartialFinal = bPartial ? bitReader.ReadBit() : false;

            var chType = ChannelType.CHTYPE_None;
            var chName = NAME_Type.None;
            if (Replay.Header.EngineNetworkVersionHistory < EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES)
            {
                chType = (bReliable || bOpen) ? (ChannelType)bitReader.ReadInt((int)ChannelType.CHTYPE_MAX) : ChannelType.CHTYPE_None;
            }
            else
            {
                if (bReliable || bOpen)
                {
                    //chName = UPackageMap::StaticSerializeName(Reader, Bunch.ChName);
                    Enum.TryParse(StaticParseName(bitReader), out chName);   

                    if (chName == NAME_Type.Control)
                    {
                        chType = ChannelType.CHTYPE_Control;
                    }
                    else if (chName == NAME_Type.Voice)
                    {
                        chType = ChannelType.CHTYPE_Voice;
                    }
                    else if (chName == NAME_Type.Actor)
                    {
                        chType = ChannelType.CHTYPE_Actor;
                    }
                }
                else
                {
                    chType = ChannelType.CHTYPE_None;
                    chName = NAME_Type.None;
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

            // var bNewlyOpenedActorChannel = Bunch.bOpen && (Bunch.ChName == NAME_Actor) && (!Bunch.bPartial || Bunch.bPartialInitial);
            if (bHasMustBeMappedGUIDs)
            {
                var numMustBeMappedGUIDs = bitReader.ReadUInt16();
                for (var i = 0; i < numMustBeMappedGUIDs; i++)
                {
                    bitReader.ReadUInt32();
                }
            }

            //FNetworkGUID ActorGUID;

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
                    Console.WriteLine("ReadDemoFrameIntoPlaybackPackets...");
                    if (Replay.Header.Version >= NetworkVersionHistory.HISTORY_MULTIPLE_LEVELS)
                    {
                        var currentLevelIndex = reader.ReadInt32();
                    }
                    var timeSeconds = reader.ReadSingle();
                    Console.WriteLine($"Time: {timeSeconds}...");

                    reader.ReadExportData();

                    if (Replay.Header.HasLevelStreamingFixes())
                    {
                        var numStreamingLevels = reader.ReadIntPacked();
                        for (var i = 0; i < numStreamingLevels; i++)
                        {
                            var levelName = reader.ReadFString();
                        }
                    }
                    else
                    {
                        var numStreamingLevels = reader.ReadIntPacked();
                        for (var i = 0; i < numStreamingLevels; i++)
                        {
                            var packageName = reader.ReadFString();
                            var packageNameToLoad = reader.ReadFString();
                            // FTransform
                            //var levelTransform = reader.ReadFString();
                            // filter duplicates
                        }
                    }

                    if (Replay.Header.HasLevelStreamingFixes())
                    {
                        var externalOffset = reader.ReadUInt64();
                    }

                    // if (!bForLevelFastForward)
                    reader.ReadExternalData();
                    // else skip externalOffset

                    var playbackPackets = new List<PlaybackPacket>();
                    var @continue = true;
                    while (@continue)
                    {
                        if (Replay.Header.HasLevelStreamingFixes())
                        {
                            var seenLevelIndex = reader.ReadIntPacked();
                        }

                        var packet = reader.ReadPacket();
                        playbackPackets.Add(packet);

                        @continue = packet.State switch
                        {
                            PacketState.End => false,
                            PacketState.Error => false,
                            PacketState.Success => true,
                            _ => false
                        };
                    }

                    // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L3338
                    foreach (var packet in playbackPackets)
                    {
                        if (packet.State == PacketState.Success)
                        {
                            File.WriteAllBytes($"packets/packet-{index}-{replayDataIndex}-{startPos}-{reader.BaseStream.Position}.dump", packet.Data);
                            index++;
                        }
                        //ProcessPacket(packet);
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
