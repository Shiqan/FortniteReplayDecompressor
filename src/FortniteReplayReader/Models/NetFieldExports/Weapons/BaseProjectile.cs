using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons;

public abstract class BaseProjectile : INetFieldExportGroup
{
    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public int RemoteRole { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public int Role { get; set; }

    [NetFieldExport("Owner", RepLayoutCmdType.Ignore)]
    public uint Owner { get; set; }

    [NetFieldExport("Instigator", RepLayoutCmdType.PropertyObject)]
    public uint Instigator { get; set; }

    [NetFieldExport("Team", RepLayoutCmdType.PropertyByte)]
    public byte Team { get; set; }

    [NetFieldExport("ReplicatedMovement", RepLayoutCmdType.RepMovement)]
    [RepMovement(
        locationQuantizationLevel: VectorQuantization.RoundWholeNumber,
        rotationQuantizationLevel: RotatorQuantization.ByteComponents,
        velocityQuantizationLevel: VectorQuantization.RoundWholeNumber)]
    public FRepMovement ReplicatedMovement { get; set; }

    [NetFieldExport("ReplicatedMaxSpeed", RepLayoutCmdType.PropertyFloat)]
    public float ReplicatedMaxSpeed { get; set; }

    [NetFieldExport("GravityScale", RepLayoutCmdType.PropertyFloat)]
    public float GravityScale { get; set; }
}

public abstract class BaseRocketLauncherProjectile : BaseProjectile
{
    [NetFieldExport("StopLocation", RepLayoutCmdType.PropertyVector)]
    public FVector StopLocation { get; set; }

    [NetFieldExport("DecalLocation", RepLayoutCmdType.PropertyVector)]
    public FVector DecalLocation { get; set; }

    [NetFieldExport("PawnHitResult", RepLayoutCmdType.Property)]
    public FHitResult PawnHitResult { get; set; }

    [NetFieldExport("bHasExploded", RepLayoutCmdType.PropertyBool)]
    public bool bHasExploded { get; set; }

    [NetFieldExport("bIsBeingKilled", RepLayoutCmdType.PropertyBool)]
    public bool bIsBeingKilled { get; set; }
}