using Unreal.Core.Attributes;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons;

[NetFieldExportGroup("/Game/Weapons/FORT_Rifles/Blueprints/B_Rifle_Sniper_Athena.B_Rifle_Sniper_Athena_C", minimalParseMode: ParseMode.Debug)]
public class SniperRifle : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Rifles/Blueprints/B_Rifle_Sniper_Athena_HighTier.B_Rifle_Sniper_Athena_HighTier_C", minimalParseMode: ParseMode.Debug)]
public class HighTierSniperRifle : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Rifles/Blueprints/B_Rifle_NoScope_Athena.B_Rifle_NoScope_Athena_C", minimalParseMode: ParseMode.Debug)]
public class NoScopeSniperRifle : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Rifles/Blueprints/B_Rifle_Sniper_Suppressed_Athena.B_Rifle_Sniper_Suppressed_Athena_C", minimalParseMode: ParseMode.Debug)]
public class SuppressedSniperRifle : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Rifles/Blueprints/B_Rifle_Sniper_Heavy_Athena.B_Rifle_Sniper_Heavy_Athena_C", minimalParseMode: ParseMode.Debug)]
public class HeavySniperRifle : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Sniper/Blueprints/B_Prj_Bullet_Sniper.B_Prj_Bullet_Sniper_C", minimalParseMode: ParseMode.Debug)]
public class SniperRifleBullet : BaseProjectile
{
    [NetFieldExport("FireStartLoc", RepLayoutCmdType.PropertyVector10)]
    public FVector FireStartLoc { get; set; }

    [NetFieldExport("PawnHitResult", RepLayoutCmdType.Property)]
    public FHitResult PawnHitResult { get; set; }
}