using Unreal.Core.Models;
using Xunit;

namespace Unreal.Core.Test;

public class FGameplayEffectContextHandleTest
{
    [Theory(Skip = "doesnt work yet...")]
    [InlineData(new byte[] {
        0x83, 0x22, 0x87, 0x3A, 0x3C, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x20}, 110)]
    [InlineData(new byte[] {
        0x83, 0x5A, 0x87, 0x7C, 0x4A, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x20}, 110)]
    public void GameplayEffectContextHandleTest(byte[] rawData, int bitCount)
    {
        var reader = new NetBitReader(rawData, bitCount);
        var handle = new FGameplayEffectContextHandle();
        handle.Serialize(reader);

        Assert.True(reader.AtEnd());
        Assert.False(reader.IsError);
    }
}
