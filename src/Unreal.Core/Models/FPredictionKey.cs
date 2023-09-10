using Unreal.Core.Contracts;

namespace Unreal.Core.Models;

/// <summary>
/// FPredictionKey is a generic way of supporting Clientside Prediction in the GameplayAbility system.  
/// A FPredictionKey is essentially an ID for identifying predictive actions and side effects that are done on a client.
/// </summary>
public class FPredictionKey : IProperty
{
    /// <summary>
    /// The unique ID of this prediction key
    /// </summary>
    public short CurrentKey { get; private set; }

    /// <summary>
    /// If non 0, the prediction key this was created from
    /// </summary>
    public short BaseKey { get; private set; }
    public bool bIsServerInitiated { get; set; }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/6c20d9831a968ad3cb156442bebb41a883e62152/Engine/Plugins/Runtime/GameplayAbilities/Source/GameplayAbilities/Private/GameplayPrediction.cpp#L7
    /// </summary>
    /// <param name="reader"></param>
    public void Serialize(NetBitReader reader)
    {
        var hasBaseKey = false;
        var validKeyForConnection = reader.ReadBit();
        if (validKeyForConnection)
        {
            hasBaseKey = reader.ReadBit();
        }
        bIsServerInitiated = reader.ReadBit();
        if (validKeyForConnection)
        {
            CurrentKey = reader.ReadInt16();
            if (hasBaseKey)
            {
                BaseKey = reader.ReadInt16();
            }
        }
    }
}