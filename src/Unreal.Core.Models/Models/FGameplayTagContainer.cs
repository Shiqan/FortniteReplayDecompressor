using Unreal.Core.Models.Contracts;

namespace Unreal.Core.Models
{
    /// <summary>
    /// A Tag Container holds a collection of FGameplayTags.
    /// see https://github.com/EpicGames/UnrealEngine/blob/6c20d9831a968ad3cb156442bebb41a883e62152/Engine/Source/Runtime/GameplayTags/Classes/GameplayTagContainer.h#L275
    /// </summary>
    public class FGameplayTagContainer : IProperty, IResolvable
    {
        public FGameplayTagContainer()
        {

        }

        public FGameplayTagContainer(INetBitReader reader)
        {
            Serialize(reader);
        }

        public FGameplayTag[] Tags { get; private set; }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/6c20d9831a968ad3cb156442bebb41a883e62152/Engine/Source/Runtime/GameplayTags/Private/GameplayTagContainer.cpp#L970
        /// </summary>
        /// <param name="reader"></param>
        public void Serialize(INetBitReader reader)
        {
            // 1st bit to indicate empty tag container or not (empty tag containers are frequently replicated). Early out if empty.
            if (reader.ReadBit())
            {
                return;
            }

            var numTags = reader.ReadBitsToInt(7);
            Tags = new FGameplayTag[numTags];
            for (var i = 0; i < numTags; i++)
            {
                Tags[i] = new FGameplayTag(reader);
            }
        }

        public void Resolve(INetGuidCache cache)
        {
            if (Tags == null) return;

            for (var i = 0; i < Tags?.Length; i++)
            {
                Tags[i].Resolve(cache);
            }
        }
    }
}