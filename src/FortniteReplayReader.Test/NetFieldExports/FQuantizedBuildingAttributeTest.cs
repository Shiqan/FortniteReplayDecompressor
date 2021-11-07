using FortniteReplayReader.Models.NetFieldExports;
using Unreal.Core;
using Xunit;

namespace FortniteReplayReader.Test;

public class FQuantizedBuildingAttributeTest
{
    [Theory(Skip = "doesnt work yet...")]
    [InlineData(new byte[] { 0x88, 0x88 }, 16)]
    public void BuildingAttributeTest(byte[] rawData, int bitCount)
    {
        var reader = new NetBitReader(rawData, bitCount);
        var attr = new FQuantizedBuildingAttribute();
        attr.Serialize(reader);

        Assert.True(reader.AtEnd());
        Assert.False(reader.IsError);
    }
}
