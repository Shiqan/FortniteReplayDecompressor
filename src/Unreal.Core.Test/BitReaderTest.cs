using System;
using System.Collections;
using Xunit;

namespace Unreal.Core.Test
{
    public class BitReaderTest
    {
        [Theory]
        [InlineData(new byte[] { 0x23 }, new bool[] { true, true, false, false, false, true, false, false })]
        [InlineData(new byte[] { 0x2F }, new bool[] { true, true, true, true, false, true, false, false })]
        [InlineData(new byte[] { 0xD7 }, new bool[] { true, true, true, false, true, false, true, true })]
        public void ReadBitTest(byte[] rawData, bool[] expectedBits)
        {
            var reader = new BitReader(rawData);
            for (var i = 0; i < expectedBits.Length; i++)
            {
                Assert.Equal(expectedBits[i], reader.ReadBit());
                Assert.Equal(i + 1, reader.Position);
            }
            Assert.Equal(8, reader.Position);
        }

        [Theory]
        [InlineData(new byte[] { }, 0)]
        [InlineData(new byte[] { 0x35 }, 8)]
        public void ReadBitThrowsExceptionTest(byte[] rawData, int position)
        {
            var reader = new BitReader(rawData);
            reader.Seek(position);
            reader.ReadBit();
            Assert.True(reader.IsError);
        }

        [Theory]
        [InlineData(new byte[] { 0x23 }, new bool[] { true, true, false, false, false, true, false, false })]
        [InlineData(new byte[] { 0x3F }, new bool[] { true, true, true, true, true, true, false, false })]
        public void PeekBitTest(byte[] rawData, bool[] expectedBits)
        {
            var reader = new BitReader(rawData);
            for (var i = 0; i < expectedBits.Length; i++)
            {
                Assert.Equal(expectedBits[i], reader.PeekBit());
                Assert.Equal(i + 1, reader.Position);
            }
            Assert.Equal(8, reader.Position);
        }

        [Theory]
        [InlineData(new byte[] { 0x23 }, new byte[] { 0x23 }, 7)]
        [InlineData(new byte[] { 0x23 }, new byte[] { 0x03 }, 5)]
        [InlineData(new byte[] { 0x0D, 0x0A }, new byte[] { 0x0D }, 8)]
        [InlineData(new byte[] { 0x0D, 0x0A }, new byte[] { 0x0D, 0x0A }, 16)]
        public void ReadBitsTest(byte[] rawData, byte[] expected, int bitsToRead)
        {
            var reader = new BitReader(rawData);
            var result = reader.ReadBits(bitsToRead).ToArray();

            var t = new BitArray(rawData);

            Assert.Equal(expected, result);
            Assert.Equal(bitsToRead, reader.Position);
        }

        [Theory]
        [InlineData(new byte[] { 0x23 }, new byte[] { 0x11 }, 5, 1)]
        [InlineData(new byte[] { 0x4A, 0xF1, 0xB2 }, new byte[] { 0x5E, 0x00 }, 11, 11)]
        [InlineData(new byte[] { 0xEE, 0x32, 0xC9 }, new byte[] { 0x0C }, 4, 20)]
        public void ReadBitsMisAlignedTest(byte[] rawData, byte[] expected, int bitsToRead, int position)
        {
            var reader = new BitReader(rawData);
            reader.Seek(position);
            var result = reader.ReadBits(bitsToRead).ToArray();

            Assert.Equal(expected, result);
            Assert.Equal(bitsToRead + position, reader.Position);
        }

        [Theory]
        [InlineData(new byte[] { 0x23 }, 9)]
        [InlineData(new byte[] { 0x23 }, -1)]
        public void ReadBitsThrowsTest(byte[] rawData, int bitsToRead)
        {
            var reader = new BitReader(rawData);
            reader.ReadBits(bitsToRead);
            Assert.True(reader.IsError);
        }

