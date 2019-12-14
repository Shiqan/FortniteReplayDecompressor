using System;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

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

        public int SerializePropertyInt()
        {
            return ReadInt32();
        }

        public uint SerializePropertyUInt32()
        {
            return ReadUInt32();
        }
        
        public uint SerializePropertyUInt16()
        {
            return ReadUInt16();
        }

        public ulong SerializePropertyUInt64()
        {
            return ReadUInt64();
        }

        public float SerializePropertyFloat()
        {
            return ReadSingle();
        }

        public void SerializePropertyName()
        {
            // TODO StaticSerialzeName
        }

        public string SerializePropertyString()
        {
            return ReadFString();
        }


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Classes/Engine/EngineTypes.h#L3074
        /// </summary>
        public FRepMovement SerializeRepMovement()
        {
            var repMovement = new FRepMovement();
            var flags = ReadBitsToInt(2);
            repMovement.bSimulatedPhysicSleep = (flags & (1 << 0)) == 1;
            repMovement.bRepPhysics = (flags & (1 << 1)) == 1;

            repMovement.Location = SerializePropertyQuantizedVector(VectorQuantization.RoundTwoDecimals);
            repMovement.Rotation = ReadRotation();
            repMovement.LinearVelocity = SerializePropertyQuantizedVector(VectorQuantization.RoundWholeNumber);

            if (repMovement.bRepPhysics)
            {
                repMovement.AngularVelocity = SerializePropertyQuantizedVector(VectorQuantization.RoundWholeNumber);
            }

            return repMovement;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/Core/Private/Math/UnrealMath.cpp#L57
        /// </summary>
        public FVector SerializePropertyVector()
        {
            return new FVector(ReadSingle(), ReadSingle(), ReadSingle());
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/Core/Private/Math/UnrealMath.cpp#L65
        /// </summary>
        public FVector2D SerializeVector2D()
        {
            return new FVector2D(ReadSingle(), ReadSingle());
        }

        /// <summary>
        /// NetSerialization.cpp 1858
        /// </summary>
        public FVector SerializePropertyVectorNormal()
        {
            return new FVector(ReadFixedCompressedFloat(1, 16), ReadFixedCompressedFloat(1, 16), ReadFixedCompressedFloat(1, 16));
        }

        /// <summary>
        /// NetSerialization.h 1768
        /// </summary>
        public FVector SerializePropertyVector10()
        {
            return ReadPackedVector(10, 24);
        }

        /// <summary>
        /// NetSerialization.h 1768
        /// </summary>
        public FVector SerializePropertyVector100()
        {
            return ReadPackedVector(100, 30);
        }

        public void SerializPropertyPlane()
        {

        }

        /// <summary>
        /// NetSerialization.h 1821
        /// </summary>
        public float ReadFixedCompressedFloat(int maxValue, int numBits)
        {
            int maxBitValue = (1 << (numBits - 1)) - 1;
            int bias = (1 << (numBits - 1));
            int serIntMax = (1 << (numBits - 0));
            //int maxDelta = (1 << (numBits - 0)) -1;

            uint delta = ReadSerializedInt(serIntMax);
            float unscaledValue = unchecked((int)delta) - bias;

            if (maxValue > maxBitValue)
            {
                float invScale = maxValue / (float)maxBitValue;

                return unscaledValue * invScale;
            }
            else
            {
                float scale = maxBitValue / (float)maxValue;
                float invScale = 1f / scale;

                return unscaledValue * invScale;
            }
        }

        /// <summary>
        /// Unrealmath.cpp 65
        /// </summary>
        public FRotator SerializePropertyRotator()
        {
            return ReadRotationShort();
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/CoreUObject/Private/UObject/PropertyByte.cpp#L83
        /// </summary>
        public int SerializePropertyByte(int enumMaxValue)
        {
            //Ar.SerializeBits( Data, Enum ? FMath::CeilLogTwo(Enum->GetMaxEnumValue()) : 8 );
            return ReadBitsToInt(enumMaxValue > 0 ? (int)Math.Ceiling(Math.Log2(enumMaxValue)) : 8);
        }

        public int SerializePropertyByte()
        {
            return SerializePropertyByte(-1);
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/CoreUObject/Private/UObject/PropertyBool.cpp#L331
        /// </summary>
        public bool SerializePropertyBool()
        {
            return ReadBit();
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/CoreUObject/Private/UObject/PropertyBool.cpp#L331
        /// </summary>
        public bool SerializePropertyNativeBool()
        {
            return ReadBit();
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/CoreUObject/Private/UObject/EnumProperty.cpp#L142
        /// </summary>
        public int SerializePropertyEnum()
        {
            return ReadBitsToInt(GetBitsLeft());
            // Ar.SerializeBits(Data, FMath::CeilLogTwo64(Enum->GetMaxEnumValue()));
        }
        
        public int SerializePropertyEnum(int enumMaxValue)
        {
            return ReadBitsToInt((int)CeilLogTwo64((ulong)enumMaxValue));
            // Ar.SerializeBits(Data, FMath::CeilLogTwo64(Enum->GetMaxEnumValue()));
        }

        /// <summary>
        /// PropertyBaseObject.cpp 84
        /// </summary>
        public uint SerializePropertyObject()
        {
            //InternalLoadObject(); // TODO make available in archive

            var netGuid = new NetworkGUID()
            {
                Value = ReadIntPacked()
            };
            return netGuid.Value;

            //if (!netGuid.IsValid())
            //{
            //    return;
            //}

            //if (netGuid.IsDefault() || exportGUIDs)
            //{
            //    var flags = archive.ReadByteAsEnum<ExportFlags>();

            //    // outerguid
            //    if (flags == ExportFlags.bHasPath || flags == ExportFlags.bHasPathAndNetWorkChecksum || flags == ExportFlags.All)
            //    {
            //        var outerGuid = InternalLoadObject(archive, true); // TODO: archetype?

            //        var pathName = archive.ReadFString();

            //        if (!NetGuidCache.ContainsKey(netGuid.Value))
            //        {
            //            NetGuidCache.Add(netGuid.Value, pathName);
            //        }

            //        if (flags >= ExportFlags.bHasNetworkChecksum)
            //        {
            //            var networkChecksum = archive.ReadUInt32();
            //        }

            //        return netGuid;
            //    }
            //}

            //return netGuid;

            //UObject* Object = GetObjectPropertyValue(Data);
            //bool Result = Map->SerializeObject(Ar, PropertyClass, Object);
            //SetObjectPropertyValue(Data, Object);
            //return Result;
        }


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Classes/Engine/EngineTypes.h#L3047
        /// </summary>
        public FVector SerializePropertyQuantizedVector(VectorQuantization quantizationLevel = VectorQuantization.RoundWholeNumber)
        {
            return quantizationLevel switch
            {
                VectorQuantization.RoundTwoDecimals => ReadPackedVector(100, 30),
                VectorQuantization.RoundOneDecimal => ReadPackedVector(10, 27),
                _ => ReadPackedVector(1, 24),
            };
        }


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/Engine/Private/OnlineReplStructs.cpp#L209
        /// </summary>
        public string SerializePropertyNetId()
        {
            // Use highest value for type for other (out of engine) oss type 
            const byte typeHashOther = 31;

            var encodingFlags = ReadByteAsEnum<UniqueIdEncodingFlags>();
            var encoded = false;
            if ((encodingFlags & UniqueIdEncodingFlags.IsEncoded) == UniqueIdEncodingFlags.IsEncoded)
            {
                encoded = true;
                if ((encodingFlags & UniqueIdEncodingFlags.IsEmpty) == UniqueIdEncodingFlags.IsEmpty)
                {
                    // empty cleared out unique id
                    return "";
                }
            }

            // Non empty and hex encoded
            var typeHash = ((int)(encodingFlags & UniqueIdEncodingFlags.TypeMask)) >> 3;
            if (typeHash == 0)
            {
                // If no type was encoded, assume default
                //TypeHash = UOnlineEngineInterface::Get()->GetReplicationHashForSubsystem(UOnlineEngineInterface::Get()->GetDefaultOnlineSubsystemName());
                return "NULL";
            }

            bool bValidTypeHash = typeHash != 0;
            string typeString;
            if (typeHash == typeHashOther)
            {
                typeString = ReadFString();
                if (typeString == UnrealNames.None.ToString())
                {
                    bValidTypeHash = false;
                }
            }

            if (bValidTypeHash)
            {
                if (encoded)
                {
                    var encodedSize = ReadByte();

                    // https://github.com/dotnet/corefx/issues/10013
                    return BitConverter.ToString(ReadBytes(encodedSize)).Replace("-", "");
                }
                else
                {
                    return ReadFString();
                }
            }

            return "";
        }


        /// <summary>
        /// Computes the base 2 logarithm for a 64-bit value that is greater than 0.
        /// The result is rounded down to the nearest integer.
        /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/Core/Public/GenericPlatform/GenericPlatformMath.h#L332
        /// </summary>
        /// <param name="Value">The value to compute the log of</param>
        /// <returns>Log2 of Value. 0 if Value is 0.</returns>
        public static ulong FloorLog2_64(ulong Value)
        {
            ulong pos = 0;
            if (Value >= 1ul << 32) { Value >>= 32; pos += 32; }
            if (Value >= 1ul << 16) { Value >>= 16; pos += 16; }
            if (Value >= 1ul << 8) { Value >>= 8; pos += 8; }
            if (Value >= 1ul << 4) { Value >>= 4; pos += 4; }
            if (Value >= 1ul << 2) { Value >>= 2; pos += 2; }
            if (Value >= 1ul << 1) { pos += 1; }
            return (Value == 0) ? 0 : pos;
        }

        /// <summary>
        /// Counts the number of leading zeros in the bit representation of the 64-bit value.
        /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/Core/Public/GenericPlatform/GenericPlatformMath.h#L364
        /// </summary>
        /// <param name="Value">the value to determine the number of leading zeros for</param>
        /// <returns>the number of zeros before the first "on" bit</returns>
        public static ulong CountLeadingZeros64(ulong Value)
        {
            if (Value == 0) return 64;
            return 63 - FloorLog2_64(Value);
        }

        public static ulong CeilLogTwo64(ulong Arg)
        {
            ulong Bitmask = ((ulong)(CountLeadingZeros64(Arg) << 57)) >> 63;
            return (64 - CountLeadingZeros64(Arg - 1)) & (~Bitmask);
        }
    }
}
