namespace Unreal.Core.Models
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/996ef9a9f4ad5a899abf70fb292d2914a46d0876/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Public/LocalFileNetworkReplayStreaming.h#L34
    /// </summary>
    [System.Flags]
    public enum ReplayVersionHistory : uint
    {
        HISTORY_INITIAL = 0,
        HISTORY_FIXEDSIZE_FRIENDLY_NAME = 1,
        HISTORY_COMPRESSION = 2,
        HISTORY_RECORDED_TIMESTAMP = 3,
        HISTORY_STREAM_CHUNK_TIMES = 4,
        HISTORY_FRIENDLY_NAME_ENCODING = 5,
        HISTORY_ENCRYPTION = 6,
        HISTORY_CUSTOM_VERSIONS = 7,

        HISTORY_PLUS_ONE,
        LATEST = HISTORY_PLUS_ONE - 1
    }
}