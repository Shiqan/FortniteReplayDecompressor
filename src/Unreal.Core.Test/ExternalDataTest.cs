using System.IO;
using Unreal.Core.Test.Mocks;
using Xunit;

namespace Unreal.Core.Test
{
    public class ExternalDataTest
    {
        [Fact]
        public void ReadExternalDataTest()
        {
            for (var i = 0; i < 5; i++)
            {
                var data = $"ExternalData/externaldata{i}.dump";
                using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var archive = new Unreal.Core.BinaryReader(stream);
                var reader = new MockReplayReader();
                reader.ReadExternalData(archive);
                Assert.True(archive.AtEnd());
            }
        }
    }
}
