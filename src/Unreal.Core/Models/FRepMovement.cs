using Unreal.Core.Models.Enums;

namespace Unreal.Core.Models
{
    /// <summary>
    /// Replicated movement data of our RootComponent.
    /// see  https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Classes/Engine/EngineTypes.h#L3005
    /// </summary>
    public struct FRepMovement
    {
        /** Velocity of component in world space */
        public FVector LinearVelocity { get; set; }

        /** Velocity of rotation for component */
        public FVector AngularVelocity { get; set; }

        /** Location in world space */
        public FVector Location { get; set; }

        /** Current rotation */
        public FRotator Rotation { get; set; }

        /** If set, RootComponent should be sleeping. */
        public bool bSimulatedPhysicSleep { get; set; }

        /** If set, additional physic data (angular velocity) will be replicated. */
        public bool bRepPhysics { get; set; }

        /** Allows tuning the compression level for the replicated location vector. You should only need to change this from the default if you see visual artifacts. */
        public VectorQuantization LocationQuantizationLevel { get; set; }

        /** Allows tuning the compression level for the replicated velocity vectors. You should only need to change this from the default if you see visual artifacts. */
        public VectorQuantization VelocityQuantizationLevel { get; set; }

        /** Allows tuning the compression level for replicated rotation. You should only need to change this from the default if you see visual artifacts. */
        public RotatorQuantization RotationQuantizationLevel { get; set;}
    }
}
