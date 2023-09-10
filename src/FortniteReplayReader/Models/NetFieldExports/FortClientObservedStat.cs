using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportGroup("/Script/FortniteGame.FortClientObservedStat", minimalParseMode: ParseMode.Debug)]
public class FortClientObservedStat : INetFieldExportGroup
{
    [NetFieldExport("StatName", RepLayoutCmdType.PropertyName)]
    public string StatName { get; set; }

    [NetFieldExport("StatValue", RepLayoutCmdType.PropertyInt)]
    public int? StatValue { get; set; }
}
