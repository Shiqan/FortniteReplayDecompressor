using System;
using Unreal.Core.Contracts;

namespace Unreal.Core.Models;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/6c20d9831a968ad3cb156442bebb41a883e62152/Engine/Source/Runtime/Core/Public/Internationalization/Text.h#L325
/// </summary>
public class FText : IProperty
{
    public string Namespace { get; set; }
    public string Key { get; set; }
    public string Text { get; set; }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/6c20d9831a968ad3cb156442bebb41a883e62152/Engine/Source/Runtime/Core/Private/Internationalization/Text.cpp#L794
    /// </summary>
    /// <param name="reader"></param>
    public void Serialize(NetBitReader reader)
    {
        var flags = reader.ReadInt32();
        var historyType = reader.ReadByteAsEnum<ETextHistoryType>();
        switch (historyType)
        {
            case ETextHistoryType.Base:
                Namespace = reader.ReadFString();
                Key = reader.ReadFString();
                Text = reader.ReadFString();
                break;
        }
    }
}

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/6c20d9831a968ad3cb156442bebb41a883e62152/Engine/Source/Runtime/Core/Public/Internationalization/Text.h#L35
/// </summary>
[Flags]
public enum FTextType
{
    Transient = (1 << 0),
    CultureInvariant = (1 << 1),
    ConvertedProperty = (1 << 2),
    Immutable = (1 << 3),
    InitializedFromString = (1 << 4),  // this ftext was initialized using FromString
};

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/6c20d9831a968ad3cb156442bebb41a883e62152/Engine/Source/Runtime/Core/Private/Internationalization/TextHistory.h#L16
/// </summary>
public enum ETextHistoryType
{
    None = -1,
    Base = 0,
    NamedFormat,
    OrderedFormat,
    ArgumentFormat,
    AsNumber,
    AsPercent,
    AsCurrency,
    AsDate,
    AsTime,
    AsDateTime,
    Transform,
    StringTableEntry,
    TextGenerator
};