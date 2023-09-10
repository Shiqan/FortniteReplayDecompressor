using Unreal.Core.Models;
using Xunit;

namespace Unreal.Core.Test;

public class FGameplayAbilityRepAnimMontageTest
{
    [Theory]
    [InlineData(new byte[] {
        0x00, 0xB5, 0x86, 0x00, 0x00, 0x00, 0xF8, 0x23,
        0xA0, 0x89, 0xD8, 0x03, 0x00 }, 102)]
    [InlineData(new byte[] {
        0x00, 0xF5, 0xA6, 0x00, 0x00, 0x00, 0xF8, 0x23,
        0xB0, 0x72, 0xD0, 0x03, 0x00 }, 102)]
    [InlineData(new byte[] {
        0x00, 0xF5, 0xC2, 0x00, 0x00, 0x00, 0xF8, 0x23,
        0xA0, 0x89, 0xD8, 0x03, 0x00 }, 102)]
    public void GameplayAbilityRepAnimMontageTest(byte[] rawData, int bitCount)
    {
        var reader = new NetBitReader(rawData, bitCount);
        var montage = new FGameplayAbilityRepAnimMontage();
        montage.Serialize(reader);

        Assert.True(reader.AtEnd());
        Assert.False(reader.IsError);
    }
}
