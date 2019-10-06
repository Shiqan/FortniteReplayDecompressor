namespace Unreal.Core.Models.Enums
{
    /// <summary>
    ///  Describes rules for network replicating a vector efficiently
    ///  see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Classes/Engine/EngineTypes.h#L2980
    /// </summary>
    public enum VectorQuantization
    {
        /** Each vector component will be rounded to the nearest whole number. */
        RoundWholeNumber,
        /** Each vector component will be rounded, preserving one decimal place. */
        RoundOneDecimal,
        /** Each vector component will be rounded, preserving two decimal places. */
        RoundTwoDecimals
    }
}
