namespace Unreal.Core.Contracts;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L60
/// </summary>
public interface IExternalData
{
    public uint NetGUID { get; init; }
    public FArchive Archive { get; init; }
    public int TimeSeconds { get; init; }
}
