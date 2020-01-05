using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Game/Athena/Items/Traps/TrapTool_ContextTrap_Athena.TrapTool_ContextTrap_Athena_C", minimalParseMode: ParseMode.Debug)]
    public class Trap : BaseWeapon, INetFieldExportGroup
    {
        [NetFieldExport("ItemDefinition", RepLayoutCmdType.PropertyObject)]
        public uint ItemDefinition { get; set; }

        [NetFieldExport("ContextTrapItemDefinition", RepLayoutCmdType.PropertyObject)]
        public uint ContextTrapItemDefinition { get; set; }
    }
}