using FortniteReplayReader.Contracts;

namespace FortniteReplayReader.Models.TelemetryEvents
{
    public class PlayerLocationEvent : ITelemetryEvent
    {
        public float? ReplicatedWorldTimeSeconds { get; set; }
    }
}
