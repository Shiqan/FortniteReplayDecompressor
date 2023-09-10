using Xunit;

namespace FortniteReplayReader.Test;

public class BranchTest
{
    [Fact]
    public void ParseBranchTest()
    {
        var reader = new FortniteReplayReader.ReplayReader
        {
            Branch = "++PUBG+Release-11.11"
        };
        Assert.Equal(0, reader.Major);
        Assert.Equal(0, reader.Minor);

        reader.Branch = "++Fortnite+Release-5.41";
        Assert.Equal(5, reader.Major);
        Assert.Equal(41, reader.Minor);

        reader.Branch = "++Fortnite+Release-7.10";
        Assert.Equal(7, reader.Major);
        Assert.Equal(10, reader.Minor);

        reader.Branch = "++Fortnite+Release-11.11";
        Assert.Equal(11, reader.Major);
        Assert.Equal(11, reader.Minor);

        reader.Branch = "++Fortnite+Release-999.999";
        Assert.Equal(999, reader.Major);
        Assert.Equal(999, reader.Minor);
    }
}
