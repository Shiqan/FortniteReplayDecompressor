using FortniteReplayReader.Contracts;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models.TelemetryEvents
{
    public class DamageEvent : ITelemetryEvent
    {
        public float? ReplicatedWorldTimeSeconds { get; set; }
        public int? ShotPlayerId { get; set; }
        public int? HitPlayerId { get; set; }
        public bool HitPlayer => HitPlayerId > 0;
        public int? WeaponId { get; set; }
        public FVector Location { get; set; }
        public FVector Normal { get; set; }
        public float? Damage { get; set; }
        public bool? WeaponActivate { get; set; }
        public bool? IsFatal { get; set; }
        public bool? IsCritical { get; set; }
        public bool? IsShield { get; set; }
        public bool? IsShieldDestroyed { get; set; }
        public bool? IsBallistic { get; set; }
        public bool? FatalHitNonPlayer { get; set; }
        public bool? CriticalHitNonPlayer { get; set; }
    }
}
