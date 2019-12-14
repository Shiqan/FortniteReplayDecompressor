using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Models
{
    public class FName : IProperty
    {
        public string Name { get; private set; }

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

                Name = ((UnrealNames)nameIndex).ToString();

                return;
            }

            var inString = reader.ReadFString();
            var inNumber = reader.ReadInt32();

            Name = inString;
        }
    }
}