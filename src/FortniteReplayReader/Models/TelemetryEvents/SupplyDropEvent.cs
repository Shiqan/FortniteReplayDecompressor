using Unreal.Core.Contracts;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models.TelemetryEvents
{
    public class SupplyDropEvent : ITelemetryEvent
    {
        public float? ReplicatedWorldTimeSeconds { get; set; }

        public uint Id { get; set; }
        public FRepMovement ReplicatedMovement { get; set; }
        public bool HasSpawnedPickups { get; set; }
        public bool Opened { get; set; }
        public bool BalloonPopped { get; set; }
        public float FallSpeed { get; set; }
        public FVector LandingLocation { get; set; }
        public float FallHeight { get; set; }
    }
}
