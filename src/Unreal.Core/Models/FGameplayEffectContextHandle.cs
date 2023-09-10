using System;
using Unreal.Core.Contracts;

namespace Unreal.Core.Models;

/// <summary>
/// Data structure that stores an instigator and related data, such as positions and targets
/// Games can subclass this structure and add game-specific information
/// It is passed throughout effect execution so it is a great place to track transient information about an execution
/// </summary>
public class FGameplayEffectContextHandle : IProperty
{
    /// <summary>
    /// Instigator actor, the actor that owns the ability system component
    /// </summary>
    public uint Instigator { get; private set; }

    /// <summary>
    /// The physical actor that actually did the damage, can be a weapon or projectile
    /// </summary>
    public uint EffectCauser { get; private set; }

    /// <summary>
    /// The ability CDO that is responsible for this effect context (replicated
    /// </summary>
    public uint AbilityCDO { get; private set; }

    /// <summary>
    /// Object this effect was created from, can be an actor or static object. Useful to bind an effect to a gameplay object
    /// </summary>
    public uint SourceObject { get; private set; }

    /// <summary>
    /// Actors referenced by this context
    /// </summary>
    public ActorGuid[] Actors { get; private set; }

    /// <summary>
    /// Trace information
    /// </summary>
    public FHitResult HitResult { get; private set; }

    /// <summary>
    /// Stored origin, may be invalid if bHasWorldOrigin is false
    /// </summary>
    public FVector WorldOrigin { get; private set; }

    public bool bHasWorldOrigin { get; private set; }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/6c20d9831a968ad3cb156442bebb41a883e62152/Engine/Plugins/Runtime/GameplayAbilities/Source/GameplayAbilities/Private/GameplayEffectTypes.cpp#L311
    /// see https://github.com/EpicGames/UnrealEngine/blob/6c20d9831a968ad3cb156442bebb41a883e62152/Engine/Plugins/Runtime/GameplayAbilities/Source/GameplayAbilities/Private/GameplayEffectTypes.cpp#L177
    /// </summary>
    /// <param name="reader"></param>
    public void Serialize(NetBitReader reader)
    {
        var validData = reader.ReadBit();

        if (validData)
        {
            var RepBits = reader.ReadBitsToInt(7);

            if ((RepBits & (1 << 0)) > 0)
            {
                Instigator = reader.ReadIntPacked();
            }
            if ((RepBits & (1 << 1)) > 0)
            {
                EffectCauser = reader.ReadIntPacked();
            }
            if ((RepBits & (1 << 2)) > 0)
            {
                AbilityCDO = reader.ReadIntPacked();
            }
            if ((RepBits & (1 << 3)) > 0)
            {
                SourceObject = reader.ReadIntPacked();
            }
            if ((RepBits & (1 << 4)) > 0)
            {
                // SafeNetSerializeTArray_HeaderOnly
                var bitCount = (int) Math.Ceiling(Math.Log2(31));
                var arrayNum = reader.ReadBitsToInt(bitCount);

                // SafeNetSerializeTArray_Default
                Actors = new ActorGuid[arrayNum];
                for (var i = 0; i < arrayNum; i++)
                {
                    Actors[i] = new ActorGuid() { Value = reader.ReadIntPacked() };
                }
            }
            if ((RepBits & (1 << 5)) > 0)
            {
                HitResult = new FHitResult(reader);
            }
            if ((RepBits & (1 << 6)) > 0)
            {
                WorldOrigin = reader.SerializePropertyVector100();
                bHasWorldOrigin = true;
            }
            else
            {
                bHasWorldOrigin = false;
            }
        }
    }
}