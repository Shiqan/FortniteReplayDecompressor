using System;

namespace Unreal.Core.Models.Enums;

/// <summary>
/// Describes rules for network replicating a vector efficiently
/// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Classes/Engine/EngineTypes.h#L2990
/// </summary>
[Flags]
public enum RotatorQuantization
{
    /** The rotator will be compressed to 8 bits per component. */
    ByteComponents,
    /** The rotator will be compressed to 16 bits per component. */
    ShortComponents
}
