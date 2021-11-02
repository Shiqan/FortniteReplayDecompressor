using FortniteReplayReader.Models.NetFieldExports.RPC;
using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportClassNetCache("FortPlayerStateAthena_ClassNetCache", minimalParseMode: ParseMode.Debug)]
    public class FortPlayerStateCache
    {
        [NetFieldExportRPC("Client_OnNewLevel", "/Script/FortniteGame.FortPlayerStateAthena:Client_OnNewLevel", isFunction: true)]
        public OnNewLevel Client_OnNewLevel { get; set; }
    }


    [NetFieldExportGroup("/Script/FortniteGame.FortPlayerStateAthena", minimalParseMode: ParseMode.Minimal)]
    public class FortPlayerState : INetFieldExportGroup
    {
        [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
        public object RemoteRole { get; set; }

        [NetFieldExport("Owner", RepLayoutCmdType.Ignore)]
        public uint? Owner { get; set; }

        [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
        public object Role { get; set; }

        [NetFieldExport("Instigator", RepLayoutCmdType.PropertyObject)]
        public uint? Instigator { get; set; }

        [NetFieldExport("Score", RepLayoutCmdType.PropertyUInt32)]
        public uint? Score { get; set; }

        [NetFieldExport("PlayerID", RepLayoutCmdType.PropertyInt)]
        public int? PlayerID { get; set; }

        [NetFieldExport("Ping", RepLayoutCmdType.Ignore)]
        public byte? Ping { get; set; }

        [NetFieldExport("bIsABot", RepLayoutCmdType.PropertyBool)]
        public bool? bIsABot { get; set; }

        [NetFieldExport("bIsSpectator", RepLayoutCmdType.PropertyBool)]
        public bool? bIsSpectator { get; set; }

        [NetFieldExport("bOnlySpectator", RepLayoutCmdType.PropertyBool)]
        public bool? bOnlySpectator { get; set; }

        [NetFieldExport("StartTime", RepLayoutCmdType.PropertyInt)]
        public int? StartTime { get; set; }

        [NetFieldExport("UniqueId", RepLayoutCmdType.PropertyNetId)]
        public string UniqueId { get; set; }

        [NetFieldExport("PlayerNamePrivate", RepLayoutCmdType.Ignore)]
        public string PlayerNamePrivate { get; set; }

        [NetFieldExport("bIsGameSessionOwner", RepLayoutCmdType.PropertyBool)]
        public bool? bIsGameSessionOwner { get; set; }

        [NetFieldExport("bHasFinishedLoading", RepLayoutCmdType.PropertyBool)]
        public bool? bHasFinishedLoading { get; set; }

        [NetFieldExport("bHasStartedPlaying", RepLayoutCmdType.PropertyBool)]
        public bool? bHasStartedPlaying { get; set; }

        [NetFieldExport("PlayerRole", RepLayoutCmdType.Enum)]
        public int? PlayerRole { get; set; }

        [NetFieldExport("PartyOwnerUniqueId", RepLayoutCmdType.PropertyNetId)]
        public string PartyOwnerUniqueId { get; set; }

        [NetFieldExport("WorldPlayerId", RepLayoutCmdType.PropertyInt)]
        public int? WorldPlayerId { get; set; }

        [NetFieldExport("HeroType", RepLayoutCmdType.Property)]
        public ItemDefinition HeroType { get; set; }

        [NetFieldExport("Platform", RepLayoutCmdType.PropertyString)]
        public string Platform { get; set; }

        [NetFieldExport("CharacterGender", RepLayoutCmdType.Enum)]
        public int? CharacterGender { get; set; }

        [NetFieldExport("CharacterBodyType", RepLayoutCmdType.Enum)]
        public int? CharacterBodyType { get; set; }

        [NetFieldExport("WasReplicatedFlags", RepLayoutCmdType.Ignore)]
        public byte WasReplicatedFlags { get; set; }

        [NetFieldExport("Parts", RepLayoutCmdType.Property)]
        public ItemDefinition Parts { get; set; }

        [NetFieldExport("WasPartReplicatedFlags", RepLayoutCmdType.Ignore)]
        public uint? WasPartReplicatedFlags { get; set; }

        [NetFieldExport("RequiredVariantPartFlags", RepLayoutCmdType.PropertyUInt32)]
        public uint? RequiredVariantPartFlags { get; set; }

        [NetFieldExport("VariantRequiredCharacterParts", RepLayoutCmdType.DynamicArray)]
        public ItemDefinition[] VariantRequiredCharacterParts { get; set; }

        [NetFieldExport("PlayerTeamPrivate", RepLayoutCmdType.PropertyObject)]
        public uint? PlayerTeamPrivate { get; set; }

        [NetFieldExport("PlatformUniqueNetId", RepLayoutCmdType.PropertyNetId)]
        public string PlatformUniqueNetId { get; set; }

        [NetFieldExport("TeamIndex", RepLayoutCmdType.Enum)]
        public int? TeamIndex { get; set; }

        [NetFieldExport("Place", RepLayoutCmdType.PropertyInt)]
        public int? Place { get; set; }

        [NetFieldExport("ReplicatedTeamMemberState", RepLayoutCmdType.Enum)]
        public int? ReplicatedTeamMemberState { get; set; }

        [NetFieldExport("bHasEverSkydivedFromBus", RepLayoutCmdType.PropertyBool)]
        public bool? bHasEverSkydivedFromBus { get; set; }

        [NetFieldExport("bHasEverSkydivedFromBusAndLanded", RepLayoutCmdType.PropertyBool)]
        public bool? bHasEverSkydivedFromBusAndLanded { get; set; }

        [NetFieldExport("SquadListUpdateValue", RepLayoutCmdType.PropertyInt)]
        public int? SquadListUpdateValue { get; set; }

        [NetFieldExport("SquadId", RepLayoutCmdType.PropertyByte)]
        public byte SquadId { get; set; }

        [NetFieldExport("bInAircraft", RepLayoutCmdType.PropertyBool)]
        public bool? bInAircraft { get; set; }

        [NetFieldExport("bThankedBusDriver", RepLayoutCmdType.PropertyBool)]
        public bool? bThankedBusDriver { get; set; }

        [NetFieldExport("bDidNotThankBusDriver", RepLayoutCmdType.PropertyBool)]
        public bool? bDidNotThankBusDriver { get; set; }

        [NetFieldExport("TeamKillScore", RepLayoutCmdType.PropertyUInt32)]
        public uint? TeamKillScore { get; set; }

        [NetFieldExport("bUsingStreamerMode", RepLayoutCmdType.PropertyBool)]
        public bool? bUsingStreamerMode { get; set; }

        [NetFieldExport("StreamerModeName", RepLayoutCmdType.Property)]
        public FText StreamerModeName { get; set; }

        [NetFieldExport("PlayerNameCustomOverride", RepLayoutCmdType.Property)]
        public FText PlayerNameCustomOverride { get; set; }

        [NetFieldExport("TeamScorePlacement", RepLayoutCmdType.PropertyUInt32)]
        public uint? TeamScorePlacement { get; set; }

        [NetFieldExport("IconId", RepLayoutCmdType.PropertyString)]
        public string IconId { get; set; }

        [NetFieldExport("TeamScore", RepLayoutCmdType.PropertyUInt32)]
        public uint? TeamScore { get; set; }

        [NetFieldExport("ColorId", RepLayoutCmdType.PropertyString)]
        public string ColorId { get; set; }

        [NetFieldExport("Level", RepLayoutCmdType.PropertyInt)]
        public int? Level { get; set; }

        [NetFieldExport("MapIndicatorPos", RepLayoutCmdType.PropertyVector2D)]
        public FVector2D MapIndicatorPos { get; set; }

        [NetFieldExport("KillScore", RepLayoutCmdType.PropertyUInt32)]
        public uint? KillScore { get; set; }

        [NetFieldExport("FinisherOrDowner", RepLayoutCmdType.PropertyObject)]
        public uint? FinisherOrDowner { get; set; }

        [NetFieldExport("SeasonLevelUIDisplay", RepLayoutCmdType.PropertyUInt32)]
        public uint? SeasonLevelUIDisplay { get; set; }

        [NetFieldExport("bInitialized", RepLayoutCmdType.PropertyBool)]
        public bool? bInitialized { get; set; }

        [NetFieldExport("DeathCircumstance", RepLayoutCmdType.PropertyInt)]
        public int? DeathCircumstance { get; set; }

        [NetFieldExport("bUsingAnonymousMode", RepLayoutCmdType.PropertyBool)]
        public bool? bUsingAnonymousMode { get; set; }

        [NetFieldExport("bIsDisconnected", RepLayoutCmdType.PropertyBool)]
        public bool? bIsDisconnected { get; set; }

        [NetFieldExport("bDBNO", RepLayoutCmdType.PropertyBool)]
        public bool? bDBNO { get; set; }

        [NetFieldExport("DeathCause", RepLayoutCmdType.Enum)]
        public int? DeathCause { get; set; }

        [NetFieldExport("Distance", RepLayoutCmdType.PropertyFloat)]
        public float? Distance { get; set; }

        [NetFieldExport("DeathTags", RepLayoutCmdType.Property)]
        public FGameplayTagContainer DeathTags { get; set; }

        [NetFieldExport("bResurrectionChipAvailable", RepLayoutCmdType.PropertyBool)]
        public bool? bResurrectionChipAvailable { get; set; }

        [NetFieldExport("ResurrectionExpirationTime", RepLayoutCmdType.PropertyFloat)]
        public float? ResurrectionExpirationTime { get; set; }

        [NetFieldExport("ResurrectionExpirationLength", RepLayoutCmdType.PropertyFloat)]
        public float? ResurrectionExpirationLength { get; set; }

        [NetFieldExport("WorldLocation", RepLayoutCmdType.Ignore)]
        public FVector WorldLocation { get; set; }

        [NetFieldExport("bResurrectingNow", RepLayoutCmdType.PropertyBool)]
        public bool? bResurrectingNow { get; set; }

        [NetFieldExport("RebootCounter", RepLayoutCmdType.PropertyUInt32)]
        public uint? RebootCounter { get; set; }

        [NetFieldExport("bHoldsRebootVanLock", RepLayoutCmdType.PropertyBool)]
        public bool? bHoldsRebootVanLock { get; set; }

        [NetFieldExport("BotUniqueId", RepLayoutCmdType.PropertyNetId)]
        public string BotUniqueId { get; set; }

        [NetFieldExport("DeathLocation", RepLayoutCmdType.PropertyVector)]
        public FVector DeathLocation { get; set; }

        [NetFieldExport("SimulatedAttributes", RepLayoutCmdType.Ignore)]
        public DebuggingObject SimulatedAttributes { get; set; }

        [NetFieldExport("KickedFromSessionReason", RepLayoutCmdType.Enum)]
        public int? KickedFromSessionReason { get; set; }

        [NetFieldExport("NumRejoins", RepLayoutCmdType.PropertyInt)]
        public int? NumRejoins { get; set; }

        [NetFieldExport("bIsAnAthenaGameParticipant", RepLayoutCmdType.PropertyBool)]
        public bool? bIsAnAthenaGameParticipant { get; set; }
    }
}