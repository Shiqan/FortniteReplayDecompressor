using System;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace Unreal.Core;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Serialization/BitArchive.h
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Serialization/BitArchive.cpp
/// </summary>
public abstract class FBitArchive : FArchive
{
    /// <summary>
    /// Returns the bit at <see cref="Position"/> and does not advance the <see cref="Position"/> by one bit.
    /// </summary>
    /// <returns>The value of the bit at position index.</returns>
    /// <seealso cref="ReadBit"/>
    public abstract bool PeekBit();

    /// <summary>
    /// Returns the byte at <see cref="Position"/>
    /// </summary>
    /// <returns>The value of the byte at <see cref="Position"/> index.</returns>
    public abstract byte PeekByte();

    /// <summary>
    /// Returns the bit at <see cref="Position"/> and advances the <see cref="Position"/> by one bit.
    /// </summary>
    /// <returns>The value of the bit at position index.</returns>
    /// <seealso cref="PeekBit"/>
    public abstract bool ReadBit();

    /// <summary>
    /// Retuns bool[] and advances the <see cref="Position"/> by <paramref name="bitCount"/> bits.
    /// </summary>
    /// <param name="bits">The number of bits to read.</param>
    /// <returns>bool[]</returns>
    public abstract ReadOnlySpan<byte> ReadBits(int bitCount);

    /// <summary>
    /// Retuns bool[] and advances the <see cref="Position"/> by <paramref name="bitCount"/> bits.
    /// </summary>
    /// <param name="bits">The number of bits to read.</param>
    /// <returns>bool[]</returns>
    public abstract ReadOnlySpan<byte> ReadBits(uint bitCount);

    /// <summary>
    /// Retuns int and advances the <see cref="Position"/> by <paramref name="bitCount"/> bits.
    /// </summary>
    /// <param name="bits">The number of bits to read.</param>
    /// <returns>int</returns>
    public abstract int ReadBitsToInt(int bitCount);

    /// <summary>
    /// Retuns int and advances the <see cref="Position"/> by <paramref name="bitCount"/> bits.
    /// </summary>
    /// <param name="bits">The number of bits to read.</param>
    /// <returns>int</returns>
    public abstract ulong ReadBitsToLong(int bitCount);

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Serialization/BitReader.h#L69
    /// </summary>
    /// <param name="maxValue"></param>
    /// <returns>uint</returns>
    public abstract uint ReadSerializedInt(int maxValue);

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/NetSerialization.h#L1210
    /// </summary>
    /// <returns>Vector</returns>
    public abstract FVector ReadPackedVector(int scaleFactor, int maxBits);

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Math/UnrealMath.cpp#L79
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Math/Rotator.h#L654
    /// </summary>
    /// <returns></returns>
    public abstract FRotator ReadRotation();

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Math/UnrealMath.cpp#L79
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Math/Rotator.h#L654
    /// </summary>
    /// <returns></returns>
    public abstract FRotator ReadRotationShort();

    /// <summary>
    /// Skip next <paramref name="bitCount"/> bits.
    /// </summary>
    /// <param name="bitCount"></param>
    public abstract void SkipBits(int bitCount);

    /// <summary>
    /// Skip next <paramref name="bitCount"/> bits.
    /// </summary>
    /// <param name="bitCount"></param>
    public abstract void SkipBits(uint bitCount);

    /// <summary>
    /// For pushing and popping FBitReaderMark positions.
    /// </summary>
    public abstract int MarkPosition { get; protected set; }

    /// <summary>
    /// Save Position to <see cref="MarkPosition"/> so we can reset back to this point.
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Serialization/BitReader.h#L228
    /// </summary>
    public abstract void Mark();

    /// <summary>
    /// Set Position back to <see cref="MarkPosition"/>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Serialization/BitReader.h#L228
    /// </summary>
    public abstract void Pop();

    /// <summary>
    /// Get number of bits left, including any bits after <see cref="LastBit"/>.
    /// </summary>
    /// <returns>int</returns>
    public abstract int GetBitsLeft();

    /// <summary>
    /// Append bool array to this archive.
    /// </summary>
    /// <param name="data"></param>
    public abstract void AppendDataFromChecked(ReadOnlySpan<byte> data, int bitCount);

    /// <summary>
    /// Pretend this archive ends earlier to prevent reading bytes twice and reduce alloctions. Make sure to call <see cref="RestoreTemp(int)"/> afterwards.
    /// </summary>
    public abstract void SetTempEnd(int size, FBitArchiveEndIndex index);


    /// <summary>
    /// Restore the original end of the archive.
    /// </summary>
    public abstract void RestoreTempEnd(FBitArchiveEndIndex index);
}
