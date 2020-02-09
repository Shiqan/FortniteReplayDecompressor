using FortniteReplayReader.Contracts;

namespace FortniteReplayReader.Models.TelemetryEvents
{
    public class LlamaEvent : ITelemetryEvent
    {
        public float? ReplicatedWorldTimeSeconds { get; set; }
    }
}
