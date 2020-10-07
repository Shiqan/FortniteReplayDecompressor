using System;

namespace Unreal.Core.Models.Enums
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/0218ad46444accdba786b9a82bee3f445d9fa938/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L160
    /// </summary>
    [Flags]
    public enum ReplayHeaderFlags
    {
        None = 0,
        ClientRecorded = (1 << 0),
        HasStreamingFixes = (1 << 1),
        DeltaCheckpoints = (1 << 2),
        GameSpecificFrameData = (1 << 3),
    }
}
