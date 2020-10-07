using Unreal.Core;
using Unreal.Core.Attributes;
using Unreal.Core.Models;
using Unreal.Core.Models.Contracts;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Script/FortniteGame.ActiveGameplayModifier", minimalParseMode: ParseMode.Minimal)]
    public class ActiveGameplayModifier : INetFieldExportGroup
    {
        [NetFieldExport("ModifierDef", RepLayoutCmdType.Property)]
        public ItemDefinition ModifierDef { get; set; }
    }
}
