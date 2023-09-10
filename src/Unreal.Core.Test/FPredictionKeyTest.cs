using Unreal.Core.Models;
using Xunit;

namespace Unreal.Core.Test;

public class FPredictionKeyTest
{
    [Theory]
    [InlineData(new byte[] { 0x00 }, 2)]
    [InlineData(new byte[] { 0xF5, 0xF9, 0x02 }, 19)]
    public void PredictionKeyTest(byte[] rawData, int bitCount)
    {
        var reader = new NetBitReader(rawData, bitCount);
        var key = new FPredictionKey();
        key.Serialize(reader);

        Assert.True(reader.AtEnd());
        Assert.False(reader.IsError);
    }
}
