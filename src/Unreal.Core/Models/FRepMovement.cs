using Unreal.Core.Models.Enums;

namespace Unreal.Core.Models;

/// <summary>
/// Replicated movement data of our RootComponent.
/// see  https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Classes/Engine/EngineTypes.h#L3005
/// </summary>
public struct FRepMovement
{
    /// <summary>
    /// Velocity of component in world space
    /// </summary>
    public FVector LinearVelocity { get; set; }

    /// <summary>
    /// Velocity of rotation for component
    /// </summary>
    public FVector AngularVelocity { get; set; }

    /// <summary>
    /// Location in world space 
    /// </summary>
    public FVector Location { get; set; }

    /// <summary>
    /// Current rotation
    /// </summary>
    public FRotator Rotation { get; set; }

    /// <summary>
    /// Acceleration of component in world space. Only valid if bRepAcceleration is set.
    /// </summary>
    public FVector? Acceleration { get; set; }
    
    /// <summary>
    /// If set, RootComponent should be sleeping.
    /// </summary>
    public bool bSimulatedPhysicSleep { get; set; }

    /// <summary>
    /// If set, additional physic data (angular velocity) will be replicated.
    /// </summary>
    public bool bRepPhysics { get; set; }

    /// <summary>
    /// If set, additional acceleration data will be replicated.
    /// </summary>
    public bool bRepAcceleration { get; set; }

    /// <summary>
    /// Server physics step
    /// </summary>
    public uint ServerFrame { get; set; }

    /// <summary>
    /// ID assigned by server used to ensure determinism by physics.
    /// </summary>
    public uint ServerPhysicsHandle { get; set; }

    /// <summary>
    /// Allows tuning the compression level for the replicated location vector. You should only need to change this from the default if you see visual artifacts.
    /// </summary>
    public VectorQuantization LocationQuantizationLevel { get; set; }

    /// <summary>
    /// Allows tuning the compression level for the replicated velocity vectors. You should only need to change this from the default if you see visual artifacts.
    /// </summary>
    public VectorQuantization VelocityQuantizationLevel { get; set; }

    /// <summary>
    /// Allows tuning the compression level for replicated rotation. You should only need to change this from the default if you see visual artifacts.
    /// </summary>
    public RotatorQuantization RotationQuantizationLevel { get; set; }

}
