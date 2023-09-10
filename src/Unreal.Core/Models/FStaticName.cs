using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Models;

public class FStaticName : IProperty
{
    public string Value { get; private set; }

    public void Serialize(NetBitReader reader)
    {
        var isHardcoded = reader.ReadBoolean();

        if (isHardcoded)
        {
            uint nameIndex;
            if (reader.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES)
            {
                nameIndex = reader.ReadUInt32();
            }
            else
            {
                nameIndex = reader.ReadIntPacked();
            }

            Value = ((UnrealNames) nameIndex).ToString();

            return;
        }

        var inString = reader.ReadFString();
        var inNumber = reader.ReadInt32();

        Value = inString;
    }

    public override string ToString() => Value;
}