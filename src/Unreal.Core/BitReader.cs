using System;
using System.IO;
using System.Text;
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
        private ReadOnlyMemory<byte> Buffer { get; set; }

        public override int Position { get; protected set; }

        private int CurrentByte => Position >> 3;

        public int LastBit { get; private set; }

        public override int MarkPosition { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the BitReader class based on the specified bytes.
        /// </summary>
        /// <param name="input">The input bytes.</param>
        public BitReader(ReadOnlySpan<byte> input)
        {
            Buffer = input.ToArray();
            LastBit = Buffer.Length * 8;
        }

        /// <summary>
        /// Initializes a new instance of the BitReader class based on the specified bytes.
        /// </summary>
        /// <param name="input">The input bool[].</param>
        /// <param name="bitCount">Set last bit position.</param>
        public BitReader(ReadOnlySpan<byte> input, int bitCount)
        {
            Buffer = input.ToArray();
            LastBit = bitCount;
        }


        public override bool AtEnd()
        {
            return Position >= LastBit;
        }

        public override bool CanRead(int count)
        {
            return Position + count <= LastBit;
        }

        public override bool PeekBit()
        {
            return (Buffer.Span[CurrentByte] & (1 << (Position & 7))) > 0;
        }

        public override bool ReadBit()
        {
            if (AtEnd() || IsError)
            {
                IsError = true;
                return false;
            }

            var result = (Buffer.Span[CurrentByte] & (1 << (Position & 7))) > 0;
            Position++;
            return result;
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
            return result;
        }

        public override ReadOnlySpan<byte> ReadBits(int bitCount)
        {
            if (!CanRead(bitCount) || bitCount < 0)
            {
                IsError = true;
                return ReadOnlySpan<byte>.Empty;
            }

            var bitCountUsedInByte = Position & 7;
            if (bitCountUsedInByte == 0 && bitCount % 8 == 0)
            {
                return ReadBytes(bitCount >> 3);
            }

            Span<byte> result = new byte[((bitCount + 7) / 8)];

            var bitCountLeftInByte = 8 - (Position & 7);
            var byteCount = bitCount / 8;
            for (var i = 0; i < byteCount; i++)
            {
                result[i] = (byte)((Buffer.Span[CurrentByte + i] >> bitCountUsedInByte) | ((Buffer.Span[CurrentByte + 1 + i] & ((1 << bitCountUsedInByte) - 1)) << bitCountLeftInByte));
            }
            Position += (byteCount * 8);

            bitCount %= 8;
            for (var i = 0; i < bitCount; i++)
            {
                if (ReadBit())
                {
                    result[^1] |= (byte)(1 << i);
                }
            }

            return result;
        }


        public override ReadOnlySpan<byte> ReadBits(uint bitCount)
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
            var bitCountUsedInByte = Position & 7;
            var bitCountLeftInByte = 8 - (Position & 7);

            var result = (bitCountUsedInByte == 0) ? Buffer.Span[CurrentByte] : (byte)((Buffer.Span[CurrentByte] >> bitCountUsedInByte) | ((Buffer.Span[CurrentByte + 1] & ((1 << bitCountUsedInByte) - 1)) << bitCountLeftInByte));

            Position += 8;
            return result;
        }

        public override T ReadByteAsEnum<T>()
        {
            return (T)Enum.ToObject(typeof(T), ReadByte());
        }

        public override ReadOnlySpan<byte> ReadBytes(int byteCount)
        {
            if (!CanRead(byteCount * 8) || byteCount < 0)
            {
                IsError = true;
                return Span<byte>.Empty;
            }

            var bitCountUsedInByte = Position & 7;
            var bitCountLeftInByte = 8 - (Position & 7);
            ReadOnlySpan<byte> result;
            if (bitCountUsedInByte == 0)
            {
                result = Buffer.Span[CurrentByte..(CurrentByte + byteCount)];
            }
            else
            {
                Span<byte> output = new byte[byteCount];
                for (var i = 0; i < byteCount; i++)
                {
                    output[i] = (byte)((Buffer.Span[CurrentByte + i] >> bitCountUsedInByte) | ((Buffer.Span[CurrentByte + 1 + i] & ((1 << bitCountUsedInByte) - 1)) << bitCountLeftInByte));
                }
                result = output;
            }

            Position += (byteCount * 8);
            return result;
        }

        public override ReadOnlySpan<byte> ReadBytes(uint byteCount)
        {
            return ReadBytes((int)byteCount);
        }

        public override string ReadBytesToString(int count)
        {
            // https://github.com/dotnet/corefx/issues/10013
            return Encoding.Default.GetString(ReadBytes(count)).Replace("-", "");
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
            //var BitsUsed = Position % 8;
            //var BitsLeft = 8 - BitsUsed;
            //var SourceMask0 = (1 << BitsLeft) - 1;
            //var SourceMask1 = (1 << BitsUsed) - 1;

            //uint value = 0;

            //var OldPos = Position;

            //var shift = 0;
            //for (var it = 0; it < 5; it++)
            //{
            //    if (IsError)
            //    {
            //        return 0;
            //    }

            //    var currentBytePos = Position / 8;
            //    var byteAlignedPositon = currentBytePos * 8;

            //    Position = byteAlignedPositon;

            //    var currentByte = ReadByte();
            //    var nextByte = currentByte;
            //    if (BitsUsed != 0)
            //    {
            //        nextByte = (Position + 8 <= LastBit) ? PeekByte() : new byte();
            //    }

            //    OldPos += 8;

            //    var readByte = ((currentByte >> BitsUsed) & SourceMask0) | ((nextByte & SourceMask1) << (BitsLeft & 7));
            //    value = (uint)((readByte >> 1) << shift) | value;

            //    if ((readByte & 1) == 0)
            //    {
            //        break;
            //    }
            //    shift += 7;
            //}
            //Position = OldPos;

            //return value;



            var bitCountUsedInByte = Position & 7;
            var bitCountLeftInByte = 8 - (Position & 7);
            var srcMaskByte0 = (byte)((1U << bitCountLeftInByte) - 1U);
            var srcMaskByte1 = (byte)((1U << bitCountUsedInByte) - 1U);
            var srcIndex = CurrentByte;
            var nextSrcIndex = bitCountUsedInByte != 0 ? srcIndex + 1 : srcIndex;

            uint value = 0;
            for (int It = 0, shiftCount = 0; It < 5; ++It, shiftCount += 7)
            {
                if (!CanRead(8))
                {
                    IsError = true;
                    break;
                }

                if (nextSrcIndex >= Buffer.Length)
                {
                    nextSrcIndex = srcIndex;
                }

                Position += 8;

                var readByte = (byte)(((Buffer.Span[srcIndex] >> bitCountUsedInByte) & srcMaskByte0) | ((Buffer.Span[nextSrcIndex] & srcMaskByte1) << (bitCountLeftInByte & 7)));
                value = (uint)((readByte >> 1) << shiftCount) | value;
                srcIndex++;
                nextSrcIndex++;

                if ((readByte & 1) == 0)
                {
                    break;
                }
            }

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
            if (offset < 0 || offset >> 3 > Buffer.Length || (offset >> 3 == Buffer.Length && (offset & 7) > 0) || (seekOrigin == SeekOrigin.Current && offset + Position > (Buffer.Length * 8)))
            {
                IsError = true;
                return;
            }

            _ = (seekOrigin switch
            {
                SeekOrigin.Begin => Position = offset,
                SeekOrigin.End => Position = (Buffer.Length * 8) - offset,
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
            Position = MarkPosition;
        }

        public override int GetBitsLeft()
        {
            return LastBit - Position;
        }

        public override void AppendDataFromChecked(ReadOnlySpan<byte> data, int bitCount)
        {
            LastBit += bitCount;

            // this works only because partial bunches are enforced to be byte aligned
            var combined = new byte[Buffer.Span.Length + data.Length];
            Buffer.CopyTo(combined);
            data.ToArray().CopyTo(combined, Buffer.Span.Length);

            Buffer = combined;
        }
    }
}
