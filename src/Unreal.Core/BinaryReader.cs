using System;
using System.IO;
using System.Text;
using Unreal.Core.Models.Enums;

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

        public override bool CanRead(int count)
        {
            return Reader.BaseStream.Position + count < Reader.BaseStream.Length;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Reader.Dispose();
            }
        }

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
            return Reader.ReadBoolean();
        }

        public override byte ReadByte()
        {
            return Reader.ReadByte();
        }

        public override T ReadByteAsEnum<T>()
        {
            return (T)Enum.ToObject(typeof(T), ReadByte());
        }

        public override byte[] ReadBytes(int byteCount)
        {
            return Reader.ReadBytes(byteCount);
        }

        public override byte[] ReadBytes(uint byteCount)
        {
            return Reader.ReadBytes((int)byteCount);
        }

        public override string ReadBytesToString(int count)
        {
            // https://github.com/dotnet/corefx/issues/10013
            return BitConverter.ToString(ReadBytes(count)).Replace("-", "");
        }

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

        public override string ReadFName()
        {
            var isHardcoded = ReadBoolean();
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
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.h#L31
                // hard coded names in "UnrealNames.inl"
                // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.inl
                // https://github.com/EpicGames/UnrealEngine/blob/375ba9730e72bf85b383c07a5e4a7ba98774bcb9/Engine/Source/Runtime/Core/Public/UObject/NameTypes.h#L599
                // https://github.com/EpicGames/UnrealEngine/blob/375ba9730e72bf85b383c07a5e4a7ba98774bcb9/Engine/Source/Runtime/Core/Private/UObject/UnrealNames.cpp#L283
                return ((UnrealNames)nameIndex).ToString();
            }

            // https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.h#L17
            // MAX_NETWORKED_HARDCODED_NAME = 410

            // https://github.com/EpicGames/UnrealEngine/blob/375ba9730e72bf85b383c07a5e4a7ba98774bcb9/Engine/Source/Runtime/Core/Public/UObject/NameTypes.h#L34
            // NAME_SIZE = 1024

            // InName.GetComparisonIndex() <= MAX_NETWORKED_HARDCODED_NAME;
            // InName.GetPlainNameString();
            // InName.GetNumber();

            var inString = ReadFString();
            var inNumber = ReadInt32();
            return inString;
        }

        public override string ReadGUID()
        {
            return ReadBytesToString(16);
        }

        public override string ReadGUID(int size)
        {
            return ReadBytesToString(size);
        }

        public override short ReadInt16()
        {
            return Reader.ReadInt16();
        }

        public override int ReadInt32()
        {
            return Reader.ReadInt32();
        }

        public override bool ReadInt32AsBoolean()
        {
            return ReadUInt32() >= 1;
        }

        public override long ReadInt64()
        {
            return Reader.ReadInt64();
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
                value += (uint)nextByte << (7 * count++);   // Add to total value
            }
            return value;
        }

        public override sbyte ReadSByte()
        {
            return Reader.ReadSByte();
        }

        public override float ReadSingle()
        {
            return Reader.ReadSingle();
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
            return Reader.ReadUInt16();
        }

        public override uint ReadUInt32()
        {
            return Reader.ReadUInt32();
        }

        public override bool ReadUInt32AsBoolean()
        {
            return ReadUInt32() == 1u;
        }

        public override T ReadUInt32AsEnum<T>()
        {
            return (T)Enum.ToObject(typeof(T), ReadUInt32());
        }

        public override ulong ReadUInt64()
        {
            return Reader.ReadUInt64();
        }

        public override void Seek(int offset, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            Reader.BaseStream.Seek(offset, seekOrigin);
        }

        public override void SkipBytes(uint byteCount)
        {
            Reader.BaseStream.Seek(byteCount, SeekOrigin.Current);
        }

        public override void SkipBytes(int byteCount)
        {
            Reader.BaseStream.Seek(byteCount, SeekOrigin.Current);
        }
    }
}
