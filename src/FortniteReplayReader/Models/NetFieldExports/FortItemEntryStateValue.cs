using Unreal.Core.Attributes;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportSubGroup("/Script/FortniteGame.FortPickupAthena", minimalParseMode: ParseMode.Ignore)]
public class FortItemEntryStateValue
{
    [NetFieldExport("StateType", RepLayoutCmdType.Enum)]
    public int StateType { get; set; }

    [NetFieldExport("IntValue", RepLayoutCmdType.PropertyInt)]
    public int? IntValue { get; set; }

    [NetFieldExport("NameValue", RepLayoutCmdType.Property)]
    public FName NameValue { get; set; }
}
