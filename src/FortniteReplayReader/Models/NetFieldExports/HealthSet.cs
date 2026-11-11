using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

/// <summary>
/// Health and shield attributes of a player (subobject "HealthSet" of the FortPlayerStateAthena).
/// Every attribute is a FFortGameplayAttributeData that uses 10 handles:
/// 0 BaseValue, 1 CurrentValue, 3 Maximum, 7 UnclampedBaseValue, 8 UnclampedCurrentValue.
/// Health starts at handle 0, shield at handle 30 (verified on ++Fortnite+Release-43.00).
/// The values are nullable: <c>null</c> means "not sent in this update".
/// </summary>
[NetFieldExportGroup("/Script/FortniteGame.FortRegenHealthSet", minimalParseMode: ParseMode.Full)]
public class HealthSet : INetFieldExportGroup
{
    [NetFieldExportHandle(0, RepLayoutCmdType.PropertyFloat)]
    public float? HealthBaseValue { get; set; }

    [NetFieldExportHandle(1, RepLayoutCmdType.PropertyFloat)]
    public float? HealthCurrentValue { get; set; }

    [NetFieldExportHandle(3, RepLayoutCmdType.PropertyFloat)]
    public float? HealthMaxValue { get; set; }

    [NetFieldExportHandle(7, RepLayoutCmdType.PropertyFloat)]
    public float? HealthUnclampedBaseValue { get; set; }

    [NetFieldExportHandle(8, RepLayoutCmdType.PropertyFloat)]
    public float? HealthUnclampedCurrentValue { get; set; }

    [NetFieldExportHandle(30, RepLayoutCmdType.PropertyFloat)]
    public float? ShieldBaseValue { get; set; }

    [NetFieldExportHandle(31, RepLayoutCmdType.PropertyFloat)]
    public float? ShieldCurrentValue { get; set; }

    [NetFieldExportHandle(33, RepLayoutCmdType.PropertyFloat)]
    public float? ShieldMaxValue { get; set; }

    [NetFieldExportHandle(37, RepLayoutCmdType.PropertyFloat)]
    public float? ShieldUnclampedBaseValue { get; set; }

    [NetFieldExportHandle(38, RepLayoutCmdType.PropertyFloat)]
    public float? ShieldUnclampedCurrentValue { get; set; }
}
