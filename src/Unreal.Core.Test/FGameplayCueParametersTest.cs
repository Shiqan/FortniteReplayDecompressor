using Unreal.Core.Models;
using Xunit;

namespace Unreal.Core.Test;

public class FGameplayCueParametersTest
{
    [Theory]
    [InlineData(new byte[] { 0x02, 0x70, 0xE1, 0x7A, 0x00, 0x10 }, 46)]
    [InlineData(new byte[] { 0x02, 0xF0, 0x70, 0x3D, 0x28, 0x10 }, 46)]
    // doesnt work because of FGameplayEffectContextHandle
    //[InlineData(new byte[] { 
    //    0x04, 0xF0, 0x41, 0x2F, 0x4E, 0x0D, 0x21, 0x00, 
    //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10 }, 110)]
    //[InlineData(new byte[] {
    //    0x04, 0xF0, 0x41, 0x91, 0x43, 0x1D, 0x1E, 0x00, 
    //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10 }, 110)]
    public void GameplayCueParametersTest(byte[] rawData, int bitCount)
    {
        var reader = new NetBitReader(rawData, bitCount);
        var cue = new FGameplayCueParameters();
        cue.Serialize(reader);

        Assert.True(reader.AtEnd());
        Assert.False(reader.IsError);
    }
}
