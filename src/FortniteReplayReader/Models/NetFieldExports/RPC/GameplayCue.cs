using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    public abstract class GameplayCue
    {
        [NetFieldExport("GameplayCueTag", RepLayoutCmdType.Property)]
        public FGameplayTag GameplayCueTag { get; set; }

        [NetFieldExport("Parameters", RepLayoutCmdType.Ignore)]
        public FGameplayCueParameters Parameters { get; set; }

        [NetFieldExport("PredictionKey", RepLayoutCmdType.Property)]
        public FPredictionKey PredictionKey { get; set; }
    }

    [NetFieldExportGroup("/Script/FortniteGame.FortPawn:NetMulticast_InvokeGameplayCueAdded_WithParams", minimalParseMode: ParseMode.Debug)]
    public class GameplayCueAdded : GameplayCue, INetFieldExportGroup
    {
    }

    [NetFieldExportGroup("/Script/FortniteGame.FortPawn:NetMulticast_InvokeGameplayCueExecuted_WithParams", minimalParseMode: ParseMode.Debug)]
    public class GameplayCueExecuted : GameplayCue, INetFieldExportGroup
    {
    }
}
