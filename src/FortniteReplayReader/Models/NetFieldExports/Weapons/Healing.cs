using Unreal.Core.Attributes;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons;

public class Healing
{
    [NetFieldExportGroup("/Game/Athena/Items/Gameplay/Lotus/Mustache/B_Ranged_Lotus_Mustache.B_Ranged_Lotus_Mustache_C", minimalParseMode: ParseMode.Debug)]
    public class BandageBazooka : BaseWeapon
    {
        [NetFieldExport("OverheatState", RepLayoutCmdType.Enum)]
        public int? OverheatState { get; set; }
    }
}
