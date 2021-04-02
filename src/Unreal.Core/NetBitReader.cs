﻿using System;
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
        public NetBitReader(ReadOnlySpan<byte> input) : base(input.ToArray()) { }
        public NetBitReader(ReadOnlySpan<byte> input, int bitCount) : base(input.ToArray(), bitCount) { }

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

        public string SerializePropertyName()
        {
            return ReadFName();
        }

        public string SerializePropertyString()
        {
            return ReadFString();
        }


        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Classes/Engine/EngineTypes.h#L3074
        /// </summary>
        public FRepMovement SerializeRepMovement(
            VectorQuantization locationQuantizationLevel = VectorQuantization.RoundTwoDecimals,
            RotatorQuantization rotationQuantizationLevel = RotatorQuantization.ByteComponents,
            VectorQuantization velocityQuantizationLevel = VectorQuantization.RoundWholeNumber)
        {
            var repMovement = new FRepMovement();
            var flags = ReadBitsToInt(2);
            repMovement.bSimulatedPhysicSleep = (flags & (1 << 0)) > 0;
            repMovement.bRepPhysics = (flags & (1 << 1)) > 0;

            repMovement.Location = SerializePropertyQuantizedVector(locationQuantizationLevel);
            repMovement.Rotation = rotationQuantizationLevel == RotatorQuantization.ByteComponents ? ReadRotation() : ReadRotationShort();

            repMovement.LinearVelocity = SerializePropertyQuantizedVector(velocityQuantizationLevel);

            if (repMovement.bRepPhysics)
            {
                repMovement.AngularVelocity = SerializePropertyQuantizedVector(velocityQuantizationLevel);
            }

            return repMovement;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/Core/Private/Math/UnrealMath.cpp#L57
        /// </summary>
        public FVector SerializePropertyVector()
        {
            return ReadVector();
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/Core/Private/Math/UnrealMath.cpp#L65
        /// </summary>
        public FVector2D SerializePropertyVector2D()
        {
            return new FVector2D(ReadSingle(), ReadSingle());
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/c920e8b7d7b2d8c7168bebd63b4cd32cba422d49/Engine/Source/Runtime/Engine/Classes/Engine/NetSerialization.h#L2044
        /// </summary>
        public FVector SerializePropertyVectorNormal()
        {
            return new FVector(ReadFixedCompressedFloat(1, 16), ReadFixedCompressedFloat(1, 16), ReadFixedCompressedFloat(1, 16));
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/c920e8b7d7b2d8c7168bebd63b4cd32cba422d49/Engine/Source/Runtime/Engine/Classes/Engine/NetSerialization.h#L1954
        /// </summary>
        public FVector SerializePropertyVector10()
        {
            return ReadPackedVector(10, 24);
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/c920e8b7d7b2d8c7168bebd63b4cd32cba422d49/Engine/Source/Runtime/Engine/Classes/Engine/NetSerialization.h#L2000
        /// </summary>
        public FVector SerializePropertyVector100()
        {
            return ReadPackedVector(100, 30);
        }

        public void SerializPropertyPlane()
        {

        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/c920e8b7d7b2d8c7168bebd63b4cd32cba422d49/Engine/Source/Runtime/Engine/Classes/Engine/NetSerialization.h#L1821
        /// </summary>
        public float ReadFixedCompressedFloat(int maxValue, int numBits)
        {
            var maxBitValue = (1 << (numBits - 1)) - 1;
            var bias = (1 << (numBits - 1));
            var serIntMax = (1 << (numBits - 0));
            //int maxDelta = (1 << (numBits - 0)) -1;

            var delta = ReadSerializedInt(serIntMax);
            float unscaledValue = (int)delta - bias;

            if (maxValue > maxBitValue)
            {
                var invScale = maxValue / (float)maxBitValue;

                return unscaledValue * invScale;
            }
            else
            {
                var scale = maxBitValue / (float)maxValue;
                var invScale = 1f / scale;

                return unscaledValue * invScale;
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/c920e8b7d7b2d8c7168bebd63b4cd32cba422d49/Engine/Source/Runtime/Core/Private/Math/UnrealMath.cpp#L72
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
            return ReadBitsToInt(enumMaxValue > 0 ? (int)Math.Ceiling(Math.Log(enumMaxValue, 2)) : 8);
        }

        public int SerializePropertyByte()
        {
            return ReadByte();
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

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/CoreUObject/Private/UObject/PropertyBaseObject.cpp#L84
        /// </summary>
        public uint SerializePropertyObject()
        {
            //InternalLoadObject(); // TODO make available in archive

            return ReadIntPacked();

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

            var bValidTypeHash = typeHash != 0;
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
                    return ReadBytesToString(encodedSize);
                }
                else
                {
                    return ReadFString();
                }
            }

            return "";
        }
    }
}
