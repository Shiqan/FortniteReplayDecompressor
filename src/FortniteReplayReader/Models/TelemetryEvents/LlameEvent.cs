using FortniteReplayReader.Contracts;

namespace FortniteReplayReader.Models.TelemetryEvents
{
    public class SupplyDropEvent : ITelemetryEvent
    {
        public float? ReplicatedWorldTimeSeconds { get; set; }
    }
}
