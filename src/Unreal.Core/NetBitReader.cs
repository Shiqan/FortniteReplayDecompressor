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

        /// <summary>
        /// Read the property.
        /// see RepLayout 2516
        /// </summary>
        /// <param name="type"></param>
        public void NetSerializeItem(RepLayoutCmdType type)
        {
            switch (type)
            {
                case RepLayoutCmdType.RepMovement:
                    SerializeRepMovement();
                    break;
                case RepLayoutCmdType.PropertyByte:
                    //SerializePropertyByte();
                    break;
                case RepLayoutCmdType.PropertyObject:
                    SerializePropertyObject();
                    break;
                case RepLayoutCmdType.PropertyVector:
                    SerializePropertyVector();
                    break;
                case RepLayoutCmdType.PropertyVector10:
                    SerializePropertyVector10();
                    break;
                case RepLayoutCmdType.PropertyVector100:
                    SerializePropertyVector100();
                    break;
                case RepLayoutCmdType.PropertyRotator:
                    SerializePropertyRotator();
                    break;
                case RepLayoutCmdType.PropertyVectorNormal:
                    SerializePropertyVectorNormal();
                    break;
                case RepLayoutCmdType.PropertyVectorQ:
                    break;
                case RepLayoutCmdType.PropertyNetId:
                    SerializePropertyNetId();
                    break;
                case RepLayoutCmdType.PropertyBool:
                    SerializePropertyBool();
                    break;
                case RepLayoutCmdType.PropertyFloat:
                    SerializePropertyFloat();
                    break;
                case RepLayoutCmdType.DynamicArray:
                    SerializeDynamicArray();
                    break;
                case RepLayoutCmdType.Property:
                    break;
                case RepLayoutCmdType.PropertyInt:
                    SerializePropertyInt();
                    break;
                case RepLayoutCmdType.PropertyName:
                    SerializePropertyName();
                    break;
                case RepLayoutCmdType.PropertyUInt32:
                    SerializePropertyUInt32();
                    break;
                case RepLayoutCmdType.PropertyPlane:
                    SerializPropertyPlane();
                    break;
                case RepLayoutCmdType.PropertyString:
                    SerializePropertyString();
                    break;
                case RepLayoutCmdType.PropertyUInt64:
                    SerializePropertyUInt64();
                    break;
                case RepLayoutCmdType.PropertyNativeBool:
                    SerializePropertyNativeBool();
                    break;
                default:
                    throw new NotImplementedException();
            };
        }

        /// <summary>
        /// replayout.cpp 3142
        /// </summary>
        public void SerializeDynamicArray()
        {
            var arrayNum = ReadIntPacked();
        }

        public void SerializePropertyInt()
        {
            ReadInt32();
        }

        public void SerializePropertyUInt32()
        {
            ReadUInt32();
        }

        public void SerializePropertyUInt64()
        {

        }

        public void SerializePropertyFloat()
        {
            ReadSingle();
        }

        public void SerializePropertyName()
        {
            // TODO StaticSerialzeName
        }

        public void SerializePropertyString()
        {
            ReadFString();
        }


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Classes/Engine/EngineTypes.h#L3074
        /// </summary>
        public void SerializeRepMovement()
        {
            var flags = ReadBitsToInt(2);
            var bSimulatedPhysicSleep = (flags & (1 << 0)) == 1;
            var bRepPhysics = (flags & (1 << 1)) == 1;

            var location = SerializeQuantizedVector(VectorQuantization.RoundTwoDecimals);
            var rotation = ReadRotation();
            var velocity = SerializeQuantizedVector(VectorQuantization.RoundWholeNumber);

            if (bRepPhysics)
            {
                var angularVelocity = SerializeQuantizedVector(VectorQuantization.RoundWholeNumber);
            }

            //// pack bitfield with flags
            //uint8 Flags = (bSimulatedPhysicSleep << 0) | (bRepPhysics << 1);
            //Ar.SerializeBits(&Flags, 2);
            //bSimulatedPhysicSleep = (Flags & (1 << 0)) ? 1 : 0;
            //bRepPhysics = (Flags & (1 << 1)) ? 1 : 0;

            //bOutSuccess = true;

            //// update location, rotation, linear velocity
            //bOutSuccess &= SerializeQuantizedVector(Ar, Location, LocationQuantizationLevel);

            //switch (RotationQuantizationLevel)
            //{
            //    case ERotatorQuantization::ByteComponents:
            //        {
            //            Rotation.SerializeCompressed(Ar);
            //            break;
            //        }

            //    case ERotatorQuantization::ShortComponents:
            //        {
            //            Rotation.SerializeCompressedShort(Ar);
            //            break;
            //        }
            //}

            //bOutSuccess &= SerializeQuantizedVector(Ar, LinearVelocity, VelocityQuantizationLevel);

            //// update angular velocity if required
            //if (bRepPhysics)
            //{
            //    bOutSuccess &= SerializeQuantizedVector(Ar, AngularVelocity, VelocityQuantizationLevel);
            //}

            //return true;
        }

        /// <summary>
        /// Unrealmath.cpp 59
        /// </summary>
        public void SerializePropertyVector()
        {
            var x = ReadSingle();
            var y = ReadSingle();
            var z = ReadSingle();
        }

        /// <summary>
        /// Unrealmath.cpp 65
        /// </summary>
        public void SerializeVector2D()
        {
            var x = ReadSingle();
            var y = ReadSingle();
        }

        /// <summary>
        /// NetSerialization.cpp 1858
        /// </summary>
        public void SerializePropertyVectorNormal()
        {
            ReadFixedCompressedFloat();
            ReadFixedCompressedFloat();
            ReadFixedCompressedFloat();
        }

        /// <summary>
        /// NetSerialization.h 1768
        /// </summary>
        public void SerializePropertyVector10()
        {
            ReadPackedVector(10, 30);
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
        public void ReadFixedCompressedFloat()
        {
            // Note: enums are used in this function to force bit shifting to be done at compile time
            // NumBits = 8:
            //enum { MaxBitValue = (1 << (NumBits - 1)) - 1 };    //   0111 1111 - Max abs value we will serialize
            //enum { Bias = (1 << (NumBits - 1)) };       //   1000 0000 - Bias to pivot around (in order to support signed values)
            //enum { SerIntMax = (1 << (NumBits - 0)) };      // 1 0000 0000 - What we pass into SerializeInt
            //enum { MaxDelta = (1 << (NumBits - 0)) - 1 };   //   1111 1111 - Max delta is

            //    uint32 Delta;
            //    Ar.SerializeInt(Delta, SerIntMax);
            //    float UnscaledValue = static_cast<float>(static_cast<int32>(Delta) - Bias);

            //    if (MaxValue > MaxBitValue)
            //    {
            //        // We have to scale down, scale needs to be a float:
            //        const float InvScale = MaxValue / (float)MaxBitValue;
            //        Value = UnscaledValue * InvScale;
            //    }
            //    else
            //    {

            //enum { scale = MaxBitValue / MaxValue };
            //const float InvScale = 1.f / (float)scale;

            //Value = UnscaledValue* InvScale;
        }

        /// <summary>
        /// Unrealmath.cpp 65
        /// </summary>
        public FRotator SerializePropertyRotator()
        {
            return ReadRotationShort();
        }

        /// <summary>
        /// PropertyByte.cpp 83
        /// </summary>
        public int SerializePropertyByte(int enumMaxValue)
        {
            //Ar.SerializeBits( Data, Enum ? FMath::CeilLogTwo(Enum->GetMaxEnumValue()) : 8 );
            return ReadBitsToInt(enumMaxValue > 0 ? (int)Math.Ceiling(Math.Log2(enumMaxValue)) : 8);
        }

        /// <summary>
        /// PropertyBool.cpp 331
        /// </summary>
        public bool SerializePropertyBool()
        {
            return ReadBit();
            //uint8* ByteValue = (uint8*)Data + ByteOffset;
            //uint8 Value = ((*ByteValue & FieldMask) != 0);
            //Ar.SerializeBits(&Value, 1);
            //*ByteValue = ((*ByteValue) & ~FieldMask) | (Value ? ByteMask : 0);
            //return true;
        }

        /// <summary>
        /// PropertyBool.cpp 331
        /// </summary>
        public void SerializePropertyNativeBool()
        {
            //uint8* ByteValue = (uint8*)Data + ByteOffset;
            //uint8 Value = ((*ByteValue & FieldMask) != 0);
            //Ar.SerializeBits(&Value, 1);
            //*ByteValue = ((*ByteValue) & ~FieldMask) | (Value ? ByteMask : 0);
            //return true;
        }

        /// <summary>
        /// EnumProperty.cpp 14
        /// </summary>
        public void SerializePropertyEnum()
        {
            // Ar.SerializeBits(Data, FMath::CeilLogTwo64(Enum->GetMaxEnumValue()));
        }

        /// <summary>
        /// PropertyBaseObject.cpp 84
        /// </summary>
        public void SerializePropertyObject()
        {
            //InternalLoadObject(); // TODO make available in archive

            var netGuid = new NetworkGUID()
            {
                Value = ReadIntPacked()
            };

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
        public FVector SerializeQuantizedVector(VectorQuantization quantizationLevel)
        {
            return quantizationLevel switch
            {
                VectorQuantization.RoundTwoDecimals => ReadPackedVector(100, 30),
                VectorQuantization.RoundOneDecimal => ReadPackedVector(10, 27),
                _ => ReadPackedVector(1, 24),
            };
        }

        /// <summary>
        /// OnlineReplStruct.cpp 209
        /// </summary>
        public void SerializePropertyNetId()
        {
            //EUniqueIdEncodingFlags EncodingFlags = EUniqueIdEncodingFlags::NotEncoded;
            //Ar << EncodingFlags;
        }
    }
}
