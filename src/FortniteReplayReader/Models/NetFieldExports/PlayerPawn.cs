using FortniteReplayReader.Models.NetFieldExports.RPC;
using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportClassNetCache("PlayerPawn_Athena_C_ClassNetCache", minimalParseMode: ParseMode.Full)]
public class PlayerPawnCache
{
    [NetFieldExportRPC("ClientObservedStats", "/Script/FortniteGame.FortClientObservedStat", enablePropertyChecksum: false)]
    public FortClientObservedStat ClientObservedStats { get; set; }

    //[NetFieldExportRPCFunction("FastSharedReplication", "/Script/FortniteGame.FortPlayerPawnAthena:FastSharedReplication")]
    //public FRepMovement FastSharedReplication { get; set; }

    [NetFieldExportRPC("NetMulticast_Athena_BatchedDamageCues", "/Script/FortniteGame.FortPawn:NetMulticast_Athena_BatchedDamageCues", isFunction: true)]
    public BatchedDamageCues DamageCues { get; set; }

    [NetFieldExportRPC("NetMulticast_InvokeGameplayCueAdded_WithParams", "/Script/FortniteGame.FortPawn:NetMulticast_InvokeGameplayCueAdded_WithParams", isFunction: true)]
    public GameplayCue InvokeGameplayCueAdded { get; set; }
}

[NetFieldExportGroup("/Game/Athena/PlayerPawn_Athena.PlayerPawn_Athena_C", minimalParseMode: ParseMode.Minimal)]
public class PlayerPawn : INetFieldExportGroup
{
    [NetFieldExport("Owner", RepLayoutCmdType.Property)]
    public ActorGuid Owner { get; set; }

    [NetFieldExport("bHidden", RepLayoutCmdType.Ignore)]
    public bool? bHidden { get; set; }

    [NetFieldExport("bReplicateMovement", RepLayoutCmdType.Ignore)]
    public bool? bReplicateMovement { get; set; }

    [NetFieldExport("bTearOff", RepLayoutCmdType.Ignore)]
    public bool? bTearOff { get; set; }

    [NetFieldExport("bCanBeDamaged", RepLayoutCmdType.PropertyBool)]
    public bool? bCanBeDamaged { get; set; }

    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public int? RemoteRole { get; set; }

    [NetFieldExport("ReplicatedMovement", RepLayoutCmdType.RepMovement, ParseMode.Full)]
    public FRepMovement? ReplicatedMovement { get; set; }

    [NetFieldExport("AttachParent", RepLayoutCmdType.Ignore)]
    public uint? AttachParent { get; set; }

    [NetFieldExport("LocationOffset", RepLayoutCmdType.PropertyVector100)]
    public FVector LocationOffset { get; set; }

    [NetFieldExport("RelativeScale3D", RepLayoutCmdType.PropertyVector100)]
    public FVector RelativeScale3D { get; set; }

    [NetFieldExport("RotationOffset", RepLayoutCmdType.PropertyRotator)]
    public FRotator RotationOffset { get; set; }

    [NetFieldExport("AttachSocket", RepLayoutCmdType.Property)]
    public FName AttachSocket { get; set; }

    [NetFieldExport("ExitSocketIndex", RepLayoutCmdType.PropertyByte)]
    public byte? ExitSocketIndex { get; set; }

