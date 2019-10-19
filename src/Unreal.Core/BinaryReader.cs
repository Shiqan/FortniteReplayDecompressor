using System;
using System.IO;
using System.Text;

namespace Unreal.Core
{
    /// <summary>
    /// Custom Binary Reader with methods for Unreal Engine replay files
    /// </summary>
    public class BinaryReader : FArchive, IDisposable
    {
        private readonly System.IO.BinaryReader Reader;
        public Stream BaseStream => Reader.BaseStream;
        public override int Position { get => (int)BaseStream.Position; protected set => Seek(value); }

        /// <summary>
        /// Initializes a new instance of the CustomBinaryReader class based on the specified stream.
        /// </summary>
        /// <param name="input">An stream.</param>
        /// <seealso cref="System.IO.BinaryReader"/> 
        public BinaryReader(Stream input)
        {
            Reader = new System.IO.BinaryReader(input);
        }

        public override bool AtEnd()
        {
            return Reader.BaseStream.Position >= Reader.BaseStream.Length;
        }

        public void Dispose()
        {
            Reader.Dispose();
        }

        /// <summary>
        /// Reads an array of <typeparamref name="T"/> from the current stream. The array is prefixed with the number of items in it.
        /// see https://github.com/EpicGames/UnrealEngine/blob/7d9919ac7bfd80b7483012eab342cb427d60e8c9/Engine/Source/Runtime/Core/Public/Containers/Array.h#L1069
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="func1">The function to the value.</param>
        /// <returns>An array of tuples.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
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

        /// <summary>
        /// Reads a Boolean value from the current stream and advances the current position of the stream by one byte.
        /// </summary>
        /// <returns>true if the byte is nonzero; otherwise, false.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override bool ReadBoolean()
        {
            return Reader.ReadBoolean();
        }

        /// <summary>
        /// Reads the next byte from the current stream and advances the current position of the stream by one byte.
        /// </summary>
        /// <returns>The next byte read from the current stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override byte ReadByte()
        {
            return Reader.ReadByte();
        }

        /// <summary>
        /// Reads a byte from the current stream and casts it to an Enum.
        /// Then advances the position of the stream by 1 byte.
        /// </summary>
        ///  <typeparam name="T">The element type of the enum.</typeparam>
        /// <returns>A value of enum T.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override T ReadByteAsEnum<T>()
        {
            return (T)Enum.ToObject(typeof(T), ReadByte());
        }


        /// <summary>
        /// Reads the specified number of bytes from the current stream into a byte array and advances the current position by that number of bytes.
        /// </summary>
        /// <param name="byteCount">The number of bytes to read.</param>
        /// <returns>A byte array containing data read from the underlying stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when count is negative.</exception>
        public override byte[] ReadBytes(int byteCount)
        {
            return Reader.ReadBytes(byteCount);
        }

        /// <summary>
        /// Reads the specified number of bytes from the current stream into a byte array and advances the current position by that number of bytes.
        /// </summary>
        /// <param name="byteCount">The number of bytes to read.</param>
        /// <returns>A byte array containing data read from the underlying stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when count is negative.</exception>
        public override byte[] ReadBytes(uint byteCount)
        {
            return Reader.ReadBytes((int)byteCount);
        }

        /// <summary>
        /// Reads <paramref name="count"/> bytes from the current stream and advances the current position of the stream by <paramref name="count"/>-bytes.
        /// </summary>
        /// <param name="count">Numer of bytes to read.</param>
        /// <returns>A string.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override string ReadBytesToString(int count)
        {
            // https://github.com/dotnet/corefx/issues/10013
            return BitConverter.ToString(ReadBytes(count)).Replace("-", "");
        }

        /// <summary>
        /// Reads a string from the current stream. The string is prefixed with the length as an 4-byte signed integer.
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Containers/String.cpp#L1390
        /// </summary>
        /// <returns>A string read from this stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override string ReadFString()
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

        /// <summary>
        /// Reads 16 bytes from the current stream and advances the current position of the stream by 16-bytes.
        /// </summary>
        /// <returns>A GUID in string format read from this stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override string ReadGUID()
        {
            return ReadBytesToString(16);
        }

        /// <summary>
        /// Reads <paramref name="size"/> bytes from the current stream and advances the current position of the stream by <paramref name="size"/>-bytes.
        /// </summary>
        /// <param name="size">size</param>
        /// <returns>A GUID in string format read from this stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override string ReadGUID(int size)
        {
            return ReadBytesToString(size);
        }

