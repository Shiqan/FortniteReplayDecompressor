using System;

namespace Unreal.Core
{
    /// <summary>
    /// A <see cref="BitReader"/> used for reading everything related to RepLayout. 
    /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/CoreUObject/Public/UObject/CoreNet.h#L303
    /// </summary>
    public class NetBitReader : BitReader
    {
        public NetBitReader(byte[] input) : base(input) { }
        public NetBitReader(byte[] input, int bitCount) : base(input, bitCount) { }
        public NetBitReader(bool[] input) : base(input) { }
        public NetBitReader(bool[] input, int bitCount) : base(input, bitCount) { }

        public override void NetSerializeItem()
        {
            // PropertyByte.cpp 83
            //Ar.SerializeBits( Data, Enum ? FMath::CeilLogTwo(Enum->GetMaxEnumValue()) : 8 );

            // PropertyBool.cpp 331
            //uint8* ByteValue = (uint8*)Data + ByteOffset;
            //uint8 Value = ((*ByteValue & FieldMask) != 0);
            //Ar.SerializeBits(&Value, 1);
            //*ByteValue = ((*ByteValue) & ~FieldMask) | (Value ? ByteMask : 0);
            //return true;

            // EnumProperty.cpp 142
            // Ar.SerializeBits(Data, FMath::CeilLogTwo64(Enum->GetMaxEnumValue()));

            // Repmovement
            // https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Classes/Engine/EngineTypes.h#L3074


            throw new NotImplementedException();
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Classes/Engine/EngineTypes.h#L3047
        /// </summary>
        public void SerializeQuantizedVector()
        {
            throw new NotImplementedException();
            //switch (QuantizationLevel)
            //{
            //    case EVectorQuantization::RoundTwoDecimals:
            //        {
            //            return SerializePackedVector < 100, 30 > (Vector, Ar);
            //        }

            //    case EVectorQuantization::RoundOneDecimal:
            //        {
            //            return SerializePackedVector < 10, 27 > (Vector, Ar);
            //        }

            //    default:
            //        {
            //            return SerializePackedVector < 1, 24 > (Vector, Ar);
            //        }
            //}
        }
    }
}
