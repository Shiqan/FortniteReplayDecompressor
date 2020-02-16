using Unreal.Core.Contracts;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models.TelemetryEvents
{
    public class LootEvent : ITelemetryEvent
    {
        public float? ReplicatedWorldTimeSeconds { get; set; }
        public int? PlayerId { get; set; }
        public FRepMovement? ReplicatedMovement { get; set; }
        public int? Count { get; set; }
        public uint? ItemOwner { get; set; }
        public string ItemDefinition { get; set; }
        public float? Durability { get; set; }
        public int? Level { get; set; }
        public int? LoadedAmmo { get; set; }
        public uint? A { get; set; }
        public uint? B { get; set; }
        public uint? C { get; set; }
        public uint? D { get; set; }
        public string CombineTarget { get; set; }
        public uint? PickupTarget { get; set; }
        public FVector LootInitialPosition { get; set; }
        public FVector LootFinalPosition { get; set; }
        public float? FlyTime { get; set; }
        public FVector StartDirection { get; set; }
        public FVector FinalTossRestLocation { get; set; }
        public int? TossState { get; set; }
        public int? OptionalOwnerID { get; set; }
        public bool? IsPickedUp { get; set; }
        public uint? DroppedBy { get; set; }
        public ushort? OrderIndex { get; set; }
    }
}
