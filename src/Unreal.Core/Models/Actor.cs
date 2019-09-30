using System;
using System.Collections.Generic;
using System.Text;

namespace Unreal.Core.Models
{
    public class Actor
    {
        public NetworkGUID ActorNetGUID { get; set; }
        public NetworkGUID Archetype { get; set; }
        public NetworkGUID Level { get; set; }
        public FVector Location { get; set; }
        public FVector Rotation { get; set; }
        public FVector Scale { get; set; }
        public FVector Velocity { get; set; }
    }
}
