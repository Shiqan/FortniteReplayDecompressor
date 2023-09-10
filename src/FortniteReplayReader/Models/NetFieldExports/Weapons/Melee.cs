using Unreal.Core.Attributes;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons;

[NetFieldExportGroup("/Game/Weapons/FORT_Melee/Blueprints/B_Athena_Pickaxe_Generic.B_Athena_Pickaxe_Generic_C", minimalParseMode: ParseMode.Debug)]
public class Pickaxe : BaseWeapon
{
}

[NetFieldExportGroup("/Game/Weapons/FORT_Melee/Blueprints/B_Athena_Pickaxe_DualWield_Generic.B_Athena_Pickaxe_DualWield_Generic_C", minimalParseMode: ParseMode.Debug)]
public class DualWieldPickaxe : BaseWeapon
{
}