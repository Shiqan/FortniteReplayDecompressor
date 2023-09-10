using System;
using System.IO;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace Unreal.Core;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/Core/Public/Serialization/Archive.h
/// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/Core/Private/Serialization/Archive.cpp
/// </summary>
public abstract class FArchive : IDisposable
{
    /// <summary>
    /// <see cref="EngineNetworkVersionHistory"/> of current Archive.
    /// </summary>
    public EngineNetworkVersionHistory EngineNetworkVersion { get; set; }

    /// <summary>
    /// <see cref="ReplayHeaderFlags"/> of current Archive.
    /// </summary>
    public ReplayHeaderFlags ReplayHeaderFlags { get; set; }

    /// <summary>
    /// <see cref="NetworkVersionHistory"/> of current Archive.
    /// </summary>
    public NetworkVersionHistory NetworkVersion { get; set; }

    /// <summary>
    /// <see cref="ReplayVersionHistory"/> of current Archive.
    /// </summary>
    public ReplayVersionHistory ReplayVersion { get; set; }

    /// <summary>
    /// <see cref="NetworkReplayVersion"/> of current Archive.
    /// </summary>
    public NetworkReplayVersion NetworkReplayVersion { get; set; }

    /// <summary>
    /// Position in current Archive. Set with <see cref="Seek(int, SeekOrigin)"/>
    /// </summary>
    public abstract int Position { get; protected set; }
    public bool IsError { get; protected set; } = false;

    /// <summary>
    /// Set <see cref="IsError"/> to true.
    /// </summary>
    public virtual void SetError() => IsError = true;

    /// <summary>
    /// Reset <see cref="Position"/> of current Archive.
    /// </summary>
    public void Reset()
    {
        IsError = false;
        Position = 0;
    }

    /// <summary>
    /// Returns whether or not this replay was recorded / is playing with Level Streaming fixes.
    /// see https://github.com/EpicGames/UnrealEngine/blob/811c1ce579564fa92ecc22d9b70cbe9c8a8e4b9a/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L693
    /// </summary>
    public virtual bool HasLevelStreamingFixes() => ReplayHeaderFlags.HasFlag(ReplayHeaderFlags.HasStreamingFixes);

    /// <summary>
    /// Returns whether or not this replay was recorded / is playing with Game Specific Frame Data.
    /// see https://github.com/EpicGames/UnrealEngine/blob/0218ad46444accdba786b9a82bee3f445d9fa938/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L928
    /// </summary>
    public virtual bool HasGameSpecificFrameData() => ReplayHeaderFlags.HasFlag(ReplayHeaderFlags.GameSpecificFrameData);

    /// <summary>
    /// Returns whether or not this replay was recorded / is playing with delta checkpoints.
    /// see https://github.com/EpicGames/UnrealEngine/blob/feeb3c7469e8e881cd4fb67dbcd830d6f94e5e8b/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L901
    /// </summary>
    public virtual bool HasDeltaCheckpoints() => ReplayHeaderFlags.HasFlag(ReplayHeaderFlags.DeltaCheckpoints);

    /// <summary>
    /// Returns whether <see cref="Position"/> in current Archive is greater than the length of the current Archive.
    /// </summary>
    /// <returns>true, if <see cref="Position"/> is greater than lenght, false otherwise</returns>
    public abstract bool AtEnd();

    /// <summary>
    /// Returns whether or not we can read <paramref name="count"/> bits or bytes.
    /// </summary>
    /// <param name="count"></param>
    /// <returns>true if we can read, false otherwise</returns>
    public abstract bool CanRead(int count);

    /// <summary>
    /// Reads a 4-byte unsigned integer from the current stream and casts it to an Enum.
    /// Then advances the position of the stream by four bytes.
    /// </summary>
    ///  <typeparam name="T">The element type of the enum.</typeparam>
    /// <returns>A value of enum T.</returns>
    public abstract T ReadUInt32AsEnum<T>();

