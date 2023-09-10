using Unreal.Core.Models;
using Xunit;

namespace Unreal.Core.Test;

public class FDateTimeTest
{
    [Theory]
    [InlineData(new byte[] {
        0xD0, 0xB7, 0x92, 0xAE, 0xCA, 0xB8, 0xD7, 0x08 }, 64)]
    [InlineData(new byte[] {
        0x00, 0x93, 0x7D, 0xA9, 0x76, 0x53, 0xD7, 0x08 }, 64)]
    public void DateTimeTest(byte[] rawData, int bitCount)
    {
        var reader = new NetBitReader(rawData, bitCount);
        var playerName = new FDateTime();
        playerName.Serialize(reader);

        Assert.True(reader.AtEnd());
        Assert.False(reader.IsError);
    }
}
