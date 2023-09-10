using Unreal.Core.Attributes;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons;

[NetFieldExportGroup("/Game/Weapons/FORT_Pistols/Blueprints/B_Pistol_Vigilante_Athena.B_Pistol_Vigilante_Athena_C", minimalParseMode: ParseMode.Debug)]
public class Pistol : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Pistols/Blueprints/B_Pistol_Vigilante_Athena_HighTier.B_Pistol_Vigilante_Athena_HighTier_C", minimalParseMode: ParseMode.Debug)]
public class HighTierPistol : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Pistols/Blueprints/B_Pistol_Vigilante_Supp_Athena.B_Pistol_Vigilante_Supp_Athena_C", minimalParseMode: ParseMode.Debug)]
public class SuppressedPistol : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Pistols/Blueprints/B_Pistol_SingleActionRevolver_Athena.B_Pistol_SingleActionRevolver_Athena_C", minimalParseMode: ParseMode.Debug)]
public class Revolver : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Pistols/Blueprints/B_Pistol_Revolver_Futuristic_Athena.B_Pistol_Revolver_Futuristic_Athena_C", minimalParseMode: ParseMode.Debug)]
public class RevolverHighTier : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Pistols/Blueprints/B_DualPistol_Athena.B_DualPistol_Athena_C", minimalParseMode: ParseMode.Debug)]
public class DualPistols : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Pistols/Blueprints/B_Pistol_Handcannon_Athena.B_Pistol_Handcannon_Athena_C", minimalParseMode: ParseMode.Debug)]
public class HandCannon : BaseWeapon
{
}