using Unreal.Core.Contracts;

namespace Unreal.Core.Models
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/6c20d9831a968ad3cb156442bebb41a883e62152/Engine/Source/Runtime/GameplayTags/Private/GameplayTagContainer.cpp#L970
    /// </summary>
    public class FGameplayTagContainer : IProperty
    {
        public FGameplayTag[] Tags { get; private set; }

        public void Serialize(NetBitReader reader)
        {
            // 1st bit to indicate empty tag container or not (empty tag containers are frequently replicated). Early out if empty.
            if (reader.ReadBit())
            {
                return;
            }

            var numTags = reader.ReadByte();
            //var numTags = reader.ReadSerializedInt(1);
            Tags = new FGameplayTag[numTags];
            for (var i = 0; i < numTags; i++)
            {
                var tag = new FGameplayTag();
                tag.Serialize(reader);
                Tags[i] = tag;
            }
        }
    }
}