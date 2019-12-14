using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Script/FortniteGame.FortPlayerStateAthena")]
    public class FortPlayerState : INetFieldExportGroup
    {
        [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore, 4, "RemoteRole", "", 2)]
        public object RemoteRole { get; set; } //Type:  Bits: 2

        [NetFieldExport("Owner", RepLayoutCmdType.Pointer, 12, "Owner", "AActor*", 16)]
        public uint? Owner { get; set; } //Type: AActor* Bits: 16

        [NetFieldExport("Role", RepLayoutCmdType.Ignore, 13, "Role", "", 2)]
        public object Role { get; set; } //Type:  Bits: 2

        [NetFieldExport("Instigator", RepLayoutCmdType.PropertyUInt32, 14, "Instigator", "", 8)]
        public uint? Instigator { get; set; } //Type:  Bits: 8

        [NetFieldExport("Score", RepLayoutCmdType.PropertyUInt32, 15, "Score", "", 32)]
        public uint? Score { get; set; } //Type:  Bits: 32

        [NetFieldExport("PlayerID", RepLayoutCmdType.PropertyInt, 16, "PlayerID", "int32", 32)]
        public int? PlayerID { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("Ping", RepLayoutCmdType.PropertyByte, 17, "Ping", "uint8", 8)]
        public byte? Ping { get; set; } //Type: uint8 Bits: 8

        [NetFieldExport("bIsABot", RepLayoutCmdType.PropertyBool, 19, "bIsABot", "", 1)]
        public bool? bIsABot { get; set; } //Type:  Bits: 1

        [NetFieldExport("bOnlySpectator", RepLayoutCmdType.PropertyBool, 22, "bOnlySpectator", "uint8", 1)]
        public bool? bOnlySpectator { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("StartTime", RepLayoutCmdType.PropertyInt, 23, "StartTime", "int32", 32)]
        public int? StartTime { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("UniqueId", RepLayoutCmdType.PropertyNetId, 24, "UniqueId", "FUniqueNetIdRepl", 144)]
        public string UniqueId { get; set; } //Type: FUniqueNetIdRepl Bits: 144

        [NetFieldExport("PlayerNamePrivate", RepLayoutCmdType.PropertyString, 25, "PlayerNamePrivate", "FString", 128)]
        public string PlayerNamePrivate { get; set; } //Type: FString Bits: 128

        [NetFieldExport("bIsGameSessionOwner", RepLayoutCmdType.PropertyBool, 26, "bIsGameSessionOwner", "", 1)]
        public bool? bIsGameSessionOwner { get; set; } //Type:  Bits: 1

        [NetFieldExport("bHasFinishedLoading", RepLayoutCmdType.PropertyBool, 28, "bHasFinishedLoading", "uint8", 1)]
        public bool? bHasFinishedLoading { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bHasStartedPlaying", RepLayoutCmdType.PropertyBool, 29, "bHasStartedPlaying", "uint8", 1)]
        public bool? bHasStartedPlaying { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("PlayerRole", RepLayoutCmdType.Enum, 35, "PlayerRole", "EFortPlayerRole", 0)]
        public int? PlayerRole { get; set; } //Type: EFortPlayerRole Bits: 0

        [NetFieldExport("PartyOwnerUniqueId", RepLayoutCmdType.PropertyNetId, 36, "PartyOwnerUniqueId", "FUniqueNetIdRepl", 144)]
        public string PartyOwnerUniqueId { get; set; } //Type: FUniqueNetIdRepl Bits: 144

        [NetFieldExport("WorldPlayerId", RepLayoutCmdType.PropertyInt, 37, "WorldPlayerId", "int32", 32)]
        public int? WorldPlayerId { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("HeroType", RepLayoutCmdType.Pointer, 39, "HeroType", "UFortHeroType*", 16)]
        public uint? HeroType { get; set; } //Type: UFortHeroType* Bits: 16

        [NetFieldExport("Platform", RepLayoutCmdType.PropertyString, 55, "Platform", "FString", 64)]
        public string Platform { get; set; } //Type: FString Bits: 64

        [NetFieldExport("CharacterGender", RepLayoutCmdType.Enum, 56, "CharacterGender", "TEnumAsByte<EFortCustomGender::Type>", 2)]
        public int? CharacterGender { get; set; } //Type: TEnumAsByte<EFortCustomGender::Type> Bits: 2

        [NetFieldExport("CharacterBodyType", RepLayoutCmdType.Enum, 57, "CharacterBodyType", "TEnumAsByte<EFortCustomBodyType::Type>", 4)]
        public int? CharacterBodyType { get; set; } //Type: TEnumAsByte<EFortCustomBodyType::Type> Bits: 4

        [NetFieldExport("WasReplicatedFlags", RepLayoutCmdType.PropertyByte, 58, "WasReplicatedFlags", "uint8", 8)]
        public byte? WasReplicatedFlags { get; set; } //Type: uint8 Bits: 8

        [NetFieldExport("Parts", RepLayoutCmdType.Pointer, 59, "Parts", "UCustomCharacterPart*", 16)]
        public uint? Parts { get; set; } //Type: UCustomCharacterPart* Bits: 16

        [NetFieldExport("WasPartReplicatedFlags", RepLayoutCmdType.PropertyUInt32, 59, "WasPartReplicatedFlags", "", 8)]
        public uint? WasPartReplicatedFlags { get; set; } //Type:  Bits: 8

        [NetFieldExport("RequiredVariantPartFlags", RepLayoutCmdType.PropertyUInt32, 60, "RequiredVariantPartFlags", "", 32)]
        public uint? RequiredVariantPartFlags { get; set; } //Type:  Bits: 32

        [NetFieldExport("VariantRequiredCharacterParts", RepLayoutCmdType.DynamicArray, 71, "VariantRequiredCharacterParts", "", 160)]
        public int[] VariantRequiredCharacterParts { get; set; } //Type:  Bits: 160

        [NetFieldExport("PlayerTeamPrivate", RepLayoutCmdType.Pointer, 75, "PlayerTeamPrivate", "AFortTeamPrivateInfo*", 8)]
        public uint? PlayerTeamPrivate { get; set; } //Type: AFortTeamPrivateInfo* Bits: 8

        [NetFieldExport("PlatformUniqueNetId", RepLayoutCmdType.PropertyNetId, 215, "PlatformUniqueNetId", "FUniqueNetIdRepl", 96)]
        public string PlatformUniqueNetId { get; set; } //Type: FUniqueNetIdRepl Bits: 96

        [NetFieldExport("TeamIndex", RepLayoutCmdType.Enum, 237, "TeamIndex", "TEnumAsByte<EFortTeam::Type>", 7)]
        public int? TeamIndex { get; set; } //Type: TEnumAsByte<EFortTeam::Type> Bits: 7

        [NetFieldExport("Place", RepLayoutCmdType.PropertyInt, 240, "Place", "int32", 32)]
        public int? Place { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("ReplicatedTeamMemberState", RepLayoutCmdType.Enum, 244, "ReplicatedTeamMemberState", "EReplicatedTeamMemberState", 4)]
        public int? ReplicatedTeamMemberState { get; set; } //Type:  Bits: 4

        [NetFieldExport("bHasEverSkydivedFromBus", RepLayoutCmdType.PropertyBool, 249, "bHasEverSkydivedFromBus", "", 1)]
        public bool? bHasEverSkydivedFromBus { get; set; } //Type:  Bits: 1

        [NetFieldExport("bHasEverSkydivedFromBusAndLanded", RepLayoutCmdType.PropertyBool, 250, "bHasEverSkydivedFromBusAndLanded", "", 1)]
        public bool? bHasEverSkydivedFromBusAndLanded { get; set; } //Type:  Bits: 1

        [NetFieldExport("SquadListUpdateValue", RepLayoutCmdType.PropertyInt, 253, "SquadListUpdateValue", "int32", 32)]
        public int? SquadListUpdateValue { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("SquadId", RepLayoutCmdType.PropertyByte, 254, "SquadId", "uint8", 8)]
        public byte? SquadId { get; set; } //Type: uint8 Bits: 8

        [NetFieldExport("bInAircraft", RepLayoutCmdType.PropertyBool, 255, "bInAircraft", "uint8", 1)]
        public bool? bInAircraft { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bThankedBusDriver", RepLayoutCmdType.PropertyBool, 256, "bThankedBusDriver", "uint8", 1)]
        public bool? bThankedBusDriver { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("TeamKillScore", RepLayoutCmdType.PropertyUInt32, 257, "TeamKillScore", "", 32)]
        public uint? TeamKillScore { get; set; } //Type:  Bits: 32

        [NetFieldExport("bUsingStreamerMode", RepLayoutCmdType.PropertyBool, 257, "bUsingStreamerMode", "uint8", 1)]
        public bool? bUsingStreamerMode { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("StreamerModeName", RepLayoutCmdType.Property, 258, "StreamerModeName", "FText", 448)]
        public FText StreamerModeName { get; set; } //Type: FText Bits: 448

        [NetFieldExport("TeamScorePlacement", RepLayoutCmdType.PropertyUInt32, 259, "TeamScorePlacement", "", 32)]
        public uint? TeamScorePlacement { get; set; } //Type:  Bits: 32

        [NetFieldExport("IconId", RepLayoutCmdType.PropertyString, 259, "IconId", "FString", 144)]
        public string IconId { get; set; } //Type: FString Bits: 144

        [NetFieldExport("TeamScore", RepLayoutCmdType.PropertyUInt32, 260, "TeamScore", "", 32)]
        public uint? TeamScore { get; set; } //Type:  Bits: 32

        [NetFieldExport("ColorId", RepLayoutCmdType.PropertyString, 260, "ColorId", "FString", 152)]
        public string ColorId { get; set; } //Type: FString Bits: 152

        [NetFieldExport("Level", RepLayoutCmdType.PropertyInt, 261, "Level", "int32", 32)]
        public int? Level { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("MapIndicatorPos", RepLayoutCmdType.Property, 262, "MapIndicatorPos", "FVector2D", 64)]
        public FVector2D MapIndicatorPos { get; set; } //Type: FVector2D Bits: 64

        [NetFieldExport("KillScore", RepLayoutCmdType.PropertyUInt32, 263, "KillScore", "int32", 32)]
        public uint? KillScore { get; set; } //Type:  Bits: 32

        [NetFieldExport("FinisherOrDowner", RepLayoutCmdType.Pointer, 263, "FinisherOrDowner", "AFortPlayerStateAthena*", 16)]
        public uint? FinisherOrDowner { get; set; } //Type: AFortPlayerStateAthena* Bits: 16

        [NetFieldExport("SeasonLevelUIDisplay", RepLayoutCmdType.PropertyUInt32, 264, "SeasonLevelUIDisplay", "", 32)]
        public uint? SeasonLevelUIDisplay { get; set; } //Type:  Bits: 32

        [NetFieldExport("bInitialized", RepLayoutCmdType.PropertyBool, 267, "bInitialized", "bool", 1)]
        public bool? bInitialized { get; set; } //Type: bool Bits: 1

        [NetFieldExport("DeathCircumstance", RepLayoutCmdType.PropertyInt, 268, "DeathCircumstance", "int32", 32)]
        public int? DeathCircumstance { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("bUsingAnonymousMode", RepLayoutCmdType.PropertyBool, 283, "bUsingAnonymousMode", "", 1)]
        public bool? bUsingAnonymousMode { get; set; } //Type:  Bits: 1

        [NetFieldExport("bIsDisconnected", RepLayoutCmdType.PropertyBool, 285, "bIsDisconnected", "", 1)]
        public bool? bIsDisconnected { get; set; } //Type:  Bits: 1

        [NetFieldExport("bDBNO", RepLayoutCmdType.PropertyBool, 287, "bDBNO", "", 1)]
        public bool? bDBNO { get; set; } //Type:  Bits: 1

        [NetFieldExport("DeathCause", RepLayoutCmdType.Enum, 288, "DeathCause", "EDeathCause", 6)]
        public int? DeathCause { get; set; } //Type:  Bits: 6

        [NetFieldExport("Distance", RepLayoutCmdType.PropertyFloat, 289, "Distance", "float", 32)]
        public float? Distance { get; set; } //Type:  Bits: 32

        [NetFieldExport("DeathTags", RepLayoutCmdType.Ignore, 291, "DeathTags", "", 40)]
        public DebuggingObject DeathTags { get; set; } //Type:  Bits: 40

        [NetFieldExport("bResurrectionChipAvailable", RepLayoutCmdType.PropertyBool, 294, "bResurrectionChipAvailable", "", 1)]
        public bool? bResurrectionChipAvailable { get; set; } //Type:  Bits: 1

        [NetFieldExport("ResurrectionExpirationTime", RepLayoutCmdType.PropertyFloat, 295, "ResurrectionExpirationTime", "float", 32)]
        public float? ResurrectionExpirationTime { get; set; } //Type:  Bits: 32

        [NetFieldExport("ResurrectionExpirationLength", RepLayoutCmdType.PropertyUInt32, 296, "ResurrectionExpirationLength", "", 32)]
        public uint? ResurrectionExpirationLength { get; set; } //Type:  Bits: 32

        [NetFieldExport("WorldLocation", RepLayoutCmdType.Ignore, 297, "WorldLocation", "", 96)]
        public DebuggingObject WorldLocation { get; set; } //Type:  Bits: 96

        [NetFieldExport("bResurrectingNow", RepLayoutCmdType.PropertyBool, 298, "bResurrectingNow", "", 1)]
        public bool? bResurrectingNow { get; set; } //Type:  Bits: 1

        [NetFieldExport("RebootCounter", RepLayoutCmdType.PropertyUInt32, 299, "RebootCounter", "", 32)]
        public uint? RebootCounter { get; set; } //Type:  Bits: 32

        [NetFieldExport("bHoldsRebootVanLock", RepLayoutCmdType.PropertyBool, 300, "bHoldsRebootVanLock", "", 1)]
        public bool? bHoldsRebootVanLock { get; set; } //Type:  Bits: 1

        [NetFieldExport("BotUniqueId", RepLayoutCmdType.PropertyNetId, 300, "BotUniqueId", "FUniqueNetIdRepl", 128)]
        public string BotUniqueId { get; set; } //Type:  Bits: 128

    }
}