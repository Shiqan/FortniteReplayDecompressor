using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportClassNetCache("Athena_GameState_C_ClassNetCache", minimalParseMode: ParseMode.Minimal)]
public class GameStateCache
{
    [NetFieldExportRPC("ActiveGameplayModifiers", "/Script/FortniteGame.ActiveGameplayModifier", enablePropertyChecksum: false)]
    public ActiveGameplayModifier[] ActiveGameplayModifiers { get; set; }

    [NetFieldExportRPC("GameMemberInfoArray", "/Script/FortniteGame.GameMemberInfo", enablePropertyChecksum: false)]
    public GameMemberInfo[] GameMemberInfoArray { get; set; }

    [NetFieldExportRPC("CurrentPlaylistInfo", "CurrentPlaylistInfo", customStruct: true)]
    public PlaylistInfo CurrentPlaylistInfo { get; set; }

    [NetFieldExportRPC("SpawnMachineRepData", "/Script/FortniteGame.SpawnMachineRepData", enablePropertyChecksum: false)]
    public SpawnMachineRepData SpawnMachineRepData { get; set; }
}

[NetFieldExportGroup("/Game/Athena/Athena_GameState.Athena_GameState_C", minimalParseMode: ParseMode.Minimal)]
public class GameState : INetFieldExportGroup
{
    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public object RemoteRole { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public object Role { get; set; }

    [NetFieldExport("GameModeClass", RepLayoutCmdType.Ignore)]
    public ItemDefinition GameModeClass { get; set; }

    [NetFieldExport("SpectatorClass", RepLayoutCmdType.Ignore)]
    public uint? SpectatorClass { get; set; }

    [NetFieldExport("FortTimeOfDayManager", RepLayoutCmdType.Ignore)]
    public uint? FortTimeOfDayManager { get; set; }

    [NetFieldExport("PoiManager", RepLayoutCmdType.Ignore)]
    public uint? PoiManager { get; set; }

    [NetFieldExport("FeedbackManager", RepLayoutCmdType.Ignore)]
    public uint? FeedbackManager { get; set; }

    [NetFieldExport("MissionManager", RepLayoutCmdType.Ignore)]
    public uint? MissionManager { get; set; }

    [NetFieldExport("AnnouncementManager", RepLayoutCmdType.Ignore)]
    public uint? AnnouncementManager { get; set; }

    [NetFieldExport("WorldManager", RepLayoutCmdType.Ignore)]
    public uint? WorldManager { get; set; }

    [NetFieldExport("MusicManagerSubclass", RepLayoutCmdType.Ignore)]
    public uint? MusicManagerSubclass { get; set; }

    [NetFieldExport("MusicManagerBank", RepLayoutCmdType.Ignore)]
    public uint? MusicManagerBank { get; set; }

    [NetFieldExport("PawnForReplayRelevancy", RepLayoutCmdType.Ignore)]
    public uint? PawnForReplayRelevancy { get; set; }

    [NetFieldExport("RecorderPlayerState", RepLayoutCmdType.Property)]
    public ActorGuid? RecorderPlayerState { get; set; }

    [NetFieldExport("GlobalEnvironmentAbilityActor", RepLayoutCmdType.Ignore)]
    public uint? GlobalEnvironmentAbilityActor { get; set; }

    [NetFieldExport("UIMapManager", RepLayoutCmdType.Ignore)]
    public uint? UIMapManager { get; set; }

    [NetFieldExport("CreativePlotManager", RepLayoutCmdType.Ignore)]
    public uint? CreativePlotManager { get; set; }

    [NetFieldExport("PlayspaceManager", RepLayoutCmdType.Ignore)]
    public uint? PlayspaceManager { get; set; }

    [NetFieldExport("ItemCollector", RepLayoutCmdType.Ignore)]
    public uint? ItemCollector { get; set; }

    [NetFieldExport("SpecialActorData", RepLayoutCmdType.Ignore)]
    public uint? SpecialActorData { get; set; }

    [NetFieldExport("SupplyDropWaveStartedSoundCue", RepLayoutCmdType.Ignore)]
    public uint? SupplyDropWaveStartedSoundCue { get; set; }

    [NetFieldExport("TeamXPlayersLeft", RepLayoutCmdType.Ignore)]
    public int? TeamXPlayersLeft { get; set; }

    [NetFieldExport("SafeZoneIndicator", RepLayoutCmdType.Ignore)]
    public uint? SafeZoneIndicator { get; set; }

    [NetFieldExport("MapInfo", RepLayoutCmdType.Ignore)]
    public ItemDefinition MapInfo { get; set; }

    [NetFieldExport("GoldenPoiLocationTags", RepLayoutCmdType.Property)]
    public FGameplayTagContainer GoldenPoiLocationTags { get; set; }

    [NetFieldExport("DefaultBattleBus", RepLayoutCmdType.Property)]
    public ItemDefinition DefaultBattleBus { get; set; }

    [NetFieldExport("bReplicatedHasBegunPlay", RepLayoutCmdType.PropertyBool)]
    public bool? bReplicatedHasBegunPlay { get; set; }

    [NetFieldExport("ReplicatedWorldTimeSeconds", RepLayoutCmdType.PropertyFloat)]
    public float? ReplicatedWorldTimeSeconds { get; set; }

    [NetFieldExport("ReplicatedWorldTimeSecondsDouble", RepLayoutCmdType.PropertyDouble)]
    public double? ReplicatedWorldTimeSecondsDouble { get; set; }

    [NetFieldExport("ReplicatedWorldRealTimeSecondsDouble", RepLayoutCmdType.PropertyDouble)]
    public double? ReplicatedWorldRealTimeSecondsDouble { get; set; }

    [NetFieldExport("MatchState", RepLayoutCmdType.Property)]
    public FName MatchState { get; set; }

    [NetFieldExport("ElapsedTime", RepLayoutCmdType.PropertyInt)]
    public int? ElapsedTime { get; set; }

    [NetFieldExport("WorldLevel", RepLayoutCmdType.PropertyInt)]
    public int? WorldLevel { get; set; }

    [NetFieldExport("CraftingBonus", RepLayoutCmdType.PropertyInt)]
    public int? CraftingBonus { get; set; }

    [NetFieldExport("TeamCount", RepLayoutCmdType.PropertyInt)]
    public int? TeamCount { get; set; }

    [NetFieldExport("TeamSize", RepLayoutCmdType.PropertyInt)]
    public int? TeamSize { get; set; }

    [NetFieldExport("GameFlagData", RepLayoutCmdType.PropertyInt)]
    public int? GameFlagData { get; set; }

    [NetFieldExport("AdditionalPlaylistLevelsStreamed", RepLayoutCmdType.DynamicArray)]
    public FName[] AdditionalPlaylistLevelsStreamed { get; set; }

    [NetFieldExport("WorldDaysElapsed", RepLayoutCmdType.PropertyInt)]
    public int? WorldDaysElapsed { get; set; }

    [NetFieldExport("GameplayState", RepLayoutCmdType.Enum)]
    public int? GameplayState { get; set; }

    [NetFieldExport("GameSessionId", RepLayoutCmdType.PropertyString)]
    public string GameSessionId { get; set; }

    [NetFieldExport("SpawnPointsCap", RepLayoutCmdType.PropertyInt)]
    public int? SpawnPointsCap { get; set; }

    [NetFieldExport("SpawnPointsAllocated", RepLayoutCmdType.PropertyInt)]
    public int? SpawnPointsAllocated { get; set; }

    [NetFieldExport("PlayerSharedMaxTrapAttributes", RepLayoutCmdType.DynamicArray)]
    public float[] PlayerSharedMaxTrapAttributes { get; set; }

    [NetFieldExport("TotalPlayerStructures", RepLayoutCmdType.PropertyInt)]
    public int? TotalPlayerStructures { get; set; }

    [NetFieldExport("ServerGameplayTagIndexHash", RepLayoutCmdType.PropertyUInt32)]
    public uint? ServerGameplayTagIndexHash { get; set; }

    [NetFieldExport("GameDifficulty", RepLayoutCmdType.PropertyFloat)]
    public float? GameDifficulty { get; set; }

    [NetFieldExport("bAllowLayoutRequirementsFeature", RepLayoutCmdType.PropertyBool)]
    public bool? bAllowLayoutRequirementsFeature { get; set; }

    [NetFieldExport("ServerStability", RepLayoutCmdType.Enum)]
    public int? ServerStability { get; set; }

    [NetFieldExport("RoundTimeAccumulated", RepLayoutCmdType.PropertyInt)]
    public int? RoundTimeAccumulated { get; set; }

    [NetFieldExport("RoundTimeCriticalThreshold", RepLayoutCmdType.PropertyInt)]
    public int? RoundTimeCriticalThreshold { get; set; }

    [NetFieldExport("ServerChangelistNumber", RepLayoutCmdType.PropertyInt)]
    public int? ServerChangelistNumber { get; set; }

    [NetFieldExport("CreativeRealEstatePlotManager", RepLayoutCmdType.Ignore)]
    public uint? CreativeRealEstatePlotManager { get; set; }

    [NetFieldExport("WarmupCountdownStartTime", RepLayoutCmdType.PropertyFloat)]
    public float? WarmupCountdownStartTime { get; set; }

    [NetFieldExport("WarmupCountdownEndTime", RepLayoutCmdType.PropertyFloat)]
    public float? WarmupCountdownEndTime { get; set; }

    [NetFieldExport("bSafeZonePaused", RepLayoutCmdType.PropertyBool)]
    public bool? bSafeZonePaused { get; set; }

    [NetFieldExport("AircraftStartTime", RepLayoutCmdType.PropertyFloat)]
    public float? AircraftStartTime { get; set; }

    [NetFieldExport("bSkyTubesShuttingDown", RepLayoutCmdType.PropertyBool)]
    public bool? bSkyTubesShuttingDown { get; set; }

    [NetFieldExport("SafeZonesStartTime", RepLayoutCmdType.PropertyFloat)]
    public float? SafeZonesStartTime { get; set; }

    [NetFieldExport("bSkyTubesDisabled", RepLayoutCmdType.PropertyBool)]
    public bool? bSkyTubesDisabled { get; set; }

    [NetFieldExport("PlayersLeft", RepLayoutCmdType.PropertyInt)]
    public int? PlayersLeft { get; set; }

    [NetFieldExport("ReplOverrideData", RepLayoutCmdType.Ignore)]
    public uint? ReplOverrideData { get; set; }

    [NetFieldExport("EndGameStartTime", RepLayoutCmdType.PropertyFloat)]
    public float? EndGameStartTime { get; set; }

    [NetFieldExport("TeamsLeft", RepLayoutCmdType.PropertyInt)]
    public int? TeamsLeft { get; set; }

    [NetFieldExport("EndGameKickPlayerTime", RepLayoutCmdType.PropertyFloat)]
    public float? EndGameKickPlayerTime { get; set; }

    [NetFieldExport("ServerToClientPreloadList", RepLayoutCmdType.Ignore)]
    public ItemDefinition[] ServerToClientPreloadList { get; set; }

    [NetFieldExport("ClientVehicleClassesToLoad", RepLayoutCmdType.Ignore)]
    public ItemDefinition[] ClientVehicleClassesToLoad { get; set; }

    [NetFieldExport("bAllowUserPickedCosmeticBattleBus", RepLayoutCmdType.PropertyBool)]
    public bool? bAllowUserPickedCosmeticBattleBus { get; set; }

    [NetFieldExport("TeamFlightPaths", RepLayoutCmdType.DynamicArray)]
    public Aircraft[] TeamFlightPaths { get; set; }

    [NetFieldExport("StormCapState", RepLayoutCmdType.Enum)]
    public int? StormCapState { get; set; }

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

    [NetFieldExport("UtcTimeStartedMatch", RepLayoutCmdType.Property)]
    public FDateTime UtcTimeStartedMatch { get; set; }

    [NetFieldExport("SafeZonePhase", RepLayoutCmdType.PropertyByte)]
    public byte? SafeZonePhase { get; set; }

    [NetFieldExport("GamePhase", RepLayoutCmdType.Enum)]
    public int? GamePhase { get; set; }

    [NetFieldExport("Aircrafts", RepLayoutCmdType.DynamicArray)]
    public ItemDefinition[] Aircrafts { get; set; }

    [NetFieldExport("bAircraftIsLocked", RepLayoutCmdType.PropertyBool)]
    public bool? bAircraftIsLocked { get; set; }

    [NetFieldExport("LobbyAction", RepLayoutCmdType.PropertyInt)]
    public int? LobbyAction { get; set; }

    [NetFieldExport("WinningPlayerState", RepLayoutCmdType.Property)]
    public ActorGuid WinningPlayerState { get; set; }

    [NetFieldExport("WinningPlayerList", RepLayoutCmdType.DynamicArray)]
    public int[] WinningPlayerList { get; set; }

    [NetFieldExport("WinningTeam", RepLayoutCmdType.PropertyUInt32)]
    public uint? WinningTeam { get; set; }

    [NetFieldExport("WinningScore", RepLayoutCmdType.PropertyUInt32)]
    public uint? WinningScore { get; set; }

    [NetFieldExport("CurrentHighScore", RepLayoutCmdType.PropertyUInt32)]
    public uint? CurrentHighScore { get; set; }

    [NetFieldExport("CurrentHighScoreTeam", RepLayoutCmdType.PropertyUInt32)]
    public uint? CurrentHighScoreTeam { get; set; }

    [NetFieldExport("bStormReachedFinalPosition", RepLayoutCmdType.PropertyBool)]
    public bool? bStormReachedFinalPosition { get; set; }

    [NetFieldExport("SpectateAPartyMemberAvailable", RepLayoutCmdType.PropertyBool)]
    public bool? SpectateAPartyMemberAvailable { get; set; }

    [NetFieldExport("HopRockDuration", RepLayoutCmdType.PropertyFloat)]
    public float? HopRockDuration { get; set; }

    [NetFieldExport("bIsLargeTeamGame", RepLayoutCmdType.PropertyBool)]
    public bool? bIsLargeTeamGame { get; set; }

    [NetFieldExport("ActiveTeamNums", RepLayoutCmdType.DynamicArray)]
    public NetworkGUID[] ActiveTeamNums { get; set; }

    [NetFieldExport("AirCraftBehavior", RepLayoutCmdType.Enum)]
    public int? AirCraftBehavior { get; set; }

    [NetFieldExport("DefaultGliderRedeployCanRedeploy", RepLayoutCmdType.PropertyFloat)]
    public float? DefaultGliderRedeployCanRedeploy { get; set; }

    [NetFieldExport("DefaultRedeployGliderLateralVelocityMult", RepLayoutCmdType.PropertyFloat)]
    public float? DefaultRedeployGliderLateralVelocityMult { get; set; }

    [NetFieldExport("DefaultRedeployGliderHeightLimit", RepLayoutCmdType.PropertyFloat)]
    public float? DefaultRedeployGliderHeightLimit { get; set; }

    [NetFieldExport("EventTournamentRound", RepLayoutCmdType.Enum)]
    public int? EventTournamentRound { get; set; }

    [NetFieldExport("EventId", RepLayoutCmdType.PropertyInt)]
    public int? EventId { get; set; }

    [NetFieldExport("PlayerBotsLeft", RepLayoutCmdType.PropertyInt)]
    public int? PlayerBotsLeft { get; set; }

    [NetFieldExport("DefaultParachuteDeployTraceForGroundDistance", RepLayoutCmdType.PropertyFloat)]
    public float? DefaultParachuteDeployTraceForGroundDistance { get; set; }

    [NetFieldExport("DefaultRebootMachineHotfix", RepLayoutCmdType.PropertyFloat)]
    public float? DefaultRebootMachineHotfix { get; set; }

    [NetFieldExport("SignalInStormRegenSpeed", RepLayoutCmdType.PropertyFloat)]
    public float? SignalInStormRegenSpeed { get; set; }

    [NetFieldExport("MutatorGenericInt", RepLayoutCmdType.PropertyUInt32)]
    public uint? MutatorGenericInt { get; set; }

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

    [NetFieldExport("bEnabled", RepLayoutCmdType.PropertyBool)]
    public bool? bEnabled { get; set; }

    [NetFieldExport("bConnectedToRoot", RepLayoutCmdType.PropertyBool)]
    public bool? bConnectedToRoot { get; set; }

    [NetFieldExport("GameServerNodeType", RepLayoutCmdType.Enum)]
    public int? GameServerNodeType { get; set; }

    [NetFieldExport("VolumeManager", RepLayoutCmdType.Ignore)]
    public uint? VolumeManager { get; set; }

    [NetFieldExport("TrackedCosmetics", RepLayoutCmdType.DynamicArray)]
    public ItemDefinition[] TrackedCosmetics { get; set; }

    [NetFieldExport("VariantUsageByCosmetic", RepLayoutCmdType.DynamicArray)]
    public ItemDefinition[] VariantUsageByCosmetic { get; set; }

    [NetFieldExport("PrioritizedCosmeticIndices", RepLayoutCmdType.DynamicArray)]
    public ItemDefinition[] PrioritizedCosmeticIndices { get; set; }

    [NetFieldExport("Mappings", RepLayoutCmdType.DynamicArray)]
    public ItemDefinition[] Mappings { get; set; }

    [NetFieldExport("PlayersLoaded", RepLayoutCmdType.PropertyFloat)]
    public float PlayersLoaded { get; set; }

    [NetFieldExport("bIsCustomMatch", RepLayoutCmdType.PropertyBool)]
    public bool bIsCustomMatch { get; set; }

    [NetFieldExport("bCraftingEnabled", RepLayoutCmdType.PropertyBool)]
    public bool bCraftingEnabled { get; set; }

    [NetFieldExport("MatchStartTime", RepLayoutCmdType.PropertyFloat)]
    public float MatchStartTime { get; set; }

    [NetFieldExport("RealMatchStartTime", RepLayoutCmdType.PropertyDouble)]
    public double RealMatchStartTime { get; set; }

}