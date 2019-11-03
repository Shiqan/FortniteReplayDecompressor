using System.Collections.Generic;

namespace Unreal.Core.Models
{
    public class Frame
    {
        public int Index { get; set; }
        public float Time { get; set; }
        public IList<ActorState> ActorStates { get; set; }
    }
}
