using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons
{
    [NetFieldExportGroup("/Game/Weapons/FORT_Shotguns/Blueprints/B_Shotgun_Heavy_Athena.B_Shotgun_Heavy_Athena_C", minimalParseMode: ParseMode.Debug)]
    public class HeavyShotgun : BaseWeapon
    {
    }

    [NetFieldExportGroup("/Game/Weapons/FORT_Shotguns/Blueprints/B_Shotgun_Standard_Athena.B_Shotgun_Standard_Athena_C", minimalParseMode: ParseMode.Debug)]
    public class StandardShotgun : BaseWeapon
    {
    }
    
    [NetFieldExportGroup("/Game/Weapons/FORT_Shotguns/Blueprints/B_Shotgun_Standard_TopTier_Athena.B_Shotgun_Standard_TopTier_Athena_C", minimalParseMode: ParseMode.Debug)]
    public class StandardTopTierShotgun : BaseWeapon
    {
    }
    
    [NetFieldExportGroup("/Game/Weapons/FORT_Shotguns/Blueprints/B_Shotgun_HighSemiAuto_Athena.B_Shotgun_HighSemiAuto_Athena_C", minimalParseMode: ParseMode.Debug)]
    public class HighSemiAutoShotgun : BaseWeapon
    {
    }
}