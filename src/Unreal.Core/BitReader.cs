using System;
using System.Collections;
using System.IO;
using System.Text;
using Unreal.Core.Extensions;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace Unreal.Core
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Serialization/BitArchive.h
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Serialization/BitArchive.cpp
    /// </summary>
    public class BitReader : FBitArchive
    {
        private BitArray Bits { get; set; }

        public override int Position { get; protected set; }

        public int LastBit { get; private set; }

        public override int MarkPosition { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the BitReader class based on the specified bytes.
        /// </summary>
        /// <param name="input">The input bytes.</param>
        public BitReader(byte[] input)
        {
            Bits = new BitArray(input);
            LastBit = Bits.Length;
        }

        /// <summary>
        /// Initializes a new instance of the BitReader class based on the specified bytes.
        /// </summary>
        /// <param name="input">The input bool[].</param>
        /// <param name="bitCount">Set last bit position.</param>
        public BitReader(byte[] input, int bitCount)
        {
            Bits = new BitArray(input);
            LastBit = bitCount;
        }

        /// <summary>
        /// Initializes a new instance of the BitReader class based on the specified bool[].
        /// </summary>
        /// <param name="input">The input bool[].</param>
        public BitReader(bool[] input)
        {
            Bits = new BitArray(input);
            LastBit = Bits.Length;
        }

        /// <summary>
        /// Initializes a new instance of the BitReader class based on the specified bool[].
        /// </summary>
        /// <param name="input">The input bool[].</param>
        /// <param name="bitCount">Set last bit position.</param>
        public BitReader(bool[] input, int bitCount)
        {
            Bits = new BitArray(input);
            LastBit = bitCount;
        }


        public override bool AtEnd()
        {
            return Position >= LastBit || Position >= Bits.Length;
        }

        public override bool CanRead(int count)
        {
            return Position + count <= LastBit || Position + count <= Bits.Length;
        }

        public override bool PeekBit()
        {
            return Bits[Position];
        }

        public override bool ReadBit()
        {
            if (AtEnd() || IsError)
            {
                IsError = true;
                return false;
            }
            return Bits[Position++];
        }

        public override T[] ReadArray<T>(Func<T> func1)
        {
            throw new NotImplementedException();
        }

        public override int ReadBitsToInt(int bitCount)
        {
            var result = new byte();
            for (var i = 0; i < bitCount; i++)
            {
                if (IsError)
                {
                    return 0;
                }

                if (ReadBit())
                {
                    result |= (byte)(1 << i);
                }
            }
            return (int)result;
        }

        public override bool[] ReadBits(int bitCount)
        {
            if (!CanRead(bitCount))
            {
                IsError = true;
                return Array.Empty<bool>();
            }

            var result = new bool[bitCount];
            for (var i = 0; i < bitCount; i++)
            {
                if (IsError)
                {
                    return Array.Empty<bool>();
                }
                result[i] = ReadBit();
            }
            return result;
        }

        public override bool[] ReadBits(uint bitCount)
        {
            return ReadBits((int)bitCount);
        }

        public override bool ReadBoolean()
        {
            return ReadBit();
        }

        public override byte PeekByte()
        {
            var result = ReadByte();
            Position -= 8;

            return result;
        }

        public override byte ReadByte()
        {
            var result = new byte();
            for (var i = 0; i < 8; i++)
            {
                if (ReadBit())
                {
                    result |= (byte)(1 << i);
                }
            }

            return result;
        }

        public override T ReadByteAsEnum<T>()
        {
            return (T)Enum.ToObject(typeof(T), ReadByte());
        }

        public override byte[] ReadBytes(int byteCount)
        {
            if (!CanRead(byteCount) || byteCount < 0)
            {
                IsError = true;
                return Array.Empty<byte>();
            }

            var result = new byte[byteCount];
            for (var i = 0; i < byteCount; i++)
            {
                if (IsError)
                {
                    return result;
                }

                result[i] = ReadByte();
            }
            return result;
        }

        public override byte[] ReadBytes(uint byteCount)
        {
            return ReadBytes((int)byteCount);
        }

        public override string ReadBytesToString(int count)
        {
            // https://github.com/dotnet/corefx/issues/10013
            return BitConverter.ToString(ReadBytes(count)).Replace("-", "");
        }

        public override string ReadFString()
        {
            var length = ReadInt32();

            if (!CanRead(length))
            {
                IsError = true;
                return "";
            }

            if (length == 0 || IsError)
            {
                return "";
            }

            string value;
            if (length < 0)
            {
                value = Encoding.Unicode.GetString(ReadBytes(-2 * length));
            }
            else
            {
                value = Encoding.Default.GetString(ReadBytes(length));
            }

            return value.Trim(new[] { ' ', '\0' });
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

                return ((UnrealNames)nameIndex).ToString();
            }

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
            var value = ReadBytes(2);
            return IsError ? (short)0 : BitConverter.ToInt16(value);
        }

        public override int ReadInt32()
        {
            var value = ReadBytes(4);
            return IsError ? 0 : BitConverter.ToInt32(value);
        }

        public override bool ReadInt32AsBoolean()
        {
            return ReadInt32() == 1;
        }

        public override long ReadInt64()
        {
            var value = ReadBytes(8);
            return IsError ? 0 : BitConverter.ToInt64(value);
        }

        public override uint ReadIntPacked()
        {
            int BitsUsed = (int)Position % 8;
            int BitsLeft = 8 - BitsUsed;
            int SourceMask0 = (1 << BitsLeft) - 1;
            int SourceMask1 = (1 << BitsUsed) - 1;

            uint value = 0;

            int OldPos = Position;

            int shift = 0;
            for (var it = 0; it < 5; it++)
            {
                if (IsError)
                {
                    return 0;
                }

                int currentBytePos = (int)Position / 8;
                int byteAlignedPositon = currentBytePos * 8;

                Position = byteAlignedPositon;

                byte currentByte = ReadByte();
                byte nextByte = currentByte;
                if (BitsUsed != 0)
                {
                    nextByte = (Position + 8 <= LastBit) ? PeekByte() : new byte();
                }

                OldPos += 8;

                int readByte = ((currentByte >> BitsUsed) & SourceMask0) | ((nextByte & SourceMask1) << (BitsLeft & 7));
                value = (uint)((readByte >> 1) << shift) | value;

                if ((readByte & 1) == 0)
                {
                    break;
                }
                shift += 7;
            }
            Position = OldPos;

            return value;
        }

        public override FVector ReadVector()
        {
            return new FVector(ReadSingle(), ReadSingle(), ReadSingle());
        }

        public override FVector ReadPackedVector(int scaleFactor, int maxBits)
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

            if (ReadBit()) // Pitch
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

        public override sbyte ReadSByte()
        {
            throw new NotImplementedException();
        }

        public override float ReadSingle()
        {
            return BitConverter.ToSingle(ReadBytes(4));
        }

        public override (T, U)[] ReadTupleArray<T, U>(Func<T> func1, Func<U> func2)
        {
            throw new NotImplementedException();
        }

        public override ushort ReadUInt16()
        {
            return BitConverter.ToUInt16(ReadBytes(2));
        }

        public override uint ReadUInt32()
        {
            return BitConverter.ToUInt32(ReadBytes(4));
        }

        public override bool ReadUInt32AsBoolean()
        {
            throw new NotImplementedException();
        }

        public override T ReadUInt32AsEnum<T>()
        {
            throw new NotImplementedException();
        }

        public override ulong ReadUInt64()
        {
            return BitConverter.ToUInt64(ReadBytes(8));
        }

        public override void Seek(int offset, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            if (offset < 0 || offset > Bits.Length || (seekOrigin == SeekOrigin.Current && offset + Position > Bits.Length))
            {
                IsError = true;
                return;
            }

            _ = (seekOrigin switch
            {
                SeekOrigin.Begin => Position = offset,
                SeekOrigin.End => Position = Bits.Length - offset,
                SeekOrigin.Current => Position += offset,
                _ => Position = offset,
            });
        }

        public override void SkipBytes(uint byteCount)
        {
            SkipBytes((int)byteCount);
        }

        public override void SkipBytes(int byteCount)
        {
            Seek(byteCount * 8, SeekOrigin.Current);
        }

        public override void SkipBits(int numbits)
        {
            Seek(numbits, SeekOrigin.Current);
        }

        public override void SkipBits(uint numbits)
        {
            SkipBits((int)numbits);
        }

        public override void Mark()
        {
            MarkPosition = Position;
        }

        public override void Pop()
        {
            // TODO: pop makes it sound like a list...
            Position = MarkPosition;
        }

        public override int GetBitsLeft()
        {
            return Bits.Length - Position;
        }

        public override void AppendDataFromChecked(bool[] data)
        {
            LastBit += data.Length;
            Bits = Bits.Append(data);
        }
    }
}
