using System;
using Xunit;

namespace Unreal.Core.Test
{
    public class BitReaderTest
    {
        byte[] expectedBytes;
        bool[] expectedBits;
        BitReader reader;

        public BitReaderTest()
        {
            expectedBits = new bool[] { true, true, false, false, false, true, false, false };
            expectedBytes = new byte[] { 35, 47, 215 };
            reader = new BitReader(expectedBytes);
        }

        [Fact]
        public void ReadBitTest()
        {
            for (var i = 0; i < expectedBits.Length; i++)
            {
                Assert.Equal(expectedBits[i], reader.ReadBit());
                Assert.Equal(i + 1, reader.Position);
            }
            Assert.Equal(8, reader.Position);
        }

        [Fact]
        public void ReadBitThrowsExceptionTest()
        {
            reader.Seek(24);
            reader.ReadBit();
            Assert.True(reader.IsError);
        }

        [Fact]
        public void PeekBitTest()
        {
            Assert.True(reader.PeekBit());
            Assert.Equal(0, reader.Position);
        }

        [Fact]
        public void SeekTest()
        {
            reader.Seek(8);
            Assert.Equal(8, reader.Position);
        }

        [Fact]
        public void SeekThrowsExceptionTest()
        {
            reader.Seek(25);
            Assert.True(reader.IsError);
            reader.Reset();

            reader.Seek(-1);
            Assert.True(reader.IsError);
            reader.Reset();

            reader.Seek(25, System.IO.SeekOrigin.End);
            Assert.True(reader.IsError);
            reader.Reset();

            reader.Seek(20);
            reader.Seek(10, System.IO.SeekOrigin.Current);
            Assert.True(reader.IsError);
        }

        [Fact]
        public void ReadByteTest()
        {
            for (var i = 0; i < expectedBytes.Length; i++)
            {
                Assert.Equal(expectedBytes[i], reader.ReadByte());
            }
            Assert.Equal(expectedBytes.Length * 8, reader.Position);
        }

        [Fact]
        public void ReadBytesTest()
        {
            Assert.Equal(expectedBytes, reader.ReadBytes(3));
            Assert.Equal(expectedBytes.Length * 8, reader.Position);
        }


        [Fact]
        public void ReadUInt32Test()
        {
            reader = new BitReader(new byte[] { 10, 58, 0, 0 });
            Assert.Equal(14858u, reader.ReadUInt32());
        }

        [Fact]
        public void ReadInt32Test()
        {
            reader = new BitReader(new byte[] { 10, 58, 0, 0 });
            Assert.Equal(14858, reader.ReadInt32());
        }

        [Fact]
        public void ReadPackedIntTest()
        {
            reader = new BitReader(new byte[] { 14 });
            Assert.Equal(7u, reader.ReadIntPacked());
            
            reader = new BitReader(new byte[] { 36, 64 });
            Assert.Equal(18u, reader.ReadIntPacked());
        }

        [Fact]
        public void ReadSerializedIntTest()
        {
            reader = new BitReader(new byte[] { 1 });
            Assert.Equal(1u, reader.ReadSerializedInt(3));
        }
        
        [Fact]
        public void ReadSerializedIntTest2()
        {
            reader = new BitReader(new byte[] { 0x64 });
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
    }
}
