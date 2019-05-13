namespace FortniteReplayReaderDecompressor.Core.Models.Enums
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/Channel.h#L21
    /// </summary>
    public enum ChannelType
    {
        CHTYPE_None = 0,  // Invalid type.
        CHTYPE_Control = 1,  // Connection control.
        CHTYPE_Actor = 2,  // Actor-update channel.

        // @todo: Remove and reassign number to CHTYPE_Voice (breaks net compatibility)
        CHTYPE_File = 3,  // Binary file transfer.

        CHTYPE_Voice = 4,  // VoIP data channel
        CHTYPE_MAX = 8,  // Maximum.
    };
}
