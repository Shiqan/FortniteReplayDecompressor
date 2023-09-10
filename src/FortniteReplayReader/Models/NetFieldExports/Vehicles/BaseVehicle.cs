using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Vehicles;

[NetFieldExportGroup("/Script/FortniteGame.FortVehicleSeatComponent", minimalParseMode: ParseMode.Ignore)]
public class SeatComponent : INetFieldExportGroup
{
    [NetFieldExport("PlayerSlots", RepLayoutCmdType.Property)]
    public DebuggingObject PlayerSlots { get; set; }

    [NetFieldExport("PlayerEntryTime", RepLayoutCmdType.Property)]
    public DebuggingObject PlayerEntryTime { get; set; }

    [NetFieldExport("WeaponComponent", RepLayoutCmdType.Property)]
    public DebuggingObject WeaponComponent { get; set; }
}

[NetFieldExportGroup("/Script/FortniteGame.FortVehicleSeatWeaponComponent", minimalParseMode: ParseMode.Ignore)]
public class WeaponSeatComponent : INetFieldExportGroup
{
    [NetFieldExport("bWeaponEquipped", RepLayoutCmdType.PropertyBool)]
    public bool bWeaponEquipped { get; set; }

    [NetFieldExport("AmmoInClip", RepLayoutCmdType.Property)]
    public DebuggingObject AmmoInClip { get; set; }

    [NetFieldExport("LastFireTime", RepLayoutCmdType.PropertyFloat)]
    public float LastFireTime { get; set; }

    [NetFieldExport("bHasPrevious", RepLayoutCmdType.PropertyBool)]
    public bool bHasPrevious { get; set; }
}