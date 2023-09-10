using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Items;

public abstract class BaseContainer : INetFieldExportGroup
{
    [NetFieldExport("bHidden", RepLayoutCmdType.Ignore)]
    public bool bHidden { get; set; }

    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public int RemoteRole { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public int Role { get; set; }

    [NetFieldExport("ForceMetadataRelevant", RepLayoutCmdType.Ignore)]
    public DebuggingObject ForceMetadataRelevant { get; set; }

    [NetFieldExport("bDestroyed", RepLayoutCmdType.PropertyBool)]
    public bool bDestroyed { get; set; }

    [NetFieldExport("bInstantDeath", RepLayoutCmdType.PropertyBool)]
    public bool bInstantDeath { get; set; }

    [NetFieldExport("StaticMesh", RepLayoutCmdType.Ignore)]
    public uint StaticMesh { get; set; }

    [NetFieldExport("AltMeshIdx", RepLayoutCmdType.Ignore)]
    public uint AltMeshIdx { get; set; }

    [NetFieldExport("WeaponData", RepLayoutCmdType.Property)]
    public ItemDefinition WeaponData { get; set; }

    [NetFieldExport("BuildTime", RepLayoutCmdType.Ignore)]
    public FQuantizedBuildingAttribute BuildTime { get; set; }

    [NetFieldExport("RepairTime", RepLayoutCmdType.Ignore)]
    public FQuantizedBuildingAttribute RepairTime { get; set; }

    [NetFieldExport("Health", RepLayoutCmdType.PropertyUInt16)]
    public ushort Health { get; set; }

    [NetFieldExport("MaxHealth", RepLayoutCmdType.PropertyUInt16)]
    public ushort MaxHealth { get; set; }

    [NetFieldExport("SearchedMesh", RepLayoutCmdType.Ignore)]
    public uint SearchedMesh { get; set; }

    [NetFieldExport("ReplicatedLootTier", RepLayoutCmdType.PropertyInt)]
    public int ReplicatedLootTier { get; set; }

    [NetFieldExport("bAlreadySearched", RepLayoutCmdType.PropertyBool)]
    public bool bAlreadySearched { get; set; }

    [NetFieldExport("BounceNormal", RepLayoutCmdType.PropertyVector)]
    public FVector BounceNormal { get; set; }

    [NetFieldExport("SearchAnimationCount", RepLayoutCmdType.PropertyUInt32)]
    public uint SearchAnimationCount { get; set; }

    [NetFieldExport("ChosenRandomUpgrade", RepLayoutCmdType.PropertyInt)]
    public int? ChosenRandomUpgrade { get; set; }

    [NetFieldExport("bMirrored", RepLayoutCmdType.PropertyBool)]
    public bool bMirrored { get; set; }

    [NetFieldExport("ReplicatedDrawScale3D", RepLayoutCmdType.PropertyVector100)]
    public FVector ReplicatedDrawScale3D { get; set; }

    [NetFieldExport("bIsInitiallyBuilding", RepLayoutCmdType.PropertyBool)]
    public bool? bIsInitiallyBuilding { get; set; }

    [NetFieldExport("bForceReplayRollback", RepLayoutCmdType.PropertyBool)]
    public bool? bForceReplayRollback { get; set; }
}