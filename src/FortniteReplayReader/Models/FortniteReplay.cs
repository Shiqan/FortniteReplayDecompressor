using FortniteReplayReader.Models.Events;
using System.Collections.Generic;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models
{
    public class FortniteReplay : Replay
    {
        public IList<PlayerElimination> Eliminations { get; set; } = new List<PlayerElimination>();
        public Stats Stats { get; set; }
        public TeamStats TeamStats { get; set; }

        public GameData GameData { get; set; } = new GameData();
        public IList<TeamData> Teams { get; set; } = new List<TeamData>();
        public IList<PlayerData> Players { get; set; } = new List<PlayerData>();
        public IList<KillFeedEntry> KillFeed { get; set; } = new List<KillFeedEntry>();
        public MapData MapData { get; set; } = new MapData();
    }
}
