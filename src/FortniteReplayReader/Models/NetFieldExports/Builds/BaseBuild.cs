using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Vehicles;

public abstract class BaseBuild : INetFieldExportGroup
{
    [NetFieldExport("bHidden", RepLayoutCmdType.Ignore)]
    public bool? bHidden { get; set; }

    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public int? RemoteRole { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public int? Role { get; set; }

    [NetFieldExport("OwnerPersistentID", RepLayoutCmdType.PropertyUInt32)]
    public uint? OwnerPersistentID { get; set; }

    [NetFieldExport("bDestroyed", RepLayoutCmdType.PropertyBool)]
    public bool? bDestroyed { get; set; }

    [NetFieldExport("bPlayerPlaced", RepLayoutCmdType.PropertyBool)]
    public bool? bPlayerPlaced { get; set; }

    [NetFieldExport("bInstantDeath", RepLayoutCmdType.PropertyBool)]
    public bool? bInstantDeath { get; set; }

    [NetFieldExport("bCollisionBlockedByPawns", RepLayoutCmdType.PropertyBool)]
    public bool? bCollisionBlockedByPawns { get; set; }

    [NetFieldExport("bIsInitiallyBuilding", RepLayoutCmdType.PropertyBool)]
    public bool? bIsInitiallyBuilding { get; set; }

    [NetFieldExport("TeamIndex", RepLayoutCmdType.Enum)]
    public int? TeamIndex { get; set; }

    [NetFieldExport("BuildingAnimation", RepLayoutCmdType.Enum)]
    public int? BuildingAnimation { get; set; }

    [NetFieldExport("BuildTime", RepLayoutCmdType.Ignore)]
    public FQuantizedBuildingAttribute BuildTime { get; set; }

    [NetFieldExport("RepairTime", RepLayoutCmdType.Ignore)]
    public FQuantizedBuildingAttribute RepairTime { get; set; }

    [NetFieldExport("Health", RepLayoutCmdType.PropertyInt16)]
    public short? Health { get; set; }

    [NetFieldExport("MaxHealth", RepLayoutCmdType.PropertyInt16)]
    public short? MaxHealth { get; set; }

    [NetFieldExport("EditingPlayer", RepLayoutCmdType.Property)]
    public ActorGuid EditingPlayer { get; set; }

    [NetFieldExport("ProxyGameplayCueDamagePhysicalMagnitude", RepLayoutCmdType.Ignore)]
    public DebuggingObject ProxyGameplayCueDamagePhysicalMagnitude { get; set; }

    [NetFieldExport("EffectContext", RepLayoutCmdType.Ignore)]
    public DebuggingObject EffectContext { get; set; }

    [NetFieldExport("bAttachmentPlacementBlockedFront", RepLayoutCmdType.PropertyBool)]
    public bool? bAttachmentPlacementBlockedFront { get; set; }

    [NetFieldExport("bAttachmentPlacementBlockedBack", RepLayoutCmdType.PropertyBool)]
    public bool? bAttachmentPlacementBlockedBack { get; set; }

    [NetFieldExport("bUnderConstruction", RepLayoutCmdType.PropertyBool)]
    public bool? bUnderConstruction { get; set; }

    [NetFieldExport("StaticMesh", RepLayoutCmdType.Ignore)]
    public uint? StaticMesh { get; set; }

    [NetFieldExport("Gnomed", RepLayoutCmdType.PropertyBool)]
    public bool? Gnomed { get; set; }

    [NetFieldExport("InitialOverlappingVehicles", RepLayoutCmdType.Property)]
    public DebuggingObject InitialOverlappingVehicles { get; set; }
}