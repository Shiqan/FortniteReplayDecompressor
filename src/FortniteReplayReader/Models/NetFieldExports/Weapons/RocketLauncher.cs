using Unreal.Core.Attributes;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons;

[NetFieldExportGroup("/Game/Weapons/FORT_RocketLaunchers/Blueprints/B_RocketLauncher_Generic_Athena.B_RocketLauncher_Generic_Athena_C", minimalParseMode: ParseMode.Debug)]
public class RocketLauncher : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_RocketLaunchers/Blueprints/B_RocketLauncher_Generic_Athena_HighTier.B_RocketLauncher_Generic_Athena_HighTier_C", minimalParseMode: ParseMode.Debug)]
public class HighTierRocketLauncher : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_RocketLaunchers/Blueprints/B_Prj_Pumpkin_RPG_Athena_LowTier.B_Prj_Pumpkin_RPG_Athena_LowTier_C", minimalParseMode: ParseMode.Debug)]
public class PumpkinLauncher : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_RocketLaunchers/Blueprints/B_Launcher_Pumpkin_RPG_Athena.B_Launcher_Pumpkin_RPG_Athena_C", minimalParseMode: ParseMode.Debug)]
public class HighTierPumpkinLauncher : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_RocketLaunchers/Blueprints/B_RocketLauncher_Military_Athena.B_RocketLauncher_Military_Athena_C", minimalParseMode: ParseMode.Debug)]
public class QuadLauncher : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_GrenadeLaunchers/Blueprints/B_GrenadeLauncher_Prox_Athena.B_GrenadeLauncher_Prox_Athena_C", minimalParseMode: ParseMode.Debug)]
public class ProximityLauncher : BaseWeapon
{
}


[NetFieldExportGroup("/Game/Weapons/FORT_RocketLaunchers/Blueprints/B_Prj_Ranged_Rocket_Athena.B_Prj_Ranged_Rocket_Athena_C", minimalParseMode: ParseMode.Debug)]
public class RocketLauncherProjectile : BaseRocketLauncherProjectile
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_RocketLaunchers/Blueprints/B_Prj_Ranged_Rocket_Athena_LowTier.B_Prj_Ranged_Rocket_Athena_LowTier_C", minimalParseMode: ParseMode.Debug)]
public class LowTierRocketLauncherProjectile : BaseRocketLauncherProjectile
{
}



[NetFieldExportClassNetCache("B_Prj_Ranged_Rocket_Athena_C_ClassNetCache", minimalParseMode: ParseMode.Debug)]
public class RocketLauncherProjectileClassNetCache : BaseExplosion
{
}

[NetFieldExportClassNetCache("B_Prj_Ranged_Rocket_Athena_LowTier_C_ClassNetCache", minimalParseMode: ParseMode.Debug)]
public class LowTierRocketLauncherProjectileClassNetCache : BaseExplosion
{
}