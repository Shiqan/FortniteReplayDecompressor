using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Game/Weapons/FORT_RocketLaunchers/Blueprints/B_RocketLauncher_Generic_Athena.B_RocketLauncher_Generic_Athena_C", minimalParseMode: ParseMode.Debug)]
    public class RocketLauncher : BaseWeapon, INetFieldExportGroup
    {
    }
    
    [NetFieldExportGroup("/Game/Weapons/FORT_RocketLaunchers/Blueprints/B_RocketLauncher_Generic_Athena_HighTier.B_RocketLauncher_Generic_Athena_HighTier_C", minimalParseMode: ParseMode.Debug)]
    public class HighTierRocketLauncher : BaseWeapon, INetFieldExportGroup
    {
    }

    public abstract class BaseRocketLauncherProjectile : BaseProjectile, INetFieldExportGroup
    {
        [NetFieldExport("StopLocation", RepLayoutCmdType.PropertyVector)]
        public FVector StopLocation { get; set; }

        [NetFieldExport("DecalLocation", RepLayoutCmdType.PropertyVector)]
        public FVector DecalLocation { get; set; }

        [NetFieldExport("PawnHitResult", RepLayoutCmdType.Property)]
        public FHitResult PawnHitResult { get; set; }

        [NetFieldExport("bHasExploded", RepLayoutCmdType.PropertyBool)]
        public bool bHasExploded { get; set; }

        [NetFieldExport("bIsBeingKilled", RepLayoutCmdType.PropertyBool)]
        public bool bIsBeingKilled { get; set; }
    }

    [NetFieldExportGroup("/Game/Weapons/FORT_RocketLaunchers/Blueprints/B_Prj_Ranged_Rocket_Athena.B_Prj_Ranged_Rocket_Athena_C", minimalParseMode: ParseMode.Debug)]
    public class RocketLauncherProjectile : BaseRocketLauncherProjectile
    {
    }
    [NetFieldExportGroup("/Game/Weapons/FORT_RocketLaunchers/Blueprints/B_Prj_Ranged_Rocket_Athena_LowTier.B_Prj_Ranged_Rocket_Athena_LowTier_C", minimalParseMode: ParseMode.Debug)]
    public class LowTierRocketLauncherProjectile : BaseRocketLauncherProjectile
    {
        // replicated movement is one decimal...
    }
}