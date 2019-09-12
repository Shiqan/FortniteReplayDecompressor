namespace Unreal.Core.Models.Enums
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L930
    /// </summary>
    public enum CheckpointSaveState
    {
        ECheckpointSaveState_Idle,
        ECheckpointSaveState_ProcessCheckpointActors,
        ECheckpointSaveState_SerializeDeletedStartupActors,
        ECheckpointSaveState_SerializeGuidCache,
        ECheckpointSaveState_SerializeNetFieldExportGroupMap,
        ECheckpointSaveState_SerializeDemoFrameFromQueuedDemoPackets,
        ECheckpointSaveState_Finalize,
    }
}
