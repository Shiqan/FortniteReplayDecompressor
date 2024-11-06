using Unreal.Core.Contracts;

namespace Unreal.Core.Models;

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
        var bSerializeReplicationMethod = reader.EngineNetworkVersion >= Enums.EngineNetworkVersionHistory.CustomExports;
        var bUseFastReplication = true;
        var bUseDynamicReplication = false;

        if (bSerializeReplicationMethod)
        {
            bUseFastReplication = reader.ReadBit();
            if (!bUseFastReplication)
            {
                bUseDynamicReplication = reader.ReadBit();
            }
        }

        if (bUseFastReplication)
        {
            // https://github.com/EpicGames/UnrealEngine/blob/55e6c59d10463ba45392a915f89cdf31660a41c7/Engine/Source/Runtime/GameplayTags/Private/GameplayTagContainer.cpp#L1226
            TagIndex = reader.ReadIntPacked();
        }
        else if (bUseDynamicReplication)
        {
            // https://github.com/EpicGames/UnrealEngine/blob/55e6c59d10463ba45392a915f89cdf31660a41c7/Engine/Source/Runtime/GameplayTags/Private/GameplayTagContainer.cpp#L1393

            var byteCountToRead = reader.ReadBitsToInt(2) + 1;
            var bitCountToRead = byteCountToRead * 8;
            TagIndex = (uint) reader.ReadBitsToInt(bitCountToRead);
        }
        //else
        //{
        //    // not implemented
        //    // Ar << TagName;
        //    // reader.ReadFName();
        //}

    }

    public void Resolve(NetGuidCache cache)
    {
        if (cache.TryGetTagName(TagIndex, out var name))
        {
            TagName = name;
        }
    }
}