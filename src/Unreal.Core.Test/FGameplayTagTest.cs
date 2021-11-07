using Unreal.Core.Models;
using Xunit;

namespace Unreal.Core.Test;

public class FGameplayTagTest
{
    [Theory]
    [InlineData(new byte[] { 0xFB, 0x80 }, 16)]
    [InlineData(new byte[] { 0xAD, 0x8A }, 16)]
    public void GameplayTagTeste(byte[] rawData, int bitCount)
    {
        var reader = new NetBitReader(rawData, bitCount);
        var tag = new FGameplayTag();
        tag.Serialize(reader);

        Assert.True(reader.AtEnd());
        Assert.False(reader.IsError);
    }
}
