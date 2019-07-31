using FortniteReplayReaderDecompressor.Core.Contracts;
using System.Collections.Generic;

namespace FortniteReplayReaderDecompressor.Core.Models
{
    public class Replay
    {
        public ReplayInfo Info { get; set; }
        public ReplayHeader Header { get; set; }
        public IList<IEvent> Events { get; set; } = new List<IEvent>();
    }
}
