using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportGroup("/Game/Athena/Playspace_GameState.Playspace_GameState_C", ParseMode.Full)]
public class PlayspaceGameState : INetFieldExportGroup
{
    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public object RemoteRole { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public object Role { get; set; }

    [NetFieldExport("GameModeClass", RepLayoutCmdType.Ignore)]
    public ItemDefinition GameModeClass { get; set; }

    [NetFieldExport("SpectatorClass", RepLayoutCmdType.Ignore)]
    public uint? SpectatorClass { get; set; }

    [NetFieldExport("bReplicatedHasBegunPlay", RepLayoutCmdType.PropertyBool)]
    public bool? bReplicatedHasBegunPlay { get; set; }

    [NetFieldExport("ReplicatedWorldTimeSecondsDouble", RepLayoutCmdType.PropertyDouble)]
    public double? ReplicatedWorldTimeSecondsDouble { get; set; }

    [NetFieldExport("MatchState", RepLayoutCmdType.Property)]
    public FName MatchState { get; set; }

    [NetFieldExport("ElapsedTime", RepLayoutCmdType.PropertyInt)]
    public int? ElapsedTime { get; set; }

    [NetFieldExport("FortTimeOfDayManager", RepLayoutCmdType.Ignore)]
    public uint? FortTimeOfDayManager { get; set; }

    [NetFieldExport("WorldLevel", RepLayoutCmdType.PropertyInt)]
    public int? WorldLevel { get; set; }

    [NetFieldExport("CraftingBonus", RepLayoutCmdType.PropertyInt)]
    public int? CraftingBonus { get; set; }

    [NetFieldExport("TeamCount", RepLayoutCmdType.PropertyInt)]
    public int? TeamCount { get; set; }

    [NetFieldExport("Mnemonic", RepLayoutCmdType.Ignore)]
    public string Mnemonic { get; set; }

    [NetFieldExport("Version", RepLayoutCmdType.Ignore)]
    public string Version { get; set; }

    [NetFieldExport("MatchmakingLinkType", RepLayoutCmdType.Ignore)]
    public string MatchmakingLinkType { get; set; }

    [NetFieldExport("PoiManager", RepLayoutCmdType.Ignore)]
    public uint? PoiManager { get; set; }

    [NetFieldExport("bFishingCollectionEnabled", RepLayoutCmdType.PropertyBool)]
    public bool? bFishingCollectionEnabled { get; set; }

    [NetFieldExport("MatchStartTime", RepLayoutCmdType.PropertyFloat)]
    public float? MatchStartTime { get; set; }

    [NetFieldExport("RealMatchStartTime", RepLayoutCmdType.PropertyDouble)]
    public double? RealMatchStartTime { get; set; }

    [NetFieldExport("ReplicatedWorldRealTimeSecondsDouble", RepLayoutCmdType.PropertyDouble)]
    public double? ReplicatedWorldRealTimeSecondsDouble { get; set; }

    [NetFieldExport("MissionManager", RepLayoutCmdType.Ignore)]
    public uint? MissionManager { get; set; }

    [NetFieldExport("AnnouncementManager", RepLayoutCmdType.Ignore)]
    public uint? AnnouncementManager { get; set; }

    [NetFieldExport("WorldManager", RepLayoutCmdType.Ignore)]
    public uint? WorldManager { get; set; }

    [NetFieldExport("GameplayState", RepLayoutCmdType.Enum)]
    public int? GameplayState { get; set; }

    [NetFieldExport("GameSessionId", RepLayoutCmdType.PropertyString)]
    public string GameSessionId { get; set; }

    [NetFieldExport("PawnForReplayRelevancy", RepLayoutCmdType.Ignore)]
    public uint? PawnForReplayRelevancy { get; set; }

    [NetFieldExport("RecorderPlayerState", RepLayoutCmdType.Property)]
    public ActorGuid? RecorderPlayerState { get; set; }

    [NetFieldExport("ServerGameplayTagIndexHash", RepLayoutCmdType.PropertyUInt32)]
    public uint? ServerGameplayTagIndexHash { get; set; }

    [NetFieldExport("ItemId", RepLayoutCmdType.Property)]
    public ItemDefinition[] ItemId { get; set; }  // Assuming multiple item IDs can be tracked

    [NetFieldExport("Payload", RepLayoutCmdType.DynamicArray)]
    public string[] Payload { get; set; }  // Assuming a dynamic array of Payload data

    [NetFieldExport("ServerStability", RepLayoutCmdType.Enum)]
    public int? ServerStability { get; set; }

    [NetFieldExport("CreativeRealEstatePlotManager", RepLayoutCmdType.Ignore)]
    public uint? CreativeRealEstatePlotManager { get; set; }

    [NetFieldExport("bIsGrassFireBoundsUpdateEnabled", RepLayoutCmdType.PropertyBool)]
    public bool? bIsGrassFireBoundsUpdateEnabled { get; set; }

    [NetFieldExport("bIsUsingDownloadOnDemand", RepLayoutCmdType.PropertyBool)]
    public bool? bIsUsingDownloadOnDemand { get; set; }

    [NetFieldExport("ServerChangelistNumber", RepLayoutCmdType.PropertyInt)]
    public int? ServerChangelistNumber { get; set; }

    [NetFieldExport("SpecialActorData", RepLayoutCmdType.Ignore)]
    public uint? SpecialActorData { get; set; }

    [NetFieldExport("ReplOverrideData", RepLayoutCmdType.Ignore)]
    public uint? ReplOverrideData { get; set; }

    [NetFieldExport("PlayersLeft", RepLayoutCmdType.PropertyInt)]
    public int? PlayersLeft { get; set; }

    [NetFieldExport("PlayersLoaded", RepLayoutCmdType.PropertyFloat)]
    public float PlayersLoaded { get; set; }

    [NetFieldExport("TeamsLeft", RepLayoutCmdType.PropertyInt)]
    public int? TeamsLeft { get; set; }

    [NetFieldExport("DefaultBattleBus", RepLayoutCmdType.Property)]
    public ItemDefinition DefaultBattleBus { get; set; }

    [NetFieldExport("TeamFlightPaths", RepLayoutCmdType.DynamicArray)]
    public Aircraft[] TeamFlightPaths { get; set; }

    [NetFieldExport("FlightStartLocation", RepLayoutCmdType.PropertyVector100)]
    public FVector FlightStartLocation { get; set; }

    [NetFieldExport("FlightStartRotation", RepLayoutCmdType.PropertyRotator)]
    public FRotator FlightStartRotation { get; set; }

    [NetFieldExport("FlightSpeed", RepLayoutCmdType.PropertyFloat)]
    public float? FlightSpeed { get; set; }

    [NetFieldExport("TimeTillFlightEnd", RepLayoutCmdType.PropertyFloat)]
    public float? TimeTillFlightEnd { get; set; }

    [NetFieldExport("TimeTillDropStart", RepLayoutCmdType.PropertyFloat)]
    public float? TimeTillDropStart { get; set; }

    [NetFieldExport("TimeTillDropEnd", RepLayoutCmdType.PropertyFloat)]
    public float? TimeTillDropEnd { get; set; }

    [NetFieldExport("bEjectPlayersAtDropZoneExit", RepLayoutCmdType.PropertyBool)]
    public bool? bEjectPlayersAtDropZoneExit { get; set; }

    [NetFieldExport("bVisiblePostAthenaAircraftPhase", RepLayoutCmdType.PropertyBool)]
    public bool? bVisiblePostAthenaAircraftPhase { get; set; }

    [NetFieldExport("UtcTimeStartedMatch", RepLayoutCmdType.Property)]
    public FDateTime UtcTimeStartedMatch { get; set; }

    [NetFieldExport("MapInfo", RepLayoutCmdType.Property)]
    public ItemDefinition MapInfo { get; set; }

    [NetFieldExport("DefaultGliderRedeployCanRedeploy", RepLayoutCmdType.PropertyFloat)]
    public float? DefaultGliderRedeployCanRedeploy { get; set; }

    [NetFieldExport("DefaultRedeployGliderLateralVelocityMult", RepLayoutCmdType.PropertyFloat)]
    public float? DefaultRedeployGliderLateralVelocityMult { get; set; }

    [NetFieldExport("DefaultRedeployGliderHeightLimit", RepLayoutCmdType.PropertyFloat)]
    public float? DefaultRedeployGliderHeightLimit { get; set; }

    [NetFieldExport("DefaultParachuteDeployTraceForGroundDistance", RepLayoutCmdType.PropertyFloat)]
    public float? DefaultParachuteDeployTraceForGroundDistance { get; set; }

    [NetFieldExport("SignalInStormRegenSpeed", RepLayoutCmdType.PropertyFloat)]
    public float? SignalInStormRegenSpeed { get; set; }

    [NetFieldExport("SignalInStormLostSpeed", RepLayoutCmdType.PropertyFloat)]
    public float? SignalInStormLostSpeed { get; set; }

    [NetFieldExport("StormCNDamageVulnerabilityLevel0", RepLayoutCmdType.PropertyFloat)]
    public float? StormCNDamageVulnerabilityLevel0 { get; set; }

    [NetFieldExport("StormCNDamageVulnerabilityLevel1", RepLayoutCmdType.PropertyFloat)]
    public float? StormCNDamageVulnerabilityLevel1 { get; set; }

    [NetFieldExport("StormCNDamageVulnerabilityLevel2", RepLayoutCmdType.PropertyFloat)]
    public float? StormCNDamageVulnerabilityLevel2 { get; set; }

    [NetFieldExport("StormCNDamageVulnerabilityLevel3", RepLayoutCmdType.PropertyFloat)]
    public float? StormCNDamageVulnerabilityLevel3 { get; set; }

    [NetFieldExport("VolumeManager", RepLayoutCmdType.Ignore)]
    public uint? VolumeManager { get; set; }

    [NetFieldExport("LocalizationService", RepLayoutCmdType.Ignore)]
    public string LocalizationService { get; set; }

    [NetFieldExport("bDamageComboHUDEnabled", RepLayoutCmdType.PropertyBool)]
    public bool? bDamageComboHUDEnabled { get; set; }

    [NetFieldExport("bCraftingEnabled", RepLayoutCmdType.PropertyBool)]
    public bool? bCraftingEnabled { get; set; }
}
