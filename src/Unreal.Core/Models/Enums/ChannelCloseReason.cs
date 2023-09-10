namespace Unreal.Core.Models.Enums;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/CoreUObject/Public/UObject/CoreNetTypes.h#L36
/// </summary>
public enum ChannelCloseReason
{
    Destroyed,
    Dormancy,
    LevelUnloaded,
    Relevancy,
    TearOff,
    /* reserved */
    MAX = 15		// this value is used for serialization, modifying it may require a network version change
}
