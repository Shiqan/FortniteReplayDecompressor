namespace Unreal.Core.Models
{
    public abstract class ActorState
    {
        public uint Id { get; set; }
        public FVector Position { get; set; }
        public FRotator Rotation { get; set; }
        public FVector Velocity { get; set; }
        public FVector AngularVelocity { get; set; }
    }
}
