using System;
using System.Collections;

namespace FortniteReplayReaderDecompressor
{
    /// <summary>
    /// Reads primitive data types as binary values in a <see cref="System.Collections.BitArray"/>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Serialization/BitWriter.cpp
    /// </summary>
    public class BitReader
    {
        private readonly BitArray Bits;
        private int Position;

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
        public virtual bool ReadBit()
        {
            return Bits[Position++];
        }

    }
}
