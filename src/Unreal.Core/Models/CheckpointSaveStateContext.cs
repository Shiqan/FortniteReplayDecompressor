using Unreal.Core.Models.Enums;

namespace Unreal.Core.Models
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L942
    /// </summary>
    class CheckpointSaveStateContext
    {
        CheckpointSaveState CheckpointSaveState;                       // Current state of checkpoint SaveState
        PackageMapAckState CheckpointAckState;                         // Current ack state of packagemap for the current checkpoint being saved
        PendingCheckPointActor[] PendingCheckpointActors;        // Actors to be serialized by pending checkpoint
        double TotalCheckpointSaveTimeSeconds;              // Total time it took to save checkpoint including the finaling part across all frames
        double TotalCheckpointReplicationTimeSeconds;       // Total time it took to write all replicated objects across all frames
        bool bWriteCheckpointOffset;
        int TotalCheckpointSaveFrames;                    // Total number of frames used to save a checkpoint
        int CheckpointOffset;
        uint GuidCacheSize;
    }
}
