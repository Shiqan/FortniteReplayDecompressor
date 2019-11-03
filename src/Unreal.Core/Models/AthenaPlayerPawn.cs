namespace Unreal.Core.Models
{
    public class AthenaPlayerPawn : ActorState
    {
        public int PawnUniqueID { get; set; }
        public FRepMovement RepMovement { get; set; }
        public int ReplicatedMovementMode { get; set; }
        public float ReplayLastTransformUpdateTimeStamp { get; set; }
        public uint AccelerationPack { get; set; }
        public int AccelerationZPack { get; set; }
        public uint RemoteViewData32 { get; set; }
        public bool bCanBeDamaged { get; set; }
        public uint Instigator { get; set; } // APawn
        public uint PlayerState { get; set; } // APlayerState
        public string[] VocalChords { get; set; }
        public float CapsuleRadiusAthena { get; set; }
        public float CapsuleHalfHeightAthena { get; set; }
        public float WalkSpeed { get; set; }
        public float RunSpeed { get; set; }
        public float SprintSpeed { get; set; }
        public float CrouchedRunSpeed { get; set; }
        public float CrouchedSprintSpeed { get; set; }
        public string BannerIconId { get; set; }
        public string BannerColorId { get; set; }
        public uint SkyDiveContrail { get; set; }
        public uint Glider { get; set; } // UAthenaGliderItemDefinition
        public uint Pickaxe { get; set; } // UAthenaPickaxeItemDefinition
        public uint Character { get; set; } // UAthenaCharacterItemDefinition
        public uint Backpack { get; set; } // UAthenaBackpackItemDefinition
        public uint LoadingScreen { get; set; } // UAthenaLoadingScreenItemDefinition
        public uint MusicPack { get; set; } // UAthenaMusicPackItemDefinition
        public uint CurrentWeapon { get; set; } // AFortWeapon
        public int JumpFlashCount { get; set; }
        public float PlayRate { get; set; }
        public float BlendTime { get; set; }
        public bool ForcePlayBit { get; set; }
        public bool bIsProxySimulationTimedOut { get; set; }
        public bool bProxyIsJumpForceApplied { get; set; }
        public bool bIsDefaultCharacter { get; set; }
        public bool bWeaponHolstered { get; set; }
        public bool bPlayBit { get; set; }
        public bool bIsCrouched { get; set; }
        public bool bIsTargeting { get; set; }
        public bool WeaponActivated { get; set; }
        public bool bIsSlopeSliding { get; set; }
        public bool bServerHasBaseComponent { get; set; }
        public int BuildingState { get; set; } // EFortBuildingState
        public int CurrentMovementStyle { get; set; } // EFortMovementStyle
        public ushort PackedReplicatedSlopeAngles { get; set; }
        public uint PetState   { get; set; } // AFortPlayerPetRepState
        public uint PetSkin { get; set; } // UAthenaPetItemDefinition
        public uint[] CharacterVariantChannels { get; set; } // TArray
        public uint AnimMontage { get; set; } //UAnimMontage
        public uint PawnMontage { get; set; } //UAnimMontage
        public int RepAnimMontageStartSection { get; set; }
        public int EncryptedPawnReplayData { get; set; } // FAthenaPawnReplayData
        public int MovementBase { get; set; } // UPrimitiveComponent

        //case "ReplicatedMovement":
        //    cmdReader.SerializeRepMovement();
        //    break;


    }
}
