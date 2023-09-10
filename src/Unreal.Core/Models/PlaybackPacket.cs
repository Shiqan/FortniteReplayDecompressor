using Unreal.Core.Models.Enums;

namespace Unreal.Core.Models;

/// <summary>
/// PlaybackPackets are used to buffer packets up when we read a demo frame, which we can then process when the time is right
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L84
/// </summary>
public class PlaybackPacket
{
    public byte[] Data { get; set; }
    public float TimeSeconds { get; set; }
    public int LevelIndex { get; set; }
    public uint SeenLevelIndex { get; set; }
    public PacketState State { get; set; }
}
