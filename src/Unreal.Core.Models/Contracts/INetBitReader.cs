using System.Collections.Generic;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace Unreal.Core
{
    public interface INetBitReader
    {
        /*
         * TODO: added these methods because source generators are only netstandard 2.0
         * try to clean this up
         */
        EngineNetworkVersionHistory EngineNetworkVersion { get; set; }
        bool ReadBit();
        int ReadBitsToInt(int bitCount);
        uint ReadIntPacked();
        bool ReadBoolean();
        string ReadFString();
        short ReadInt16();
        int ReadInt32();
        uint ReadUInt32();
        long ReadInt64();
        ulong ReadUInt64();
        FVector ReadPackedVector(int scaleFactor, int maxBits);
        byte ReadByte();
        float ReadSingle();
        void SkipBytes(int byteCount);
        void SkipBits(int bitCount);

        float ReadFixedCompressedFloat(int maxValue, int numBits);
        bool SerializePropertyBool();
        byte SerializePropertyByte();
        int SerializePropertyByte(int enumMaxValue);
        int SerializePropertyEnum();
        float SerializePropertyFloat();
        int SerializePropertyInt();
        string SerializePropertyName();
        bool SerializePropertyNativeBool();
        string SerializePropertyNetId();
        uint SerializePropertyObject();
        FVector SerializePropertyQuantizedVector(VectorQuantization quantizationLevel = VectorQuantization.RoundWholeNumber);
        FRotator SerializePropertyRotator();
        string SerializePropertyString();
        ushort SerializePropertyUInt16();
        uint SerializePropertyUInt32();
        ulong SerializePropertyUInt64();
        FVector SerializePropertyVector();
        FVector SerializePropertyVector10();
        FVector SerializePropertyVector100();
        FVector2D SerializePropertyVector2D();
        FVector SerializePropertyVectorNormal();
        FRepMovement SerializeRepMovement(VectorQuantization locationQuantizationLevel = VectorQuantization.RoundTwoDecimals, RotatorQuantization rotationQuantizationLevel = RotatorQuantization.ByteComponents, VectorQuantization velocityQuantizationLevel = VectorQuantization.RoundWholeNumber);
        void SerializPropertyPlane();
    }
}