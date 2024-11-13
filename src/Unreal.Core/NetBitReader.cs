using System;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace Unreal.Core;

/// <summary>
/// A <see cref="BitReader"/> used for reading everything related to RepLayout. 
/// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/CoreUObject/Public/UObject/CoreNet.h#L303
/// </summary>
public class NetBitReader : BitReader
{
    public NetBitReader() : base() { }
    public NetBitReader(byte[] input) : base(input) { }
    public NetBitReader(byte[] input, int bitCount) : base(input, bitCount) { }
    public NetBitReader(ReadOnlySpan<byte> input) : base(input.ToArray()) { }
    public NetBitReader(ReadOnlySpan<byte> input, int bitCount) : base(input.ToArray(), bitCount) { }

    public int SerializePropertyInt() => ReadInt32();

    public uint SerializePropertyUInt32() => ReadUInt32();

    public uint SerializePropertyUInt16() => ReadUInt16();

    public ulong SerializePropertyUInt64() => ReadUInt64();

    public float SerializePropertyFloat() => ReadSingle();

    public double SerializePropertyDouble() => ReadDouble();

    public string SerializePropertyName() => ReadFName();

    public string SerializePropertyString() => ReadFString();


    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Classes/Engine/EngineTypes.h#L3074
    /// see https://github.com/EpicGames/UnrealEngine/blob/41caf70e76701a52b935fc1495c7992e41486f86/Engine/Source/Runtime/Engine/Private/Engine/ReplicatedState.cpp#L63
    /// </summary>
    public FRepMovement SerializeRepMovement(
        VectorQuantization locationQuantizationLevel = VectorQuantization.RoundTwoDecimals,
        RotatorQuantization rotationQuantizationLevel = RotatorQuantization.ByteComponents,
        VectorQuantization velocityQuantizationLevel = VectorQuantization.RoundWholeNumber)
    {
        var bSimulatedPhysicSleep = ReadBit();
        var bRepPhysics = ReadBit();
        var bRepServerFrame = false;
        var bRepServerHandle = false;

        if (EngineNetworkVersion >= EngineNetworkVersionHistory.HISTORY_REPMOVE_SERVERFRAME_AND_HANDLE
            && EngineNetworkVersion != EngineNetworkVersionHistory.HISTORY_21_AND_VIEWPITCH_ONLY_DO_NOT_USE)
        {
            bRepServerFrame = ReadBit();
            bRepServerHandle = ReadBit();
        }

        var repMovement = new FRepMovement
        {
            bSimulatedPhysicSleep = bSimulatedPhysicSleep,
            bRepPhysics = bRepPhysics,
            Location = SerializePropertyQuantizedVector(locationQuantizationLevel),
            Rotation = rotationQuantizationLevel == RotatorQuantization.ByteComponents ? ReadRotation() : ReadRotationShort(),
            LinearVelocity = SerializePropertyQuantizedVector(velocityQuantizationLevel)
        };

        if (repMovement.bRepPhysics)
        {
            repMovement.AngularVelocity = SerializePropertyQuantizedVector(velocityQuantizationLevel);
        }

        if (bRepServerFrame)
        {
            repMovement.ServerFrame = ReadIntPacked();
        }

        if (bRepServerHandle)
        {
            repMovement.ServerPhysicsHandle = ReadIntPacked();
        }

        if (EngineNetworkVersion >= EngineNetworkVersionHistory.RepMoveOptionalAcceleration)
        {
            repMovement.bRepAcceleration = ReadBit();
            if (repMovement.bRepAcceleration)
            {
                repMovement.Acceleration = SerializePropertyQuantizedVector(velocityQuantizationLevel);
            }
        }

        return repMovement;
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/Core/Private/Math/UnrealMath.cpp#L57
    /// </summary>
    public FVector SerializePropertyVector() => ReadFVector();

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/Core/Private/Math/UnrealMath.cpp#L65
    /// </summary>
    public FVector2D SerializePropertyVector2D() => new(ReadSingle(), ReadSingle());

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/c920e8b7d7b2d8c7168bebd63b4cd32cba422d49/Engine/Source/Runtime/Engine/Classes/Engine/NetSerialization.h#L2044
    /// </summary>
    public FVector SerializePropertyVectorNormal() => new(ReadFixedCompressedFloat(1, 16), ReadFixedCompressedFloat(1, 16), ReadFixedCompressedFloat(1, 16));

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/c920e8b7d7b2d8c7168bebd63b4cd32cba422d49/Engine/Source/Runtime/Engine/Classes/Engine/NetSerialization.h#L1954
    /// </summary>
    public FVector SerializePropertyVector10() => ReadPackedVector(10, 24);

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/c920e8b7d7b2d8c7168bebd63b4cd32cba422d49/Engine/Source/Runtime/Engine/Classes/Engine/NetSerialization.h#L2000
    /// </summary>
    public FVector SerializePropertyVector100() => ReadPackedVector(100, 30);

    public void SerializPropertyPlane() => throw new NotImplementedException("SerializPropertyPlane in NetBitReader not implemented");

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/c920e8b7d7b2d8c7168bebd63b4cd32cba422d49/Engine/Source/Runtime/Engine/Classes/Engine/NetSerialization.h#L1821
    /// </summary>
    public float ReadFixedCompressedFloat(int maxValue, int numBits)
    {
        var maxBitValue = (1 << (numBits - 1)) - 1;
        var bias = (1 << (numBits - 1));
        var serIntMax = (1 << (numBits - 0));

        var delta = ReadSerializedInt(serIntMax);
        float unscaledValue = (int) delta - bias;

        if (maxValue > maxBitValue)
        {
            var invScale = maxValue / (float) maxBitValue;

            return unscaledValue * invScale;
        }
        else
        {
            var scale = maxBitValue / (float) maxValue;
            var invScale = 1f / scale;

            return unscaledValue * invScale;
        }
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/c920e8b7d7b2d8c7168bebd63b4cd32cba422d49/Engine/Source/Runtime/Core/Private/Math/UnrealMath.cpp#L72
    /// </summary>
    public FRotator SerializePropertyRotator() => ReadRotationShort();

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/CoreUObject/Private/UObject/PropertyByte.cpp#L83
    /// </summary>
    public int SerializePropertyByte(int enumMaxValue) =>
        //Ar.SerializeBits( Data, Enum ? FMath::CeilLogTwo(Enum->GetMaxEnumValue()) : 8 );
        ReadBitsToInt(enumMaxValue > 0 ? (int) Math.Ceiling(Math.Log2(enumMaxValue)) : 8);

    public int SerializePropertyByte() => ReadByte();

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/CoreUObject/Private/UObject/PropertyBool.cpp#L331
    /// </summary>
    public bool SerializePropertyBool() => ReadBit();

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/CoreUObject/Private/UObject/PropertyBool.cpp#L331
    /// </summary>
    public bool SerializePropertyNativeBool() => ReadBit();

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/CoreUObject/Private/UObject/EnumProperty.cpp#L142
    /// </summary>
    public int SerializePropertyEnum() => ReadBitsToInt(GetBitsLeft());// Ar.SerializeBits(Data, FMath::CeilLogTwo64(Enum->GetMaxEnumValue()));

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/CoreUObject/Private/UObject/PropertyBaseObject.cpp#L84
    /// </summary>
    public uint SerializePropertyObject() =>
        //InternalLoadObject(); // TODO make available in archive
        ReadIntPacked();


    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Classes/Engine/EngineTypes.h#L3047
    /// </summary>
    public FVector SerializePropertyQuantizedVector(VectorQuantization quantizationLevel = VectorQuantization.RoundWholeNumber) => quantizationLevel switch
    {
        VectorQuantization.RoundTwoDecimals => ReadPackedVector(100, 30),
        VectorQuantization.RoundOneDecimal => ReadPackedVector(10, 27),
        _ => ReadPackedVector(1, 24),
    };


    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/Engine/Private/OnlineReplStructs.cpp#L209
    /// </summary>
    public string SerializePropertyNetId()
    {
        // Use highest value for type for other (out of engine) oss type 
        const byte typeHashOther = 31;

        var encodingFlags = ReadByteAsEnum<UniqueIdEncodingFlags>();
        var encoded = false;
        if (encodingFlags.HasFlag(UniqueIdEncodingFlags.IsEncoded))
        {
            encoded = true;
            if (encodingFlags.HasFlag(UniqueIdEncodingFlags.IsEmpty))
            {
                // empty cleared out unique id
                return "";
            }
        }

        // Non empty and hex encoded
        var typeHash = ((int) (encodingFlags & UniqueIdEncodingFlags.TypeMask)) >> 3;
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
