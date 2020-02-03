using System.Collections.Generic;
using Unreal.Core.Contracts;

namespace Unreal.Core.Models
{
    public class Replay
    {
        public ReplayInfo Info { get; set; } = new ReplayInfo();
        public ReplayHeader Header { get; set; } = new ReplayHeader();
        public IList<IEvent> Events { get; set; } = new List<IEvent>();
    }
}
