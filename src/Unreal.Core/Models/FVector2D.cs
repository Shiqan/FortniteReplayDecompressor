namespace Unreal.Core.Models;

public class FVector2D
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Math/Vector2D.h#L17
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    public FVector2D(float X, float Y)
    {
        this.X = X;
        this.Y = Y;
    }

    public float X { get; set; }
    public float Y { get; set; }

    public override string ToString() => $"X: {X}, Y: {Y}";

    public static bool operator ==(FVector2D v1, FVector2D v2) => v1?.X == v2?.X && v1?.Y == v2?.Y;

    public static bool operator !=(FVector2D v1, FVector2D v2) => v1?.X != v2?.X || v1?.Y != v2?.Y;

    public override bool Equals(object obj)
    {
        var other = obj as FVector2D;
        return X == other?.X && Y == other?.Y;
    }

    public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();
}
