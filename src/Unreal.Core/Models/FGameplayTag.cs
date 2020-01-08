using Unreal.Core.Contracts;

namespace Unreal.Core.Models
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/6c20d9831a968ad3cb156442bebb41a883e62152/Engine/Source/Runtime/GameplayTags/Classes/GameplayTagContainer.h#L52
    /// </summary>
    public class FGameplayTag : IProperty, IResolvable
    {
        public FGameplayTag()
        {

        }

        public FGameplayTag(NetBitReader reader)
        {
            Serialize(reader);
        }

        public string TagName { get; set; }
        public uint TagIndex { get; set; }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/6c20d9831a968ad3cb156442bebb41a883e62152/Engine/Source/Runtime/GameplayTags/Private/GameplayTagContainer.cpp#L1210
        /// </summary>
        /// <param name="reader"></param>
        public void Serialize(NetBitReader reader)
        {
            TagIndex = reader.ReadIntPacked();
        }

        public void Resolve(NetGuidCache cache)
        {
            if (cache.TryGetTagName(TagIndex, out var name))
            {
                TagName = name;
            }
        }
    }
}