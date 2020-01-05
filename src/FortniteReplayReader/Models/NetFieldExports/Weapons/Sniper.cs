using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Game/Weapons/FORT_Rifles/Blueprints/B_Rifle_Sniper_Athena.B_Rifle_Sniper_Athena_C", minimalParseMode: ParseMode.Debug)]
    public class SniperRifle : BaseWeapon, INetFieldExportGroup
    {
    }
    
    [NetFieldExportGroup("/Game/Weapons/FORT_Sniper/Blueprints/B_Prj_Bullet_Sniper.B_Prj_Bullet_Sniper_C", minimalParseMode: ParseMode.Debug)]
    public class SniperRifleBullet : BaseProjectile, INetFieldExportGroup
    {
        [NetFieldExport("FireStartLoc", RepLayoutCmdType.PropertyVector100)]
        public FVector FireStartLoc { get; set; }

        [NetFieldExport("PawnHitResult", RepLayoutCmdType.Property)]
        public FHitResult PawnHitResult { get; set; }
    }
}