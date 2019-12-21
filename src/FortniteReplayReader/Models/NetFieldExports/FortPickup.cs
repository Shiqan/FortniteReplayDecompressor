using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Script/FortniteGame.FortPickupAthena")]
    public class FortPickup : INetFieldExportGroup
    {
        [NetFieldExport("bReplicateMovement", RepLayoutCmdType.PropertyBool, 1, "bReplicateMovement", "uint8", 1)]
        public bool? bReplicateMovement { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore, 4, "RemoteRole", "", 2)]
        public object RemoteRole { get; set; } //Type:  Bits: 2

        [NetFieldExport("ReplicatedMovement", RepLayoutCmdType.RepMovement, 5, "ReplicatedMovement", "FRepMovement", 102)]
        public FRepMovement ReplicatedMovement { get; set; } //Type: FRepMovement Bits: 102

        [NetFieldExport("AttachParent", RepLayoutCmdType.PropertyUInt32, 6, "AttachParent", "", 16)]
        public uint? AttachParent { get; set; } //Type:  Bits: 16

        [NetFieldExport("LocationOffset", RepLayoutCmdType.PropertyVector100, 7, "LocationOffset", "", 35)]
        public FVector LocationOffset { get; set; } //Type:  Bits: 35

        [NetFieldExport("RelativeScale3D", RepLayoutCmdType.PropertyVector100, 8, "RelativeScale3D", "", 23)]
        public FVector RelativeScale3D { get; set; } //Type:  Bits: 23

        [NetFieldExport("RotationOffset", RepLayoutCmdType.PropertyRotator, 9, "RotationOffset", "", 51)]
        public FRotator RotationOffset { get; set; } //Type:  Bits: 51

        [NetFieldExport("AttachComponent", RepLayoutCmdType.PropertyUInt32, 11, "AttachComponent", "", 16)]
        public uint? AttachComponent { get; set; } //Type:  Bits: 16

        [NetFieldExport("Owner", RepLayoutCmdType.PropertyObject, 12, "Owner", "AActor*", 0)]
        public uint? Owner { get; set; } //Type: AActor* Bits: 0

        [NetFieldExport("Role", RepLayoutCmdType.Ignore, 13, "Role", "", 2)]
        public object Role { get; set; } //Type:  Bits: 2

        [NetFieldExport("bRandomRotation", RepLayoutCmdType.PropertyBool, 15, "bRandomRotation", "bool", 1)]
        public bool? bRandomRotation { get; set; } //Type: bool Bits: 1

        [NetFieldExport("Count", RepLayoutCmdType.PropertyInt, 16, "Count", "int32", 32)]
        public int? Count { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("ItemDefinition", RepLayoutCmdType.PropertyObject, 17, "ItemDefinition", "UFortItemDefinition*", 16)]
        public uint? ItemDefinition { get; set; } //Type: UFortItemDefinition* Bits: 16

        [NetFieldExport("Durability", RepLayoutCmdType.PropertyFloat, 18, "Durability", "float", 32)]
        public float? Durability { get; set; } //Type: float Bits: 32

        [NetFieldExport("Level", RepLayoutCmdType.PropertyInt, 19, "Level", "int32", 32)]
        public int? Level { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("LoadedAmmo", RepLayoutCmdType.PropertyInt, 20, "LoadedAmmo", "int32", 32)]
        public int? LoadedAmmo { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("A", RepLayoutCmdType.PropertyInt, 25, "A", "int32", 32)]
        public int? A { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("B", RepLayoutCmdType.PropertyInt, 26, "B", "int32", 32)]
        public int? B { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("C", RepLayoutCmdType.PropertyInt, 27, "C", "int32", 32)]
        public int? C { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("D", RepLayoutCmdType.PropertyInt, 28, "D", "int32", 32)]
        public int? D { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("bUpdateStatsOnCollection", RepLayoutCmdType.PropertyBool, 29, "bUpdateStatsOnCollection", "", 1)]
        public bool? bUpdateStatsOnCollection { get; set; } //Type:  Bits: 1

        [NetFieldExport("bIsDirty", RepLayoutCmdType.PropertyBool, 31, "bIsDirty", "bool", 1)]
        public bool? bIsDirty { get; set; } //Type: bool Bits: 1

        [NetFieldExport("StateValues", RepLayoutCmdType.DynamicArray, 34, "StateValues", "TArray", 343)]
        public FortItemEntryStateValue[] StateValues { get; set; } //Type: TArray Bits: 343

        [NetFieldExport("GenericAttributeValues", RepLayoutCmdType.DynamicArray, 41, "GenericAttributeValues", "", 80)]
        public float[] GenericAttributeValues { get; set; } //Type:  Bits: 80

        [NetFieldExport("CombineTarget", RepLayoutCmdType.PropertyUInt32, 73, "CombineTarget", "", 16)]
        public uint? CombineTarget { get; set; } //Type:  Bits: 16

        [NetFieldExport("PickupTarget", RepLayoutCmdType.PropertyObject, 80, "PickupTarget", "AFortPawn*", 16)]
        public uint? PickupTarget { get; set; } //Type: AFortPawn* Bits: 16

        [NetFieldExport("ItemOwner", RepLayoutCmdType.PropertyObject, 82, "ItemOwner", "AFortPawn*", 8)]
        public uint? ItemOwner { get; set; } //Type: AFortPawn* Bits: 8

        [NetFieldExport("LootInitialPosition", RepLayoutCmdType.PropertyVector10, 83, "LootInitialPosition", "FVector_NetQuantize10", 71)]
        public FVector LootInitialPosition { get; set; } //Type: FVector_NetQuantize10 Bits: 71

        [NetFieldExport("LootFinalPosition", RepLayoutCmdType.PropertyVector10, 84, "LootFinalPosition", "FVector_NetQuantize10", 71)]
        public FVector LootFinalPosition { get; set; } //Type: FVector_NetQuantize10 Bits: 71

        [NetFieldExport("FlyTime", RepLayoutCmdType.PropertyFloat, 85, "FlyTime", "float", 32)]
        public float? FlyTime { get; set; } //Type: float Bits: 32

        [NetFieldExport("StartDirection", RepLayoutCmdType.PropertyVectorNormal, 86, "StartDirection", "FVector_NetQuantizeNormal", 48)]
        public FVector StartDirection { get; set; } //Type: FVector_NetQuantizeNormal Bits: 48

        [NetFieldExport("FinalTossRestLocation", RepLayoutCmdType.PropertyVector10, 87, "FinalTossRestLocation", "FVector_NetQuantize10", 71)]
        public FVector FinalTossRestLocation { get; set; } //Type: FVector_NetQuantize10 Bits: 71

        [NetFieldExport("TossState", RepLayoutCmdType.Enum, 88, "TossState", "EFortPickupTossState", 2)]
        public int? TossState { get; set; } //Type: EFortPickupTossState Bits: 2

        [NetFieldExport("bCombinePickupsWhenTossCompletes", RepLayoutCmdType.PropertyBool, 88, "bCombinePickupsWhenTossCompletes", "", 1)]
        public bool? bCombinePickupsWhenTossCompletes { get; set; } //Type:  Bits: 1

        [NetFieldExport("OptionalOwnerID", RepLayoutCmdType.PropertyInt, 90, "OptionalOwnerID", "int32", 32)]
        public int? OptionalOwnerID { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("bPickedUp", RepLayoutCmdType.PropertyBool, 91, "bPickedUp", "bool", 1)]
        public bool? bPickedUp { get; set; } //Type: bool Bits: 1

        [NetFieldExport("bTossedFromContainer", RepLayoutCmdType.PropertyBool, 92, "bTossedFromContainer", "bool", 1)]
        public bool? bTossedFromContainer { get; set; } //Type: bool Bits: 1

        [NetFieldExport("bServerStoppedSimulation", RepLayoutCmdType.PropertyBool, 94, "bServerStoppedSimulation", "bool", 1)]
        public bool? bServerStoppedSimulation { get; set; } //Type: bool Bits: 1

        [NetFieldExport("ServerImpactSoundFlash", RepLayoutCmdType.PropertyByte, 95, "ServerImpactSoundFlash", "uint8", 8)]
        public byte? ServerImpactSoundFlash { get; set; } //Type: uint8 Bits: 8

        [NetFieldExport("PawnWhoDroppedPickup", RepLayoutCmdType.PropertyObject, 96, "PawnWhoDroppedPickup", "AFortPawn*", 8)]
        public uint? PawnWhoDroppedPickup { get; set; } //Type: AFortPawn* Bits: 8
    }
}