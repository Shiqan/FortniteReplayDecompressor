using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace Unreal.Core;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Serialization/BitArchive.h
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Serialization/BitArchive.cpp
/// </summary>
public class BitReader : FBitArchive
{
    private ReadOnlyMemory<byte> Buffer { get; set; }

    public override int Position { get; protected set; }

    private int CurrentByte => Position >> 3;

    public int LastBit { get; private set; }

    public override int MarkPosition { get; protected set; }

    private readonly Dictionary<FBitArchiveEndIndex, int> _tempLastBit = [];


    public BitReader()
    {

    }

    /// <summary>
    /// Initializes a new instance of the BitReader class based on the specified bytes.
    /// </summary>
    /// <param name="input">The input bytes.</param>
    public BitReader(ReadOnlySpan<byte> input)
    {
        Buffer = input.ToArray();
        LastBit = Buffer.Length * 8;
    }

    /// <summary>
    /// Initializes a new instance of the BitReader class based on the specified bytes.
    /// </summary>
    /// <param name="input">The input bool[].</param>
    /// <param name="bitCount">Set last bit position.</param>
    public BitReader(ReadOnlySpan<byte> input, int bitCount)
    {
        Buffer = input.ToArray();
        LastBit = bitCount;
    }

    /// <summary>
    /// Fill the buffer and reset this BitReader. Useful when created with the empty constructor.
    /// </summary>
    public void FillBuffer(ReadOnlySpan<byte> input)
    {
        Buffer = input.ToArray();
        LastBit = Buffer.Length * 8;
        Position = 0;
        IsError = false;
    }

    /// <summary>
    /// Fill the buffer and reset this BitReader. Useful when created with the empty constructor.
    /// </summary>
    public void FillBuffer(ReadOnlySpan<byte> input, int bitCount)
    {
        Buffer = input.ToArray();
        LastBit = bitCount;
        Position = 0;
        IsError = false;
    }

    public override bool AtEnd() => Position >= LastBit;

    public override bool CanRead(int count) => Position + count <= LastBit;

    public override bool PeekBit() => (Buffer.Span[CurrentByte] & (1 << (Position & 7))) > 0;

    public override bool ReadBit()
    {
        if (AtEnd() || IsError)
        {
            IsError = true;
            return false;
        }

        var result = (Buffer.Span[CurrentByte] & (1 << (Position & 7))) > 0;
        Position++;
        return result;
    }

    public override T[] ReadArray<T>(Func<T> func1) => throw new NotImplementedException();

    public override int ReadBitsToInt(int bitCount)
    {
        var result = new byte();
        for (var i = 0; i < bitCount; i++)
        {
            if (IsError)
            {
                return 0;
            }

            if (ReadBit())
            {
                result |= (byte) (1 << i);
            }
        }
        return result;
    }

    public override ulong ReadBitsToLong(int bitCount)
    {
        var result = new ulong();
        for (var i = 0; i < bitCount; i++)
        {
            if (ReadBit())
            {
                result |= (1UL << i);
            }
        }

        return result;
    }

    public override ReadOnlySpan<byte> ReadBits(int bitCount)
    {
        if (!CanRead(bitCount) || bitCount < 0)
        {
            IsError = true;
            return [];
        }

        var bitCountUsedInByte = Position & 7;
        var byteCount = bitCount / 8;
        var extraBits = bitCount % 8;
        if (bitCountUsedInByte == 0 && extraBits == 0)
        {
            var byteResult = Buffer.Span.Slice(CurrentByte, byteCount);
            Position += bitCount;
            return byteResult;
        }

        Span<byte> result = new byte[(bitCount + 7) / 8];

        var bitCountLeftInByte = 8 - (Position & 7);
        var currentByte = CurrentByte;
        var span = Buffer.Span;
        var shiftDelta = (1 << bitCountUsedInByte) - 1;
        for (var i = 0; i < byteCount; i++)
        {
            result[i] = (byte) (
                (span[currentByte + i] >> bitCountUsedInByte) |
                ((span[currentByte + i + 1] & shiftDelta) << bitCountLeftInByte)
                );
        }
        Position += (byteCount * 8);

        bitCount %= 8;
        for (var i = 0; i < bitCount; i++)
        {
            var bit = (Buffer.Span[CurrentByte] & (1 << (Position & 7))) > 0;
            Position++;
            if (bit)
            {
                result[^1] |= (byte) (1 << i);
            }
        }

        return result;
    }


