using Unreal.Core.Contracts;

namespace Unreal.Core.Models;

/// <summary>
/// Data about montages that is replicated to simulated clients
/// see https://github.com/EpicGames/UnrealEngine/blob/4f421ab90c6e9ff4f0b5c9ec4de6dcd3297ad488/Engine/Plugins/Runtime/GameplayAbilities/Source/GameplayAbilities/Public/Abilities/GameplayAbilityTypes.h#L177
/// </summary>
public class FGameplayAbilityRepAnimMontage : IProperty
{
    /// <summary>
    /// AnimMontage ref
    /// </summary>
    public NetworkGUID AnimMontage { get; private set; }

    /// <summary>
    /// Play rate
    /// </summary>
    public float PlayRate { get; private set; }

    /// <summary>
    /// Montage position
    /// </summary>
    public float Position { get; private set; }

    /// <summary>
    /// Montage current blend time
    /// </summary>
    public float BlendTime { get; private set; }

    /// <summary>
    /// NextSectionID
    /// </summary>
    public byte NextSectionID { get; private set; }

    /// <summary>
    /// flag indicating we should serialize the position or the current section id
    /// </summary>
    public bool bRepPosition { get; private set; } = true;

    /// <summary>
    /// Bit set when montage has been stopped.
    /// </summary>
    public bool IsStopped { get; private set; } = true;

    /// <summary>
    /// Bit flipped every time a new Montage is played. To trigger replication when the same montage is played again.
    /// </summary>
    public bool ForcePlayBit { get; private set; }

    /// <summary>
    /// Stops montage position from replicating at all to save bandwidth
    /// </summary>
    public bool SkipPositionCorrection { get; private set; }

    /// <summary>
    /// Stops PlayRate from replicating to save bandwidth. PlayRate will be assumed to be 1.f.
    /// </summary>
    public bool bSkipPlayRate { get; private set; }

    public FPredictionKey PredictionKey { get; private set; }

    /// <summary>
    /// The current section Id used by the montage. Will only be valid if bRepPosition is false
    /// </summary>
    public int SectionIdToPlay { get; private set; }

    /// <summary>
    /// ID incremented every time a montage is played, used to trigger replication when the same montage is played multiple times. This ID wraps around when it reaches its max value.
    /// </summary>
    public NetworkGUID PlayInstanceId;

    public void Serialize(NetBitReader reader)
    {
        var repPosition = reader.ReadBoolean();

        if (repPosition)
        {
            bRepPosition = true;
            SectionIdToPlay = 0;
            SkipPositionCorrection = false;

            var packedPosition = reader.ReadIntPacked();

            Position = packedPosition / 100;
        }
        else
        {
            bRepPosition = false;

            SkipPositionCorrection = true;
            Position = 0;
            SectionIdToPlay = reader.ReadBitsToInt(7);
        }

        if (reader.EngineNetworkVersion < Enums.EngineNetworkVersionHistory.HISTORY_MONTAGE_PLAY_INST_ID_SERIALIZATION)
        {
            ForcePlayBit = reader.ReadBit();
        }

        IsStopped = reader.ReadBit();
        SkipPositionCorrection = reader.ReadBit();
        bSkipPlayRate = reader.ReadBit();

        AnimMontage = new NetworkGUID { Value = reader.ReadIntPacked() };
        PlayRate = reader.SerializePropertyFloat();
        BlendTime = reader.SerializePropertyFloat();
        NextSectionID = reader.ReadByte();

        if (reader.EngineNetworkVersion >= Enums.EngineNetworkVersionHistory.HISTORY_MONTAGE_PLAY_INST_ID_SERIALIZATION)
        {
            PlayInstanceId = new NetworkGUID { Value = reader.ReadIntPacked() };
        }


        PredictionKey = new FPredictionKey();
        PredictionKey.Serialize(reader);
    }
}