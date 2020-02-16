using Unreal.Core.Contracts;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models.TelemetryEvents
{
    public class HealthEvent : ITelemetryEvent
    {
        public float? ReplicatedWorldTimeSeconds { get; set; }
        public int? PlayerId { get; set; }
        public float HealthBaseValue { get; set; }
        public float HealthCurrentValue { get; set; }
        public float HealthMaxValue { get; set; }
        public float HealthUnclampedBaseValue { get; set; }
        public float HealthUnclampedCurrentValue { get; set; }
        public float ShieldBaseValue { get; set; }
        public float ShieldCurrentValue { get; set; }
        public float ShieldMaxValue { get; set; }
    }
}
