namespace Unreal.Core.Models.Enums;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/NetConnection.cpp#L1720
/// </summary>
public enum ChannelName
{
    Control,
    Voice,
    Actor,
    //MaxHardcodedNameIndex, corenet.cpp#L296
    None
};
