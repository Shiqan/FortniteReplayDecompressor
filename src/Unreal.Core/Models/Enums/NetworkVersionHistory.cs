using System;

namespace Unreal.Core.Models.Enums
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/811c1ce579564fa92ecc22d9b70cbe9c8a8e4b9a/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L84
    /// </summary>
    [Flags]
    public enum NetworkVersionHistory
    {
        HISTORY_REPLAY_INITIAL = 1,
        HISTORY_SAVE_ABS_TIME_MS = 2,               // We now save the abs demo time in ms for each frame (solves accumulation errors)
        HISTORY_INCREASE_BUFFER = 3,                // Increased buffer size of packets, which invalidates old replays
        HISTORY_SAVE_ENGINE_VERSION = 4,            // Now saving engine net version + InternalProtocolVersion
        HISTORY_EXTRA_VERSION = 5,                  // We now save engine/game protocol version, checksum, and changelist
        HISTORY_MULTIPLE_LEVELS = 6,                // Replays support seamless travel between levels
        HISTORY_MULTIPLE_LEVELS_TIME_CHANGES = 7,   // Save out the time that level changes happen
        HISTORY_DELETED_STARTUP_ACTORS = 8,         // Save DeletedNetStartupActors inside checkpoints
        HISTORY_HEADER_FLAGS = 9,                   // Save out enum flags with demo header
        HISTORY_LEVEL_STREAMING_FIXES = 10,         // Optional level streaming fixes.
        HISTORY_SAVE_FULL_ENGINE_VERSION = 11,      // Now saving the entire FEngineVersion including branch name
        HISTORY_HEADER_GUID = 12,                   // Save guid to demo header
        HISTORY_CHARACTER_MOVEMENT = 13,            // Change to using replicated movement and not interpolation
        HISTORY_CHARACTER_MOVEMENT_NOINTERP = 14,   // No longer recording interpolated movement samples
    }
}
