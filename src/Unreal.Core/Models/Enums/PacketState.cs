namespace Unreal.Core.Models.Enums;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L667
/// </summary>
public enum PacketState
{
    Success,    // A packet was read successfully and there may be more in the frame archive.
    End,        // No more data is present in the archive.
    Error,		// An error occurred while reading.
}
