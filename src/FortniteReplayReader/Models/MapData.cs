using System.Collections.Generic;

namespace FortniteReplayReader.Models
{
    public class MapData
    {
        public IEnumerable<BattleBus> BattleBusFlightPaths { get; set; }
        public IList<SafeZone> SafeZones { get; set; } = new List<SafeZone>();

        //public IEnumerable<LLama> lLamas { get; set; }
        //public IEnumerable<RebootVan> RebootVans { get; set; }
        //public IEnumerable<SupplyDrop> SupplyDrops { get; set; }

        //public FortPoiManager PoiManager { get; set; }

        // chests
        // loot
        // markers
    }
}
