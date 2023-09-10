using System.Collections.Generic;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models;

public class MapData
{
    public IEnumerable<BattleBus> BattleBusFlightPaths { get; internal set; }
    public IList<SafeZone> SafeZones { get; internal set; } = new List<SafeZone>();
    public IList<Llama> Llamas { get; internal set; } = new List<Llama>();
    public IList<SupplyDrop> SupplyDrops { get; internal set; } = new List<SupplyDrop>();
    public IList<RebootVan> RebootVans { get; internal set; } = new List<RebootVan>();


    public FVector2D WorldGridStart { get; internal set; }
    public FVector2D WorldGridEnd { get; internal set; }
    public FVector2D WorldGridSpacing { get; internal set; }
    public int? GridCountX { get; internal set; }
    public int? GridCountY { get; internal set; }
    public FVector2D WorldGridTotalSize { get; internal set; }
}
