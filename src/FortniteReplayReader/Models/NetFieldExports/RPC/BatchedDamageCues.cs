using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.RPC;

[NetFieldExportGroup("/Script/FortniteGame.FortPawn:NetMulticast_Athena_BatchedDamageCues", minimalParseMode: ParseMode.Full)]
public class BatchedDamageCues : INetFieldExportGroup
{
    [NetFieldExport("HitActor", RepLayoutCmdType.PropertyObject)]
    public uint? HitActor { get; set; }

    [NetFieldExport("Location", RepLayoutCmdType.PropertyVector100)]
    public FVector Location { get; set; }

    [NetFieldExport("Normal", RepLayoutCmdType.PropertyVectorNormal)]
    public FVector Normal { get; set; }

    [NetFieldExport("Magnitude", RepLayoutCmdType.PropertyFloat)]
    public float? Magnitude { get; set; }

    [NetFieldExport("bWeaponActivate", RepLayoutCmdType.PropertyBool)]
    public bool? bWeaponActivate { get; set; }

    [NetFieldExport("bIsFatal", RepLayoutCmdType.PropertyBool)]
    public bool? bIsFatal { get; set; }

    [NetFieldExport("bIsCritical", RepLayoutCmdType.PropertyBool)]
    public bool? bIsCritical { get; set; }

    [NetFieldExport("bIsShield", RepLayoutCmdType.PropertyBool)]
    public bool? bIsShield { get; set; }

    [NetFieldExport("bIsShieldDestroyed", RepLayoutCmdType.PropertyBool)]
    public bool? bIsShieldDestroyed { get; set; }

    [NetFieldExport("bIsShieldApplied", RepLayoutCmdType.PropertyBool)]
    public bool? bIsShieldApplied { get; set; }

    [NetFieldExport("bIsBallistic", RepLayoutCmdType.PropertyBool)]
    public bool? bIsBallistic { get; set; }

    [NetFieldExport("NonPlayerHitActor", RepLayoutCmdType.PropertyObject)]
    public uint? NonPlayerHitActor { get; set; }

    [NetFieldExport("NonPlayerLocation", RepLayoutCmdType.PropertyVector10)]
    public FVector NonPlayerLocation { get; set; }

    [NetFieldExport("NonPlayerNormal", RepLayoutCmdType.PropertyVectorNormal)]
    public FVector NonPlayerNormal { get; set; }

    [NetFieldExport("NonPlayerMagnitude", RepLayoutCmdType.PropertyFloat)]
    public float? NonPlayerMagnitude { get; set; }

    [NetFieldExport("NonPlayerbIsFatal", RepLayoutCmdType.PropertyBool)]
    public bool? NonPlayerbIsFatal { get; set; }

    [NetFieldExport("NonPlayerbIsCritical", RepLayoutCmdType.PropertyBool)]
    public bool? NonPlayerbIsCritical { get; set; }

    [NetFieldExport("bIsValid", RepLayoutCmdType.PropertyBool)]
    public bool? bIsValid { get; set; }
}
