namespace Unreal.Core.Models;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L45
/// see https://github.com/EpicGames/UnrealEngine/blob/407acc04a93f09ecb42c07c98b74fd00cc967100/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Public/LocalFileNetworkReplayStreaming.h#LL21C24-L21C24
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