using FortniteReplayReader.Contracts;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models.TelemetryEvents
{
    public class LlamaEvent : ITelemetryEvent
    {
        public float? ReplicatedWorldTimeSeconds { get; set; }


        public uint Id { get; set; }
        public FRepMovement ReplicatedMovement { get; set; }
        public bool Opened { get; set; }
        public bool BalloonPopped { get; set; }
    }
}
