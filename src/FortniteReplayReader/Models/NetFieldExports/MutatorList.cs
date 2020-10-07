using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Contracts;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Script/FortniteGame.FortMutatorListComponent", minimalParseMode: ParseMode.Ignore)]
    public class MutatorList : INetFieldExportGroup
    {
        [NetFieldExport("OverrideMode", RepLayoutCmdType.Property)]
        public object OverrideMode { get; set; }
    }
}
