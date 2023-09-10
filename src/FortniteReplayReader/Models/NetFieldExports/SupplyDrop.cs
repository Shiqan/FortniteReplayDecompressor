using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportGroup("/Game/Athena/SupplyDrops/AthenaSupplyDrop.AthenaSupplyDrop_C", minimalParseMode: ParseMode.Full)]
public class SupplyDrop : INetFieldExportGroup
{
    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public object RemoteRole { get; set; }

    [NetFieldExport("ReplicatedMovement", RepLayoutCmdType.RepMovement)]
    [RepMovement(
        locationQuantizationLevel: VectorQuantization.RoundWholeNumber,
        rotationQuantizationLevel: RotatorQuantization.ByteComponents,
        velocityQuantizationLevel: VectorQuantization.RoundWholeNumber)]
    public FRepMovement ReplicatedMovement { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public object Role { get; set; }

    [NetFieldExport("bDestroyed", RepLayoutCmdType.PropertyBool)]
    public bool bDestroyed { get; set; }

    [NetFieldExport("bEditorPlaced", RepLayoutCmdType.PropertyBool)]
    public bool bEditorPlaced { get; set; }

    [NetFieldExport("bInstantDeath", RepLayoutCmdType.PropertyBool)]
    public bool bInstantDeath { get; set; }

    [NetFieldExport("bHasSpawnedPickups", RepLayoutCmdType.PropertyBool)]
    public bool bHasSpawnedPickups { get; set; }

    [NetFieldExport("Opened", RepLayoutCmdType.PropertyBool)]
    public bool Opened { get; set; }

    [NetFieldExport("BalloonPopped", RepLayoutCmdType.PropertyBool)]
    public bool BalloonPopped { get; set; }

    [NetFieldExport("FallSpeed", RepLayoutCmdType.PropertyFloat)]
    public float FallSpeed { get; set; }

    [NetFieldExport("LandingLocation", RepLayoutCmdType.PropertyVector)]
    public FVector LandingLocation { get; set; }

    [NetFieldExport("FallHeight", RepLayoutCmdType.PropertyFloat)]
    public float FallHeight { get; set; }
}