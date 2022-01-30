using System.Security.Cryptography;
using System.Text.RegularExpressions;
using FortniteReplayReader.Exceptions;
using FortniteReplayReader.Extensions;
using FortniteReplayReader.Models;
using FortniteReplayReader.Models.Enums;
using FortniteReplayReader.Models.Events;
using FortniteReplayReader.Models.NetFieldExports;
using FortniteReplayReader.Models.NetFieldExports.Weapons;
using FortniteReplayReader.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Unreal.Core;
using Unreal.Core.Contracts;
using Unreal.Core.Exceptions;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;
using Unreal.Encryption;

namespace FortniteReplayReader;

public class ReplayReader : Unreal.Core.ReplayReader<FortniteReplay>
{
    private FortniteReplayBuilder _builder;
    private FortniteReplayOptions? _options;

    public ReplayReader(INetGuidCache guidCache, INetFieldParser parser, ILogger? logger = null, IOptions<FortniteReplayOptions>? options = default) : base(guidCache, parser, logger)
    {
        _options = options?.Value;
    }

    public FortniteReplay ReadReplay(string fileName, ParseMode parseMode)
    {
        using var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return ReadReplay(stream, parseMode);
    }

    public FortniteReplay ReadReplay(Stream stream, ParseMode parseMode)
    {
        using var archive = new Unreal.Core.BinaryReader(stream);

        _builder = new FortniteReplayBuilder();
        ReadReplay(archive, parseMode);

        return _builder.Build(Replay);
    }

    private string _branch;
    protected int Major { get; set; }
    protected int Minor { get; set; }
    protected string Branch
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

    protected override void OnChannelOpened(uint channelIndex, NetworkGUID? actor)
    {
        if (actor != null)
        {
            _builder.AddActorChannel(channelIndex, actor.Value);
        }
    }

    protected override void OnChannelClosed(uint channelIndex, NetworkGUID? actor)
    {
        if (actor != null)
        {
            _builder.RemoveChannel(channelIndex);
        }
    }

    protected override void OnNetDeltaRead(uint channelIndex, NetDeltaUpdate update)
    {
        switch (update.Export)
        {
            case ActiveGameplayModifier modifier:
                _builder.UpdateGameplayModifiers(modifier);
                break;
            //case FortPickup pickup:
            //Builder.CreatePickupEvent(channelIndex, pickup);
            //break;
            //case FortInventory inventory:
            //    Builder.UpdateInventory(channelIndex, inventory);
            //    break;
            case SpawnMachineRepData spawnMachine:
                _builder.UpdateRebootVan(channelIndex, spawnMachine);
                break;
        }
    }

    protected override void OnExportRead(uint channelIndex, INetFieldExportGroup? exportGroup)
    {
        switch (exportGroup)
        {
            case GameState state:
                _builder.UpdateGameState(state);
                break;
            case PlaylistInfo playlist:
                _builder.UpdatePlaylistInfo(playlist);
                break;
            case FortPlayerState state:
                _builder.UpdatePlayerState(channelIndex, state);
                break;
            case PlayerPawn pawn:
                _builder.UpdatePlayerPawn(channelIndex, pawn);
                break;
            //case FortPickup pickup:
            //Builder.CreatePickupEvent(channelIndex, pickup);
            //break;
            //case FortInventory inventory:
            //    Builder.UpdateInventory(channelIndex, inventory);
            //    break;
            //case BroadcastExplosion explosion:
            //    Builder.UpdateExplosion(explosion);
            //    break;
            case SafeZoneIndicator safeZone:
                _builder.UpdateSafeZones(safeZone);
                break;
            case SupplyDropLlama llama:
                _builder.UpdateLlama(channelIndex, llama);
                break;
            case Models.NetFieldExports.SupplyDrop drop:
                _builder.UpdateSupplyDrop(channelIndex, drop);
                break;
            case FortPoiManager poimanager:
                _builder.UpdatePoiManager(poimanager);
                break;
            //case GameplayCue gameplayCue:
            //    Builder.UpdateGameplayCue(channelIndex, gameplayCue);
            //    break;
            case BaseWeapon weapon:
                _builder.UpdateWeapon(channelIndex, weapon);
                break;
        }
    }

    protected override void OnExternalDataRead(uint channelIndex, IExternalData? externalData)
    {
        // TODO: at the very least, only use PlayerNameData when handle and netfieldgroup match...
        if (externalData != null)
        {
            _builder.UpdatePrivateName(channelIndex, new PlayerNameData(externalData.Archive));
        }
    }

    protected override void ReadReplayHeader(FArchive archive)
    {
        base.ReadReplayHeader(archive);
        Branch = Replay.Header.Branch;
    }

    /// <summary>
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L363"/>
    /// </summary>
    /// <param name="archive"></param>
    /// <returns></returns>
    protected override void ReadEvent(FArchive archive)
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

        _logger?.LogDebug("Encountered event {group} ({metadata}) at {startTime} of size {sizeInBytes}", info.Group, info.Metadata, info.StartTime, info.SizeInBytes);

        using var decryptedArchive = DecryptBuffer(archive, info.SizeInBytes);

        // Every event seems to start with some unknown int
        if (info.Group == ReplayEventTypes.PLAYER_ELIMINATION)
        {
            var elimination = ParseElimination(decryptedArchive, info);
            Replay.Eliminations.Add(elimination);
            return;
        }

        else if (info.Metadata == ReplayEventTypes.MATCH_STATS)
        {
            Replay.Stats = ParseMatchStats(decryptedArchive, info);
            return;
        }

