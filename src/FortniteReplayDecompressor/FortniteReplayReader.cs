using FortniteReplayDecompressor.Core.Models;
using FortniteReplayReaderDecompressor.Core.Exceptions;
using FortniteReplayReaderDecompressor.Core.Models;
using FortniteReplayReaderDecompressor.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Unreal.Core;
using Unreal.Core.Exceptions;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayDecompressor
{
    public class FortniteReplayReader : Unreal.Core.ReplayReader<FortniteReplay>
    {
        public FortniteReplayReader()
        {
            Replay = new FortniteReplay();
        }

        public Replay ReadReplay(string fileName)
        {
            using var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream);
            return ReadReplay(archive);
        }

        public Replay ReadReplay(Stream stream)
        {
            using var archive = new Unreal.Core.BinaryReader(stream);
            return ReadReplay(archive);
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

            _logger?.LogDebug($"Encountered event {info.Group} ({info.Metadata}) at {info.StartTime} of size {info.SizeInBytes}");

            // Every event seems to start with some unknown int
            if (info.Group == ReplayEventTypes.PLAYER_ELIMINATION)
            {
                var elimination = ParseElimination(archive, info);
                Replay.Eliminations.Add(elimination);
                return;
            }

            else if (info.Metadata == ReplayEventTypes.MATCH_STATS)
            {
                Replay.Stats = ParseMatchStats(archive, info);
                return;
            }

            else if (info.Metadata == ReplayEventTypes.TEAM_STATS)
            {
                Replay.TeamStats = ParseTeamStats(archive, info);
                return;
            }

            else if (info.Metadata == ReplayEventTypes.ENCRYPTION_KEY)
            {
                ParseEncryptionKeyEvent(archive, info);
                return;
            }

            else if (info.Metadata == ReplayEventTypes.CHARACTER_SAMPLE)
            {
                ParseCharacterSample(archive, info);
                return;
            }

            else if (info.Group == ReplayEventTypes.ZONE_UPDATE)
            {
                ParseZoneUpdateEvent(archive, info);
                return;
            }

            else if (info.Group == ReplayEventTypes.BATTLE_BUS)
            {
                ParseBattleBusFlightEvent(archive, info);
                return;
            }

            else if (info.Group == "fortBenchEvent")
            {
                return;
            }

            _logger?.LogWarning($"Unknown event {info.Group} ({info.Metadata}) of size {info.SizeInBytes}");
            // optionally throw?
            throw new UnknownEventException($"Unknown event {info.Group} ({info.Metadata}) of size {info.SizeInBytes}");
        }

        public virtual CharacterSample ParseCharacterSample(FArchive archive, EventInfo info)
        {
            return new CharacterSample()
            {
                Info = info,
            };
        }

        public virtual EncryptionKey ParseEncryptionKeyEvent(FArchive archive, EventInfo info)
        {
            return new EncryptionKey()
            {
                Info = info,
                Key = archive.ReadBytesToString(32)
            };
        }

        public virtual ZoneUpdate ParseZoneUpdateEvent(FArchive archive, EventInfo info)
        {
            // 21 bytes in 9, 20 in 9.10...
            return new ZoneUpdate()
            {
                Info = info,
            };
        }

        public virtual BattleBusFlight ParseBattleBusFlightEvent(FArchive archive, EventInfo info)
        {
            // Added in 9 and removed again in 9.10?
            return new BattleBusFlight()
            {
                Info = info,
            };
        }

        public virtual TeamStats ParseTeamStats(FArchive archive, EventInfo info)
        {
            return new TeamStats()
            {
                Info = info,
                Unknown = archive.ReadUInt32(),
                Position = archive.ReadUInt32(),
                TotalPlayers = archive.ReadUInt32()
            };
        }

        public virtual Stats ParseMatchStats(FArchive archive, EventInfo info)
        {
            return new Stats()
            {
                Info = info,
                Unknown = archive.ReadUInt32(),
                Accuracy = archive.ReadSingle(),
                Assists = archive.ReadUInt32(),
                Eliminations = archive.ReadUInt32(),
                WeaponDamage = archive.ReadUInt32(),
                OtherDamage = archive.ReadUInt32(),
                Revives = archive.ReadUInt32(),
                DamageTaken = archive.ReadUInt32(),
                DamageToStructures = archive.ReadUInt32(),
                MaterialsGathered = archive.ReadUInt32(),
                MaterialsUsed = archive.ReadUInt32(),
                TotalTraveled = archive.ReadUInt32()
            };
        }

        public virtual PlayerElimination ParseElimination(FArchive archive, EventInfo info)
        {
            try
            {
                var changeList = Replay.Header.Changelist;
                var elim = new PlayerElimination
                {
                    Info = info,
                };

                // "++Fortnite+Release-9.10"
                if (archive.EngineNetworkVersion >= EngineNetworkVersionHistory.HISTORY_UPDATE9 && changeList >= 6573057)
                {
                    archive.SkipBytes(87);
                    elim.Eliminated = archive.ReadGUID();
                    archive.SkipBytes(2);
                    elim.Eliminator = archive.ReadGUID();
                }
                else
                {
                    // "++Fortnite+Release-4.0"
                    if (changeList <= 4039451)
                    {
                        archive.SkipBytes(12);
                    }
                    // "++Fortnite+Release-4.2"
                    else if (changeList <= 4072250)
                    {
                        archive.SkipBytes(40);
                    }
                    else
                    {
                        archive.SkipBytes(45);
                    }
                    elim.Eliminated = archive.ReadFString();
                    elim.Eliminator = archive.ReadFString();
                }

                elim.GunType = archive.ReadByte();
                elim.Knocked = archive.ReadUInt32AsBoolean();
                elim.Time = info.StartTime.MillisecondsToTimeStamp();
                return elim;
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Error while parsing PlayerElimination at timestamp {info.StartTime}");
                throw new PlayerEliminationException($"Error while parsing PlayerElimination at timestamp {info.StartTime}", ex);
            }
        }

    }
}
