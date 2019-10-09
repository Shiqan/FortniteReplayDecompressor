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
        public override void NetSerializeItem(RepLayoutCmdType type)
        {
            switch (type)
            {
                case RepLayoutCmdType.RepMovement:
                    SerializeRepMovement();
                    break;
                case RepLayoutCmdType.PropertyByte:
                    SerializePropertyByte();
                    break;
                case RepLayoutCmdType.PropertyObject:
                    SerializePropertyObject();
                    break;
                default:
                    throw new NotImplementedException();
            };
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Classes/Engine/EngineTypes.h#L3074
        /// </summary>
        public void SerializeRepMovement()
        {
            // flags
            ReadBits(2); // TODO read bits as int
            // location
            SerializeQuantizedVector(VectorQuantization.RoundWholeNumber);
            // rotation
            ReadRotationShort();
            // velocity
            SerializeQuantizedVector(VectorQuantization.RoundWholeNumber);
            // angular velocity
            // SerializeQuantizedVector(VectorQuantization.RoundWholeNumber);

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
        /// PropertyByte.cpp 83
        /// </summary>
        public void SerializePropertyByte(bool isEnum = true)
        {
            // EngineTypes.h
            // ERole
            var max = 4; 
            ReadBits(isEnum ? (int) Math.Log2(max) : 8);
            //Ar.SerializeBits( Data, Enum ? FMath::CeilLogTwo(Enum->GetMaxEnumValue()) : 8 );
        }

        /// <summary>
        /// PropertyBool.cpp 331
        /// </summary>
        public void SerializePropertyBool()
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
    }
}
