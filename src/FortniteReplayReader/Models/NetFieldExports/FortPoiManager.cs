using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports;

[NetFieldExportGroup("/Script/FortniteGame.FortPoiManager", minimalParseMode: ParseMode.Full)]
public class FortPoiManager : INetFieldExportGroup
{
    [NetFieldExport("WorldGridStart", RepLayoutCmdType.PropertyVector2D)]
    public FVector2D WorldGridStart { get; set; }

    [NetFieldExport("WorldGridEnd", RepLayoutCmdType.PropertyVector2D)]
    public FVector2D WorldGridEnd { get; set; }

    [NetFieldExport("WorldGridSpacing", RepLayoutCmdType.PropertyVector2D)]
    public FVector2D WorldGridSpacing { get; set; }

    [NetFieldExport("GridCountX", RepLayoutCmdType.PropertyInt)]
    public int? GridCountX { get; set; }

    [NetFieldExport("GridCountY", RepLayoutCmdType.PropertyInt)]
    public int? GridCountY { get; set; }

    [NetFieldExport("WorldGridTotalSize", RepLayoutCmdType.PropertyVector2D)]
    public FVector2D WorldGridTotalSize { get; set; }

    [NetFieldExport("PoiTagContainerTable", RepLayoutCmdType.DynamicArray)]
    public FGameplayTagContainer[] PoiTagContainerTable { get; set; }

    [NetFieldExport("PoiTagContainerTableSize", RepLayoutCmdType.PropertyInt)]
    public int? PoiTagContainerTableSize { get; set; }
}
