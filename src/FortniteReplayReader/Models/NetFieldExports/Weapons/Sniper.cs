using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Game/Weapons/FORT_Rifles/Blueprints/B_Rifle_Sniper_Athena.B_Rifle_Sniper_Athena_C", minimalParseMode: ParseMode.Debug)]
    public class SniperRifle : BaseWeapon, INetFieldExportGroup
    {
    }
}