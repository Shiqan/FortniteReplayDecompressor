using Unreal.Core.Models;
using Xunit;

namespace Unreal.Core.Test;

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
            Assert.Equal(expectedBits[0], reader.PeekBit());
        }
        Assert.Equal(0, reader.Position);
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

        Assert.Equal(expected, result);
        Assert.Equal(bitsToRead, reader.Position);
    }

    [Theory]
    [InlineData(new byte[] { 0b10011001 }, new byte[] { 0b1100 }, 1, 4)]
    [InlineData(new byte[] { 0b10011001 }, new byte[] { 0b10 }, 2, 2)]
    [InlineData(new byte[] { 0b10011001 }, new byte[] { 0b11 }, 3, 3)]
    public void ReadBitsWithOffsetTest(byte[] rawData, byte[] expected, int position, int bitsToRead)
    {
        var reader = new BitReader(rawData);
        reader.Seek(position);
        var result = reader.ReadBits(bitsToRead).ToArray();

        Assert.Equal(expected, result);
        Assert.Equal(bitsToRead + position, reader.Position);
    }

    [Theory]
    [InlineData(new byte[] { 0x23 }, new byte[] { 0x11 }, 5, 1)]
    [InlineData(new byte[] { 0x4A, 0xF1, 0xB2 }, new byte[] { 0x5E, 0x06 }, 11, 11)]
    [InlineData(new byte[] { 0xEE, 0x32, 0xC9 }, new byte[] { 0x0C }, 4, 20)]
    [InlineData(new byte[] { 0x58, 0x22, 0x38 }, new byte[] { 0x4B, 0x04 }, 16, 3)]
    [InlineData(new byte[] { 0x58, 0x22, 0x38 }, new byte[] { 0x4B, 0x04, 0x03 }, 18, 3)]
    [InlineData(new byte[] { 0x94, 0xA6, 0x7C, 0x0D }, new byte[] { 0xD2, 0x94, 0x2F }, 23, 3)]
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
    [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, 0x91, 1)]
    [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, 0x5C, 14)]
    [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, 0xF2, 4)]
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
    [InlineData(new byte[] { 0x23, 0x2F, 0xD7 }, new byte[] { 0x91 }, 1, 1)]
    [InlineData(new byte[] { 0x4E, 0xE5, 0x8A, 0x3F }, new byte[] { 0x2B, 0xFE }, 2, 14)]
    [InlineData(new byte[] { 0xAB, 0x46, 0x65, 0x72, 0x72, 0x6F, 0x6E, 0x99 }, new byte[] { 0x93, 0x93 }, 2, 21)]
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
    [InlineData(510, new byte[] { 0xFE, 0x01 })]
    [InlineData(16, new byte[] { 0x10, 0x00 })]
    public void ReadUInt16Test(ushort expected, byte[] rawData)
    {
        var reader = new BitReader(rawData);
        Assert.Equal(expected, reader.ReadUInt16());
    }

    [Theory]
    [InlineData(109, new byte[] { 0x6D, 0x00 })]
    [InlineData(-255, new byte[] { 0x01, 0xFF })]
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

    [Theory]
    [InlineData(14858u, new byte[] { 0x0A, 0x3A, 0x00, 0x00 })]
    [InlineData(73909u, new byte[] { 0xB5, 0x20, 0x01, 0x00 })]
    public void ReadUInt32Test(uint expected, byte[] rawData)
    {
        var reader = new BitReader(rawData);
        Assert.Equal(expected, reader.ReadUInt32());
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
    [InlineData(123456789123456789, new byte[] { 0x15, 0x5F, 0xD0, 0xAC, 0x4B, 0x9B, 0xB6, 0x01 })]
    [InlineData(420, new byte[] { 0xA4, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
    public void ReadUInt64Test(ulong expected, byte[] rawData)
    {
        var reader = new BitReader(rawData);
        Assert.Equal(expected, reader.ReadUInt64());
    }

    [Theory]
    [InlineData(123456789123456789, new byte[] { 0x15, 0x5F, 0xD0, 0xAC, 0x4B, 0x9B, 0xB6, 0x01 })]
    [InlineData(420, new byte[] { 0xA4, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
    [InlineData(-420, new byte[] { 0x5C, 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF })]
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

    [Theory]
    [InlineData(0u, new byte[] { 0x64 }, 3)]
    [InlineData(1u, new byte[] { 0x01 }, 2)]
    public void ReadSerializedIntTest(uint expected, byte[] rawData, int bitsToRead)
    {
        var reader = new BitReader(rawData);
        Assert.Equal(expected, reader.ReadSerializedInt(bitsToRead));
    }

    [Theory]
    [InlineData(new byte[] { 0x99, 0xF1 })]
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
    [InlineData(new byte[] { 0x70, 0x99, 0x7F, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F }, 0.998435020446777, 1, 1)]
    [InlineData(new byte[] { 0xD3, 0x89, 0x7F, 0x3F, 0xBB, 0x08, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F }, 0.99819678068161, 1.00026643276215, 1)]
    public void ReadVectorTest(byte[] rawData, float x, float y, float z)
    {
        var reader = new BitReader(rawData);
        var result = reader.ReadFVector();

        var expected = new FVector(x, y, z);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(new byte[] { 0xB4, 0xC5, 0x5C, 0xEF, 0x81, 0x33, 0x76, 0x33, 0x3F }, 10, 24, 176286, -167520, -2618)]
    [InlineData(new byte[] { 0x74, 0xF3, 0x74, 0xC7, 0xB4, 0x2D, 0x62, 0x51, 0x3F }, 10, 24, 181237, -172272, -2235)]
    [InlineData(new byte[] { 0x98, 0xE4, 0x52, 0x62, 0x07, 0x9A, 0x75, 0x70, 0x4F, 0xF9, 0x03 }, 100, 30, 179955, -181401, -2192)]
    [InlineData(new byte[] { 0x98, 0x5A, 0xF6, 0x63, 0x8C, 0x4B, 0x7A, 0x46, 0x08, 0xF8, 0x03 }, 100, 30, 188546, -175249, -2610)]
    [InlineData(new byte[] { 0x40, 0x05 }, 1, 24, 0, 0, 0)]
    public void ReadPackedVectorTest(byte[] rawData, int scaleFactor, int maxBits, float x, float y, float z)
    {
        var reader = new BitReader(rawData);
        var result = reader.ReadPackedVector(scaleFactor, maxBits);

        var expected = new FVector(x, y, z);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(new byte[] { 0x99, 0xF1 }, new byte[] { 0x21, 0xA1 })]
    [InlineData(new byte[] { 0x81, 0xEE, 0x7A, 0x00, 0x06 }, new byte[] { 0x84, 0xE3 })]
    public void AppendDataFromCheckedTest(byte[] rawData, byte[] rawData2)
    {
        var archive = new BitReader(rawData);
        Assert.Equal(rawData.Length * 8, archive.GetBitsLeft());

        archive.AppendDataFromChecked(rawData2, rawData2.Length * 8);
        Assert.Equal(rawData.Length * 8 + rawData2.Length * 8, archive.GetBitsLeft());
    }
}