    public override ReadOnlySpan<byte> ReadBits(uint bitCount) => ReadBits((int) bitCount);

    public override bool ReadBoolean() => ReadBit();

    public override byte PeekByte()
    {
        var result = ReadByte();
        Position -= 8;

        return result;
    }

    public override byte ReadByte()
    {
        var bitCountUsedInByte = Position & 7;
        var bitCountLeftInByte = 8 - (Position & 7);

        var result = (bitCountUsedInByte == 0) ? Buffer.Span[CurrentByte] : (byte) ((Buffer.Span[CurrentByte] >> bitCountUsedInByte) | ((Buffer.Span[CurrentByte + 1] & ((1 << bitCountUsedInByte) - 1)) << bitCountLeftInByte));

        Position += 8;
        return result;
    }

    public override T ReadByteAsEnum<T>() => (T) Enum.ToObject(typeof(T), ReadByte());

    public override ReadOnlySpan<byte> ReadBytes(int byteCount)
    {
        if (!CanRead(byteCount * 8) || byteCount < 0)
        {
            IsError = true;
            return [];
        }

        var bitCountUsedInByte = Position & 7;
        var bitCountLeftInByte = 8 - (Position & 7);
        ReadOnlySpan<byte> result;
        if (bitCountUsedInByte == 0)
        {
            result = Buffer.Span[CurrentByte..(CurrentByte + byteCount)];
        }
        else
        {
            Span<byte> output = new byte[byteCount];
            for (var i = 0; i < byteCount; i++)
            {
                output[i] = (byte) ((Buffer.Span[CurrentByte + i] >> bitCountUsedInByte) | ((Buffer.Span[CurrentByte + 1 + i] & ((1 << bitCountUsedInByte) - 1)) << bitCountLeftInByte));
            }
            result = output;
        }

        Position += (byteCount * 8);
        return result;
    }

    public override ReadOnlySpan<byte> ReadBytes(uint byteCount) => ReadBytes((int) byteCount);

    public override string ReadBytesToString(int count) => Convert.ToHexString(ReadBytes(count)).Replace("-", "");

    public override string ReadFString()
    {
        var length = ReadInt32();

        if (length == 0)
        {
            return "";
        }

        var isUnicode = length < 0;
        if (isUnicode)
        {
            length = -2 * length;
        }

        var encoding = isUnicode ? Encoding.Unicode : Encoding.Default;
        return encoding.GetString(ReadBytes(length)).Trim(new[] { ' ', '\0' });
    }

    public override string ReadFName()
    {
        var isHardcoded = ReadBit();
        if (isHardcoded)
        {
            uint nameIndex;
            if (EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES)
            {
                nameIndex = ReadUInt32();
            }
            else
            {
                nameIndex = ReadIntPacked();
            }

            return ((UnrealNames) nameIndex).ToString();
        }

        var inString = ReadFString();
        var inNumber = ReadInt32();

        return inString;
    }

    public override FTransform ReadFTransfrom() => throw new NotImplementedException();

    public override string ReadGUID() => ReadBytesToString(16);

    public override string ReadGUID(int size) => ReadBytesToString(size);

    public override uint ReadSerializedInt(int maxValue)
    {
        uint value = 0;
        for (uint mask = 1; (value + mask) < maxValue; mask *= 2)
        {
            if (ReadBit())
            {
                value |= mask;
            }
        }

        return value;
    }

    public override short ReadInt16()
    {
        var value = ReadBytes(2);
        return IsError ? (short) 0 : BitConverter.ToInt16(value);
    }

    public override int ReadInt32()
    {
        var value = ReadBytes(4);
        return IsError ? 0 : BitConverter.ToInt32(value);
    }

    public override bool ReadInt32AsBoolean() => ReadInt32() == 1;

    public override long ReadInt64()
    {
        var value = ReadBytes(8);
        return IsError ? 0 : BitConverter.ToInt64(value);
    }