    [NetFieldExport("AttachComponent", RepLayoutCmdType.PropertyObject)]
    public uint? AttachComponent { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public object Role { get; set; }

    [NetFieldExport("Instigator", RepLayoutCmdType.PropertyObject)]
    public uint? Instigator { get; set; }

    [NetFieldExport("RemoteViewPitch", RepLayoutCmdType.PropertyByte)]
    public byte? RemoteViewPitch { get; set; }

    [NetFieldExport("PlayerState", RepLayoutCmdType.PropertyObject)]
    public uint? PlayerState { get; set; }

    [NetFieldExport("Controller", RepLayoutCmdType.PropertyObject)]
    public uint? Controller { get; set; }

    [NetFieldExport("MovementBase", RepLayoutCmdType.PropertyObject)]
    public uint? MovementBase { get; set; }

    [NetFieldExport("BoneName", RepLayoutCmdType.Property)]
    public FName BoneName { get; set; }

    [NetFieldExport("Location", RepLayoutCmdType.PropertyVector100)]
    public FVector Location { get; set; }

    [NetFieldExport("Rotation", RepLayoutCmdType.PropertyRotator)]
    public FRotator Rotation { get; set; }

    [NetFieldExport("bServerHasBaseComponent", RepLayoutCmdType.PropertyBool)]
    public bool? bServerHasBaseComponent { get; set; }

    [NetFieldExport("bRelativeRotation", RepLayoutCmdType.PropertyBool)]
    public bool? bRelativeRotation { get; set; }

    [NetFieldExport("bServerHasVelocity", RepLayoutCmdType.PropertyBool)]
    public bool? bServerHasVelocity { get; set; }

    [NetFieldExport("ReplayLastTransformUpdateTimeStamp", RepLayoutCmdType.PropertyFloat)]
    public float? ReplayLastTransformUpdateTimeStamp { get; set; }

    [NetFieldExport("ReplicatedMovementMode", RepLayoutCmdType.PropertyByte)]
    public byte? ReplicatedMovementMode { get; set; }

    [NetFieldExport("bIsCrouched", RepLayoutCmdType.PropertyBool)]
    public bool? bIsCrouched { get; set; }

    [NetFieldExport("bProxyIsJumpForceApplied", RepLayoutCmdType.PropertyBool)]
    public bool? bProxyIsJumpForceApplied { get; set; }

    [NetFieldExport("bIsActive", RepLayoutCmdType.PropertyBool)]
    public bool? bIsActive { get; set; }

    [NetFieldExport("Position", RepLayoutCmdType.PropertyFloat)]
    public float? Position { get; set; }

    [NetFieldExport("Acceleration", RepLayoutCmdType.Ignore)]
    public object Acceleration { get; set; }

    [NetFieldExport("LinearVelocity", RepLayoutCmdType.PropertyVector10)]
    public FVector LinearVelocity { get; set; }

    [NetFieldExport("CurrentMovementStyle", RepLayoutCmdType.Enum)]
    public int? CurrentMovementStyle { get; set; }

    [NetFieldExport("bIgnoreNextFallingDamage", RepLayoutCmdType.PropertyBool)]
    public bool? bIgnoreNextFallingDamage { get; set; }

    [NetFieldExport("TeleportCounter", RepLayoutCmdType.PropertyByte)]
    public byte? TeleportCounter { get; set; }

    [NetFieldExport("PawnUniqueID", RepLayoutCmdType.PropertyInt)]
    public int? PawnUniqueID { get; set; }

    [NetFieldExport("bIsDying", RepLayoutCmdType.PropertyBool)]
    public bool? bIsDying { get; set; }

    [NetFieldExport("CurrentWeapon", RepLayoutCmdType.PropertyObject)]
    public uint? CurrentWeapon { get; set; }

    [NetFieldExport("bIsInvulnerable", RepLayoutCmdType.PropertyBool)]
    public bool? bIsInvulnerable { get; set; }

    [NetFieldExport("bMovingEmote", RepLayoutCmdType.PropertyBool)]
    public bool? bMovingEmote { get; set; }

    [NetFieldExport("bWeaponActivated", RepLayoutCmdType.PropertyBool)]
    public bool? bWeaponActivated { get; set; }

    [NetFieldExport("bIsDBNO", RepLayoutCmdType.PropertyBool)]
    public bool? bIsDBNO { get; set; }

    [NetFieldExport("bWasDBNOOnDeath", RepLayoutCmdType.PropertyBool)]
    public bool? bWasDBNOOnDeath { get; set; }

    [NetFieldExport("JumpFlashCount", RepLayoutCmdType.PropertyByte)]
    public byte? JumpFlashCount { get; set; }

    [NetFieldExport("bWeaponHolstered", RepLayoutCmdType.PropertyBool)]
    public bool? bWeaponHolstered { get; set; }

    [NetFieldExport("FeedbackAudioComponent", RepLayoutCmdType.Ignore)]
    public uint? FeedbackAudioComponent { get; set; }

    [NetFieldExport("VocalChords", RepLayoutCmdType.Ignore)]
    public object[] VocalChords { get; set; }

    [NetFieldExport("SpawnImmunityTime", RepLayoutCmdType.PropertyFloat)]
    public float? SpawnImmunityTime { get; set; }

    [NetFieldExport("JumpFlashCountPacked", RepLayoutCmdType.Ignore)]
    public uint? JumpFlashCountPacked { get; set; }

    [NetFieldExport("LandingFlashCountPacked", RepLayoutCmdType.Ignore)]
    public uint? LandingFlashCountPacked { get; set; }

    [NetFieldExport("bInterruptCurrentLine", RepLayoutCmdType.PropertyBool)]
    public bool? bInterruptCurrentLine { get; set; }

    [NetFieldExport("LastReplicatedEmoteExecuted", RepLayoutCmdType.PropertyObject)]
    public uint? LastReplicatedEmoteExecuted { get; set; }

    [NetFieldExport("bCanBeInterrupted", RepLayoutCmdType.PropertyBool)]
    public bool? bCanBeInterrupted { get; set; }

    [NetFieldExport("bCanQue", RepLayoutCmdType.PropertyBool)]
    public bool? bCanQue { get; set; }

    [NetFieldExport("ForwardAlpha", RepLayoutCmdType.PropertyFloat)]
    public float? ForwardAlpha { get; set; }

    [NetFieldExport("RightAlpha", RepLayoutCmdType.PropertyFloat)]
    public float? RightAlpha { get; set; }

    [NetFieldExport("TurnDelta", RepLayoutCmdType.PropertyFloat)]
    public float? TurnDelta { get; set; }

    [NetFieldExport("SteerAlpha", RepLayoutCmdType.PropertyFloat)]
    public float? SteerAlpha { get; set; }

    [NetFieldExport("GravityScale", RepLayoutCmdType.PropertyFloat)]
    public float? GravityScale { get; set; }

    [NetFieldExport("WorldLookDir", RepLayoutCmdType.PropertyVectorQ)]
    public FVector WorldLookDir { get; set; }

    [NetFieldExport("bIgnoreForwardInAir", RepLayoutCmdType.PropertyBool)]
    public bool? bIgnoreForwardInAir { get; set; }

    [NetFieldExport("bIsHonking", RepLayoutCmdType.PropertyBool)]
    public bool? bIsHonking { get; set; }

    [NetFieldExport("bIsJumping", RepLayoutCmdType.PropertyBool)]
    public bool? bIsJumping { get; set; }

    [NetFieldExport("bIsSprinting", RepLayoutCmdType.PropertyBool)]
    public bool? bIsSprinting { get; set; }

    [NetFieldExport("Vehicle", RepLayoutCmdType.PropertyObject)]
    public uint? Vehicle { get; set; }

    [NetFieldExport("VehicleApexZ", RepLayoutCmdType.PropertyFloat)]
    public float? VehicleApexZ { get; set; }

    [NetFieldExport("SeatIndex", RepLayoutCmdType.PropertyByte)]
    public byte? SeatIndex { get; set; }

    [NetFieldExport("bIsWaterJump", RepLayoutCmdType.PropertyBool)]
    public bool? bIsWaterJump { get; set; }

    [NetFieldExport("bIsWaterSprintBoost", RepLayoutCmdType.PropertyBool)]
    public bool? bIsWaterSprintBoost { get; set; }

    [NetFieldExport("bIsWaterSprintBoostPending", RepLayoutCmdType.PropertyBool)]
    public bool? bIsWaterSprintBoostPending { get; set; }

    [NetFieldExport("StasisMode", RepLayoutCmdType.Ignore)]
    public object StasisMode { get; set; }

    [NetFieldExport("BuildingState", RepLayoutCmdType.Enum)]
    public int? BuildingState { get; set; }

    [NetFieldExport("bIsTargeting", RepLayoutCmdType.PropertyBool)]
    public bool? bIsTargeting { get; set; }

    [NetFieldExport("PawnMontage", RepLayoutCmdType.PropertyObject)]
    public uint? PawnMontage { get; set; }

    [NetFieldExport("bPlayBit", RepLayoutCmdType.PropertyBool)]
    public bool? bPlayBit { get; set; }

    [NetFieldExport("bIsPlayingEmote", RepLayoutCmdType.PropertyBool)]
    public bool? bIsPlayingEmote { get; set; }

    [NetFieldExport("FootstepBankOverride", RepLayoutCmdType.PropertyObject)]
    public uint? FootstepBankOverride { get; set; }

    [NetFieldExport("PackedReplicatedSlopeAngles", RepLayoutCmdType.PropertyUInt16)]
    public ushort? PackedReplicatedSlopeAngles { get; set; }

    [NetFieldExport("bStartedInteractSearch", RepLayoutCmdType.PropertyBool)]
    public bool? bStartedInteractSearch { get; set; }

    [NetFieldExport("AccelerationPack", RepLayoutCmdType.PropertyUInt16)]
    public ushort? AccelerationPack { get; set; }

    [NetFieldExport("AccelerationZPack", RepLayoutCmdType.PropertyByte)]
    public byte? AccelerationZPack { get; set; }

    [NetFieldExport("bIsWaitingForEmoteInteraction", RepLayoutCmdType.PropertyBool)]
    public bool? bIsWaitingForEmoteInteraction { get; set; }

    [NetFieldExport("GroupEmoteLookTarget", RepLayoutCmdType.Property)]
    public ItemDefinition GroupEmoteLookTarget { get; set; }

    [NetFieldExport("bIsSkydiving", RepLayoutCmdType.PropertyBool)]
    public bool? bIsSkydiving { get; set; }

    [NetFieldExport("bIsParachuteOpen", RepLayoutCmdType.PropertyBool)]
    public bool? bIsParachuteOpen { get; set; }

    [NetFieldExport("bIsParachuteForcedOpen", RepLayoutCmdType.PropertyBool)]
    public bool? bIsParachuteForcedOpen { get; set; }

    [NetFieldExport("bIsSkydivingFromBus", RepLayoutCmdType.PropertyBool)]
    public bool? bIsSkydivingFromBus { get; set; }

    [NetFieldExport("bReplicatedIsInSlipperyMovement", RepLayoutCmdType.PropertyBool)]
    public bool? bReplicatedIsInSlipperyMovement { get; set; }

    [NetFieldExport("MovementDir", RepLayoutCmdType.Ignore)]
    public object MovementDir { get; set; }

    [NetFieldExport("bIsInAnyStorm", RepLayoutCmdType.PropertyBool)]
    public bool? bIsInAnyStorm { get; set; }

    [NetFieldExport("bIsSlopeSliding", RepLayoutCmdType.PropertyBool)]
    public bool? bIsSlopeSliding { get; set; }

    [NetFieldExport("bIsProxySimulationTimedOut", RepLayoutCmdType.PropertyBool)]
    public bool? bIsProxySimulationTimedOut { get; set; }

    [NetFieldExport("bIsInsideSafeZone", RepLayoutCmdType.PropertyBool)]
    public bool? bIsInsideSafeZone { get; set; }

    [NetFieldExport("bIsOutsideSafeZone", RepLayoutCmdType.PropertyBool)]
    public bool? bIsOutsideSafeZone { get; set; }

    [NetFieldExport("Zipline", RepLayoutCmdType.Property)]
    public ItemDefinition Zipline { get; set; }

    [NetFieldExport("PetState", RepLayoutCmdType.PropertyObject)]
    public uint? PetState { get; set; }

    [NetFieldExport("bIsZiplining", RepLayoutCmdType.PropertyBool)]
    public bool? bIsZiplining { get; set; }

    [NetFieldExport("bJumped", RepLayoutCmdType.PropertyBool)]
    public bool? bJumped { get; set; }

    [NetFieldExport("ParachuteAttachment", RepLayoutCmdType.PropertyObject)]
    public uint? ParachuteAttachment { get; set; }

    [NetFieldExport("AuthoritativeValue", RepLayoutCmdType.Ignore)]
    public uint AuthoritativeValue { get; set; }

    [NetFieldExport("AuthoritativeRootMotion", RepLayoutCmdType.Ignore)]
    public object AuthoritativeRootMotion { get; set; }

    [NetFieldExport("SocketOffset", RepLayoutCmdType.Ignore)]
    public object SocketOffset { get; set; }

    [NetFieldExport("RemoteViewData32", RepLayoutCmdType.PropertyUInt32)]
    public uint? RemoteViewData32 { get; set; }

    [NetFieldExport("bNetMovementPrioritized", RepLayoutCmdType.PropertyBool)]
    public bool? bNetMovementPrioritized { get; set; }

    [NetFieldExport("EntryTime", RepLayoutCmdType.PropertyUInt32)]
    public uint? EntryTime { get; set; }

    [NetFieldExport("CapsuleRadiusAthena", RepLayoutCmdType.PropertyFloat)]
    public float? CapsuleRadiusAthena { get; set; }

    [NetFieldExport("CapsuleHalfHeightAthena", RepLayoutCmdType.PropertyFloat)]
    public float? CapsuleHalfHeightAthena { get; set; }

    [NetFieldExport("WalkSpeed", RepLayoutCmdType.PropertyFloat)]
    public float? WalkSpeed { get; set; }

    [NetFieldExport("RunSpeed", RepLayoutCmdType.PropertyFloat)]
    public float? RunSpeed { get; set; }

    [NetFieldExport("SprintSpeed", RepLayoutCmdType.PropertyFloat)]
    public float? SprintSpeed { get; set; }

    [NetFieldExport("CrouchedRunSpeed", RepLayoutCmdType.PropertyFloat)]
    public float? CrouchedRunSpeed { get; set; }

    [NetFieldExport("CrouchedSprintSpeed", RepLayoutCmdType.PropertyFloat)]
    public float? CrouchedSprintSpeed { get; set; }

    [NetFieldExport("AnimMontage", RepLayoutCmdType.Ignore)]
    public uint? AnimMontage { get; set; }

    [NetFieldExport("AnimRootMotionTranslationScale", RepLayoutCmdType.Ignore)]
    public uint? AnimRootMotionTranslationScale { get; set; }

    [NetFieldExport("PlayRate", RepLayoutCmdType.PropertyFloat)]
    public float? PlayRate { get; set; }

    [NetFieldExport("BlendTime", RepLayoutCmdType.PropertyFloat)]
    public float? BlendTime { get; set; }

    [NetFieldExport("ForcePlayBit", RepLayoutCmdType.PropertyBool)]
    public bool? ForcePlayBit { get; set; }

    [NetFieldExport("IsStopped", RepLayoutCmdType.PropertyBool)]
    public bool? IsStopped { get; set; }

    [NetFieldExport("SkipPositionCorrection", RepLayoutCmdType.PropertyBool)]
    public bool? SkipPositionCorrection { get; set; }

    [NetFieldExport("RepAnimMontageStartSection", RepLayoutCmdType.PropertyInt)]
    public int? RepAnimMontageStartSection { get; set; }

    [NetFieldExport("ReplayRepAnimMontageInfo", RepLayoutCmdType.Ignore)]
    public FGameplayAbilityRepAnimMontage ReplayRepAnimMontageInfo { get; set; }

    [NetFieldExport("RepAnimMontageInfo", RepLayoutCmdType.Ignore)]
    public FGameplayAbilityRepAnimMontage RepAnimMontageInfo { get; set; }

    [NetFieldExport("SimulatedProxyGameplayCues", RepLayoutCmdType.Enum)]
    public int? SimulatedProxyGameplayCues { get; set; }

    [NetFieldExport("WeaponActivated", RepLayoutCmdType.PropertyBool)]
    public bool? WeaponActivated { get; set; }

    [NetFieldExport("bIsInWaterVolume", RepLayoutCmdType.PropertyBool)]
    public bool? bIsInWaterVolume { get; set; }

    [NetFieldExport("BannerIconId", RepLayoutCmdType.PropertyString)]
    public string BannerIconId { get; set; }

    [NetFieldExport("BannerColorId", RepLayoutCmdType.PropertyString)]
    public string BannerColorId { get; set; }

    [NetFieldExport("ItemWraps", RepLayoutCmdType.DynamicArray)]
    public ItemDefinition[] ItemWraps { get; set; }

    [NetFieldExport("SkyDiveContrail", RepLayoutCmdType.Property)]
    public ItemDefinition SkyDiveContrail { get; set; }

    [NetFieldExport("Glider", RepLayoutCmdType.Property)]
    public ItemDefinition Glider { get; set; }

    [NetFieldExport("Pickaxe", RepLayoutCmdType.Property)]
    public ItemDefinition Pickaxe { get; set; }

    [NetFieldExport("bIsDefaultCharacter", RepLayoutCmdType.PropertyBool)]
    public bool? bIsDefaultCharacter { get; set; }

    [NetFieldExport("Character", RepLayoutCmdType.Property)]
    public ItemDefinition Character { get; set; }

    [NetFieldExport("CharacterVariantChannels", RepLayoutCmdType.Ignore)]
    public uint[] CharacterVariantChannels { get; set; }

    [NetFieldExport("DBNOHoister", RepLayoutCmdType.Property)]
    public ActorGuid DBNOHoister { get; set; }

    [NetFieldExport("DBNOCarryEvent", RepLayoutCmdType.Ignore)]
    public DebuggingObject DBNOCarryEvent { get; set; }

    [NetFieldExport("Backpack", RepLayoutCmdType.Property)]
    public ItemDefinition Backpack { get; set; }

    [NetFieldExport("LoadingScreen", RepLayoutCmdType.Property)]
    public ItemDefinition LoadingScreen { get; set; }

    [NetFieldExport("Dances", RepLayoutCmdType.DynamicArray)]
    public ItemDefinition[] Dances { get; set; }

    [NetFieldExport("MusicPack", RepLayoutCmdType.Property)]
    public ItemDefinition MusicPack { get; set; }

    [NetFieldExport("PetSkin", RepLayoutCmdType.Property)]
    public ItemDefinition PetSkin { get; set; }

    [NetFieldExport("EncryptedPawnReplayData", RepLayoutCmdType.Property)]
    public FAthenaPawnReplayData EncryptedPawnReplayData { get; set; }

    [NetFieldExport("GravityFloorAltitude", RepLayoutCmdType.PropertyUInt32)]
    public uint? GravityFloorAltitude { get; set; }

    [NetFieldExport("GravityFloorWidth", RepLayoutCmdType.PropertyUInt32)]
    public uint? GravityFloorWidth { get; set; }

    [NetFieldExport("GravityFloorGravityScalar", RepLayoutCmdType.PropertyUInt32)]
    public uint? GravityFloorGravityScalar { get; set; }

    [NetFieldExport("ReplicatedWaterBody", RepLayoutCmdType.PropertyObject)]
    public uint? ReplicatedWaterBody { get; set; }

    [NetFieldExport("DBNORevivalStacking", RepLayoutCmdType.PropertyByte)]
    public byte? DBNORevivalStacking { get; set; }

    [NetFieldExport("ServerWorldTimeRevivalTime", RepLayoutCmdType.PropertyUInt32)]
    public uint? ServerWorldTimeRevivalTime { get; set; }

    [NetFieldExport("ItemSpecialActorID", RepLayoutCmdType.Ignore)]
    public object ItemSpecialActorID { get; set; }

    [NetFieldExport("FlySpeed", RepLayoutCmdType.PropertyFloat)]
    public float? FlySpeed { get; set; }

    [NetFieldExport("NextSectionID", RepLayoutCmdType.Ignore)]
    public uint? NextSectionID { get; set; }

    [NetFieldExport("FastReplicationMinimalReplicationTags", RepLayoutCmdType.Ignore)]
    public object FastReplicationMinimalReplicationTags { get; set; }

    [NetFieldExport("bIsCreativeGhostModeActivated", RepLayoutCmdType.Ignore)]
    public bool? bIsCreativeGhostModeActivated { get; set; }

    [NetFieldExport("PlayRespawnFXOnSpawn", RepLayoutCmdType.Ignore)]
    public bool? PlayRespawnFXOnSpawn { get; set; }

    [NetFieldExport("bIsSkydivingFromLaunchPad", RepLayoutCmdType.PropertyBool)]
    public bool? bIsSkydivingFromLaunchPad { get; set; }

    [NetFieldExport("bInGliderRedeploy", RepLayoutCmdType.PropertyBool)]
    public bool? bInGliderRedeploy { get; set; }

    [NetFieldExport("bReplicatedIsInVortex", RepLayoutCmdType.PropertyBool)]
    public bool? bReplicatedIsInVortex { get; set; }

    [NetFieldExport("bIsTacticalSprinting", RepLayoutCmdType.PropertyBool)]
    public bool? bIsTacticalSprinting { get; set; }
}