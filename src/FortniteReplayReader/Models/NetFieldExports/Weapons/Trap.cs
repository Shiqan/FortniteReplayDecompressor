using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons;

[NetFieldExportGroup("/Game/Athena/Items/Traps/TrapTool_ContextTrap_Athena.TrapTool_ContextTrap_Athena_C", minimalParseMode: ParseMode.Debug)]
public class Trap : BaseWeapon
{
    [NetFieldExport("ItemDefinition", RepLayoutCmdType.Property)]
    public ItemDefinition ItemDefinition { get; set; }

    [NetFieldExport("ContextTrapItemDefinition", RepLayoutCmdType.Property)]
    public ItemDefinition ContextTrapItemDefinition { get; set; }
}

[NetFieldExportGroup("/Game/Athena/Items/Traps/Launchpad/BluePrint/Trap_Floor_Player_Launch_Pad.Trap_Floor_Player_Launch_Pad_C", minimalParseMode: ParseMode.Debug)]
public class LaunchPad : INetFieldExportGroup
{
    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public int? RemoteRole { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public int? Role { get; set; }

    [NetFieldExport("OwnerPersistentID", RepLayoutCmdType.PropertyUInt32)]
    public uint OwnerPersistentID { get; set; }

    [NetFieldExport("bPlayerPlaced", RepLayoutCmdType.PropertyBool)]
    public bool? bPlayerPlaced { get; set; }

    [NetFieldExport("TeamIndex", RepLayoutCmdType.Enum)]
    public int? TeamIndex { get; set; }

    [NetFieldExport("TrapData", RepLayoutCmdType.Property)]
    public ItemDefinition TrapData { get; set; }

    [NetFieldExport("AttachedTo", RepLayoutCmdType.PropertyInt)]
    public int? AttachedTo { get; set; }
}