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
            // https://github.com/EpicGames/UnrealEngine/blob/85fe5dd282a00eb15e7ba41cbe46a46c440dea79/Engine/Source/Runtime/Experimental/Iris/Core/Private/Iris/ReplicationSystem/NetTokenStore.cpp#L117
            TagIndex = reader.ReadIntPacked();

            //if (const bool bIsValid = (TokenIndex != FNetToken::InvalidTokenIndex))
            if (TagIndex > 0)
            {
                var bIsAssignedByAuthority = reader.ReadBit();

                // pray we dont need this :)
                //if (TokenTypeId == FNetToken::InvalidTokenTypeId)
                //{
                //    uint32 TempTokenTypeId = 0;
                //    Ar.SerializeBits(&TempTokenTypeId, FNetToken::TokenTypeIdBits);
                //    TokenTypeId = TempTokenTypeId;
                //}
            }
        }
        //else
        //{
        //    // not implemented
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