using System;
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
        public void PeekBitTest(byte[] rawData, bool[] expectedBits)
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
        [InlineData(new byte[] { 0x23, 0x2F, 0xD7 })]
        public void ReadBytesTest(byte[] rawData)
        {
            var reader = new BitReader(rawData);
            Assert.Equal(rawData, reader.ReadBytes(3).ToArray());
            Assert.Equal(rawData.Length * 8, reader.Position);
        }


        [Fact]
        public void ReadUInt32Test()
        {
            var reader = new BitReader(new byte[] { 10, 58, 0, 0 });
            Assert.Equal(14858u, reader.ReadUInt32());
        }

        [Fact]
        public void ReadInt32Test()
        {
            var reader = new BitReader(new byte[] { 10, 58, 0, 0 });
            Assert.Equal(14858, reader.ReadInt32());
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