    public override uint ReadIntPacked()
    {
        var bitCountUsedInByte = Position & 7;
        var bitCountLeftInByte = 8 - (Position & 7);
        var srcMaskByte0 = (byte) ((1U << bitCountLeftInByte) - 1U);
        var srcMaskByte1 = (byte) ((1U << bitCountUsedInByte) - 1U);
        var srcIndex = CurrentByte;
        var nextSrcIndex = bitCountUsedInByte != 0 ? srcIndex + 1 : srcIndex;

        uint value = 0;
        for (int It = 0, shiftCount = 0; It < 5; ++It, shiftCount += 7)
        {
            if (!CanRead(8))
            {
                IsError = true;
                break;
            }

            if (nextSrcIndex >= Buffer.Length)
            {
                nextSrcIndex = srcIndex;
            }

            Position += 8;

            var readByte = (byte) (((Buffer.Span[srcIndex] >> bitCountUsedInByte) & srcMaskByte0) | ((Buffer.Span[nextSrcIndex] & srcMaskByte1) << (bitCountLeftInByte & 7)));
            value = (uint) ((readByte >> 1) << shiftCount) | value;
            srcIndex++;
            nextSrcIndex++;

            if ((readByte & 1) == 0)
            {
                break;
            }
        }
        return value;
    }

    public override FQuat ReadFQuat() => throw new NotImplementedException();

    public override FVector ReadFVector()
    {
        if (EngineNetworkVersion >= EngineNetworkVersionHistory.HISTORY_PACKED_VECTOR_LWC_SUPPORT)
        {
            return new FVector(ReadDouble(), ReadDouble(), ReadDouble());
        }
        else
        {
            return new FVector(ReadSingle(), ReadSingle(), ReadSingle());
        }
    }

