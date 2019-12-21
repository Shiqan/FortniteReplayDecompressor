using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Game/Athena/SupplyDrops/AthenaSupplyDropBalloon.AthenaSupplyDropBalloon_C")]
    public class SupplyDropBalloon : INetFieldExportGroup
    {
        [NetFieldExport("bHidden", RepLayoutCmdType.PropertyBool, 0, "bHidden", "uint8", 1)]
        public bool? bHidden { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bCanBeDamaged", RepLayoutCmdType.PropertyBool, 3, "bCanBeDamaged", "uint8", 1)]
        public bool? bCanBeDamaged { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore, 4, "RemoteRole", "", 2)]
        public object RemoteRole { get; set; } //Type:  Bits: 2

        [NetFieldExport("AttachParent", RepLayoutCmdType.PropertyObject, 6, "AttachParent", "AActor*", 16)]
        public uint? AttachParent { get; set; } //Type: AActor* Bits: 16

        [NetFieldExport("LocationOffset", RepLayoutCmdType.Ignore, 7, "LocationOffset", "", 41)]
        public object LocationOffset { get; set; } //Type:  Bits: 41

        [NetFieldExport("RelativeScale3D", RepLayoutCmdType.PropertyVector100, 8, "RelativeScale3D", "FVector_NetQuantize100", 29)]
        public FVector RelativeScale3D { get; set; } //Type: FVector_NetQuantize100 Bits: 29

        [NetFieldExport("AttachComponent", RepLayoutCmdType.PropertyObject, 11, "AttachComponent", "USceneComponent*", 16)]
        public uint? AttachComponent { get; set; } //Type: USceneComponent* Bits: 16

        [NetFieldExport("Role", RepLayoutCmdType.Ignore, 13, "Role", "", 2)]
        public object Role { get; set; } //Type:  Bits: 2

        [NetFieldExport("A", RepLayoutCmdType.PropertyInt, 15, "A", "int32", 32)]
        public int? A { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("B", RepLayoutCmdType.PropertyInt, 16, "B", "int32", 32)]
        public int? B { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("C", RepLayoutCmdType.PropertyInt, 17, "C", "int32", 32)]
        public int? C { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("D", RepLayoutCmdType.PropertyInt, 18, "D", "int32", 32)]
        public int? D { get; set; } //Type: int32 Bits: 32

        [NetFieldExport("ReplicatedBuildingAttributeSet", RepLayoutCmdType.PropertyObject, 21, "ReplicatedBuildingAttributeSet", "UFortBuildingActorSet*", 16)]
        public uint? ReplicatedBuildingAttributeSet { get; set; } //Type: UFortBuildingActorSet* Bits: 16

        [NetFieldExport("ReplicatedAbilitySystemComponent", RepLayoutCmdType.PropertyObject, 22, "ReplicatedAbilitySystemComponent", "UFortAbilitySystemComponent*", 16)]
        public uint? ReplicatedAbilitySystemComponent { get; set; } //Type: UFortAbilitySystemComponent* Bits: 16

        [NetFieldExport("bDestroyed", RepLayoutCmdType.PropertyBool, 25, "bDestroyed", "uint8", 1)]
        public bool? bDestroyed { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bEditorPlaced", RepLayoutCmdType.PropertyBool, 26, "bEditorPlaced", "uint8", 1)]
        public bool? bEditorPlaced { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("bInstantDeath", RepLayoutCmdType.PropertyBool, 33, "bInstantDeath", "uint8", 1)]
        public bool? bInstantDeath { get; set; } //Type: uint8 Bits: 1

        [NetFieldExport("AttachmentPlacementBlockingActors", RepLayoutCmdType.DynamicArray, 63, "AttachmentPlacementBlockingActors", "TArray", 16)]
        public object[] AttachmentPlacementBlockingActors { get; set; } //Type: TArray Bits: 16

    }
}