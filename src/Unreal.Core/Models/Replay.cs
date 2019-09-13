using System.Collections.Generic;
using Unreal.Core.Contracts;

namespace Unreal.Core.Models
{
    public class Replay
    {
        public ReplayInfo Info { get; set; }
        public ReplayHeader Header { get; set; }
        public IList<IEvent> Events { get; set; } = new List<IEvent>();
    }
}
