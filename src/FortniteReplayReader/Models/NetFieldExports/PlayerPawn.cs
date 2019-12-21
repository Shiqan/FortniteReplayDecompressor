using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Game/Athena/PlayerPawn_Athena.PlayerPawn_Athena_C")]
    public class PlayerPawn : INetFieldExportGroup
    {
        [NetFieldExport("bHidden", RepLayoutCmdType.PropertyBool, 0, "bHidden", "", 1)]
        public bool? bHidden { get; set; } //Type:  Bits: 1

        [NetFieldExport("bReplicateMovement", RepLayoutCmdType.PropertyBool, 1, "bReplicateMovement", "uint8", 1)]
        public bool? bReplicateMovement { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bTearOff", RepLayoutCmdType.PropertyBool, 2, "bTearOff", "uint8", 1)]
        public bool? bTearOff { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bCanBeDamaged", RepLayoutCmdType.PropertyBool, 3, "bCanBeDamaged", "uint8", 1)]
        public bool? bCanBeDamaged { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("RemoteRole", RepLayoutCmdType.Enum, 4, "RemoteRole", "TEnumAsByte<ENetRole>", 2)]
        public int? RemoteRole { get; set; } //Type: TEnumAsByte<ENetRole> Bits: 2

        [NetFieldExport("ReplicatedMovement", RepLayoutCmdType.RepMovement, 5, "ReplicatedMovement", "FRepMovement", 127)]
        public FRepMovement ReplicatedMovement { get; set; } //Type: FRepMovement Bits: 127

        [NetFieldExport("AttachParent", RepLayoutCmdType.PropertyObject, 6, "AttachParent", "AActor*", 16)]
        public uint? AttachParent { get; set; } //Type: AActor* Bits: 16

        [NetFieldExport("LocationOffset", RepLayoutCmdType.PropertyVector100, 7, "LocationOffset", "FVector_NetQuantize100", 20)]
        public FVector LocationOffset { get; set; } //Type: FVector_NetQuantize100 Bits: 20

        [NetFieldExport("RelativeScale3D", RepLayoutCmdType.PropertyVector100, 8, "RelativeScale3D", "FVector_NetQuantize100", 29)]
        public FVector RelativeScale3D { get; set; } //Type: FVector_NetQuantize100 Bits: 29

        [NetFieldExport("RotationOffset", RepLayoutCmdType.PropertyRotator, 9, "RotationOffset", "FRotator", 3)]
        public FRotator RotationOffset { get; set; } //Type: FRotator Bits: 3

        [NetFieldExport("AttachSocket", RepLayoutCmdType.Property, 10, "AttachSocket", "FName", 121)]
        public FName AttachSocket { get; set; } //Type: FName Bits: 121

        [NetFieldExport("AttachComponent", RepLayoutCmdType.PropertyObject, 11, "AttachComponent", "USceneComponent*", 16)]
        public uint? AttachComponent { get; set; } //Type: USceneComponent* Bits: 16

        [NetFieldExport("Role", RepLayoutCmdType.Ignore, 13, "Role", "Enum", 2)]
        public object Role { get; set; } //Type: Enum Bits: 2

        [NetFieldExport("Instigator", RepLayoutCmdType.PropertyObject, 14, "Instigator", "APawn*", 8)]
        public uint? Instigator { get; set; } //Type: APawn* Bits: 8

        [NetFieldExport("RemoteViewPitch", RepLayoutCmdType.PropertyByte, 15, "RemoteViewPitch", "uint8", 8)]
        public byte? RemoteViewPitch { get; set; } //Type: uint8 Bits: 8

        [NetFieldExport("PlayerState", RepLayoutCmdType.PropertyObject, 16, "PlayerState", "APlayerState*", 8)]
        public uint? PlayerState { get; set; } //Type: APlayerState* Bits: 8

        [NetFieldExport("Controller", RepLayoutCmdType.PropertyObject, 17, "Controller", "AController*", 8)]
        public uint? Controller { get; set; } //Type: AController* Bits: 8

        [NetFieldExport("MovementBase", RepLayoutCmdType.PropertyObject, 18, "MovementBase", "UPrimitiveComponent*", 16)]
        public uint? MovementBase { get; set; } //Type: UPrimitiveComponent* Bits: 16

        [NetFieldExport("BoneName", RepLayoutCmdType.Property, 19, "BoneName", "FName", 113)]
        public FName BoneName { get; set; } //Type: FName Bits: 113

        [NetFieldExport("Location", RepLayoutCmdType.PropertyVector100, 20, "Location", "FVector_NetQuantize100", 47)]
        public FVector Location { get; set; } //Type: FVector Bits: 47

        [NetFieldExport("Rotation", RepLayoutCmdType.PropertyRotator, 21, "Rotation", "FRotator", 19)]
        public FRotator Rotation { get; set; } //Type: FRotator Bits: 19

        [NetFieldExport("bServerHasBaseComponent", RepLayoutCmdType.PropertyBool, 22, "bServerHasBaseComponent", "bool", 1)]
        public bool? bServerHasBaseComponent { get; set; } //Type: bool Bits: 1

        [NetFieldExport("bRelativeRotation", RepLayoutCmdType.PropertyBool, 23, "bRelativeRotation", "bool", 1)]
        public bool? bRelativeRotation { get; set; } //Type: bool Bits: 1

        [NetFieldExport("bServerHasVelocity", RepLayoutCmdType.PropertyBool, 24, "bServerHasVelocity", "bool", 1)]
        public bool? bServerHasVelocity { get; set; } //Type: bool Bits: 1

        [NetFieldExport("ReplayLastTransformUpdateTimeStamp", RepLayoutCmdType.PropertyFloat, 27, "ReplayLastTransformUpdateTimeStamp", "float", 32)]
        public float? ReplayLastTransformUpdateTimeStamp { get; set; } //Type: float Bits: 32

        [NetFieldExport("ReplicatedMovementMode", RepLayoutCmdType.PropertyByte, 28, "ReplicatedMovementMode", "uint8", 8)]
        public byte? ReplicatedMovementMode { get; set; } //Type: uint8 Bits: 8

        [NetFieldExport("bIsCrouched", RepLayoutCmdType.PropertyBool, 29, "bIsCrouched", "uint8", 1)]
        public bool? bIsCrouched { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bProxyIsJumpForceApplied", RepLayoutCmdType.PropertyBool, 30, "bProxyIsJumpForceApplied", "uint8", 1)]
        public bool? bProxyIsJumpForceApplied { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bIsActive", RepLayoutCmdType.PropertyBool, 33, "bIsActive", "bool", 1)]
        public bool? bIsActive { get; set; } //Type: bool Bits: 1

        [NetFieldExport("Position", RepLayoutCmdType.PropertyFloat, 35, "Position", "float", 32)]
        public float? Position { get; set; } //Type: float Bits: 32

        [NetFieldExport("Acceleration", RepLayoutCmdType.Ignore, 43, "Acceleration", "", 46)]
        public object Acceleration { get; set; } //Type:  Bits: 46

        [NetFieldExport("LinearVelocity", RepLayoutCmdType.PropertyVector10, 44, "LinearVelocity", "FVector_NetQuantize10", 26)]
        public FVector LinearVelocity { get; set; } //Type: FVector_NetQuantize10 Bits: 26

        [NetFieldExport("CurrentMovementStyle", RepLayoutCmdType.Enum, 45, "CurrentMovementStyle", "TEnumAsByte<EFortMovementStyle::Type>", 3)]
        public int? CurrentMovementStyle { get; set; } //Type: TEnumAsByte<EFortMovementStyle::Type> Bits: 3

        [NetFieldExport("bIgnoreNextFallingDamage", RepLayoutCmdType.PropertyBool, 45, "bIgnoreNextFallingDamage", "", 1)]
        public bool? bIgnoreNextFallingDamage { get; set; } //Type:  Bits: 1

        [NetFieldExport("TeleportCounter", RepLayoutCmdType.PropertyByte, 46, "TeleportCounter", "uint8", 8)]
        public byte? TeleportCounter { get; set; } //Type: uint8 Bits: 8

        [NetFieldExport("PawnUniqueID", RepLayoutCmdType.PropertyInt, 48, "PawnUniqueID", "int32", 32)]
        public int? PawnUniqueID { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("bIsDying", RepLayoutCmdType.PropertyBool, 49, "bIsDying", "bool", 1)]
        public bool? bIsDying { get; set; } //Type: bool Bits: 1

        [NetFieldExport("CurrentWeapon", RepLayoutCmdType.PropertyObject, 49, "CurrentWeapon", "AFortWeapon*", 16)]
        public uint? CurrentWeapon { get; set; } //Type: AFortWeapon* Bits: 16

        [NetFieldExport("bIsInvulnerable", RepLayoutCmdType.PropertyBool, 50, "bIsInvulnerable", "", 1)]
        public bool? bIsInvulnerable { get; set; } //Type:  Bits: 1

        [NetFieldExport("bMovingEmote", RepLayoutCmdType.PropertyBool, 51, "bMovingEmote", "", 1)]
        public bool? bMovingEmote { get; set; } //Type:  Bits: 1

        [NetFieldExport("bWeaponActivated", RepLayoutCmdType.PropertyBool, 55, "bWeaponActivated", "", 1)]
        public bool? bWeaponActivated { get; set; } //Type:  Bits: 1

        [NetFieldExport("bIsDBNO", RepLayoutCmdType.PropertyBool, 57, "bIsDBNO", "", 1)]
        public bool? bIsDBNO { get; set; } //Type:  Bits: 1

        [NetFieldExport("bWasDBNOOnDeath", RepLayoutCmdType.PropertyBool, 58, "bWasDBNOOnDeath", "", 1)]
        public bool? bWasDBNOOnDeath { get; set; } //Type:  Bits: 1

        [NetFieldExport("JumpFlashCount", RepLayoutCmdType.PropertyByte, 58, "JumpFlashCount", "uint8", 8)]
        public byte? JumpFlashCount { get; set; } //Type: uint8 Bits: 8

        [NetFieldExport("bWeaponHolstered", RepLayoutCmdType.PropertyBool, 62, "bWeaponHolstered", "bool", 1)]
        public bool? bWeaponHolstered { get; set; } //Type: bool Bits: 1

        [NetFieldExport("FeedbackAudioComponent", RepLayoutCmdType.PropertyObject, 64, "FeedbackAudioComponent", "UAudioComponent*", 0)]
        public uint? FeedbackAudioComponent { get; set; } //Type: UAudioComponent* Bits: 0

        [NetFieldExport("VocalChords", RepLayoutCmdType.DynamicArray, 64, "VocalChords", "TArray", 115)]
        public object[] VocalChords { get; set; } //Type: TArray Bits: 115

        [NetFieldExport("SpawnImmunityTime", RepLayoutCmdType.PropertyFloat, 64, "SpawnImmunityTime", "float", 32)]
        public float? SpawnImmunityTime { get; set; } //Type: float Bits: 32

        [NetFieldExport("JumpFlashCountPacked", RepLayoutCmdType.PropertyUInt32, 69, "JumpFlashCountPacked", "", 8)]
        public uint? JumpFlashCountPacked { get; set; } //Type:  Bits: 8

        [NetFieldExport("LandingFlashCountPacked", RepLayoutCmdType.PropertyUInt32, 70, "LandingFlashCountPacked", "", 8)]
        public uint? LandingFlashCountPacked { get; set; } //Type:  Bits: 8

        [NetFieldExport("bInterruptCurrentLine", RepLayoutCmdType.PropertyBool, 71, "bInterruptCurrentLine", "bool", 33)]
        public bool? bInterruptCurrentLine { get; set; } //Type: bool Bits: 33

        [NetFieldExport("LastReplicatedEmoteExecuted", RepLayoutCmdType.PropertyObject, 71, "LastReplicatedEmoteExecuted", "*", 16)]
        public uint? LastReplicatedEmoteExecuted { get; set; } //Type: * Bits: 16

        [NetFieldExport("bCanBeInterrupted", RepLayoutCmdType.PropertyBool, 72, "bCanBeInterrupted", "bool", 95)]
        public bool? bCanBeInterrupted { get; set; } //Type: bool Bits: 95

        [NetFieldExport("bCanQue", RepLayoutCmdType.PropertyBool, 73, "bCanQue", "bool", 0)]
        public bool? bCanQue { get; set; } //Type: bool Bits: 0

        [NetFieldExport("ForwardAlpha", RepLayoutCmdType.PropertyFloat, 88, "ForwardAlpha", "float", 32)]
        public float? ForwardAlpha { get; set; } //Type: float Bits: 32

        [NetFieldExport("RightAlpha", RepLayoutCmdType.PropertyFloat, 89, "RightAlpha", "float", 32)]
        public float? RightAlpha { get; set; } //Type: float Bits: 32

        [NetFieldExport("TurnDelta", RepLayoutCmdType.PropertyFloat, 91, "TurnDelta", "float", 32)]
        public float? TurnDelta { get; set; } //Type: float Bits: 32

        [NetFieldExport("SteerAlpha", RepLayoutCmdType.PropertyFloat, 92, "SteerAlpha", "float", 32)]
        public float? SteerAlpha { get; set; } //Type: float Bits: 32

        [NetFieldExport("GravityScale", RepLayoutCmdType.PropertyFloat, 93, "GravityScale", "float", 32)]
        public float? GravityScale { get; set; } //Type: float Bits: 32

        [NetFieldExport("WorldLookDir", RepLayoutCmdType.PropertyVectorQ, 94, "WorldLookDir", "FVector_NetQuantize", 11)]
        public FVector WorldLookDir { get; set; } //Type: FVector_NetQuantize Bits: 11

        [NetFieldExport("bIgnoreForwardInAir", RepLayoutCmdType.PropertyBool, 95, "bIgnoreForwardInAir", "uint8", 1)]
        public bool? bIgnoreForwardInAir { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bIsHonking", RepLayoutCmdType.PropertyBool, 96, "bIsHonking", "uint8", 1)]
        public bool? bIsHonking { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bIsJumping", RepLayoutCmdType.PropertyBool, 97, "bIsJumping", "uint8", 1)]
        public bool? bIsJumping { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bIsSprinting", RepLayoutCmdType.PropertyBool, 98, "bIsSprinting", "uint8", 1)]
        public bool? bIsSprinting { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("Vehicle", RepLayoutCmdType.PropertyObject, 103, "Vehicle", "AFortAthenaVehicle*", 16)]
        public uint? Vehicle { get; set; } //Type: AFortAthenaVehicle* Bits: 16

        [NetFieldExport("VehicleApexZ", RepLayoutCmdType.PropertyFloat, 104, "VehicleApexZ", "float", 32)]
        public float? VehicleApexZ { get; set; } //Type: float Bits: 32

        [NetFieldExport("SeatIndex", RepLayoutCmdType.PropertyByte, 105, "SeatIndex", "uint8", 8)]
        public byte? SeatIndex { get; set; } //Type: uint8 Bits: 8

        [NetFieldExport("bIsWaterJump", RepLayoutCmdType.PropertyBool, 108, "bIsWaterJump", "", 1)]
        public bool? bIsWaterJump { get; set; } //Type:  Bits: 1

        [NetFieldExport("bIsWaterSprintBoost", RepLayoutCmdType.PropertyBool, 109, "bIsWaterSprintBoost", "", 1)]
        public bool? bIsWaterSprintBoost { get; set; } //Type:  Bits: 1

        [NetFieldExport("bIsWaterSprintBoostPending", RepLayoutCmdType.PropertyBool, 110, "bIsWaterSprintBoostPending", "", 1)]
        public bool? bIsWaterSprintBoostPending { get; set; } //Type:  Bits: 1

        [NetFieldExport("StasisMode", RepLayoutCmdType.Ignore, 112, "StasisMode", "Enum", 3)]
        public object StasisMode { get; set; } //Type: Enum Bits: 3

        [NetFieldExport("BuildingState", RepLayoutCmdType.Enum, 115, "BuildingState", "TEnumAsByte<EFortBuildingState::Type>", 2)]
        public int? BuildingState { get; set; } //Type: TEnumAsByte<EFortBuildingState::Type> Bits: 2

        [NetFieldExport("bIsTargeting", RepLayoutCmdType.PropertyBool, 116, "bIsTargeting", "bool", 1)]
        public bool? bIsTargeting { get; set; } //Type: bool Bits: 1

        [NetFieldExport("PawnMontage", RepLayoutCmdType.PropertyObject, 127, "PawnMontage", "UAnimMontage*", 16)]
        public uint? PawnMontage { get; set; } //Type: UAnimMontage* Bits: 16

        [NetFieldExport("bPlayBit", RepLayoutCmdType.PropertyBool, 128, "bPlayBit", "bool", 1)]
        public bool? bPlayBit { get; set; } //Type: bool Bits: 1

        [NetFieldExport("bIsPlayingEmote", RepLayoutCmdType.PropertyBool, 128, "bIsPlayingEmote", "", 1)]
        public bool? bIsPlayingEmote { get; set; } //Type:  Bits: 1

        [NetFieldExport("FootstepBankOverride", RepLayoutCmdType.PropertyObject, 129, "FootstepBankOverride", "UFortFootstepAudioBank*", 16)]
        public uint? FootstepBankOverride { get; set; } //Type: UFortFootstepAudioBank* Bits: 16

        [NetFieldExport("PackedReplicatedSlopeAngles", RepLayoutCmdType.PropertyUInt16, 130, "PackedReplicatedSlopeAngles", "uint16", 16)]
        public ushort? PackedReplicatedSlopeAngles { get; set; } //Type: uint16 Bits: 16

        [NetFieldExport("bStartedInteractSearch", RepLayoutCmdType.PropertyBool, 131, "bStartedInteractSearch", "bool", 1)]
        public bool? bStartedInteractSearch { get; set; } //Type:  Bits: 1

        [NetFieldExport("AccelerationPack", RepLayoutCmdType.PropertyUInt16, 133, "AccelerationPack", "uint16", 16)]
        public ushort? AccelerationPack { get; set; } //Type: uint16 Bits: 16

        [NetFieldExport("AccelerationZPack", RepLayoutCmdType.PropertyByte, 134, "AccelerationZPack", "int8", 8)]
        public byte? AccelerationZPack { get; set; } //Type: int8 Bits: 8

        [NetFieldExport("bIsWaitingForEmoteInteraction", RepLayoutCmdType.PropertyBool, 135, "bIsWaitingForEmoteInteraction", "", 1)]
        public bool? bIsWaitingForEmoteInteraction { get; set; } //Type:  Bits: 1

        [NetFieldExport("GroupEmoteLookTarget", RepLayoutCmdType.PropertyUInt32, 138, "GroupEmoteLookTarget", "", 16)]
        public uint? GroupEmoteLookTarget { get; set; } //Type:  Bits: 16

        [NetFieldExport("bIsSkydiving", RepLayoutCmdType.PropertyBool, 141, "bIsSkydiving", "bool", 1)]
        public bool? bIsSkydiving { get; set; } //Type: bool Bits: 1

        [NetFieldExport("bIsParachuteOpen", RepLayoutCmdType.PropertyBool, 142, "bIsParachuteOpen", "bool", 1)]
        public bool? bIsParachuteOpen { get; set; } //Type: bool Bits: 1

        [NetFieldExport("bIsParachuteForcedOpen", RepLayoutCmdType.PropertyBool, 143, "bIsParachuteForcedOpen", "bool", 1)]
        public bool? bIsParachuteForcedOpen { get; set; } //Type: bool Bits: 1

        [NetFieldExport("bIsSkydivingFromBus", RepLayoutCmdType.PropertyBool, 144, "bIsSkydivingFromBus", "bool", 1)]
        public bool? bIsSkydivingFromBus { get; set; } //Type: bool Bits: 1

        [NetFieldExport("bReplicatedIsInSlipperyMovement", RepLayoutCmdType.PropertyBool, 146, "bReplicatedIsInSlipperyMovement", "bool", 1)]
        public bool? bReplicatedIsInSlipperyMovement { get; set; } //Type: bool Bits: 1

        [NetFieldExport("MovementDir", RepLayoutCmdType.Ignore, 146, "MovementDir", "", 26)]
        public object MovementDir { get; set; } //Type:  Bits: 26

        [NetFieldExport("bIsInAnyStorm", RepLayoutCmdType.PropertyBool, 147, "bIsInAnyStorm", "", 1)]
        public bool? bIsInAnyStorm { get; set; } //Type:  Bits: 1

        [NetFieldExport("bIsSlopeSliding", RepLayoutCmdType.PropertyBool, 147, "bIsSlopeSliding", "bool", 1)]
        public bool? bIsSlopeSliding { get; set; } //Type: bool Bits: 1

        [NetFieldExport("bIsProxySimulationTimedOut", RepLayoutCmdType.PropertyBool, 148, "bIsProxySimulationTimedOut", "bool", 1)]
        public bool? bIsProxySimulationTimedOut { get; set; } //Type: bool Bits: 1

        [NetFieldExport("bIsInsideSafeZone", RepLayoutCmdType.PropertyBool, 148, "bIsInsideSafeZone", "", 1)]
        public bool? bIsInsideSafeZone { get; set; } //Type:  Bits: 1

        [NetFieldExport("Zipline", RepLayoutCmdType.PropertyUInt32, 149, "Zipline", "", 16)]
        public uint? Zipline { get; set; } //Type:  Bits: 16

        [NetFieldExport("PetState", RepLayoutCmdType.PropertyObject, 149, "PetState", "AFortPlayerPetRepState*", 8)]
        public uint? PetState { get; set; } //Type: AFortPlayerPetRepState* Bits: 8

        [NetFieldExport("bIsZiplining", RepLayoutCmdType.PropertyBool, 150, "bIsZiplining", "", 1)]
        public bool? bIsZiplining { get; set; } //Type:  Bits: 1

        [NetFieldExport("bJumped", RepLayoutCmdType.PropertyBool, 151, "bJumped", "", 1)]
        public bool? bJumped { get; set; } //Type:  Bits: 1

        [NetFieldExport("ParachuteAttachment", RepLayoutCmdType.PropertyObject, 151, "ParachuteAttachment", "AFortPlayerParachute*", 16)]
        public uint? ParachuteAttachment { get; set; } //Type: AFortPlayerParachute* Bits: 16

        [NetFieldExport("AuthoritativeValue", RepLayoutCmdType.PropertyUInt32, 152, "AuthoritativeValue", "", 32)]
        public uint? AuthoritativeValue { get; set; } //Type:  Bits: 32

        [NetFieldExport("SocketOffset", RepLayoutCmdType.Ignore, 153, "SocketOffset", "", 96)]
        public object SocketOffset { get; set; } //Type:  Bits: 96

        [NetFieldExport("RemoteViewData32", RepLayoutCmdType.PropertyUInt32, 153, "RemoteViewData32", "uint32", 32)]
        public uint? RemoteViewData32 { get; set; } //Type: uint32 Bits: 32

        [NetFieldExport("bNetMovementPrioritized", RepLayoutCmdType.PropertyBool, 159, "bNetMovementPrioritized", "bool", 1)]
        public bool? bNetMovementPrioritized { get; set; } //Type: bool Bits: 1

        [NetFieldExport("EntryTime", RepLayoutCmdType.PropertyUInt32, 161, "EntryTime", "", 32)]
        public uint? EntryTime { get; set; } //Type:  Bits: 32

        [NetFieldExport("CapsuleRadiusAthena", RepLayoutCmdType.PropertyFloat, 161, "CapsuleRadiusAthena", "float", 32)]
        public float? CapsuleRadiusAthena { get; set; } //Type: float Bits: 32

        [NetFieldExport("CapsuleHalfHeightAthena", RepLayoutCmdType.PropertyFloat, 162, "CapsuleHalfHeightAthena", "float", 32)]
        public float? CapsuleHalfHeightAthena { get; set; } //Type: float Bits: 32

        [NetFieldExport("WalkSpeed", RepLayoutCmdType.PropertyFloat, 164, "WalkSpeed", "float", 32)]
        public float? WalkSpeed { get; set; } //Type: float Bits: 32

        [NetFieldExport("RunSpeed", RepLayoutCmdType.PropertyFloat, 165, "RunSpeed", "float", 32)]
        public float? RunSpeed { get; set; } //Type: float Bits: 32

        [NetFieldExport("SprintSpeed", RepLayoutCmdType.PropertyFloat, 166, "SprintSpeed", "float", 32)]
        public float? SprintSpeed { get; set; } //Type: float Bits: 32

        [NetFieldExport("CrouchedRunSpeed", RepLayoutCmdType.PropertyFloat, 167, "CrouchedRunSpeed", "float", 32)]
        public float? CrouchedRunSpeed { get; set; } //Type: float Bits: 32

        [NetFieldExport("CrouchedSprintSpeed", RepLayoutCmdType.PropertyFloat, 168, "CrouchedSprintSpeed", "float", 32)]
        public float? CrouchedSprintSpeed { get; set; } //Type: float Bits: 32

        [NetFieldExport("AnimMontage", RepLayoutCmdType.PropertyObject, 169, "AnimMontage", "UAnimMontage*", 16)]
        public uint? AnimMontage { get; set; } //Type: UAnimMontage* Bits: 16

        [NetFieldExport("PlayRate", RepLayoutCmdType.PropertyFloat, 170, "PlayRate", "float", 32)]
        public float? PlayRate { get; set; } //Type: float Bits: 32

        [NetFieldExport("BlendTime", RepLayoutCmdType.PropertyFloat, 172, "BlendTime", "float", 32)]
        public float? BlendTime { get; set; } //Type: float Bits: 32

        [NetFieldExport("ForcePlayBit", RepLayoutCmdType.PropertyBool, 175, "ForcePlayBit", "uint8", 1)]
        public bool? ForcePlayBit { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("IsStopped", RepLayoutCmdType.PropertyBool, 176, "IsStopped", "uint8", 1)]
        public bool? IsStopped { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("SkipPositionCorrection", RepLayoutCmdType.PropertyBool, 177, "SkipPositionCorrection", "uint8", 1)]
        public bool? SkipPositionCorrection { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("RepAnimMontageStartSection", RepLayoutCmdType.PropertyInt, 179, "RepAnimMontageStartSection", "int32", 32)]
        public int? RepAnimMontageStartSection { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("SimulatedProxyGameplayCues", RepLayoutCmdType.Enum, 190, "SimulatedProxyGameplayCues", "FMinimalGameplayCueReplicationProxy", 5)]
        public int? SimulatedProxyGameplayCues { get; set; } //Type: FMinimalGameplayCueReplicationProxy Bits: 5

        [NetFieldExport("ItemWraps", RepLayoutCmdType.Ignore, 190, "ItemWraps", "", 408)]
        public object ItemWraps { get; set; } //Type:  Bits: 408

        [NetFieldExport("WeaponActivated", RepLayoutCmdType.PropertyBool, 195, "WeaponActivated", "bool", 1)]
        public bool? WeaponActivated { get; set; } //Type: bool Bits: 1

        [NetFieldExport("bIsInWaterVolume", RepLayoutCmdType.PropertyBool, 196, "bIsInWaterVolume", "bool", 1)]
        public bool? bIsInWaterVolume { get; set; } //Type: bool Bits: 1

        [NetFieldExport("BannerIconId", RepLayoutCmdType.PropertyString, 198, "BannerIconId", "FString", 144)]
        public string BannerIconId { get; set; } //Type: FString Bits: 144

        [NetFieldExport("BannerColorId", RepLayoutCmdType.PropertyString, 199, "BannerColorId", "FString", 152)]
        public string BannerColorId { get; set; } //Type: FString Bits: 152

        [NetFieldExport("SkyDiveContrail", RepLayoutCmdType.PropertyObject, 200, "SkyDiveContrail", "UAthenaSkyDiveContrailItemDefinition*", 16)]
        public uint? SkyDiveContrail { get; set; } //Type: UAthenaSkyDiveContrailItemDefinition* Bits: 16

        [NetFieldExport("Glider", RepLayoutCmdType.PropertyObject, 201, "Glider", "UAthenaGliderItemDefinition*", 16)]
        public uint? Glider { get; set; } //Type: UAthenaGliderItemDefinition* Bits: 16

        [NetFieldExport("Pickaxe", RepLayoutCmdType.PropertyObject, 202, "Pickaxe", "UAthenaPickaxeItemDefinition*", 16)]
        public uint? Pickaxe { get; set; } //Type: UAthenaPickaxeItemDefinition* Bits: 16

        [NetFieldExport("bIsDefaultCharacter", RepLayoutCmdType.PropertyBool, 202, "bIsDefaultCharacter", "bool", 1)]
        public bool? bIsDefaultCharacter { get; set; } //Type: bool Bits: 1

        [NetFieldExport("Character", RepLayoutCmdType.PropertyObject, 204, "Character", "UAthenaCharacterItemDefinition*", 16)]
        public uint? Character { get; set; } //Type: UAthenaCharacterItemDefinition* Bits: 16

        [NetFieldExport("CharacterVariantChannels", RepLayoutCmdType.DynamicArray, 205, "CharacterVariantChannels", "TArray", 280)]
        public object[] CharacterVariantChannels { get; set; } //Type: TArray Bits: 280

        [NetFieldExport("DBNOHoister", RepLayoutCmdType.PropertyUInt32, 211, "DBNOHoister", "", 16)]
        public uint? DBNOHoister { get; set; } //Type:  Bits: 16

        [NetFieldExport("DBNOCarryEvent", RepLayoutCmdType.Ignore, 212, "DBNOCarryEvent", "", 2)]
        public object DBNOCarryEvent { get; set; } //Type:  Bits: 2

        [NetFieldExport("Backpack", RepLayoutCmdType.PropertyObject, 212, "Backpack", "UAthenaBackpackItemDefinition*", 16)]
        public uint? Backpack { get; set; } //Type: UAthenaBackpackItemDefinition* Bits: 16

        [NetFieldExport("LoadingScreen", RepLayoutCmdType.PropertyObject, 213, "LoadingScreen", "UAthenaLoadingScreenItemDefinition*", 16)]
        public uint? LoadingScreen { get; set; } //Type: UAthenaLoadingScreenItemDefinition* Bits: 16

        [NetFieldExport("Dances", RepLayoutCmdType.DynamicArray, 218, "Dances", "TArray", 352)]
        public uint[] Dances { get; set; } //Type: TArray Bits: 352

        [NetFieldExport("MusicPack", RepLayoutCmdType.PropertyObject, 222, "MusicPack", "UAthenaMusicPackItemDefinition*", 16)]
        public uint? MusicPack { get; set; } //Type: UAthenaMusicPackItemDefinition* Bits: 16

        [NetFieldExport("PetSkin", RepLayoutCmdType.PropertyObject, 223, "PetSkin", "UAthenaPetItemDefinition*", 16)]
        public uint? PetSkin { get; set; } //Type: UAthenaPetItemDefinition* Bits: 16

        [NetFieldExport("EncryptedPawnReplayData", RepLayoutCmdType.Property, 225, "EncryptedPawnReplayData", "FAthenaPawnReplayData", 160)]
        public FAthenaPawnReplayData EncryptedPawnReplayData { get; set; } //Type: FAthenaPawnReplayData Bits: 160

        [NetFieldExport("GravityFloorAltitude", RepLayoutCmdType.PropertyUInt32, 237, "GravityFloorAltitude", "", 32)]
        public uint? GravityFloorAltitude { get; set; } //Type:  Bits: 32

        [NetFieldExport("GravityFloorWidth", RepLayoutCmdType.PropertyUInt32, 238, "GravityFloorWidth", "", 32)]
        public uint? GravityFloorWidth { get; set; } //Type:  Bits: 32

        [NetFieldExport("GravityFloorGravityScalar", RepLayoutCmdType.PropertyUInt32, 239, "GravityFloorGravityScalar", "", 32)]
        public uint? GravityFloorGravityScalar { get; set; } //Type:  Bits: 32

        [NetFieldExport("ReplicatedWaterBody", RepLayoutCmdType.PropertyUInt32, 242, "ReplicatedWaterBody", "", 16)]
        public uint? ReplicatedWaterBody { get; set; } //Type:  Bits: 16

        [NetFieldExport("DBNORevivalStacking", RepLayoutCmdType.PropertyUInt32, 251, "DBNORevivalStacking", "", 8)]
        public uint? DBNORevivalStacking { get; set; } //Type:  Bits: 8

        [NetFieldExport("ServerWorldTimeRevivalTime", RepLayoutCmdType.PropertyUInt32, 252, "ServerWorldTimeRevivalTime", "", 32)]
        public uint? ServerWorldTimeRevivalTime { get; set; } //Type:  Bits: 32

        [NetFieldExport("ItemSpecialActorID", RepLayoutCmdType.Ignore, 253, "ItemSpecialActorID", "", 225)]
        public object ItemSpecialActorID { get; set; } //Type:  Bits: 225

        [NetFieldExport("FlySpeed", RepLayoutCmdType.PropertyFloat, 261, "FlySpeed", "float", 32)]
        public float? FlySpeed { get; set; } //Type: float Bits: 32

        [NetFieldExport("NextSectionID", RepLayoutCmdType.PropertyUInt32, 268, "NextSectionID", "", 8)]
        public uint? NextSectionID { get; set; } //Type:  Bits: 8

        [NetFieldExport("FastReplicationMinimalReplicationTags", RepLayoutCmdType.Ignore, 275, "FastReplicationMinimalReplicationTags", "", 37)]
        public object FastReplicationMinimalReplicationTags { get; set; } //Type:  Bits: 37

        [NetFieldExport("bIsCreativeGhostModeActivated", RepLayoutCmdType.PropertyBool, 277, "bIsCreativeGhostModeActivated", "", 1)]
        public bool? bIsCreativeGhostModeActivated { get; set; } //Type:  Bits: 1

        [NetFieldExport("PlayRespawnFXOnSpawn", RepLayoutCmdType.PropertyBool, 278, "PlayRespawnFXOnSpawn", "", 1)]
        public bool? PlayRespawnFXOnSpawn { get; set; } //Type:  Bits: 1

    }
}