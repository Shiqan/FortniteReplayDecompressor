using FortniteReplayReader.Models.NetFieldExports;
using System.Collections.Generic;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models
{
    public class MapData
    {
        public IEnumerable<BattleBus> BattleBusFlightPaths { get; set; }
        public IList<SafeZone> SafeZones { get; set; } = new List<SafeZone>();
        public IList<Llama> Llamas { get; set; } = new List<Llama>();
        public IList<SupplyDrop> SupplyDrops { get; set; } = new List<SupplyDrop>();
        public IList<RebootVan> RebootVans { get; set; } = new List<RebootVan>();


        public FVector2D WorldGridStart { get; set; }
        public FVector2D WorldGridEnd { get; set; }
        public FVector2D WorldGridSpacing { get; set; }
        public int? GridCountX { get; set; }
        public int? GridCountY { get; set; }
        public FVector2D WorldGridTotalSize { get; set; }


        // chests
        // loot
        // markers
    }
}