    /// <summary>
    /// Reads a byte from the current stream and casts it to an Enum.
    /// Then advances the position of the stream by 1 byte.
    /// </summary>
    ///  <typeparam name="T">The element type of the enum.</typeparam>
    /// <returns>A value of enum T.</returns>
    public abstract T ReadByteAsEnum<T>();

    /// <summary>
    /// Reads an array of <typeparamref name="T"/> from the current stream. The array is prefixed with the number of items in it.
    /// see https://github.com/EpicGames/UnrealEngine/blob/7d9919ac7bfd80b7483012eab342cb427d60e8c9/Engine/Source/Runtime/Core/Public/Containers/Array.h#L1069
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="func1">The function to the value.</param>
    /// <returns>An array of tuples.</returns>
    public abstract T[] ReadArray<T>(Func<T> func1);

    /// <summary>
    /// Reads <paramref name="count"/> bytes from the current stream and advances the current position of the stream by <paramref name="count"/>-bytes.
    /// </summary>
    /// <param name="count">Numer of bytes to read.</param>
    /// <returns>A string.</returns>
    public abstract string ReadBytesToString(int count);

    /// <summary>
    /// Reads a 2-byte unsigned integer from the current stream using little-endian encoding and advances the position of the stream by two bytes.
    /// </summary>
    /// <returns>A 2-byte unsigned integer read from this stream.</returns>
    public abstract ushort ReadUInt16();

    /// <summary>
    /// Reads a 4-byte unsigned integer from the current stream and advances the position of the stream by four bytes.
    /// </summary>
    /// <returns>A 4-byte unsigned integer read from this stream.</returns>
    public abstract uint ReadUInt32();

    /// <summary>
    /// Reads an 8-byte unsigned integer from the current stream and advances the position of the stream by eight bytes.
    /// </summary>
    /// <returns>An 8-byte unsigned integer read from this stream.</returns>
    public abstract ulong ReadUInt64();

    /// <summary>
    /// Reads a 2-byte signed integer from the current stream and advances the current position of the stream by two bytes.
    /// </summary>
    /// <returns>A 2-byte signed integer read from the current stream.</returns>
    public abstract short ReadInt16();

    /// <summary>
    /// Reads a 4-byte signed integer from the current stream and advances the current position of the stream by four bytes.
    /// </summary>
    /// <returns>A 4-byte signed integer read from the current stream.</returns>
    public abstract int ReadInt32();

    /// <summary>
    /// Reads an 8-byte signed integer from the current stream and advances the current position of the stream by eight bytes.
    /// </summary>
    /// <returns>An 8-byte signed integer read from the current stream.</returns>
    public abstract long ReadInt64();

    /// <summary>
    /// Reads a 4-byte floating point value from the current stream and advances the current position of the stream by four bytes.
    /// </summary>
    /// <returns>A 4-byte floating point value read from the current stream.</returns>
    public abstract float ReadSingle();

    /// <summary>
    /// Reads a 8-byte double-precision floating point value from the current stream and advances the current position of the stream by four bytes.
    /// </summary>
    /// <returns>A 8-byte double-precision floating point value read from the current stream.</returns>
    public abstract double ReadDouble();

    /// <summary>
    /// Reads a string from the current stream. The string is prefixed with the length as an 4-byte signed integer.
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Containers/String.cpp#L1390
    /// </summary>
    /// <returns>A string read from this stream.</returns>
    public abstract string ReadFString();

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/CoreUObject/Private/UObject/CoreNet.cpp#L277
    /// </summary>
    public abstract string ReadFName();

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Math/TransformNonVectorized.h#L631
    /// </summary>
    public abstract FTransform ReadFTransfrom();

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/c3caf7b6bf12ae4c8e09b606f10a09776b4d1f38/Engine/Source/Runtime/Core/Public/Math/Quat.h#L582
    /// </summary>
    public abstract FQuat ReadFQuat();

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/NetSerialization.h#L1210
    /// </summary>
    public abstract FVector ReadFVector();

    /// <summary>
    /// Reads 16 bytes from the current stream and advances the current position of the stream by 16-bytes.
    /// </summary>
    /// <returns>A GUID in string format read from this stream.</returns>
    public abstract string ReadGUID();

