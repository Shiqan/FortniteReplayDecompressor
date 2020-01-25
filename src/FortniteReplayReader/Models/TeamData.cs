using System.Collections.Generic;

namespace FortniteReplayReader.Models
{
    public class TeamData
    {
        public int TeamIndex { get; set; }
        public IEnumerable<string> PlayerIds { get; set; }
        public string PartyOwnerId { get; set; }
        public int Placement { get; set; }
    }
}
