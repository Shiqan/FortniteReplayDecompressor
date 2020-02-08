using FortniteReplayReader.Contracts;

namespace FortniteReplayReader.Models.TelemetryEvents
{
    public class RebootVanEvent : ITelemetryEvent
    {
        public float? ReplicatedWorldTimeSeconds { get; set; }
        public int Id { get; set; }
        public int State { get; set; }
        public float CooldownStartTime { get; set; }
        public float CooldownEndTime { get; set; }
    }
}
