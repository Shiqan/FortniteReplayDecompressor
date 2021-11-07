namespace Unreal.Core.Models.Enums;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Private/DemoNetDriver.cpp#L1626
/// </summary>
public enum GuidCacheFlags
{
    Initial = 0,
    NoLoad = (1 << 0),
    IgnoreWhenMissing = (1 << 1)

}
