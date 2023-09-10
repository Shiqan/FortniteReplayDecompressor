using System.IO;
using Xunit;

namespace FortniteReplayReader.Test;

public class AthenaTeamStatsTest
{
    [Theory]
    [InlineData(new byte[] {
        0x00, 0x00, 0x00, 0x00, 0x23, 0x00, 0x00, 0x00, 0x60, 0x00, 0x00, 0x00
    }, 35u, 96u)]
    [InlineData(new byte[] {
        0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x63, 0x00, 0x00, 0x00
    }, 2u, 99u)]
    public void ParseAthenaTeamStatsTest(byte[] rawData, uint position, uint totalPlayers)
    {
        using var stream = new MemoryStream(rawData);
        using var archive = new Unreal.Core.BinaryReader(stream);
        var reader = new ReplayReader();
        var result = reader.ParseTeamStats(archive, null);

        Assert.True(archive.AtEnd());
        Assert.False(archive.IsError);
        Assert.Equal(position, result.Position);
        Assert.Equal(totalPlayers, result.TotalPlayers);
    }
}
