using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportGroup("/Game/Athena/SafeZone/SafeZoneIndicator.SafeZoneIndicator_C", minimalParseMode: ParseMode.Normal)]
public class SafeZoneIndicator : INetFieldExportGroup
{
    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public object RemoteRole { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public object Role { get; set; }

    [NetFieldExport("LastRadius", RepLayoutCmdType.PropertyFloat)]
    public float LastRadius { get; set; }

    [NetFieldExport("NextRadius", RepLayoutCmdType.PropertyFloat)]
    public float NextRadius { get; set; }

    [NetFieldExport("NextNextRadius", RepLayoutCmdType.PropertyFloat)]
    public float NextNextRadius { get; set; }

    [NetFieldExport("LastCenter", RepLayoutCmdType.PropertyVector100)]
    public FVector LastCenter { get; set; }

    [NetFieldExport("NextCenter", RepLayoutCmdType.PropertyVector100)]
    public FVector NextCenter { get; set; }

    [NetFieldExport("NextNextCenter", RepLayoutCmdType.PropertyVector100)]
    public FVector NextNextCenter { get; set; }

    [NetFieldExport("SafeZoneStartShrinkTime", RepLayoutCmdType.PropertyFloat)]
    public float SafeZoneStartShrinkTime { get; set; }

    [NetFieldExport("SafeZoneFinishShrinkTime", RepLayoutCmdType.PropertyFloat)]
    public float SafeZoneFinishShrinkTime { get; set; }

    [NetFieldExport("bPausedForPreview", RepLayoutCmdType.PropertyBool)]
    public bool bPausedForPreview { get; set; }

    [NetFieldExport("MegaStormDelayTimeBeforeDestruction", RepLayoutCmdType.PropertyFloat)]
    public float MegaStormDelayTimeBeforeDestruction { get; set; }

    [NetFieldExport("Radius", RepLayoutCmdType.PropertyFloat)]
    public float Radius { get; set; }

    [NetFieldExport("PreviousRadius", RepLayoutCmdType.PropertyFloat)]
    public float PreviousRadius { get; set; }

    [NetFieldExport("CurrentPhase", RepLayoutCmdType.PropertyFloat)]
    public float CurrentPhase { get; set; }

    [NetFieldExport("PreviousCenter", RepLayoutCmdType.PropertyVector100)]
    public FVector PreviousCenter { get; set; }

    [NetFieldExport("Damage", RepLayoutCmdType.PropertyFloat)]
    public float Damage { get; set; }

    [NetFieldExport("PhaseCount", RepLayoutCmdType.PropertyFloat)]
    public float PhaseCount { get; set; }

    [NetFieldExport("TimeRemainingWhenPhasePaused", RepLayoutCmdType.PropertyFloat)]
    public float TimeRemainingWhenPhasePaused { get; set; }
}