    public override FVector ReadPackedVector(int scaleFactor, int maxBits)
    {
        if (EngineNetworkVersion >= EngineNetworkVersionHistory.HISTORY_PACKED_VECTOR_LWC_SUPPORT && EngineNetworkVersion != EngineNetworkVersionHistory.HISTORY_21_AND_VIEWPITCH_ONLY_DO_NOT_USE)
        {
            return ReadQuantizedVector(scaleFactor);
        }

        return ReadPackedVectorLegacy(scaleFactor, maxBits);
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/commit/db095ed4a590f2ae0f42a2fbf9e22678fbb1fb9f#diff-2641717376a3189c7cc27e9e28c26b72ce57d5d3efcf4a26b6beb0dd92eaaa0f
    /// </summary>
    private FVector ReadQuantizedVector(int scaleFactor)
    {
        var componentBitCountAndExtraInfo = ReadSerializedInt(1 << 7);
        var componentBitCount = (int) (componentBitCountAndExtraInfo & 63U);
        var extraInfo = componentBitCountAndExtraInfo >> 6;

        if (componentBitCount > 0U)
        {
            var X = ReadBitsToLong(componentBitCount);
            var Y = ReadBitsToLong(componentBitCount);
            var Z = ReadBitsToLong(componentBitCount);

            var signBit = 1UL << componentBitCount - 1;

            double fX = (long) (X ^ signBit) - (long) signBit;
            double fY = (long) (Y ^ signBit) - (long) signBit;
            double fZ = (long) (Z ^ signBit) - (long) signBit;

            if (extraInfo > 0)
            {
                fX /= scaleFactor;
                fY /= scaleFactor;
                fZ /= scaleFactor;
            }

            return new FVector(fX, fY, fZ)
            {
                Bits = componentBitCount,
                ScaleFactor = scaleFactor,
            };
        }
        else if (extraInfo == 0)
        {
            double X = ReadSingle();
            double Y = ReadSingle();
            double Z = ReadSingle();

            return new FVector(X, Y, Z)
            {
                Bits = 32,
                ScaleFactor = scaleFactor,
            };
        }
        else
        {
            var X = ReadDouble();
            var Y = ReadDouble();
            var Z = ReadDouble();

            return new FVector(X, Y, Z)
            {
                Bits = 64,
                ScaleFactor = scaleFactor,
            };
        }
    }

    private FVector ReadPackedVectorLegacy(int scaleFactor, int maxBits)
    {
        var bits = ReadSerializedInt(maxBits);

        if (IsError)
        {
            return new FVector(0, 0, 0);
        }

        var bias = 1 << ((int) bits + 1);
        var max = 1 << ((int) bits + 2);

        var dx = ReadSerializedInt(max);
        var dy = ReadSerializedInt(max);
        var dz = ReadSerializedInt(max);

        if (IsError)
        {
            return new FVector(0, 0, 0);
        }

        var x = (dx - bias) / scaleFactor;
        var y = (dy - bias) / scaleFactor;
        var z = (dz - bias) / scaleFactor;

        return new FVector(x, y, z);
    }

    public override FRotator ReadRotation()
    {
        float pitch = 0;
        float yaw = 0;
        float roll = 0;

        if (ReadBit()) // Pitch
        {
            pitch = ReadByte() * 360 / 256;
        }

        if (ReadBit())
        {
            yaw = ReadByte() * 360 / 256;
        }

        if (ReadBit())
        {
            roll = ReadByte() * 360 / 256;
        }

        if (IsError)
        {
            return new FRotator(0, 0, 0);
        }

        return new FRotator(pitch, yaw, roll);
    }

    public override FRotator ReadRotationShort()
    {
        float pitch = 0;
        float yaw = 0;
        float roll = 0;

        if (ReadBit())
        {
            pitch = ReadUInt16() * 360 / 65536;
        }

        if (ReadBit())
        {
            yaw = ReadUInt16() * 360 / 65536;
        }

        if (ReadBit())
        {
            roll = ReadUInt16() * 360 / 65536;
        }

        if (IsError)
        {
            return new FRotator(0, 0, 0);
        }

        return new FRotator(pitch, yaw, roll);
    }

    public override sbyte ReadSByte() => throw new NotImplementedException();

    public override float ReadSingle() => BitConverter.ToSingle(ReadBytes(4));

    public override (T, U)[] ReadTupleArray<T, U>(Func<T> func1, Func<U> func2) => throw new NotImplementedException();

    public override ushort ReadUInt16() => BitConverter.ToUInt16(ReadBytes(2));

    public override uint ReadUInt32() => BitConverter.ToUInt32(ReadBytes(4));

    public override bool ReadUInt32AsBoolean() => throw new NotImplementedException();

    public override T ReadUInt32AsEnum<T>() => throw new NotImplementedException();

    public override ulong ReadUInt64() => BitConverter.ToUInt64(ReadBytes(8));

    public override double ReadDouble() => BitConverter.ToDouble(ReadBytes(8));

    public override void Seek(int offset, SeekOrigin seekOrigin = SeekOrigin.Begin)
    {
        if (offset < 0 || offset >> 3 > Buffer.Length || (offset >> 3 == Buffer.Length && (offset & 7) > 0) || (seekOrigin == SeekOrigin.Current && offset + Position > (Buffer.Length * 8)))
        {
            IsError = true;
            return;
        }

        _ = (seekOrigin switch
        {
            SeekOrigin.Begin => Position = offset,
            SeekOrigin.End => Position = (Buffer.Length * 8) - offset,
            SeekOrigin.Current => Position += offset,
            _ => Position = offset,
        });
    }

    public override void SkipBytes(uint byteCount) => SkipBytes((int) byteCount);

    public override void SkipBytes(int byteCount) => Seek(byteCount * 8, SeekOrigin.Current);

    public override void SkipBits(int numbits) => Seek(numbits, SeekOrigin.Current);

    public override void SkipBits(uint numbits) => SkipBits((int) numbits);

    public override void Mark() => MarkPosition = Position;

    public override void Pop() => Position = MarkPosition;

    public override int GetBitsLeft() => LastBit - Position;

    public override void AppendDataFromChecked(ReadOnlySpan<byte> data, int bitCount)
    {
        LastBit += bitCount;

        // this works only because partial bunches are enforced to be byte aligned
        var combined = new byte[Buffer.Span.Length + data.Length];
        Buffer.CopyTo(combined);
        data.ToArray().CopyTo(combined, Buffer.Span.Length);

        Buffer = combined;
    }

    public override void SetTempEnd(int size, FBitArchiveEndIndex index)
    {
        var setPosition = Position + size;
        if (setPosition > LastBit)
        {
            IsError = true;
            return;
        }

        _tempLastBit[index] = LastBit;
        LastBit = setPosition;
    }

    public override void RestoreTempEnd(FBitArchiveEndIndex index)
    {
        Position = LastBit;
        LastBit = _tempLastBit[index];
        IsError = false;
    }
}
