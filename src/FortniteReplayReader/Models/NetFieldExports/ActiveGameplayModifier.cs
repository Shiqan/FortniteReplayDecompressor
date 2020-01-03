using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Script/FortniteGame.ActiveGameplayModifier", minimalParseMode: ParseMode.Debug)]
    public class ActiveGameplayModifier : INetFieldExportGroup
    {
        [NetFieldExport("ModifierDef", RepLayoutCmdType.Property)]
        public DebuggingObject ModifierDef { get; set; }
    }
}
