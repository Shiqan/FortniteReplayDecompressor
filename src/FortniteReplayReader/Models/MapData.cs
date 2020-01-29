using System.Collections.Generic;

namespace FortniteReplayReader.Models
{
    public class MapData
    {
        public IEnumerable<BattleBus> BattleBusFlightPaths { get; set; }
        public IList<SafeZone> SafeZones { get; set; } = new List<SafeZone>();
        public IList<Llama> Llamas { get; set; } = new List<Llama>();
        public IList<SupplyDrop> SupplyDrops { get; set; } = new List<SupplyDrop>();

        //public IEnumerable<RebootVan> RebootVans { get; set; }

        //public FortPoiManager PoiManager { get; set; }

        // chests
        // loot
        // markers
    }
}
