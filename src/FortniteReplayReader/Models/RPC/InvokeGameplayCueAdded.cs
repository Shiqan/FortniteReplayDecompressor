using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models
{
    [NetFieldExportGroup("/Script/FortniteGame.FortPawn:NetMulticast_InvokeGameplayCueAdded_WithParams")]
    public class InvokeGameplayCueAdded : INetFieldExportGroup
    {
        [NetFieldExport("GameplayCueTag", RepLayoutCmdType.Property)]
        public FGameplayTag GameplayCueTag { get; set; }
        
        [NetFieldExport("Parameters", RepLayoutCmdType.Property)]
        public FGameplayCueParameters Parameters { get; set; }
        
        [NetFieldExport("PredictionKey", RepLayoutCmdType.Property)]
        public FPredictionKey PredictionKey { get; set; }
    }
}