        [Theory]
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, 8, 8)]
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, 6, 18, System.IO.SeekOrigin.End)]
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, 11, 11, System.IO.SeekOrigin.Current)]
        public void SeekTest(byte[] rawData, int seek, int expectedPosition, System.IO.SeekOrigin seekOrigin = System.IO.SeekOrigin.Begin)
        {
            var reader = new BitReader(rawData);
            reader.Seek(seek, seekOrigin);
            Assert.Equal(expectedPosition, reader.Position);
        }

        [Theory]
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, -1)]
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, 25)]
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, 25, System.IO.SeekOrigin.End)]
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, 25, System.IO.SeekOrigin.Current)]
        public void SeekThrowsExceptionTest(byte[] rawData, int position, System.IO.SeekOrigin seekOrigin = System.IO.SeekOrigin.Begin)
        {
            var reader = new BitReader(rawData);
            reader.Seek(position, seekOrigin);
            Assert.True(reader.IsError);
        }

        [Theory]
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 })]
        [InlineData(new byte[] { 0x5C, 0xF2, 0x0D, 0x0A, 0x3A })]
        public void ReadByteTest(byte[] rawData)
        {
            var reader = new BitReader(rawData);
            for (var i = 0; i < rawData.Length; i++)
            {
                Assert.Equal(rawData[i], reader.ReadByte());
            }
            Assert.Equal(rawData.Length * 8, reader.Position);
        }

        [Theory]
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, 0x46, 1)]
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, 0xF5, 14)]
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, 0x32, 4)]
        public void ReadByteMisalignedTest(byte[] rawData, byte expected, int bitPosition)
        {
            var reader = new BitReader(rawData);
            reader.Seek(bitPosition);

            Assert.Equal(expected, reader.ReadByte());
            Assert.Equal(bitPosition + 8, reader.Position);
        }

        [Theory]
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 })]
        [InlineData(new byte[] { 0xAB, 0x46, 0x65 })]
        [InlineData(new byte[] { 0x72, 0x6F, 0x99 })]
        public void ReadBytesTest(byte[] rawData)
        {
            var reader = new BitReader(rawData);
            Assert.Equal(rawData, reader.ReadBytes(3).ToArray());
            Assert.Equal(rawData.Length * 8, reader.Position);
        }

        [Theory]
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, new byte[] { 0x46 }, 1, 1)]
        [InlineData(new byte[] { 0x4E, 0xE5, 0x8A, 0x3F }, new byte[] { 0x62, 0x8F }, 2, 14)]
        [InlineData(new byte[] { 0xAB, 0x46, 0x65, 0x72, 0x72, 0x6F, 0x6E, 0x99 }, new byte[] { 0xAE, 0x4E }, 2, 21)]
        public void ReadBytesMisalignedTest(byte[] rawData, byte[] expected, int bytesToRead, int bitPosition)
        {
            var reader = new BitReader(rawData);
            reader.Seek(bitPosition);

            Assert.Equal(expected, reader.ReadBytes(bytesToRead).ToArray());
            Assert.Equal(bitPosition + (bytesToRead * 8), reader.Position);
        }


        [Theory]
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, 1)]
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, 14)]
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, -1)]
        public void ReadBytesThrowsTest(byte[] rawData, int bitPosition)
        {
            var reader = new BitReader(rawData);
            reader.Seek(bitPosition);
            reader.ReadBytes(3);
            Assert.True(reader.IsError);
        }

        [Theory]
        [InlineData(109, new byte[] { 0x6D, 0x00, 0x00, 0x00 })]
        [InlineData(14858, new byte[] { 0x0A, 0x3A, 0x00, 0x00 })]
        [InlineData(420, new byte[] { 0xA4, 0x01, 0x00, 0x00 })]
        [InlineData(-420, new byte[] { 0x5C, 0xFE, 0xFF, 0xFF })]
        public void ReadUInt16Test(ushort expected, byte[] rawData)
        {
            var reader = new BitReader(rawData);
            Assert.Equal(expected, reader.ReadUInt16());
        }
        
        [Theory]
        [InlineData(109, new byte[] { 0x6D, 0x00, 0x00, 0x00 })]
        [InlineData(14858, new byte[] { 0x0A, 0x3A, 0x00, 0x00 })]
        [InlineData(420, new byte[] { 0xA4, 0x01, 0x00, 0x00 })]
        [InlineData(-420, new byte[] { 0x5C, 0xFE, 0xFF, 0xFF })]
        public void ReadInt16Test(short expected, byte[] rawData)
        {
            var reader = new BitReader(rawData);
            Assert.Equal(expected, reader.ReadInt16());
        }

        [Theory]
        [InlineData(14858u, new byte[] { 0x0A, 0x3A, 0x00, 0x00 })]
        [InlineData(12345u, new byte[] { 0x39, 0x30, 0x00, 0x00 })]
        [InlineData(420u, new byte[] { 0xA4, 0x01, 0x00, 0x00 })]
        public void ReadBitsToIntTest(uint expected, byte[] rawData)
        {
            var reader = new BitReader(rawData);
            Assert.Equal(expected, reader.ReadUInt32());
        }
        
        [Fact]
        public void ReadUInt32Test()
        {
            var reader = new BitReader(new byte[] { 10, 58, 0, 0 });
            Assert.Equal(14858u, reader.ReadUInt32());
        }

        [Theory]
        [InlineData(true, new byte[] { 0x01, 0x00, 0x00, 0x00 })]
        [InlineData(false, new byte[] { 0x00, 0x00, 0x00, 0x00 })]
        public void ReadInt32AsBooleanTest(bool expected, byte[] rawData)
        {
            var reader = new BitReader(rawData);
            Assert.Equal(expected, reader.ReadInt32AsBoolean());
        }
        
        [Theory]
        [InlineData(109, new byte[] { 0x6D, 0x00, 0x00, 0x00 })]
        [InlineData(14858, new byte[] { 0x0A, 0x3A, 0x00, 0x00 })]
        [InlineData(420, new byte[] { 0xA4, 0x01, 0x00, 0x00 })]
        [InlineData(-420, new byte[] { 0x5C, 0xFE, 0xFF, 0xFF })]
        public void ReadInt32Test(int expected, byte[] rawData)
        {
            var reader = new BitReader(rawData);
            Assert.Equal(expected, reader.ReadInt32());
        }
        
        [Theory]
        [InlineData(new byte[] { 0xE7 })]
        [InlineData(new byte[] { 0x6D, 0x00, 0x00 })]
        public void ReadInt32ThrowsTest(byte[] rawData)
        {
            var reader = new BitReader(rawData);
            Assert.Equal(0, reader.ReadInt32());
            Assert.True(reader.IsError);
        }

        [Theory]
        [InlineData(109, new byte[] { 0x6D, 0x00, 0x00, 0x00 })]
        [InlineData(14858, new byte[] { 0x0A, 0x3A, 0x00, 0x00 })]
        [InlineData(420, new byte[] { 0xA4, 0x01, 0x00, 0x00 })]
        [InlineData(-420, new byte[] { 0x5C, 0xFE, 0xFF, 0xFF })]
        public void ReadUInt64Test(ulong expected, byte[] rawData)
        {
            var reader = new BitReader(rawData);
            Assert.Equal(expected, reader.ReadUInt64());
        }

        [Theory]
        [InlineData(109, new byte[] { 0x6D, 0x00, 0x00, 0x00 })]
        [InlineData(14858, new byte[] { 0x0A, 0x3A, 0x00, 0x00 })]
        [InlineData(420, new byte[] { 0xA4, 0x01, 0x00, 0x00 })]
        [InlineData(-420, new byte[] { 0x5C, 0xFE, 0xFF, 0xFF })]
        public void ReadInt64Test(long expected, byte[] rawData)
        {
            var reader = new BitReader(rawData);
            Assert.Equal(expected, reader.ReadInt64());
        }

        [Theory]
        [InlineData(7u, new byte[] { 0x0E })]
        [InlineData(18u, new byte[] { 0x24, 0x40 })]
        [InlineData(102u, new byte[] { 0xCC })]
        public void ReadPackedIntTest(uint expected, byte[] rawData)
        {
            var reader = new BitReader(rawData);
            Assert.Equal(expected, reader.ReadIntPacked());
        }

        [Fact]
        public void ReadSerializedIntTest()
        {
            var reader = new BitReader(new byte[] { 1 });
            Assert.Equal(1u, reader.ReadSerializedInt(3));
        }

        [Fact]
        public void ReadSerializedIntTest2()
        {
            var reader = new BitReader(new byte[] { 0x64 });
            Assert.Equal(0u, reader.ReadSerializedInt(2));
            Assert.Equal(1, reader.Position);
        }

        [Theory]
        [InlineData(new byte[] {
            0x99, 0xF1
        })]
        public void StaticParseNameTest(byte[] rawData)
        {
            var archive = new BitReader(rawData)
            {
                EngineNetworkVersion = Models.Enums.EngineNetworkVersionHistory.HISTORY_FAST_ARRAY_DELTA_STRUCT
            };
            var name = archive.ReadFName();
            Assert.Equal(9, archive.Position);
            Assert.Equal("Actor", name);
            Assert.False(archive.IsError);
        }

        [Theory]
        [InlineData(new byte[] { })]
        public void ReadVectorTest(byte[] rawData)
        {

        }

        [Theory]
        [InlineData(new byte[] { })]
        public void ReadPackedVectorTest(byte[] rawData)
        {

        }

        [Theory]
        [InlineData(new byte[] { 0x99, 0xF1 }, new byte[] { 0x99, 0xF1 })]
        public void AppendDataFromCheckedTest(byte[] rawData, byte[] rawData2)
        {
            var archive = new BitReader(rawData);
            Assert.Equal(16, archive.GetBitsLeft());

            archive.AppendDataFromChecked(rawData2);
            Assert.Equal(32, archive.GetBitsLeft());
        }
    }
}
