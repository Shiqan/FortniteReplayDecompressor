using System.IO;
using Unreal.Core.Test.Mocks;
using Xunit;

namespace Unreal.Core.Test
{
    public class NetExportGuidsTest
    {
        [Fact]
        public void ReadNetExportGuidsTest()
        {
            for (var i = 0; i < 3; i++)
            {
                var data = $"NetExportGuids/NetExportGuid{i}.dump";
                using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var archive = new Unreal.Core.BinaryReader(stream);
                var reader = new MockReplayReader();
                reader.InternalLoadObject(archive);
                Assert.True(archive.AtEnd());
            }
        }
    }
}
