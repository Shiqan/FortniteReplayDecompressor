using System.Collections.Generic;

namespace FortniteReplayReader.Models;

public class TeamData
{
    public int? TeamIndex { get; set; }
    public IList<int?> PlayerIds { get; set; }
    public IList<string?> PlayerNames { get; set; }
    public int? PartyOwnerId { get; set; }
    public int? Placement { get; set; }
    public uint? TeamKills { get; set; }
}
