using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportGroup("/Script/FortniteGame.FortPickupAthena", minimalParseMode: ParseMode.Normal)]
public class FortPickup : INetFieldExportGroup
{
    [NetFieldExport("bReplicateMovement", RepLayoutCmdType.PropertyBool)]
    public bool? bReplicateMovement { get; set; }

    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public object RemoteRole { get; set; }

    [NetFieldExport("ReplicatedMovement", RepLayoutCmdType.RepMovement)]
    public FRepMovement? ReplicatedMovement { get; set; }

    [NetFieldExport("AttachParent", RepLayoutCmdType.Ignore)]
    public uint? AttachParent { get; set; }

    [NetFieldExport("LocationOffset", RepLayoutCmdType.PropertyVector100)]
    public FVector LocationOffset { get; set; }

    [NetFieldExport("RelativeScale3D", RepLayoutCmdType.PropertyVector100)]
    public FVector RelativeScale3D { get; set; }

    [NetFieldExport("RotationOffset", RepLayoutCmdType.PropertyRotator)]
    public FRotator RotationOffset { get; set; }

    [NetFieldExport("AttachComponent", RepLayoutCmdType.PropertyObject)]
    public uint? AttachComponent { get; set; }

    [NetFieldExport("Owner", RepLayoutCmdType.PropertyObject)]
    public uint? Owner { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public object Role { get; set; }

    [NetFieldExport("bRandomRotation", RepLayoutCmdType.PropertyBool)]
    public bool? bRandomRotation { get; set; }

    [NetFieldExport("Count", RepLayoutCmdType.PropertyInt)]
    public int? Count { get; set; }

    [NetFieldExport("ItemDefinition", RepLayoutCmdType.Property)]
    public ItemDefinition ItemDefinition { get; set; }

    [NetFieldExport("Durability", RepLayoutCmdType.PropertyFloat)]
    public float? Durability { get; set; }

    [NetFieldExport("Level", RepLayoutCmdType.PropertyInt)]
    public int? Level { get; set; }

    [NetFieldExport("LoadedAmmo", RepLayoutCmdType.PropertyInt)]
    public int? LoadedAmmo { get; set; }

    [NetFieldExport("A", RepLayoutCmdType.PropertyUInt32)]
    public uint? A { get; set; }

    [NetFieldExport("B", RepLayoutCmdType.PropertyUInt32)]
    public uint? B { get; set; }

    [NetFieldExport("C", RepLayoutCmdType.PropertyUInt32)]
    public uint? C { get; set; }

    [NetFieldExport("D", RepLayoutCmdType.PropertyUInt32)]
    public uint? D { get; set; }

    [NetFieldExport("bUpdateStatsOnCollection", RepLayoutCmdType.PropertyBool)]
    public bool? bUpdateStatsOnCollection { get; set; }

    [NetFieldExport("bIsDirty", RepLayoutCmdType.PropertyBool)]
    public bool? bIsDirty { get; set; }

    [NetFieldExport("StateValues", RepLayoutCmdType.DynamicArray)]
    public FortItemEntryStateValue[] StateValues { get; set; }

    [NetFieldExport("GenericAttributeValues", RepLayoutCmdType.DynamicArray)]
    public float[] GenericAttributeValues { get; set; }

    [NetFieldExport("CombineTarget", RepLayoutCmdType.Property)]
    public ItemDefinition CombineTarget { get; set; }

    [NetFieldExport("PickupTarget", RepLayoutCmdType.PropertyObject)]
    public uint? PickupTarget { get; set; }

    [NetFieldExport("ItemOwner", RepLayoutCmdType.PropertyObject)]
    public uint? ItemOwner { get; set; }

    [NetFieldExport("LootInitialPosition", RepLayoutCmdType.PropertyVector10)]
    public FVector LootInitialPosition { get; set; }

    [NetFieldExport("LootFinalPosition", RepLayoutCmdType.PropertyVector10)]
    public FVector LootFinalPosition { get; set; }

    [NetFieldExport("FlyTime", RepLayoutCmdType.PropertyFloat)]
    public float? FlyTime { get; set; }

    [NetFieldExport("StartDirection", RepLayoutCmdType.PropertyVectorNormal)]
    public FVector StartDirection { get; set; }

    [NetFieldExport("FinalTossRestLocation", RepLayoutCmdType.PropertyVector10)]
    public FVector FinalTossRestLocation { get; set; }

    [NetFieldExport("TossState", RepLayoutCmdType.Enum)]
    public int? TossState { get; set; }

    [NetFieldExport("bCombinePickupsWhenTossCompletes", RepLayoutCmdType.PropertyBool)]
    public bool? bCombinePickupsWhenTossCompletes { get; set; }

    [NetFieldExport("OptionalOwnerID", RepLayoutCmdType.PropertyInt)]
    public int? OptionalOwnerID { get; set; }

    [NetFieldExport("bPickedUp", RepLayoutCmdType.PropertyBool)]
    public bool? bPickedUp { get; set; }

    [NetFieldExport("bTossedFromContainer", RepLayoutCmdType.PropertyBool)]
    public bool? bTossedFromContainer { get; set; }

    [NetFieldExport("bServerStoppedSimulation", RepLayoutCmdType.PropertyBool)]
    public bool? bServerStoppedSimulation { get; set; }

    [NetFieldExport("ServerImpactSoundFlash", RepLayoutCmdType.PropertyByte)]
    public byte? ServerImpactSoundFlash { get; set; }

    [NetFieldExport("PawnWhoDroppedPickup", RepLayoutCmdType.PropertyObject)]
    public uint? PawnWhoDroppedPickup { get; set; }

    [NetFieldExport("OrderIndex", RepLayoutCmdType.PropertyUInt16)]
    public ushort? OrderIndex { get; set; }
}