namespace Unreal.Core.Models.Enums;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Public/LocalFileNetworkReplayStreaming.h#L19
/// </summary>
public enum ReplayChunkType : uint
{
    Header,
    ReplayData,
    Checkpoint,
    Event,
    Unknown = 0xFFFFFFFF
}