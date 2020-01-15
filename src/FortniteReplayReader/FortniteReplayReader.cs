using FortniteReplayReader.Exceptions;
using FortniteReplayReader.Extensions;
using FortniteReplayReader.Models;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Unreal.Core;
using Unreal.Core.Contracts;
using Unreal.Core.Exceptions;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader
{
    public class ReplayReader : Unreal.Core.ReplayReader<FortniteReplay>
    {
        public ReplayReader(ILogger logger = null)
        {
            Replay = new FortniteReplay();
            _logger = logger;
        }

        public FortniteReplay ReadReplay(string fileName, ParseMode mode = ParseMode.Minimal)
        {
            using var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream);
            return ReadReplay(archive, mode);
        }

        public FortniteReplay ReadReplay(Stream stream, ParseMode mode = ParseMode.Minimal)
        {
            using var archive = new Unreal.Core.BinaryReader(stream);
            return ReadReplay(archive, mode);
        }

        private string _branch;
        public int Major { get; set; }
        public int Minor { get; set; }
        public string Branch
        {
            get => _branch;
            set
            {
                var regex = new Regex(@"\+\+Fortnite\+Release\-(?<major>\d+)\.(?<minor>\d*)");
                var result = regex.Match(value);
                if (result.Success)
                {
                    Major = int.Parse(result.Groups["major"]?.Value ?? "0");
                    Minor = int.Parse(result.Groups["minor"]?.Value ?? "0");
                }
                _branch = value;
            }
        }

        protected override void OnExportRead(uint channel, INetFieldExportGroup exportGroup)
        {
            _logger?.LogDebug($"Received data for group {exportGroup.GetType().Name}");
        }

        public override void ReadReplayHeader(FArchive archive)
        {
            base.ReadReplayHeader(archive);
            Branch = Replay.Header.Branch;
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

            _logger?.LogInformation($"Unknown event {info.Group} ({info.Metadata}) of size {info.SizeInBytes}");
            if (IsDebugMode)
            {
                //throw new UnknownEventException($"Unknown event {info.Group} ({info.Metadata}) of size {info.SizeInBytes}");
            }
        }

        public virtual EncryptionKey ParseEncryptionKeyEvent(FArchive archive, EventInfo info)
        {
            return new EncryptionKey()
            {
                Info = info,
                Key = archive.ReadBytesToString(32)
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
                var elim = new PlayerElimination
                {
                    Info = info,
                };

                if (archive.EngineNetworkVersion >= EngineNetworkVersionHistory.HISTORY_FAST_ARRAY_DELTA_STRUCT && Major >= 9)
                {
                    archive.SkipBytes(85);
                    elim.Eliminated = ParsePlayer(archive);
                    elim.Eliminator = ParsePlayer(archive);
                }
                else
                {
                    if (Major <= 4 && Minor < 2)
                    {
                        archive.SkipBytes(12);
                    }
                    else if (Major == 4 && Minor <= 2)
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
                elim.Time = info?.StartTime.MillisecondsToTimeStamp();
                return elim;
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Error while parsing PlayerElimination at timestamp {info.StartTime}");
                throw new PlayerEliminationException($"Error while parsing PlayerElimination at timestamp {info.StartTime}", ex);
            }
        }

        public virtual string ParsePlayer(FArchive archive)
        {
            // TODO player type enum
            var botIndicator = archive.ReadByte();
            if (botIndicator == 0x03)
            {
                return "Bot";
            }
            else if (botIndicator == 0x10)
            {
                return archive.ReadFString();
            }

            // 0x11
            var size = archive.ReadByte();
            return archive.ReadGUID(size);
        }
    }
}
