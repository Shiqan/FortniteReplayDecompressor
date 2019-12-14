using System;
using Unreal.Core.Contracts;

namespace Unreal.Core.Models
{
    public class FText : IProperty
    {
        public string Key { get; set; }
        public string Text { get; set; }

        public void Serialize(NetBitReader reader)
        {
            reader.SkipBits(72);

            Key = reader.ReadFString();
            Text = reader.ReadFString();
        }
    }

    [Flags]
    public enum FTextType
    {
        Transient = (1 << 0),
        CultureInvariant = (1 << 1),
        ConvertedProperty = (1 << 2),
        Immutable = (1 << 3),
        InitializedFromString = (1 << 4),  // this ftext was initialized using FromString
    };

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
}