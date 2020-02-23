using Unreal.Core.Contracts;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models.TelemetryEvents
{
    public class PlayerMovementEvent : ITelemetryEvent
    {
        public float? ReplicatedWorldTimeSeconds { get; set; }

        public bool? bCanBeDamaged { get; set; }
        public FRepMovement? ReplicatedMovement { get; set; }
        public FVector LocationOffset { get; set; }
        public FVector RelativeScale3D { get; set; }
        public FRotator RotationOffset { get; set; }
        public byte? RemoteViewPitch { get; set; }
        public FVector Location { get; set; }
        public FRotator Rotation { get; set; }
        public float? ReplayLastTransformUpdateTimeStamp { get; set; }
        public byte? ReplicatedMovementMode { get; set; }
        public bool? bIsCrouched { get; set; }
        public float? Position { get; set; }
        public FVector LinearVelocity { get; set; }
        public int? CurrentMovementStyle { get; set; }
        public bool? bIsDying { get; set; }
        public uint? CurrentWeapon { get; set; }
        public bool? bIsInvulnerable { get; set; }
        public bool? bMovingEmote { get; set; }
        public bool? bWeaponActivated { get; set; }
        public bool? bIsDBNO { get; set; }
        public bool? bWasDBNOOnDeath { get; set; }
        public bool? bWeaponHolstered { get; set; }
        public uint? LastReplicatedEmoteExecuted { get; set; }
        public float? ForwardAlpha { get; set; }
        public float? RightAlpha { get; set; }
        public float? TurnDelta { get; set; }
        public float? SteerAlpha { get; set; }
        public float? GravityScale { get; set; }
        public FVector WorldLookDir { get; set; }
        public bool? bIsHonking { get; set; }
        public bool? bIsJumping { get; set; }
        public bool? bIsSprinting { get; set; }
        public uint? Vehicle { get; set; }
        public float? VehicleApexZ { get; set; }
        public byte? SeatIndex { get; set; }
        public bool? bIsWaterJump { get; set; }
        public bool? bIsWaterSprintBoost { get; set; }
        public bool? bIsWaterSprintBoostPending { get; set; }
        public int? BuildingState { get; set; }
        public bool? bIsTargeting { get; set; }
        public bool? bIsPlayingEmote { get; set; }
        public bool? bStartedInteractSearch { get; set; }
        public ushort? AccelerationPack { get; set; }
        public byte? AccelerationZPack { get; set; }
        public bool? bIsWaitingForEmoteInteraction { get; set; }
        public ItemDefinition GroupEmoteLookTarget { get; set; }
        public bool? bIsSkydiving { get; set; }
        public bool? bIsParachuteOpen { get; set; }
        public bool? bIsParachuteForcedOpen { get; set; }
        public bool? bIsSkydivingFromBus { get; set; }
        public bool? bIsInAnyStorm { get; set; }
        public bool? bIsSlopeSliding { get; set; }
        public bool? bIsInsideSafeZone { get; set; }
        public bool? bIsOutsideSafeZone { get; set; }
        public ItemDefinition Zipline { get; set; }
        public bool? bIsZiplining { get; set; }
        public bool? bJumped { get; set; }
        public uint? RemoteViewData32 { get; set; }
        public uint? EntryTime { get; set; }
        public float? WalkSpeed { get; set; }
        public float? RunSpeed { get; set; }
        public float? SprintSpeed { get; set; }
        public float? CrouchedRunSpeed { get; set; }
        public float? CrouchedSprintSpeed { get; set; }
        public bool? WeaponActivated { get; set; }
        public bool? bIsInWaterVolume { get; set; }
        public ActorGuid DBNOHoister { get; set; }
        public uint? GravityFloorAltitude { get; set; }
        public uint? GravityFloorWidth { get; set; }
        public uint? GravityFloorGravityScalar { get; set; }
        public uint? ReplicatedWaterBody { get; set; }
        public byte? DBNORevivalStacking { get; set; }
        public uint? ServerWorldTimeRevivalTime { get; set; }
        public float? FlySpeed { get; set; }
        public bool? bIsSkydivingFromLaunchPad { get; set; }
        public bool? bInGliderRedeploy { get; set; }
    }
}
