using FortniteReplayReader.Test.Mocks;
using Xunit;

namespace FortniteReplayReader.Test;

public class BranchTest
{
    [Theory]
    [InlineData("++PUBG+Release-11.11", 0, 0)]
    [InlineData("++Fortnite+Release-5.41", 5, 41)]
    [InlineData("++Fortnite+Release-7.10", 7, 10)]
    [InlineData("++Fortnite+Release-11.11", 11, 11)]
    [InlineData("++Fortnite+Release-999.999", 999, 999)]
    public void ParseBranchTest(string branch, int major, int minor)
    {
        var reader = new MockReplayReader
        {
            Branch = branch
        };
        Assert.Equal(major, reader.Major);
        Assert.Equal(minor, reader.Minor);
    }
}
