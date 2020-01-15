using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Items
{
    [NetFieldExportGroup("/Game/Building/ActorBlueprints/Containers/Tiered_Ammo_Athena.Tiered_Ammo_Athena_C", minimalParseMode: ParseMode.Debug)]
    public class AmmoBox : BaseContainer, INetFieldExportGroup
    {
        [NetFieldExport("bInstantDeath", RepLayoutCmdType.PropertyBool)]
        public bool bInstantDeath { get; set; }
    }
}