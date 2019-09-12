namespace Unreal.Core.Models
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L45
    /// </summary>
    public enum ReplayVersionHistory : uint
    {
        Initial = 0,
        FixedSizeFriendlyName = 1,
        Compression = 2,
        RecordedTimestamp = 3,
        StreamChunkTimes = 4,
        FriendlyNameEncoding = 5,
        NewVersion,
        Latest = NewVersion - 1
    }
}