using System.IO;
using Unreal.Core.Test.Mocks;
using Xunit;

namespace Unreal.Core.Test
{
    public class StaticParseNameTest
    {
        [Theory]
        [InlineData(new byte[] {
            0x00, 0x06, 0x00, 0x00, 0x00, 0x4C, 0x65, 0x76,
            0x65, 0x6C, 0x00, 0x00, 0x00, 0x00, 0x00
        })]
        [InlineData(new byte[] {
            0x01, 0xAF, 0x02
        })]
        [InlineData(new byte[] {
            0x00, 0xE9, 0xFF, 0xFF, 0xFF, 0x57, 0x00, 0x61,
            0x00, 0x73, 0x00, 0x50, 0x00, 0x61, 0x00, 0x72,
            0x00, 0x74, 0x00, 0x52, 0x00, 0x65, 0x00, 0x70,
            0x00, 0x6C, 0x00, 0x69, 0x00, 0x63, 0x00, 0x61,
            0x00, 0x74, 0x00, 0x65, 0x00, 0x64, 0x00, 0x46,
            0x00, 0x6C, 0x00, 0x61, 0x00, 0x67, 0x00, 0x73,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        })]
        public void StaticParseNameBinaryTest(byte[] rawData)
        {
            using var stream = new MemoryStream(rawData);
            using var archive = new Unreal.Core.BinaryReader(stream)
            {
                EngineNetworkVersion = Models.Enums.EngineNetworkVersionHistory.HISTORY_FAST_ARRAY_DELTA_STRUCT
            };
            var reader = new MockReplayReader();
            reader.StaticParseName(archive);
            Assert.True(archive.AtEnd());
            Assert.False(archive.IsError);
        }

        [Theory]
        [InlineData(new byte[] {
            0x99, 0xF1
        })]
        public void StaticParseNameBitTest(byte[] rawData)
        {
            var archive = new Unreal.Core.BitReader(rawData)
            {
                EngineNetworkVersion = Models.Enums.EngineNetworkVersionHistory.HISTORY_FAST_ARRAY_DELTA_STRUCT
            };
            var reader = new MockReplayReader();
            var name = reader.StaticParseName(archive);
            Assert.Equal(9, archive.Position);
            Assert.Equal("Actor", name);
            Assert.False(archive.IsError);
        }
    }
}
