namespace Unreal.Core.Models.Enums;

/// <summary>
/// Various types of Properties supported for Replication.
/// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Public/Net/RepLayout.h#L677
/// </summary>
public enum RepLayoutCmdType
{
    DynamicArray = 0,   //! Dynamic array
    Return = 1,         //! Return from array, or end of stream
    Property = 2,       //! Generic property

    PropertyBool = 3,
    PropertyFloat = 4,
    PropertyInt = 5,
    PropertyByte = 6,
    PropertyName = 7,
    PropertyObject = 8,
    PropertyUInt32 = 9,
    PropertyVector = 10,
    PropertyRotator = 11,
    PropertyPlane = 12,
    PropertyVector100 = 13,
    PropertyNetId = 14,
    RepMovement = 15,
    PropertyVectorNormal = 16,
    PropertyVector10 = 17,
    PropertyVectorQ = 18,
    PropertyString = 19,
    PropertyUInt64 = 20,
    PropertyNativeBool = 21,
    PropertySoftObject = 22,
    PropertyWeakObject = 23,
    PropertyInterface = 24,
    NetSerializeStructWithObjectReferences = 25,

    PropertyDouble = 94,
    PropertyVector2D = 95,
    PropertyInt16 = 96,
    PropertyUInt16 = 97,
    PropertyQuat = 98,

    Enum = 99,
    Ignore = 100
}
