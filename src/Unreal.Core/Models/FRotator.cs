namespace Unreal.Core.Models;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Math/Rotator.h#L18
/// </summary>
public class FRotator
{
    public FRotator(float pitch, float yaw, float roll)
    {
        Pitch = pitch;
        Yaw = yaw;
        Roll = roll;
    }
    public float Pitch { get; set; }
    public float Yaw { get; set; }
    public float Roll { get; set; }

    public override string ToString() => $"Pitch: {Pitch}, Yaw: {Yaw}, Roll: {Roll}";
}
