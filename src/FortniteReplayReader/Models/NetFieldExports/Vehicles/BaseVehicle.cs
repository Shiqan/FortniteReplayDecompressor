using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Contracts;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Vehicles
{
    [NetFieldExportGroup("/Script/FortniteGame.FortVehicleSeatComponent", minimalParseMode: ParseMode.Ignore)]
    public class SeatComponent : INetFieldExportGroup
    {
        [NetFieldExport("PlayerSlots", RepLayoutCmdType.Property)]
        public object PlayerSlots { get; set; }

        [NetFieldExport("PlayerEntryTime", RepLayoutCmdType.Property)]
        public object PlayerEntryTime { get; set; }

        [NetFieldExport("WeaponComponent", RepLayoutCmdType.Property)]
        public object WeaponComponent { get; set; }
    }

    [NetFieldExportGroup("/Script/FortniteGame.FortVehicleSeatWeaponComponent", minimalParseMode: ParseMode.Ignore)]
    public class WeaponSeatComponent : INetFieldExportGroup
    {
        [NetFieldExport("bWeaponEquipped", RepLayoutCmdType.PropertyBool)]
        public bool bWeaponEquipped { get; set; }

        [NetFieldExport("AmmoInClip", RepLayoutCmdType.Property)]
        public object AmmoInClip { get; set; }

        [NetFieldExport("LastFireTime", RepLayoutCmdType.PropertyFloat)]
        public float LastFireTime { get; set; }

        [NetFieldExport("bHasPrevious", RepLayoutCmdType.PropertyBool)]
        public bool bHasPrevious { get; set; }
    }
}