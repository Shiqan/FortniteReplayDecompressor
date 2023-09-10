using Unreal.Core.Attributes;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Items;

[NetFieldExportGroup("/Game/Building/ActorBlueprints/Containers/Tiered_Chest_Athena.Tiered_Chest_Athena_C", minimalParseMode: ParseMode.Debug)]
public class Chest : BaseContainer
{
    [NetFieldExport("bDestroyOnPlayerBuildingPlacement", RepLayoutCmdType.PropertyBool)]
    public bool bDestroyOnPlayerBuildingPlacement { get; set; }

    [NetFieldExport("ResourceType", RepLayoutCmdType.Enum)]
    public int ResourceType { get; set; }

    [NetFieldExport("ProxyGameplayCueDamagePhysicalMagnitude", RepLayoutCmdType.Ignore)]
    public DebuggingObject ProxyGameplayCueDamagePhysicalMagnitude { get; set; }

    [NetFieldExport("EffectContext", RepLayoutCmdType.Ignore)]
    public DebuggingObject EffectContext { get; set; }
}

[NetFieldExportGroup("/Game/Building/ActorBlueprints/Containers/Creative_Tiered_Chest.Creative_Tiered_Chest_C", minimalParseMode: ParseMode.Debug)]
public class CreativeChest : Chest
{
    [NetFieldExport("SpawnItems", RepLayoutCmdType.Property)]
    public DebuggingObject SpawnItems { get; set; }

    [NetFieldExport("PrimaryAssetName", RepLayoutCmdType.Property)]
    public DebuggingObject PrimaryAssetName { get; set; }

    [NetFieldExport("Quantity", RepLayoutCmdType.Property)]
    public DebuggingObject Quantity { get; set; }
}

[NetFieldExportGroup("/Game/Building/ActorBlueprints/Containers/Tiered_Chest_Athena_FactionChest_NoLocks.Tiered_Chest_Athena_FactionChest_NoLocks_C", minimalParseMode: ParseMode.Debug)]
public class FactionChest : BaseContainer
{
    [NetFieldExport("bDestroyOnPlayerBuildingPlacement", RepLayoutCmdType.PropertyBool)]
    public bool? bDestroyOnPlayerBuildingPlacement { get; set; }

    [NetFieldExport("T_Faction", RepLayoutCmdType.Enum)]
    public int? Faction { get; set; }
}