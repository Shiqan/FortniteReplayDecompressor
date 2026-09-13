using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
    private ReadOnlyMemory<byte> _buffer;
    protected ReadOnlyMemory<byte> Buffer
    {
        get => _buffer;
        set
        {
            _buffer = value;
            if (MemoryMarshal.TryGetArray(value, out var seg))
            {
                _byteArray = seg.Array;
                _byteOffset = seg.Offset;
            }
            else
            {
                _byteArray = null;
                _byteOffset = 0;
            }
        }
    }
    private byte[]? _byteArray;
    private int _byteOffset;
    private byte[]? _reusableBuffer;

    public override int Position { get; protected set; }

    private int CurrentByte => Position >> 3;

    public int LastBit { get; private set; }

    public override int MarkPosition { get; protected set; }

    private readonly int[] _tempLastBit = new int[4];


    public BitReader()
    {

    }

    public BitReader(ReadOnlyMemory<byte> input)
    {
        Buffer = input;
        LastBit = Buffer.Length * 8;
    }

    public BitReader(ReadOnlyMemory<byte> input, int bitCount)
    {
        Buffer = input;
        LastBit = bitCount;
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

    public void FillBuffer(ReadOnlyMemory<byte> input)
    {
        Buffer = input;
        LastBit = Buffer.Length * 8;
        Position = 0;
        IsError = false;
    }

    public void FillBuffer(ReadOnlyMemory<byte> input, int bitCount)
    {
        Buffer = input;
        LastBit = bitCount;
        Position = 0;
        IsError = false;
    }

    /// <summary>
    /// Fill the buffer and reset this BitReader. Useful when created with the empty constructor.
    /// </summary>
    public void FillBuffer(ReadOnlySpan<byte> input)
    {
        if (_reusableBuffer == null || _reusableBuffer.Length < input.Length)
        {
            _reusableBuffer = new byte[Math.Max(input.Length * 2, 2048)];
        }
        input.CopyTo(_reusableBuffer);
        Buffer = _reusableBuffer.AsMemory(0, input.Length);
        LastBit = input.Length * 8;
        Position = 0;
        IsError = false;
    }

    /// <summary>
    /// Fill the buffer and reset this BitReader. Useful when created with the empty constructor.
    /// </summary>
    public void FillBuffer(ReadOnlySpan<byte> input, int bitCount)
    {
        if (_reusableBuffer == null || _reusableBuffer.Length < input.Length)
        {
            _reusableBuffer = new byte[Math.Max(input.Length * 2, 2048)];
        }
        input.CopyTo(_reusableBuffer);
        Buffer = _reusableBuffer.AsMemory(0, input.Length);
        LastBit = bitCount;
        Position = 0;
        IsError = false;
    }

    /// <summary>
    /// Reads <paramref name="bitCount"/> bits directly from <paramref name="source"/> into this reader's reusable buffer without allocating.
    /// </summary>
    public void FillBitsFrom(FBitArchive source, int bitCount)
    {
        if (bitCount <= 0 || !source.CanRead(bitCount))
        {
            IsError = bitCount < 0 || source.IsError;
            Buffer = ReadOnlyMemory<byte>.Empty;
            LastBit = 0;
            Position = 0;
            return;
        }

        var requiredBytes = (bitCount + 7) / 8;
        if (_reusableBuffer == null || _reusableBuffer.Length < requiredBytes)
        {
            _reusableBuffer = new byte[Math.Max(requiredBytes * 2, 2048)];
        }

        source.ReadBits(_reusableBuffer.AsSpan(0, requiredBytes), bitCount);
        Buffer = _reusableBuffer.AsMemory(0, requiredBytes);
        LastBit = bitCount;
        Position = 0;
        IsError = source.IsError;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool AtEnd() => Position >= LastBit;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool CanRead(int count) => Position + count <= LastBit;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool PeekBit()
    {
        var bit = Position & 7;
        var byteIdx = Position >> 3;
        if (_byteArray != null)
        {
            return (_byteArray[_byteOffset + byteIdx] & (1 << bit)) != 0;
        }
        return (_buffer.Span[byteIdx] & (1 << bit)) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool ReadBit()
    {
        if (Position >= LastBit || IsError)
        {
            IsError = true;
            return false;
        }

        var bit = Position & 7;
        var byteIdx = Position >> 3;
        Position++;

        if (_byteArray != null)
        {
            return (_byteArray[_byteOffset + byteIdx] & (1 << bit)) != 0;
        }

        return (_buffer.Span[byteIdx] & (1 << bit)) != 0;
    }

    public override T[] ReadArray<T>(Func<T> func1) => throw new NotImplementedException();

    public override int ReadBitsToInt(int bitCount)
    {
        if (!CanRead(bitCount) || bitCount < 0)
        {
            IsError = true;
            return 0;
        }
        if (bitCount == 0) return 0;

        var currentByte = CurrentByte;
        if (currentByte + 8 <= Buffer.Length)
        {
            var val = BinaryPrimitives.ReadUInt64LittleEndian(Buffer.Span.Slice(currentByte, 8));
            var mask = bitCount == 64 ? ~0UL : (1UL << bitCount) - 1UL;
            var res = (int)((val >> (Position & 7)) & mask);
            Position += bitCount;
            return res;
        }

        var result = 0;
        for (var i = 0; i < bitCount; i++)
        {
            if (IsError)
            {
                return 0;
            }

            if (ReadBit())
            {
                result |= (1 << i);
            }
        }
        return result;
    }

    public override ulong ReadBitsToLong(int bitCount)
    {
        if (!CanRead(bitCount) || bitCount < 0)
        {
            IsError = true;
            return 0;
        }
        if (bitCount == 0) return 0;

        var currentByte = CurrentByte;
        if (currentByte + 8 <= Buffer.Length)
        {
            var val = BinaryPrimitives.ReadUInt64LittleEndian(Buffer.Span.Slice(currentByte, 8));
            var mask = bitCount == 64 ? ~0UL : (1UL << bitCount) - 1UL;
            var res = (val >> (Position & 7)) & mask;
            Position += bitCount;
            return res;
        }

        var result = 0UL;
        for (var i = 0; i < bitCount; i++)
        {
            if (ReadBit())
            {
                result |= (1UL << i);
            }
        }

        return result;
    }

    public override void ReadBits(Span<byte> destination, int bitCount)
    {
        if (!CanRead(bitCount) || bitCount < 0)
        {
            IsError = true;
            return;
        }

        var bitCountUsedInByte = Position & 7;
        var byteCount = bitCount / 8;
        var extraBits = bitCount % 8;
        var span = Buffer.Span;
        var currentByte = CurrentByte;

        if (bitCountUsedInByte == 0)
        {
            if (byteCount > 0)
            {
                span.Slice(currentByte, byteCount).CopyTo(destination);
                Position += (byteCount * 8);
            }
            if (extraBits > 0)
            {
                destination[byteCount] = (byte)(span[CurrentByte] & ((1 << extraBits) - 1));
                Position += extraBits;
            }
            return;
        }

        var neededBytes = (bitCount + 7) / 8;
        destination[..neededBytes].Clear();

        var bitCountLeftInByte = 8 - bitCountUsedInByte;
        var shiftDelta = (1 << bitCountUsedInByte) - 1;
        for (var i = 0; i < byteCount; i++)
        {
            destination[i] = (byte)(
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
                destination[neededBytes - 1] |= (byte)(1 << i);
            }
        }
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
        ReadBits(result, bitCount);
        return result;
    }

    public override ReadOnlySpan<byte> ReadBits(uint bitCount) => ReadBits((int)bitCount);

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
        var cur = CurrentByte;
        Position += 8;

        if (_byteArray != null)
        {
            var offsetCur = _byteOffset + cur;
            return (bitCountUsedInByte == 0)
                ? _byteArray[offsetCur]
                : (byte)((_byteArray[offsetCur] >> bitCountUsedInByte) | ((_byteArray[offsetCur + 1] & ((1 << bitCountUsedInByte) - 1)) << (8 - bitCountUsedInByte)));
        }

        var span = _buffer.Span;
        return (bitCountUsedInByte == 0)
            ? span[cur]
            : (byte)((span[cur] >> bitCountUsedInByte) | ((span[cur + 1] & ((1 << bitCountUsedInByte) - 1)) << (8 - bitCountUsedInByte)));
    }

    public override T ReadByteAsEnum<T>()
    {
        var b = ReadByte();
        return Unsafe.As<byte, T>(ref b);
    }

    public void ReadBytes(Span<byte> destination)
    {
        var byteCount = destination.Length;
        if (!CanRead(byteCount * 8) || byteCount < 0)
        {
            IsError = true;
            destination.Clear();
            return;
        }

        var bitCountUsedInByte = Position & 7;
        var currentByte = CurrentByte;
        var span = Buffer.Span;

        if (bitCountUsedInByte == 0)
        {
            span.Slice(currentByte, byteCount).CopyTo(destination);
        }
        else
        {
            var bitCountLeftInByte = 8 - bitCountUsedInByte;
            var mask = (1 << bitCountUsedInByte) - 1;
            for (var i = 0; i < byteCount; i++)
            {
                destination[i] = (byte)((span[currentByte + i] >> bitCountUsedInByte) | ((span[currentByte + 1 + i] & mask) << bitCountLeftInByte));
            }
        }

        Position += (byteCount * 8);
    }

    public override ReadOnlySpan<byte> ReadBytes(int byteCount)
    {
        if (!CanRead(byteCount * 8) || byteCount < 0)
        {
            IsError = true;
            return [];
        }

        var bitCountUsedInByte = Position & 7;
        if (bitCountUsedInByte == 0)
        {
            var result = Buffer.Span.Slice(CurrentByte, byteCount);
            Position += (byteCount * 8);
            return result;
        }

        var output = new byte[byteCount];
        ReadBytes(output);
        return output;
    }

    public override ReadOnlySpan<byte> ReadBytes(uint byteCount) => ReadBytes((int)byteCount);

    public override ReadOnlyMemory<byte> ReadMemory(int byteCount)
    {
        if (!CanRead(byteCount * 8) || byteCount < 0)
        {
            IsError = true;
            return ReadOnlyMemory<byte>.Empty;
        }

        if ((Position & 7) == 0)
        {
            var result = Buffer.Slice(CurrentByte, byteCount);
            Position += (byteCount * 8);
            return result;
        }

        var output = new byte[byteCount];
        ReadBytes(output);
        return output;
    }

    public override string ReadBytesToString(int count) => Convert.ToHexString(ReadBytes(count));

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

        var bytes = ReadBytes(length);
        if (!isUnicode)
        {
            if (bytes.Length > 0 && bytes[^1] == 0)
            {
                bytes = bytes[..^1];
            }
            return Encoding.Default.GetString(bytes).Trim(' ');
        }
        else
        {
            if (bytes.Length >= 2 && bytes[^1] == 0 && bytes[^2] == 0)
            {
                bytes = bytes[..^2];
            }
            return Encoding.Unicode.GetString(bytes).Trim(' ');
        }
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

            if (nameIndex < (uint)BinaryReader.UnrealNamesCache.Length && BinaryReader.UnrealNamesCache[nameIndex] != null)
            {
                return BinaryReader.UnrealNamesCache[nameIndex];
            }
            return ((UnrealNames)nameIndex).ToString();
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
        if (!CanRead(16))
        {
            IsError = true;
            return 0;
        }
        if ((Position & 7) == 0)
        {
            var res = BinaryPrimitives.ReadInt16LittleEndian(Buffer.Span.Slice(CurrentByte, 2));
            Position += 16;
            return res;
        }
        Span<byte> bytes = stackalloc byte[2];
        ReadBytes(bytes);
        return BinaryPrimitives.ReadInt16LittleEndian(bytes);
    }

    public override int ReadInt32()
    {
        if (!CanRead(32))
        {
            IsError = true;
            return 0;
        }
        if ((Position & 7) == 0)
        {
            var res = BinaryPrimitives.ReadInt32LittleEndian(Buffer.Span.Slice(CurrentByte, 4));
            Position += 32;
            return res;
        }
        Span<byte> bytes = stackalloc byte[4];
        ReadBytes(bytes);
        return BinaryPrimitives.ReadInt32LittleEndian(bytes);
    }

    public override bool ReadInt32AsBoolean() => ReadInt32() == 1;

    public override long ReadInt64()
    {
        if (!CanRead(64))
        {
            IsError = true;
            return 0;
        }
        if ((Position & 7) == 0)
        {
            var res = BinaryPrimitives.ReadInt64LittleEndian(Buffer.Span.Slice(CurrentByte, 8));
            Position += 64;
            return res;
        }
        Span<byte> bytes = stackalloc byte[8];
        ReadBytes(bytes);
        return BinaryPrimitives.ReadInt64LittleEndian(bytes);
    }

    public override uint ReadIntPacked()
    {
        if ((Position & 7) == 0)
        {
            var span = Buffer.Span;
            var srcIndex = CurrentByte;
            uint val = 0;
            for (int it = 0, shift = 0; it < 5; ++it, shift += 7)
            {
                if (!CanRead(8))
                {
                    IsError = true;
                    break;
                }
                Position += 8;
                var b = span[srcIndex++];
                val |= (uint)((b >> 1) << shift);
                if ((b & 1) == 0)
                {
                    break;
                }
            }
            return val;
        }

        var bitCountUsedInByte = Position & 7;
        var bitCountLeftInByte = 8 - bitCountUsedInByte;
        var srcMaskByte0 = (byte)((1U << bitCountLeftInByte) - 1U);
        var srcMaskByte1 = (byte)((1U << bitCountUsedInByte) - 1U);
        var curByte = CurrentByte;
        var nextByteIdx = curByte + 1;
        var bufSpan = Buffer.Span;

        uint value = 0;
        for (int it = 0, shiftCount = 0; it < 5; ++it, shiftCount += 7)
        {
            if (!CanRead(8))
            {
                IsError = true;
                break;
            }

            if (nextByteIdx >= Buffer.Length)
            {
                nextByteIdx = curByte;
            }

            Position += 8;

            var readByte = (byte)(((bufSpan[curByte] >> bitCountUsedInByte) & srcMaskByte0) | ((bufSpan[nextByteIdx] & srcMaskByte1) << (bitCountLeftInByte & 7)));
            value = (uint)((readByte >> 1) << shiftCount) | value;
            curByte++;
            nextByteIdx++;

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
        var componentBitCount = (int)(componentBitCountAndExtraInfo & 63U);
        var extraInfo = componentBitCountAndExtraInfo >> 6;

        if (componentBitCount > 0U)
        {
            var X = ReadBitsToLong(componentBitCount);
            var Y = ReadBitsToLong(componentBitCount);
            var Z = ReadBitsToLong(componentBitCount);

            var signBit = 1UL << componentBitCount - 1;

            double fX = (long)(X ^ signBit) - (long)signBit;
            double fY = (long)(Y ^ signBit) - (long)signBit;
            double fZ = (long)(Z ^ signBit) - (long)signBit;

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

        var bias = 1 << ((int)bits + 1);
        var max = 1 << ((int)bits + 2);

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

    public override float ReadSingle()
    {
        if (!CanRead(32))
        {
            IsError = true;
            return 0;
        }
        if ((Position & 7) == 0)
        {
            var res = BinaryPrimitives.ReadSingleLittleEndian(Buffer.Span.Slice(CurrentByte, 4));
            Position += 32;
            return res;
        }
        Span<byte> bytes = stackalloc byte[4];
        ReadBytes(bytes);
        return BinaryPrimitives.ReadSingleLittleEndian(bytes);
    }

    public override (T, U)[] ReadTupleArray<T, U>(Func<T> func1, Func<U> func2) => throw new NotImplementedException();

    public override ushort ReadUInt16()
    {
        if (!CanRead(16))
        {
            IsError = true;
            return 0;
        }
        if ((Position & 7) == 0)
        {
            var res = BinaryPrimitives.ReadUInt16LittleEndian(Buffer.Span.Slice(CurrentByte, 2));
            Position += 16;
            return res;
        }
        Span<byte> bytes = stackalloc byte[2];
        ReadBytes(bytes);
        return BinaryPrimitives.ReadUInt16LittleEndian(bytes);
    }

    public override uint ReadUInt32()
    {
        if (!CanRead(32))
        {
            IsError = true;
            return 0;
        }
        if ((Position & 7) == 0)
        {
            var res = BinaryPrimitives.ReadUInt32LittleEndian(Buffer.Span.Slice(CurrentByte, 4));
            Position += 32;
            return res;
        }
        Span<byte> bytes = stackalloc byte[4];
        ReadBytes(bytes);
        return BinaryPrimitives.ReadUInt32LittleEndian(bytes);
    }

    public override bool ReadUInt32AsBoolean() => throw new NotImplementedException();

    public override T ReadUInt32AsEnum<T>()
    {
        var val = ReadUInt32();
        return Unsafe.As<uint, T>(ref val);
    }

    public override ulong ReadUInt64()
    {
        if (!CanRead(64))
        {
            IsError = true;
            return 0;
        }
        if ((Position & 7) == 0)
        {
            var res = BinaryPrimitives.ReadUInt64LittleEndian(Buffer.Span.Slice(CurrentByte, 8));
            Position += 64;
            return res;
        }
        Span<byte> bytes = stackalloc byte[8];
        ReadBytes(bytes);
        return BinaryPrimitives.ReadUInt64LittleEndian(bytes);
    }

    public override double ReadDouble()
    {
        if (!CanRead(64))
        {
            IsError = true;
            return 0;
        }
        if ((Position & 7) == 0)
        {
            var res = BinaryPrimitives.ReadDoubleLittleEndian(Buffer.Span.Slice(CurrentByte, 8));
            Position += 64;
            return res;
        }
        Span<byte> bytes = stackalloc byte[8];
        ReadBytes(bytes);
        return BinaryPrimitives.ReadDoubleLittleEndian(bytes);
    }

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

    public override void SkipBytes(uint byteCount) => SkipBytes((int)byteCount);

    public override void SkipBytes(int byteCount) => Seek(byteCount * 8, SeekOrigin.Current);

    public override void SkipBits(int numbits) => Seek(numbits, SeekOrigin.Current);

    public override void SkipBits(uint numbits) => SkipBits((int)numbits);

    public override void Mark() => MarkPosition = Position;

    public override void Pop() => Position = MarkPosition;

    public override int GetBitsLeft() => LastBit - Position;

    public override void AppendDataFromChecked(ReadOnlySpan<byte> data, int bitCount)
    {
        LastBit += bitCount;

        // this works only because partial bunches are enforced to be byte aligned
        var combined = new byte[Buffer.Length + data.Length];
        Buffer.CopyTo(combined);
        data.CopyTo(combined.AsSpan(Buffer.Length));

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

        _tempLastBit[(int)index] = LastBit;
        LastBit = setPosition;
    }

    public override void RestoreTempEnd(FBitArchiveEndIndex index)
    {
        Position = LastBit;
        LastBit = _tempLastBit[(int)index];
        IsError = false;
    }
}
