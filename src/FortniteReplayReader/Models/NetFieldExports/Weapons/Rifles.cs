using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Game/Weapons/FORT_Rifles/Blueprints/Assault/B_Assault_Auto_Athena.B_Assault_Auto_Athena_C", minimalParseMode: ParseMode.Debug)]
    public class AutoAssaultRifle : BaseWeapon, INetFieldExportGroup
    {
    }
    
    [NetFieldExportGroup("/Game/Weapons/FORT_Rifles/Blueprints/Assault/B_Assault_BurstBullpup_Athena.B_Assault_BurstBullpup_Athena_C", minimalParseMode: ParseMode.Debug)]
    public class BurstAssaultRifle : BaseWeapon, INetFieldExportGroup
    {
    }

    [NetFieldExportGroup("/Game/Weapons/FORT_Rifles/Blueprints/Assault/B_Assault_BurstBullpup_Athena_HighTier.B_Assault_BurstBullpup_Athena_HighTier_C", minimalParseMode: ParseMode.Debug)]
    public class HighTierBurstAssaultRifle : BaseWeapon, INetFieldExportGroup
    {
    }
}