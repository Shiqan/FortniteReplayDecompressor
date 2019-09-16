using FortniteReplayReader.Extensions;
using FortniteReplayReader.Models;
using System;
using System.IO;
using Xunit;

namespace FortniteReplayReader.Test
{
    public class AthenaTeamStatsTest
    {
        [Fact]
        public void AthenaTeamStats0Test()
        {
            var data = $"AthenaTeamStats/teamstats0.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream);
            var reader = new FortniteReplayReader();
            var result = reader.ParseTeamStats(archive, null);

            Assert.True(archive.AtEnd());
            Assert.Equal(35u, result.Position);
            Assert.Equal(96u, result.TotalPlayers);
        }

        [Fact]
        public void AthenaTeamStats1Test()
        {
            var data = $"AthenaTeamStats/teamstats1.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream);
            var reader = new FortniteReplayReader();
            var result = reader.ParseTeamStats(archive, null);

            Assert.True(archive.AtEnd());
            Assert.Equal(2u, result.Position);
            Assert.Equal(99u, result.TotalPlayers);
        }
    }
}
