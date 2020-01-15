using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons
{
    [NetFieldExportGroup("/Game/Weapons/FORT_RocketLaunchers/Blueprints/B_RocketLauncher_Generic_Athena.B_RocketLauncher_Generic_Athena_C", minimalParseMode: ParseMode.Debug)]
    public class RocketLauncher : BaseWeapon
    {
    }

    [NetFieldExportGroup("/Game/Weapons/FORT_RocketLaunchers/Blueprints/B_RocketLauncher_Generic_Athena_HighTier.B_RocketLauncher_Generic_Athena_HighTier_C", minimalParseMode: ParseMode.Debug)]
    public class HighTierRocketLauncher : BaseWeapon
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
}