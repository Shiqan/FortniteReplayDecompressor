using System;
using System.Collections;
using System.IO;

namespace FortniteReplayReaderDecompressor
{
    /// <summary>
    /// Reads primitive data types as binary values in a <see cref="System.Collections.BitArray"/>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Serialization/BitWriter.cpp
    /// </summary>
    public class BitReader
    {
        private readonly BitArray Bits;

        /// <summary>
        /// Position in current BitArray. Set with <see cref="Seek(int, SeekOrigin)"/>
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Initializes a new instance of the BitReader class based on the specified bytes.
        /// </summary>
        /// <param name="input">The input bytes.</param>
        /// <exception cref="System.ArgumentException">The stream does not support reading, is null, or is already closed.</exception>
        public BitReader(byte[] input)
        {
            Bits = new BitArray(input);
        }

        /// <summary>
        /// Returns whether position in current BitArray is greater than the lenght of the current BitArray.
        /// </summary>
        /// <returns>true, if position is greater than lenght, false otherwise</returns>
        public virtual bool AtEnd()
        {
            return Position > Bits.Length;
        }

        /// <summary>
        /// Returns the bit at <see cref="Position"/> and does not advance the <see cref="Position"/> by one bit.
        /// </summary>
        /// <returns>The value of the bit at position index.</returns>
        /// <seealso cref="ReadBit"/>
        public virtual bool PeekBit()
        {
            return Bits[Position];
        }

        /// <summary>
        /// Returns the bit at <see cref="Position"/> and advances the <see cref="Position"/> by one bit.
        /// </summary>
        /// <returns>The value of the bit at position index.</returns>
        /// <seealso cref="PeekBit"/>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public virtual bool ReadBit()
        {
            return Bits[Position++];
        }

        /// <summary>
        /// Sets <see cref="Position"/> within current BitArray.
        /// </summary>
        /// <param name="offset">The offset relative to the <paramref name="seekOrigin"/>.</param>
        /// <param name="seekOrigin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public virtual void Seek(int offset, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            if (offset < 0 || offset > Bits.Length || (seekOrigin == SeekOrigin.Current && offset + Position > Bits.Length))
            {
                throw new ArgumentOutOfRangeException("Specified offset doesnt fit within the BitArray buffer");
            }

            _ = (seekOrigin switch
            {
                SeekOrigin.Begin => Position = offset,
                SeekOrigin.End => Position = Bits.Length - offset,
                SeekOrigin.Current => Position += offset,
                _ => Position = offset,
            });
        }

        /// <summary>
        /// Returns the byte at <see cref="Position"/> and advances the <see cref="Position"/> by 8 bits.
        /// </summary>
        /// <returns>The value of the byte at <see cref="Position"/> index.</returns>
        public virtual byte ReadByte()
        {
            var result = new byte();
            for (int i = 0; i < 8; i++)
            {
                if (ReadBit())
                {
                    result |= (byte)(1 << i);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns <paramref name="bytes"/> bytes at <see cref="Position"/> and advances the <see cref="Position"/> by 8 bits.
        /// </summary>
        /// <returns>The value of the byte at <see cref="Position"/> index.</returns>
        public virtual byte[] ReadBytes(int bytes)
        {
            var result = new byte[bytes];
            for (int i = 0; i < bytes; i++)
            {
                result[i] = ReadByte();
            }
            return result;
        }

        /// <summary>
        /// Retuns uint and advances the stream by 4 bytes.
        /// see https://github.com/jjbott/RocketLeagueReplayParser/blob/51a72c36188de4163f0de73545d15096069a807b/RocketLeagueReplayParser/BitReader.cs#L85
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns>uint</returns>
        /// <exception cref="OverflowException"></exception>
        public virtual uint ReadUInt32(uint maxValue)
        {
            uint value = 0;
            for (uint mask = 1; (value + mask) < maxValue && mask > 0; mask *= 2, Position++)
            {
                if (Position >= Bits.Length)
                {
                    throw new OverflowException();
                }

                if (Bits[Position >> 3] & (1 << (Position & 7) > 0))
                {
                    value |= mask;
                }
            }
            return value;
        }
    } 

}
