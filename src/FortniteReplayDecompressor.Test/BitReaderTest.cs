using FortniteReplayReaderDecompressor;
using FortniteReplayReaderDecompressor.Core;
using System;
using Xunit;

namespace FortniteReplayDecompressor.Test
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
            Assert.Throws<ArgumentOutOfRangeException>(() => reader.ReadBit());
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
            Assert.Throws<ArgumentOutOfRangeException>(() => reader.Seek(25));
            Assert.Throws<ArgumentOutOfRangeException>(() => reader.Seek(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => reader.Seek(25, System.IO.SeekOrigin.End));

            reader.Seek(20);
            Assert.Throws<ArgumentOutOfRangeException>(() => reader.Seek(10, System.IO.SeekOrigin.Current));
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
            reader = new BitReader(new byte[] { 10, 58, 0,  0 });
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
            // This version is bit compatible with FArchive::SerializeIntPacked, see CustomBinaryReader
            reader = new BitReader(new byte[] { 14 });
            Assert.Equal(7u, reader.ReadIntPacked());
        }

        [Fact]
        public void ReadIntMaxTest()
        {
            reader = new BitReader(new byte[] { 1 });
            Assert.Equal(1u, reader.ReadInt(3));
        }
    }
}
