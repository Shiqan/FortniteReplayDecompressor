using Unreal.Core.Contracts;

namespace Unreal.Core.Models;

public enum RepFlag
{
    REP_NormalizedMagnitude = 0,
    REP_RawMagnitude,
    REP_EffectContext,
    REP_Location,
    REP_Normal,
    REP_Instigator,
    REP_EffectCauser,
    REP_SourceObject,
    REP_TargetAttachComponent,
    REP_PhysMaterial,
    REP_GELevel,
    REP_AbilityLevel,

    REP_MAX
};

/// <summary>
/// Metadata about a gameplay cue execution
/// </summary>
public class FGameplayCueParameters : IProperty
{
    /// <summary>
    /// Magnitude of source gameplay effect, normalzed from 0-1. Use this for "how strong is the gameplay effect" (0=min, 1=,max) 
    /// </summary>
    public float NormalizedMagnitude { get; private set; }

    /// <summary>
    /// Raw final magnitude of source gameplay effect. Use this is you need to display numbers or for other informational purposes.
    /// </summary>
    public float RawMagnitude { get; private set; }

    /// <summary>
    /// Effect context, contains information about hit result, etc
    /// </summary>
    //public FGameplayEffectContextHandle EffectContext { get; private set; }
    public object EffectContext { get; private set; }

    /// <summary>
    /// The tag name that matched this specific gameplay cue handler
    /// </summary>
    public FGameplayTag MatchedTagName { get; private set; }

    /// <summary>
    /// The original tag of the gameplay cue
    /// </summary>
    public FGameplayTag OriginalTag { get; private set; }

    /// <summary>
    /// The aggregated source tags taken from the effect spec
    /// </summary>
    public FGameplayTagContainer AggregatedSourceTags { get; private set; } = new FGameplayTagContainer();

    /// <summary>
    /// The aggregated target tags taken from the effect spec
    /// </summary>
    public FGameplayTagContainer AggregatedTargetTags { get; private set; } = new FGameplayTagContainer();

    /// <summary>
    /// Location cue took place at
    /// </summary>
    public FVector Location { get; private set; }

    /// <summary>
    /// Normal of impact that caused cue
    /// </summary>
    public FVector Normal { get; private set; }

    /// <summary>
    /// Instigator actor, the actor that owns the ability system component
    /// </summary>
    public uint Instigator { get; private set; }

    /// <summary>
    /// The physical actor that actually did the damage, can be a weapon or projectile
    /// </summary>
    public uint EffectCauser { get; private set; }

    /// <summary>
    /// Object this effect was created from, can be an actor or static object. Useful to bind an effect to a gameplay object
    /// </summary>
    public uint SourceObject { get; private set; }

    /// <summary>
    ///  PhysMat of the hit, if there was a hit.
    /// </summary>
    public uint PhysicalMaterial { get; private set; }

    /// <summary>
    ///  If originating from a GameplayEffect, the level of that GameplayEffect
    /// </summary>
    public int GameplayEffectLevel { get; private set; }

    /// <summary>
    /// If originating from an ability, this will be the level of that ability
    /// </summary>
    public int AbilityLevel { get; private set; }

    /// <summary>
    /// Could be used to say "attach FX to this component always"
    /// </summary>
    public uint TargetAttachComponent { get; private set; }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/6c20d9831a968ad3cb156442bebb41a883e62152/Engine/Plugins/Runtime/GameplayAbilities/Source/GameplayAbilities/Private/GameplayEffectTypes.cpp#L789
    /// </summary>
    /// <param name="reader"></param>
    public void Serialize(NetBitReader reader)
    {
        const byte NUM_LEVEL_BITS = 5; // need to bump this up to support 20 levels for AbilityLevel
        // const byte MAX_LEVEL = (1 << NUM_LEVEL_BITS) - 1;

        var RepBits = reader.ReadBitsToInt((int) RepFlag.REP_MAX);

        // Tag containers serialize empty containers with 1 bit, so no need to serialize this in the RepBits field.
        AggregatedSourceTags.Serialize(reader);
        AggregatedTargetTags.Serialize(reader);

        if ((RepBits & (1 << (int) RepFlag.REP_NormalizedMagnitude)) > 0)
        {
            NormalizedMagnitude = reader.ReadSingle();
        }
        if ((RepBits & (1 << (int) RepFlag.REP_RawMagnitude)) > 0)
        {
            RawMagnitude = reader.ReadSingle();
        }
        if ((RepBits & (1 << (int) RepFlag.REP_EffectContext)) > 0)
        {
            // FGameplayEffectContextHandle
            if (reader.ReadBit())
            {
                var handle = new FGameplayEffectContextHandle();
                handle.Serialize(reader);
            }
        }
        if ((RepBits & (1 << (int) RepFlag.REP_Location)) > 0)
        {
            Location = reader.SerializePropertyVector10();
        }
        if ((RepBits & (1 << (int) RepFlag.REP_Normal)) > 0)
        {
            Normal = reader.SerializePropertyVectorNormal();
        }
        if ((RepBits & (1 << (int) RepFlag.REP_Instigator)) > 0)
        {
            Instigator = reader.ReadIntPacked();
        }
        if ((RepBits & (1 << (int) RepFlag.REP_EffectCauser)) > 0)
        {
            EffectCauser = reader.ReadIntPacked();
        }
        if ((RepBits & (1 << (int) RepFlag.REP_SourceObject)) > 0)
        {
            SourceObject = reader.ReadIntPacked();
        }
        if ((RepBits & (1 << (int) RepFlag.REP_TargetAttachComponent)) > 0)
        {
            TargetAttachComponent = reader.ReadIntPacked();
        }
        if ((RepBits & (1 << (int) RepFlag.REP_PhysMaterial)) > 0)
        {
            PhysicalMaterial = reader.ReadIntPacked();
        }
        if ((RepBits & (1 << (int) RepFlag.REP_GELevel)) > 0)
        {
            GameplayEffectLevel = reader.ReadBitsToInt(NUM_LEVEL_BITS);
        }
        if ((RepBits & (1 << (int) RepFlag.REP_AbilityLevel)) > 0)
        {
            AbilityLevel = reader.ReadBitsToInt(NUM_LEVEL_BITS);
        }
    }
}