    /// <summary>
    /// Reads <paramref name="size"/> bytes from the current stream and advances the current position of the stream by <paramref name="size"/>-bytes.
    /// </summary>
    /// <param name="size">size</param>
    /// <returns>A GUID in string format read from this stream.</returns>
    public abstract string ReadGUID(int size);

    /// <summary>
    /// Retuns uint
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Serialization/BitReader.cpp#L254
    /// </summary>
    /// <returns>uint</returns>
    public abstract uint ReadIntPacked();

    /// <summary>
    /// Reads an array of tuples from the current stream. The array is prefixed with the number of items in it.
    /// see https://github.com/EpicGames/UnrealEngine/blob/7d9919ac7bfd80b7483012eab342cb427d60e8c9/Engine/Source/Runtime/Core/Public/Containers/Array.h#L1069
    /// </summary>
    /// <typeparam name="T">The type of the first value.</typeparam>
    /// <typeparam name="U">The type of the second value.</typeparam>
    /// <param name="func1">The function to parse the fist value.</param>
    /// <param name="func2">The function to parse the second value.</param>
    /// <returns>An array of tuples.</returns>
    public abstract ValueTuple<T, U>[] ReadTupleArray<T, U>(Func<T> func1, Func<U> func2);

    /// <summary>
    /// Returns the bit at <see cref="Position"/> and advances the <see cref="Position"/> by one byte or one bit.
    /// </summary>
    /// <returns>bool</returns>
    public abstract bool ReadBoolean();

    /// <summary>
    /// Reads a 4-byte signed integer from the current stream and advances the current position of the stream by four bytes.
    /// </summary>
    /// <returns>true if the integer is nonzero; otherwise, false.</returns>
    public abstract bool ReadInt32AsBoolean();

    /// <summary>
    /// Reads a 4-byte signed integer from the current stream and advances the current position of the stream by four bytes.
    /// </summary>
    /// <returns>true if the integer is nonzero; otherwise, false.</returns>
    public abstract bool ReadUInt32AsBoolean();

    /// <summary>
    /// Returns the byte at <see cref="Position"/> and advances the <see cref="Position"/> by one byte or eight bits.
    /// </summary>
    /// <returns>The value of the byte at <see cref="Position"/> index.</returns>
    public abstract byte ReadByte();

    /// <summary>
    /// Reads a signed byte from this stream and advances the current position of the stream by one byte.
    /// </summary>
    /// <returns>A signed byte read from the current stream.</returns>
    public abstract sbyte ReadSByte();

    /// <summary>
    /// Reads the specified number of bytes from the current stream into a byte array and advances the current position by that number of bytes.
    /// </summary>
    /// <param name="byteCount">The number of bytes to read.</param>
    /// <returns>A byte array containing data read from the underlying stream.</returns>
    public abstract ReadOnlySpan<byte> ReadBytes(int byteCount);

    /// <summary>
    /// Reads the specified number of bytes from the current stream into a byte array and advances the current position by that number of bytes.
    /// </summary>
    /// <param name="byteCount">The number of bytes to read.</param>
    /// <returns>A byte array containing data read from the underlying stream.</returns>
    public abstract ReadOnlySpan<byte> ReadBytes(uint byteCount);

    /// <summary>
    /// Advences the current stream by <paramref name="byteCount"/> bytes.
    /// </summary>
    /// <param name="byteCount"></param>
    public abstract void SkipBytes(uint byteCount);

    /// <summary>
    /// Advences the current stream by <paramref name="byteCount"/> bytes.
    /// </summary>
    /// <param name="byteCount"></param>
    public abstract void SkipBytes(int byteCount);

    /// <summary>
    /// Sets <see cref="Position"/> within current Archive.
    /// </summary>
    /// <param name="offset">The offset relative to the <paramref name="seekOrigin"/>.</param>
    /// <param name="seekOrigin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
    /// <returns></returns>
    public abstract void Seek(int offset, SeekOrigin seekOrigin = SeekOrigin.Begin);

    protected virtual void Dispose(bool disposing)
    {

    }

    public void Dispose() => Dispose(true);
}
