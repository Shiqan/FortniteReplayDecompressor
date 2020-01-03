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
}