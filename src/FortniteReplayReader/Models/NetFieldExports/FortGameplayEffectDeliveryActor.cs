using Unreal.Core.Attributes;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportClassNetCache("B_Prj_Athena_FragGrenade_C_ClassNetCache")]
    //[NetFieldExportClassNetCache("B_Prj_Meatball_Missile_C_ClassNetCache")]
    //[NetFieldExportClassNetCache("B_Prj_Ranged_Rocket_Athena_C_ClassNetCache")]
    //[NetFieldExportClassNetCache("B_Prj_Ranged_Rocket_Athena_LowTier_C_ClassNetCache")]
    public class FortGameplayEffectDeliveryActorCache
    {
        [NetFieldExportRPCFunction("BroadcastExplosion", "/Script/FortniteGame.FortGameplayEffectDeliveryActor:BroadcastExplosion")]
        public BroadcastExplosion BroadcastExplosion { get; set; }
    }
}
