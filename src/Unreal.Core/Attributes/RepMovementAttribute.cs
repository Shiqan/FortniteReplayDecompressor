using System;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Attributes;

/// <summary>
/// Attribute to specify <see cref="Models.FRepMovement"/> settings.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class RepMovementAttribute : Attribute
{
    /// <summary>
    /// Allows tuning the compression level for the replicated location vector. You should only need to change this from the default if you see visual artifacts.
    /// </summary>
    public VectorQuantization LocationQuantizationLevel { get; private set; }

    /// <summary>
    /// Allows tuning the compression level for the replicated velocity vectors. You should only need to change this from the default if you see visual artifacts.
    /// </summary>
    public VectorQuantization VelocityQuantizationLevel { get; private set; }

    /// <summary>
    /// Allows tuning the compression level for replicated rotation. You should only need to change this from the default if you see visual artifacts.
    /// </summary>
    public RotatorQuantization RotationQuantizationLevel { get; private set; }

    public RepMovementAttribute(
        VectorQuantization locationQuantizationLevel = VectorQuantization.RoundTwoDecimals,
        RotatorQuantization rotationQuantizationLevel = RotatorQuantization.ByteComponents,
        VectorQuantization velocityQuantizationLevel = VectorQuantization.RoundWholeNumber)
    {
        LocationQuantizationLevel = locationQuantizationLevel;
        VelocityQuantizationLevel = velocityQuantizationLevel;
        RotationQuantizationLevel = rotationQuantizationLevel;
    }
}