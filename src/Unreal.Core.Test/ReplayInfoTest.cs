using System.IO;
using Unreal.Core.Test.Mocks;
using Xunit;

namespace Unreal.Core.Test
{
    public class ReplayInfoTest
    {
        [Fact]
        public void ReadReplayInfoTest()
        {
            for (var i = 0; i < 2; i++)
            {
                var data = $"ReplayInfo/info{i}.dump";
                using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var archive = new Unreal.Core.BinaryReader(stream);
                var reader = new MockReplayReader();
                reader.ReadReplayInfo(archive);
                Assert.True(archive.AtEnd());
            }
        }
    }
}
