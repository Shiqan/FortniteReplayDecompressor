using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace Unreal.Core;

/// <summary>
/// Custom Binary Reader with methods for Unreal Engine replay files
/// </summary>
public class BinaryReader : FArchive
{
    public ReadOnlyMemory<byte> Bytes;
    private readonly int _length;
    private int _position;
    public override int Position { get => _position; protected set => Seek(value); }

    internal static readonly string[] UnrealNamesCache = InitUnrealNamesCache();

    private static string[] InitUnrealNamesCache()
    {
        var names = Enum.GetNames<UnrealNames>();
        var values = (int[])Enum.GetValuesAsUnderlyingType<UnrealNames>();
        var max = 0;
        for (var i = 0; i < values.Length; i++)
        {
            if (values[i] > max) max = values[i];
        }
        var cache = new string[max + 1];
        for (var i = 0; i < values.Length; i++)
        {
            cache[values[i]] = names[i];
        }
        return cache;
    }

    /// <summary>
    /// Initializes a new instance of the CustomBinaryReader class based on the specified stream.
    /// </summary>
    /// <param name="input">An stream.</param>
    /// <seealso cref="System.IO.BinaryReader"/>
    public BinaryReader(Stream input)
    {
        if (input.CanSeek)
        {
            var length = (int)input.Length;
            var bytes = GC.AllocateUninitializedArray<byte>(length);
            input.ReadExactly(bytes);
            Bytes = bytes;
            _length = length;
        }
        else
        {
            using var ms = new MemoryStream();
            input.CopyTo(ms);
            Bytes = ms.ToArray();
            _length = Bytes.Length;
        }
        _position = 0;
    }

    public BinaryReader(ReadOnlyMemory<byte> input)
    {
        Bytes = input;
        _length = Bytes.Length;
        _position = 0;
    }

    public BinaryReader(ReadOnlySpan<byte> input)
    {
        Bytes = input.ToArray();
        _length = Bytes.Length;
        _position = 0;
    }

    public override bool AtEnd() => _position >= _length;

    public override bool CanRead(int count) => _position + count < _length;

    public override T[] ReadArray<T>(Func<T> func1)
    {
        var count = ReadUInt32();
        var arr = new T[count];
        for (var i = 0; i < count; i++)
        {
            arr[i] = (func1.Invoke());
        }
        return arr;
    }

    public override bool ReadBoolean() => Bytes.Span[_position++] != 0;

    public override byte ReadByte() => Bytes.Span[_position++];

    public override T ReadByteAsEnum<T>()
    {
        var b = ReadByte();
        return Unsafe.As<byte, T>(ref b);
    }

    public override ReadOnlySpan<byte> ReadBytes(int byteCount)
    {
        var result = Bytes.Span.Slice(_position, byteCount);
        _position += byteCount;
        return result;
    }

    public override ReadOnlySpan<byte> ReadBytes(uint byteCount) => ReadBytes((int)byteCount);

    public override ReadOnlyMemory<byte> ReadMemory(int byteCount)
    {
        var result = Bytes.Slice(_position, byteCount);
        _position += byteCount;
        return result;
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
        var isHardcoded = ReadBoolean();
        if (isHardcoded)
        {
            var nameIndex = EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES ? ReadUInt32() : ReadIntPacked();
            if (nameIndex < (uint)UnrealNamesCache.Length && UnrealNamesCache[nameIndex] != null)
            {
                return UnrealNamesCache[nameIndex];
            }
            return ((UnrealNames)nameIndex).ToString();
        }

        var inString = ReadFString();
        ReadInt32(); // inNumber

        return inString;
    }

    public override FTransform ReadFTransfrom() => new()
    {
        Rotation = ReadFQuat(),
        Translation = ReadFVector(),
        Scale3D = ReadFVector(),
    };

    public override FQuat ReadFQuat() => new()
    {
        X = ReadSingle(),
        Y = ReadSingle(),
        Z = ReadSingle(),
        W = ReadSingle()
    };

    public override FVector ReadFVector() => new(ReadSingle(), ReadSingle(), ReadSingle());

    public override string ReadGUID() => ReadBytesToString(16);

    public override string ReadGUID(int size) => ReadBytesToString(size);

    public override short ReadInt16()
    {
        var result = BinaryPrimitives.ReadInt16LittleEndian(Bytes.Span.Slice(_position, 2));
        _position += 2;
        return result;
    }

    public override int ReadInt32()
    {
        var result = BinaryPrimitives.ReadInt32LittleEndian(Bytes.Span.Slice(_position, 4));
        _position += 4;
        return result;
    }

    public override bool ReadInt32AsBoolean() => ReadUInt32() >= 1;

    public override long ReadInt64()
    {
        var result = BinaryPrimitives.ReadInt64LittleEndian(Bytes.Span.Slice(_position, 8));
        _position += 8;
        return result;
    }

    public override uint ReadIntPacked()
    {
        uint value = 0;
        byte count = 0;
        var remaining = true;
        var span = Bytes.Span;

        while (remaining)
        {
            var nextByte = span[_position++];
            remaining = (nextByte & 1) == 1;            // Check 1 bit to see if theres more after this
            nextByte >>= 1;                             // Shift to get actual 7 bit value
            value += (uint)nextByte << (7 * count++);   // Add to total value
        }
        return value;
    }

    public override sbyte ReadSByte() => (sbyte)Bytes.Span[_position++];

    public override float ReadSingle()
    {
        var result = BinaryPrimitives.ReadSingleLittleEndian(Bytes.Span.Slice(_position, 4));
        _position += 4;
        return result;
    }

    public override double ReadDouble()
    {
        var result = BinaryPrimitives.ReadDoubleLittleEndian(Bytes.Span.Slice(_position, 8));
        _position += 8;
        return result;
    }

    public override (T, U)[] ReadTupleArray<T, U>(Func<T> func1, Func<U> func2)
    {
        var count = ReadUInt32();
        var arr = new (T, U)[count];
        for (var i = 0; i < count; i++)
        {
            arr[i] = (func1.Invoke(), func2.Invoke());
        }
        return arr;
    }

    public override ushort ReadUInt16()
    {
        var result = BinaryPrimitives.ReadUInt16LittleEndian(Bytes.Span.Slice(_position, 2));
        _position += 2;
        return result;
    }

    public override uint ReadUInt32()
    {
        var result = BinaryPrimitives.ReadUInt32LittleEndian(Bytes.Span.Slice(_position, 4));
        _position += 4;
        return result;
    }

    public override bool ReadUInt32AsBoolean() => ReadUInt32() >= 1u;

    public override T ReadUInt32AsEnum<T>()
    {
        var val = ReadUInt32();
        return Unsafe.As<uint, T>(ref val);
    }

    public override ulong ReadUInt64()
    {
        var result = BinaryPrimitives.ReadUInt64LittleEndian(Bytes.Span.Slice(_position, 8));
        _position += 8;
        return result;
    }

    public override void Seek(int offset, SeekOrigin seekOrigin = SeekOrigin.Begin)
    {
        if (offset < 0 || offset > _length || (seekOrigin == SeekOrigin.Current && offset + _position > _length))
        {
            IsError = true;
            return;
        }

        _ = (seekOrigin switch
        {
            SeekOrigin.Begin => _position = offset,
            SeekOrigin.End => _position = _length - offset,
            SeekOrigin.Current => _position += offset,
            _ => _position = offset,
        });
    }

    public override void SkipBytes(uint byteCount) => SkipBytes((int)byteCount);

    public override void SkipBytes(int byteCount) => _position += byteCount;
}
