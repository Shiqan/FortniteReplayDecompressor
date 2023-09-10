using FortniteReplayReader.Models.NetFieldExports.RPC;
using Unreal.Core.Attributes;

namespace FortniteReplayReader.Models.NetFieldExports.Weapons;

public abstract class BaseExplosion
{
    [NetFieldExportRPC("BroadcastExplosion", "/Script/FortniteGame.FortGameplayEffectDeliveryActor:BroadcastExplosion", isFunction: true)]
    public BroadcastExplosion BroadcastExplosion { get; set; }
}
