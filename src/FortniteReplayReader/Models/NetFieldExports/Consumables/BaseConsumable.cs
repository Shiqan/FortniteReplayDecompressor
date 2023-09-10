using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Consumables;

public abstract class BaseConsumable : INetFieldExportGroup
{
    [NetFieldExport("bHidden", RepLayoutCmdType.Ignore)]
    public bool bHidden { get; set; }

    [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
    public int RemoteRole { get; set; }

    [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
    public int Role { get; set; }

    [NetFieldExport("Owner", RepLayoutCmdType.Ignore)]
    public uint Owner { get; set; }

    [NetFieldExport("Instigator", RepLayoutCmdType.PropertyObject)]
    public uint Instigator { get; set; }

    [NetFieldExport("bIsEquippingWeapon", RepLayoutCmdType.PropertyBool)]
    public bool bIsEquippingWeapon { get; set; }

    [NetFieldExport("WeaponData", RepLayoutCmdType.Property)]
    public ItemDefinition WeaponData { get; set; }

    [NetFieldExport("WeaponLevel", RepLayoutCmdType.PropertyInt)]
    public int WeaponLevel { get; set; }

    [NetFieldExport("A", RepLayoutCmdType.PropertyUInt32)]
    public uint A { get; set; }

    [NetFieldExport("B", RepLayoutCmdType.PropertyUInt32)]
    public uint B { get; set; }

    [NetFieldExport("C", RepLayoutCmdType.PropertyUInt32)]
    public uint C { get; set; }

    [NetFieldExport("D", RepLayoutCmdType.PropertyUInt32)]
    public uint D { get; set; }
}
