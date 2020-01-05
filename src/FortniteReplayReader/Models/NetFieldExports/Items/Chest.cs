using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("/Game/Building/ActorBlueprints/Containers/Tiered_Chest_Athena.Tiered_Chest_Athena_C", minimalParseMode: ParseMode.Debug)]
    public class Chest : BaseContainer, INetFieldExportGroup
    {
        [NetFieldExport("bDestroyOnPlayerBuildingPlacement", RepLayoutCmdType.PropertyBool)]
        public bool bDestroyOnPlayerBuildingPlacement { get; set; }

        [NetFieldExport("ResourceType", RepLayoutCmdType.Property)]
        public DebuggingObject ResourceType { get; set; }

        [NetFieldExport("ProxyGameplayCueDamagePhysicalMagnitude", RepLayoutCmdType.Property)]
        public DebuggingObject ProxyGameplayCueDamagePhysicalMagnitude { get; set; }

        [NetFieldExport("EffectContext", RepLayoutCmdType.Property)]
        public DebuggingObject EffectContext { get; set; }
    }
}