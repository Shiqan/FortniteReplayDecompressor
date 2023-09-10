using System;
using System.IO;
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

    /// <summary>
    /// Initializes a new instance of the CustomBinaryReader class based on the specified stream.
    /// </summary>
    /// <param name="input">An stream.</param>
    /// <seealso cref="System.IO.BinaryReader"/> 
    public BinaryReader(Stream input)
    {
        using var ms = new MemoryStream((int) input.Length);
        input.CopyTo(ms);
        Bytes = new ReadOnlyMemory<byte>(ms.ToArray());
        _length = Bytes.Length;
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

    public override bool ReadBoolean()
    {
        var result = BitConverter.ToBoolean(Bytes.Slice(_position, 1).Span);
        _position++;
        return result;
    }

    public override byte ReadByte()
    {
        var result = Bytes.Slice(_position, 1).Span;
        _position++;
        return result[0];
    }

    public override T ReadByteAsEnum<T>() => (T) Enum.ToObject(typeof(T), ReadByte());

    public override ReadOnlySpan<byte> ReadBytes(int byteCount)
    {
        var result = Bytes.Slice(_position, byteCount).Span;
        _position += byteCount;
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
        return encoding.GetString(ReadBytes(length))
            .Trim(new[] { ' ', '\0' });
    }

    public override string ReadFName()
    {
        var isHardcoded = ReadBoolean();
        if (isHardcoded)
        {
            var nameIndex = EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES ? ReadUInt32() : ReadIntPacked();
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.h#L31
            // hard coded names in "UnrealNames.inl"
            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.inl
            // https://github.com/EpicGames/UnrealEngine/blob/375ba9730e72bf85b383c07a5e4a7ba98774bcb9/Engine/Source/Runtime/Core/Public/UObject/NameTypes.h#L599
            // https://github.com/EpicGames/UnrealEngine/blob/375ba9730e72bf85b383c07a5e4a7ba98774bcb9/Engine/Source/Runtime/Core/Private/UObject/UnrealNames.cpp#L283
            return ((UnrealNames) nameIndex).ToString();
        }

        // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.h#L17
        // MAX_NETWORKED_HARDCODED_NAME = 410

        // https://github.com/EpicGames/UnrealEngine/blob/375ba9730e72bf85b383c07a5e4a7ba98774bcb9/Engine/Source/Runtime/Core/Public/UObject/NameTypes.h#L34
        // NAME_SIZE = 1024

        // InName.GetComparisonIndex() <= MAX_NETWORKED_HARDCODED_NAME;
        // InName.GetPlainNameString();
        // InName.GetNumber();

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
        var result = BitConverter.ToInt16(Bytes.Slice(_position, 2).Span);
        _position += 2;
        return result;
    }

    public override int ReadInt32()
    {
        var result = BitConverter.ToInt32(Bytes.Slice(_position, 4).Span);
        _position += 4;
        return result;
    }

    public override bool ReadInt32AsBoolean() => ReadUInt32() >= 1;

    public override long ReadInt64()
    {
        var result = BitConverter.ToInt64(Bytes.Slice(_position, 8).Span);
        _position += 8;
        return result;
    }

    public override uint ReadIntPacked()
    {
        uint value = 0;
        byte count = 0;
        var remaining = true;

        while (remaining)
        {
            var nextByte = ReadByte();
            remaining = (nextByte & 1) == 1;            // Check 1 bit to see if theres more after this
            nextByte >>= 1;                             // Shift to get actual 7 bit value
            value += (uint) nextByte << (7 * count++);   // Add to total value
        }
        return value;
    }

    public override sbyte ReadSByte()
    {
        var result = Bytes.Slice(_position, 1).Span;
        _position++;
        return (sbyte) result[0];
    }

    public override float ReadSingle()
    {
        var result = BitConverter.ToSingle(Bytes.Slice(_position, 4).Span);
        _position += 4;
        return result;
    }

    public override double ReadDouble()
    {
        var result = BitConverter.ToDouble(Bytes.Slice(_position, 8).Span);
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
        var result = BitConverter.ToUInt16(Bytes.Slice(_position, 2).Span);
        _position += 2;
        return result;
    }

    public override uint ReadUInt32()
    {
        var result = BitConverter.ToUInt32(Bytes.Slice(_position, 4).Span);
        _position += 4;
        return result;
    }

    public override bool ReadUInt32AsBoolean() => ReadUInt32() >= 1u;

    public override T ReadUInt32AsEnum<T>() => (T) Enum.ToObject(typeof(T), ReadUInt32());

    public override ulong ReadUInt64()
    {
        var result = BitConverter.ToUInt64(Bytes.Slice(_position, 8).Span);
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

    public override void SkipBytes(uint byteCount) => SkipBytes((int) byteCount);

    public override void SkipBytes(int byteCount) => _position += byteCount;
}
