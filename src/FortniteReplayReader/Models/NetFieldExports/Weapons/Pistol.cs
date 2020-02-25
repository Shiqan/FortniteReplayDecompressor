using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons
{
    [NetFieldExportGroup("/Game/Weapons/FORT_Pistols/Blueprints/B_Pistol_Vigilante_Athena.B_Pistol_Vigilante_Athena_C", minimalParseMode: ParseMode.Debug)]
    public class Pistol : BaseWeapon
    {
    }    
    
    [NetFieldExportGroup("/Game/Weapons/FORT_Pistols/Blueprints/B_Pistol_Vigilante_Athena_HighTier.B_Pistol_Vigilante_Athena_HighTier_C", minimalParseMode: ParseMode.Debug)]
    public class HighTierPistol : BaseWeapon
    {
    }
}