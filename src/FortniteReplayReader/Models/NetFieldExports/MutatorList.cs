using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportGroup("/Script/FortniteGame.FortMutatorListComponent", minimalParseMode: ParseMode.Ignore)]
public class MutatorList : INetFieldExportGroup
{
    [NetFieldExport("OverrideMode", RepLayoutCmdType.Property)]
    public DebuggingObject OverrideMode { get; set; }
}
