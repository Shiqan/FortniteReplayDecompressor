using Unreal.Core.Contracts;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models.TelemetryEvents
{
    public class LlamaEvent : ITelemetryEvent
    {
        public float? ReplicatedWorldTimeSeconds { get; set; }
        public uint Id { get; set; }
        public FRepMovement? ReplicatedMovement { get; set; }
        public bool HasSpawnedPickups { get; set; }
        public bool Looted { get; set; }
        public FVector FinalDestination { get; set; }
    }
}
