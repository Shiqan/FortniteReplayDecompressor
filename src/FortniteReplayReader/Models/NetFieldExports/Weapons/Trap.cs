using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons
{
    [NetFieldExportGroup("/Game/Athena/Items/Traps/TrapTool_ContextTrap_Athena.TrapTool_ContextTrap_Athena_C", minimalParseMode: ParseMode.Debug)]
    public class Trap : BaseWeapon
    {
        [NetFieldExport("ItemDefinition", RepLayoutCmdType.Property)]
        public ItemDefinition ItemDefinition { get; set; }

        [NetFieldExport("ContextTrapItemDefinition", RepLayoutCmdType.Property)]
        public ItemDefinition ContextTrapItemDefinition { get; set; }
    }
}