        else if (info.Metadata == ReplayEventTypes.TEAM_STATS)
        {
            Replay.TeamStats = ParseTeamStats(decryptedArchive, info);
            return;
        }

        else if (info.Metadata == ReplayEventTypes.ENCRYPTION_KEY)
        {
            ParseEncryptionKeyEvent(decryptedArchive, info);
            return;
        }

        _logger?.LogInformation("Unknown event {group} ({metadata}) of size {sizeInBytes}", info.Group, info.Metadata, info.SizeInBytes);
        if (IsDebugMode)
        {
            throw new UnknownEventException($"Unknown event {info.Group} ({info.Metadata}) of size {info.SizeInBytes}");
        }
    }

    protected virtual EncryptionKey ParseEncryptionKeyEvent(FArchive archive, EventInfo info)
    {
        return new EncryptionKey()
        {
            Info = info,
            Key = archive.ReadBytesToString(32)
        };
    }

    protected virtual TeamStats ParseTeamStats(FArchive archive, EventInfo info)
    {
        return new TeamStats()
        {
            Info = info,
            Unknown = archive.ReadUInt32(),
            Position = archive.ReadUInt32(),
            TotalPlayers = archive.ReadUInt32()
        };
    }

    protected virtual Stats ParseMatchStats(FArchive archive, EventInfo info)
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

    protected virtual PlayerElimination ParseElimination(FArchive archive, EventInfo info)
    {
        try
        {
            var elim = new PlayerElimination
            {
                Info = info,
            };

            var version = archive.ReadInt32();

            if (version >= 3)
            {
                // unknown
                archive.SkipBytes(1);

                if (version >= 6)
                {
                    elim.EliminatedInfo.Rotation = archive.ReadFQuat();
                    elim.EliminatedInfo.Location = archive.ReadFVector();
                    elim.EliminatedInfo.Scale = archive.ReadFVector();
                }

                elim.EliminatorInfo.Rotation = archive.ReadFQuat();
                elim.EliminatorInfo.Location = archive.ReadFVector();
                elim.EliminatorInfo.Scale = archive.ReadFVector();
            }
            else
            {
                if (Major <= 4 && Minor < 2)
                {
                    //12 bytes including version int. Always all 0s
                    archive.SkipBytes(8);
                }
                else if (Major == 4 && Minor <= 2)
                {
                    //Likely transform data with version being part of it, but don't have a replay to verify
                    archive.SkipBytes(36);
                }
            }
            ParsePlayer(archive, elim.EliminatedInfo, version);
            ParsePlayer(archive, elim.EliminatorInfo, version);

            elim.GunType = archive.ReadByte();
            elim.Knocked = archive.ReadUInt32AsBoolean();
            elim.Time = info.StartTime.MillisecondsToTimeStamp();
            return elim;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error while parsing PlayerElimination at timestamp {}", info?.StartTime);
            throw new PlayerEliminationException($"Error while parsing PlayerElimination at timestamp {info?.StartTime}", ex);
        }
    }

    protected virtual void ParsePlayer(FArchive archive, PlayerEliminationInfo info, int version)
    {
        if (version < 6)
        {
            info.Id = archive.ReadFString();
            return;
        }

        info.PlayerType = archive.ReadByteAsEnum<PlayerTypes>();
        info.Id = info.PlayerType switch
        {
            PlayerTypes.BOT => "Bot",
            PlayerTypes.NAMED_BOT => archive.ReadFString(),
            PlayerTypes.PLAYER => archive.ReadGUID(archive.ReadByte()),
            _ => ""
        };
    }

    protected override FArchive DecryptBuffer(FArchive archive, int size)
    {
        if (!Replay.Info.IsEncrypted)
        {
            return archive;
        }

        var key = Replay.Info.EncryptionKey;
        var encryptedBytes = archive.ReadBytes(size);

        using var aesCryptoServiceProvider = new AesCryptoServiceProvider
        {
            KeySize = key.Length * 8,
            Key = key.ToArray(),
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7
        };

        using var cryptoTransform = aesCryptoServiceProvider.CreateDecryptor();
        var decryptedArray = cryptoTransform.TransformFinalBlock(encryptedBytes.ToArray(), 0, encryptedBytes.Length);

        return new Unreal.Core.BinaryReader(decryptedArray.AsMemory())
        {
            EngineNetworkVersion = archive.EngineNetworkVersion,
            NetworkVersion = archive.NetworkVersion,
            ReplayHeaderFlags = archive.ReplayHeaderFlags,
            ReplayVersion = archive.ReplayVersion
        };
    }

    protected override FArchive Decompress(FArchive archive)
    {
        if (!Replay.Info.IsCompressed)
        {
            return archive;
        }

        var decompressedSize = archive.ReadInt32();
        var compressedSize = archive.ReadInt32();
        var compressedBuffer = archive.ReadBytes(compressedSize);

        _logger?.LogDebug("Decompressed archive from {compressedSize} to {decompressedSize}.", compressedSize, decompressedSize);
        var output = Oodle.DecompressReplayData(compressedBuffer, decompressedSize);

        return new Unreal.Core.BinaryReader(output)
        {
            EngineNetworkVersion = archive.EngineNetworkVersion,
            NetworkVersion = archive.NetworkVersion,
            ReplayHeaderFlags = archive.ReplayHeaderFlags,
            ReplayVersion = archive.ReplayVersion
        };
    }
}
