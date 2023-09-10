using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportGroup("/Game/Athena/SupplyDrops/AthenaSupplyDropBalloon.AthenaSupplyDropBalloon_C", minimalParseMode: ParseMode.Ignore)]
public class SupplyDropBalloon : INetFieldExportGroup
{
    [NetFieldExport("bHidden", RepLayoutCmdType.PropertyBool)]
    public bool? bHidden { get; set; }

    [NetFieldExport("bCanBeDamaged", RepLayoutCmdType.PropertyBool)]
    public bool? bCanBeDamaged { get; set; }

    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public object RemoteRole { get; set; }

    [NetFieldExport("AttachParent", RepLayoutCmdType.PropertyObject)]
    public uint? AttachParent { get; set; }

    [NetFieldExport("LocationOffset", RepLayoutCmdType.Ignore)]
    public object LocationOffset { get; set; }

    [NetFieldExport("RelativeScale3D", RepLayoutCmdType.PropertyVector100)]
    public FVector RelativeScale3D { get; set; }

    [NetFieldExport("AttachComponent", RepLayoutCmdType.PropertyObject)]
    public uint? AttachComponent { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public object Role { get; set; }

    [NetFieldExport("A", RepLayoutCmdType.PropertyUInt32)]
    public uint? A { get; set; }

    [NetFieldExport("B", RepLayoutCmdType.PropertyUInt32)]
    public uint? B { get; set; }

    [NetFieldExport("C", RepLayoutCmdType.PropertyUInt32)]
    public uint? C { get; set; }

    [NetFieldExport("D", RepLayoutCmdType.PropertyUInt32)]
    public uint? D { get; set; }

    [NetFieldExport("ReplicatedBuildingAttributeSet", RepLayoutCmdType.PropertyObject)]
    public uint? ReplicatedBuildingAttributeSet { get; set; }

    [NetFieldExport("ReplicatedAbilitySystemComponent", RepLayoutCmdType.PropertyObject)]
    public uint? ReplicatedAbilitySystemComponent { get; set; }

    [NetFieldExport("bDestroyed", RepLayoutCmdType.PropertyBool)]
    public bool? bDestroyed { get; set; }

    [NetFieldExport("bEditorPlaced", RepLayoutCmdType.PropertyBool)]
    public bool? bEditorPlaced { get; set; }

    [NetFieldExport("bInstantDeath", RepLayoutCmdType.PropertyBool)]
    public bool? bInstantDeath { get; set; }

    [NetFieldExport("AttachmentPlacementBlockingActors", RepLayoutCmdType.Ignore)]
    public object[] AttachmentPlacementBlockingActors { get; set; }
}