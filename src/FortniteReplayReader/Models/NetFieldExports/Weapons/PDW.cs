using Unreal.Core.Attributes;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons;

[NetFieldExportGroup("/Game/Weapons/FORT_Pistols/Blueprints/B_Pistol_Light_PDW_Athena.B_Pistol_Light_PDW_Athena_C", minimalParseMode: ParseMode.Debug)]
public class LightPWD : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Pistols/Blueprints/B_Pistol_PDW_Athena_HighTier.B_Pistol_PDW_Athena_HighTier_C", minimalParseMode: ParseMode.Debug)]
public class HighTierPWD : BaseWeapon
{
}


[NetFieldExportGroup("/Game/Weapons/FORT_Pistols/Blueprints/B_Pistol_RapidFireSMG_Athena.B_Pistol_RapidFireSMG_Athena_C", minimalParseMode: ParseMode.Debug)]
public class RapidFireSMG : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Pistols/Blueprints/B_Pistol_PDW_Athena.B_Pistol_PDW_Athena_C", minimalParseMode: ParseMode.Debug)]
public class PistolPDW : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Pistols/Blueprints/B_Pistol_AutoHeavy_Athena_Supp_Child.B_Pistol_AutoHeavy_Athena_Supp_Child_C", minimalParseMode: ParseMode.Debug)]
public class SuppressedSMG : BaseWeapon
{
}