namespace Unreal.Core.Models.Enums;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/Channel.h#L21
/// </summary>
public enum ChannelType
{
    None = 0,       // Invalid type.
    Control = 1,    // Connection control.
    Actor = 2,      // Actor-update channel.
    File = 3,       // Binary file transfer.
    Voice = 4,       // VoIP data channel
    MAX = 8,        // Maximum.
}
