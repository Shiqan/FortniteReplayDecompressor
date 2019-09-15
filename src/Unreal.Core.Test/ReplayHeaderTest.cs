using System.IO;
using Unreal.Core.Test.Mocks;
using Xunit;

namespace Unreal.Core.Test
{
    public class ReplayHeaderTest
    {
        [Fact]
        public void ReadReplayHeader()
        {
            for (var i = 0; i < 2; i++)
            {
                var data = $"ReplayHeader/header{i}.dump";
                using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var archive = new Unreal.Core.BinaryReader(stream);
                var reader = new MockReplayReader();
                reader.ReadReplayHeader(archive);
                Assert.True(archive.AtEnd());
            }
        }
    }
}
