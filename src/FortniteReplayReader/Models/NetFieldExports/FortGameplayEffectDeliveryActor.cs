using Unreal.Core.Attributes;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    // TODO move in appropriate classes
    [NetFieldExportClassNetCache("B_Prj_Athena_FragGrenade_C_ClassNetCache", minimalParseMode: ParseMode.Debug)]
    //[NetFieldExportClassNetCache("B_Prj_Meatball_Missile_C_ClassNetCache")]
    //[NetFieldExportClassNetCache("B_Prj_Ranged_Rocket_Athena_C_ClassNetCache")]
    //[NetFieldExportClassNetCache("B_Prj_Ranged_Rocket_Athena_LowTier_C_ClassNetCache")]
    public class FortGameplayEffectDeliveryActorCache
    {
        [NetFieldExportRPCFunction("BroadcastExplosion", "/Script/FortniteGame.FortGameplayEffectDeliveryActor:BroadcastExplosion")]
        public BroadcastExplosion BroadcastExplosion { get; set; }
    }
}
