namespace Unreal.Core.Models.Enums;

/// <summary>
/// Various flags that describe how a Property should be handled.
/// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Public/Net/RepLayout.h#L794
/// </summary>
public enum RepLayoutFlags
{
    None = 0,                           //! No flags.
    IsSharedSerialization = (1 << 0),   //! Indicates the property is eligible for shared serialization.
    IsStruct = (1 << 1),                //! This is a struct property.
    IsEmptyArrayStruct = (1 << 2),      //! This is an ArrayProperty whose InnerProperty has no replicated properties.
}
