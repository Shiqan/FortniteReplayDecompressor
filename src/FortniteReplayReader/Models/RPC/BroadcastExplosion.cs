using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models
{
    [NetFieldExportGroup("/Script/FortniteGame.FortGameplayEffectDeliveryActor:BroadcastExplosion")]
    public class BroadcastExplosion : INetFieldExportGroup
    {
        [NetFieldExport("HitActors", RepLayoutCmdType.DynamicArray)]
        public uint[] HitActors { get; set; }

        [NetFieldExport("HitResults", RepLayoutCmdType.DynamicArray)]
        public FHitResult[] HitResults { get; set; }
    }
}
