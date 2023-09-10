using System;

namespace Unreal.Core.Models;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Math/Vector.h#L29
/// </summary>
public class FVector
{
    public FVector(double X, double Y, double Z)
    {
        this.X = X;
        this.Y = Y;
        this.Z = Z;
    }

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public int ScaleFactor { get; set; }
    public int Bits { get; set; }

    public override string ToString() => $"X: {X}, Y: {Y}, Z: {Z}";

    public float Size() => (float) Math.Sqrt(X * X + Y * Y + Z * Z);
    public double DistanceTo(FVector vector) => Math.Sqrt(DistanceSquared(vector));

    private double DistanceSquared(FVector vector) => Math.Pow(vector.X - X, 2) + Math.Pow(vector.Y - Y, 2) + Math.Pow(vector.Z - Z, 2);

    public static FVector operator -(FVector v1, FVector v2) => new(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);

    public static bool operator ==(FVector v1, FVector v2) => v1?.X == v2?.X && v1?.Y == v2?.Y && v1?.Z == v2?.Z;

    public static bool operator !=(FVector v1, FVector v2) => v1?.X != v2?.X || v1?.Y != v2?.Y || v1?.Z != v2?.Z;

    public override bool Equals(object obj)
    {
        var other = obj as FVector;
        return X == other?.X && Y == other?.Y && Z == other?.Z;
    }

    public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
}