        /// <summary>
        /// Reads a 2-byte signed integer from the current stream and advances the current position of the stream by two bytes.
        /// </summary>
        /// <returns>A 2-byte signed integer read from the current stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override short ReadInt16()
        {
            return Reader.ReadInt16();
        }

        /// <summary>
        /// Reads a 4-byte signed integer from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        /// <returns>A 4-byte signed integer read from the current stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override int ReadInt32()
        {
            return Reader.ReadInt32();
        }

        /// <summary>
        /// Reads a 4-byte signed integer from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        /// <returns>true if the integer is nonzero; otherwise, false.</returns>
        public override bool ReadInt32AsBoolean()
        {
            return ReadUInt32() >= 1;
        }

        /// <summary>
        /// Reads an 8-byte signed integer from the current stream and advances the current position of the stream by eight bytes.
        /// </summary>
        /// <returns>An 8-byte signed integer read from the current stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override long ReadInt64()
        {
            return Reader.ReadInt64();
        }

        /// <summary>
        /// Returns the byte at <see cref="Position"/> and advances the <see cref="Position"/> by 8 bits.
        /// </summary>
        /// <returns>The value of the byte at <see cref="Position"/> index.</returns>
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
                value += (uint)nextByte << (7 * count++);   // Add to total value
            }
            return value;
        }

        /// <summary>
        /// Reads a signed byte from this stream and advances the current position of the stream by one byte.
        /// </summary>
        /// <returns>A signed byte read from the current stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override sbyte ReadSByte()
        {
            return Reader.ReadSByte();
        }


        /// <summary>
        /// Reads a 4-byte floating point value from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        /// <returns>A 4-byte floating point value read from the current stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override float ReadSingle()
        {
            return Reader.ReadSingle();
        }

        /// <summary>
        /// Reads an array of tuples from the current stream. The array is prefixed with the number of items in it.
        /// see https://github.com/EpicGames/UnrealEngine/blob/7d9919ac7bfd80b7483012eab342cb427d60e8c9/Engine/Source/Runtime/Core/Public/Containers/Array.h#L1069
        /// </summary>
        /// <typeparam name="T">The type of the first value.</typeparam>
        /// <typeparam name="U">The type of the second value.</typeparam>
        /// <param name="func1">The function to parse the fist value.</param>
        /// <param name="func2">The function to parse the second value.</param>
        /// <returns>An array of tuples.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
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

        /// <summary>
        /// Reads a 2-byte unsigned integer from the current stream using little-endian encoding and advances the position of the stream by two bytes.
        /// </summary>
        /// <returns>A 2-byte unsigned integer read from this stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override ushort ReadUInt16()
        {
            return Reader.ReadUInt16();
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer from the current stream and advances the position of the stream by four bytes.
        /// </summary>
        /// <returns>A 4-byte unsigned integer read from this stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override uint ReadUInt32()
        {
            return Reader.ReadUInt32();
        }

        /// <summary>
        /// Reads a 4-byte signed integer from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        /// <returns>true if the integer is nonzero; otherwise, false.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override bool ReadUInt32AsBoolean()
        {
            return ReadUInt32() == 1u;
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer from the current stream and casts it to an Enum.
        /// Then advances the position of the stream by four bytes.
        /// </summary>
        ///  <typeparam name="T">The element type of the enum.</typeparam>
        /// <returns>A value of enum T.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override T ReadUInt32AsEnum<T>()
        {
            return (T)Enum.ToObject(typeof(T), ReadUInt32());
        }

        /// <summary>
        /// Reads an 8-byte unsigned integer from the current stream and advances the position of the stream by eight bytes.
        /// </summary>
        /// <returns>An 8-byte unsigned integer read from this stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override ulong ReadUInt64()
        {
            return Reader.ReadUInt64();
        }

        /// <summary>
        /// Set the position in the current stream, relative to SeekOrigin.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="seekOrigin"></param>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public override void Seek(int offset, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            Reader.BaseStream.Seek(offset, seekOrigin);
        }

        /// <summary>
        /// Advences the current stream by <paramref name="byteCount"/> bytes.
        /// </summary>
        /// <param name="byteCount"></param>
        public override void SkipBytes(uint byteCount)
        {
            Reader.BaseStream.Seek(byteCount, SeekOrigin.Current);
        }

        /// <summary>
        /// Advences the current stream by <paramref name="byteCount"/> bytes.
        /// </summary>
        /// <param name="byteCount"></param>
        public override void SkipBytes(int byteCount)
        {
            Reader.BaseStream.Seek(byteCount, SeekOrigin.Current);
        }
    }
}
