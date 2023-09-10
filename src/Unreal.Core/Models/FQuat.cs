using System;
using Unreal.Core.Contracts;

namespace Unreal.Core.Models;

/// <summary>
///  Floating point quaternion that can represent a rotation about an axis in 3-D space.
///  The X, Y, Z, W components also double as the Axis/Angle format.
/// </summary>
public class FQuat : IProperty
{
    /// <summary>
    /// The quaternion's X-component.
    /// </summary>
    public float X { get; set; }
    /// <summary>
    /// The quaternion's Y-component.
    /// </summary>
    public float Y { get; set; }
    /// <summary>
    /// The quaternion's Z-component
    /// </summary>
    public float Z { get; set; }
    /// <summary>
    /// The quaternion's W-component
    /// </summary>
    public float W { get; set; }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/099c2469b494de4752e9d63535767bb9fcc90d16/Engine/Source/Runtime/Core/Private/Math/UnrealMath.cpp#L832
    /// </summary>
    /// <param name="reader"></param>
    public void Serialize(NetBitReader reader)
    {
        X = reader.ReadSingle();
        Y = reader.ReadSingle();
        Z = reader.ReadSingle();

        var XYZMagSquared = (X * X + Y * Y + Z * Z);
        var WSquared = 1.0f - XYZMagSquared;

        // If mag of (X,Y,Z) <= 1.0, then we calculate W to make magnitude of Q 1.0
        if (WSquared >= 0f)
        {
            W = (float) Math.Sqrt(WSquared);
        }
        // If mag of (X,Y,Z) > 1.0, we set W to zero, and then renormalize 
        else
        {
            W = 0f;

            var XYZInvMag = (float) (1 / Math.Sqrt(XYZMagSquared));
            X *= XYZInvMag;
            Y *= XYZInvMag;
            Z *= XYZInvMag;
        }
    }
}