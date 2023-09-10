using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportGroup("/Script/FortniteGame.FortRegenHealthSet", minimalParseMode: ParseMode.Full)]
public class HealthSet : INetFieldExportGroup
{
    [NetFieldExportHandle(0, RepLayoutCmdType.PropertyFloat)]
    public float HealthBaseValue { get; set; }

    [NetFieldExportHandle(1, RepLayoutCmdType.PropertyFloat)]
    public float HealthCurrentValue { get; set; }

    [NetFieldExportHandle(3, RepLayoutCmdType.PropertyFloat)]
    public float HealthMaxValue { get; set; }

    [NetFieldExportHandle(7, RepLayoutCmdType.PropertyFloat)]
    public float HealthUnclampedBaseValue { get; set; }

    [NetFieldExportHandle(8, RepLayoutCmdType.PropertyFloat)]
    public float HealthUnclampedCurrentValue { get; set; }

    //[NetFieldExportHandle(9, RepLayoutCmdType.PropertyFloat)]
    //public float BaseValue { get; set; }

    //[NetFieldExportHandle(10, RepLayoutCmdType.PropertyFloat)]
    //public float CurrentValue { get; set; }

    //[NetFieldExportHandle(15, RepLayoutCmdType.PropertyFloat)]
    //public float Maximum { get; set; }

    //[NetFieldExportHandle(16, RepLayoutCmdType.PropertyFloat)]
    //public float UnClampedBaseValue { get; set; }

    //[NetFieldExportHandle(17, RepLayoutCmdType.PropertyFloat)]
    //public float UnclampedCurrentValue { get; set; }

    [NetFieldExportHandle(18, RepLayoutCmdType.PropertyFloat)]
    public float ShieldBaseValue { get; set; }

    [NetFieldExportHandle(19, RepLayoutCmdType.PropertyFloat)]
    public float ShieldCurrentValue { get; set; }

    [NetFieldExportHandle(21, RepLayoutCmdType.PropertyFloat)]
    public float ShieldMaxValue { get; set; }
}
