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
        FVector LinearVelocity;

        /** Velocity of rotation for component */
        FVector AngularVelocity;

        /** Location in world space */
        FVector Location;

        /** Current rotation */
        FRotator Rotation;

        /** If set, RootComponent should be sleeping. */
        bool bSimulatedPhysicSleep;

        /** If set, additional physic data (angular velocity) will be replicated. */
        bool bRepPhysics;

        /** Allows tuning the compression level for the replicated location vector. You should only need to change this from the default if you see visual artifacts. */
        VectorQuantization LocationQuantizationLevel;

        /** Allows tuning the compression level for the replicated velocity vectors. You should only need to change this from the default if you see visual artifacts. */
        VectorQuantization VelocityQuantizationLevel;

        /** Allows tuning the compression level for replicated rotation. You should only need to change this from the default if you see visual artifacts. */
        RotatorQuantization RotationQuantizationLevel;
    }
}
