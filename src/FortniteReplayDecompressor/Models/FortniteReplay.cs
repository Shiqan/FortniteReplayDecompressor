using FortniteReplayDecompressor.Core.Models;
using System.Collections.Generic;
using Unreal.Core.Models;

namespace FortniteReplayReaderDecompressor.Core.Models
{
    public class FortniteReplay : Replay
    {
        public IList<PlayerElimination> Eliminations { get; set; } = new List<PlayerElimination>();
        public Stats Stats { get; set; }
        public TeamStats TeamStats { get; set; }
    }
}
