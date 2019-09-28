using System.IO;
using Unreal.Core.Test.Mocks;
using Xunit;

namespace Unreal.Core.Test
{
    public class StaticParseNameTest
    {
        [Fact]
        public void StaticParseNameBinaryTest()
        {
            for (var i = 0; i < 4; i++)
            {
                var data = $"StaticParseNames/staticparsename-{i}.dump";
                using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var archive = new Unreal.Core.BinaryReader(stream)
                {
                    EngineNetworkVersion = Models.Enums.EngineNetworkVersionHistory.HISTORY_FAST_ARRAY_DELTA_STRUCT
                };
                var reader = new MockReplayReader();
                reader.StaticParseName(archive);
                Assert.True(archive.AtEnd());
            }
        }

        [Fact]
        public void StaticParseNameBitTest()
        {
            var data = "StaticParseNames/staticparsename-bit-0.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            var archive = new Unreal.Core.BitReader(ms.ToArray())
            {
                EngineNetworkVersion = Models.Enums.EngineNetworkVersionHistory.HISTORY_FAST_ARRAY_DELTA_STRUCT
            };
            var reader = new MockReplayReader();
            var name = reader.StaticParseName(archive);
            Assert.Equal(9, archive.Position);
            Assert.Equal("Actor", name);
        }
    }
}
