using System.Collections.Generic;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models
{
    public class FortniteReplay : Replay
    {
        public IList<PlayerElimination> Eliminations { get; set; } = new List<PlayerElimination>();
        public Stats Stats { get; set; }
        public TeamStats TeamStats { get; set; }

        public GameData GameData { get; set; }
        public IList<TeamData> Teams { get; set; }
        public IList<PlayerData> Players { get; set; }
        public IList<KillFeedEntry> KillFeed { get; set; }
        public MapData MapData { get; set; }
    }
}
