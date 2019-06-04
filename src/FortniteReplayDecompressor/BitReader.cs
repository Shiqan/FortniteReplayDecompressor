using FortniteReplayReaderDecompressor.Core.Models;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace FortniteReplayReaderDecompressor
{
    /// <summary>
    /// Reads primitive data types as binary values in a <see cref="System.Collections.BitArray"/>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Serialization/BitReader.cpp
    /// </summary>
    /// TODO add interface and convert BitArray to 1 and 0's...
    public class BitReader
    {
        public byte[] GShift = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
        public byte[] GMask = { 0x00, 0x01, 0x03, 0x07, 0x0f, 0x1f, 0x3f, 0x7f };
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
            // not sure if this is needed...
            var num = input.Length * 8 - 1; // not sure if it should be -1...
            if ((num & 7) > 0)
            {
                input[num >> 3] &= GMask[num & 7];
            }

            Bits = new BitArray(input);
        }

        public int this[int index]
        {
            get
            {
                return Bits[index] ? 1 : 0;
            }
        }

        public int this[uint index]
        {
            get
            {
                return Bits[(int)index] ? 1 : 0;
            }
        }

        /// <summary>
        /// Returns whether <see cref="Position"/> in current <see cref="Bits"/> is greater than the lenght of the current <see cref="Bits"/>.
        /// </summary>
        /// <returns>true, if <see cref="Position"/> is greater than lenght, false otherwise</returns>
        public virtual bool AtEnd()
        {
            return Position >= Bits.Length;
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

        private int Shift(int Cnt)
        {
            return 1 << Cnt;
        }

        /// <summary>
        /// Retuns uint.
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Serialization/BitReader.h#L69
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns>uint</returns>
        /// <exception cref="OverflowException"></exception>
        public virtual uint ReadInt(int maxValue)
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

        /// <summary>
        /// Retuns int and advances the stream by 4 bytes.
        /// </summary>
        /// <returns>int</returns>
        public virtual int ReadInt32()
        {
            return BitConverter.ToInt32(ReadBytes(4));
        }

        /// <summary>
        /// Retuns ushort and advances the stream by 2 bytes.
        /// </summary>
        /// <returns>ushort</returns>
        public virtual ushort ReadUInt16()
        {
            return BitConverter.ToUInt16(ReadBytes(2));
        }

        /// <summary>
        /// Retuns uint and advances the stream by 4 bytes.
        /// </summary>
        /// <returns>uint</returns>
        public virtual uint ReadUInt32()
        {
            return BitConverter.ToUInt32(ReadBytes(4));
        }

        /// <summary>
        /// Retuns uint
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Serialization/BitReader.cpp#L254
        /// </summary>
        /// <returns>uint</returns>
        public virtual uint ReadIntPacked()
        {
            uint value = 0;
            bool remaining = true;

            while (remaining)
            {
                remaining = ReadBit(); // Check 1 bit to see if theres more after this
                for (int i = 0; i < 7; i++)
                {
                    if (ReadBit())
                    {
                        value |= (byte)(1 << i); // Add to total value
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/NetSerialization.h#L1210
        /// </summary>
        /// <returns></returns>
        public virtual FVector ReadPackedVector(int scaleFactor, int maxBits)
        {
            var bits = ReadInt(maxBits);
            var bias = 1 << ((int) bits + 1);
            var Max = 1 << ((int) bits + 2);

            var dx = ReadInt(Max);
            var dy = ReadInt(Max);
            var dz = ReadInt(Max);

            var x = dx-bias / scaleFactor;
            var y = dy - bias / scaleFactor;
            var z = dz - bias / scaleFactor;

            return new FVector(x, y, z);
        }

        /// <summary>
        /// Do I need this?
        /// </summary>
        /// <returns>string</returns>
        public virtual string ReadFString()
        {
            var length = ReadInt32();

            if (length == 0)
            {
                return "";
            }

            var isUnicode = length < 0;
            byte[] data;
            string value;

            if (isUnicode)
            {
                length = -2 * length;
                data = ReadBytes(length);
                value = Encoding.Unicode.GetString(data);
            }
            else
            {
                data = ReadBytes(length);
                value = Encoding.Default.GetString(data);
            }

            return value.Trim(new[] { ' ', '\0' });
        }
    }

}
