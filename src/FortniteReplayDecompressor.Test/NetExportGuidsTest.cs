using FortniteReplayReaderDecompressor;
using System.IO;
using Xunit;

namespace FortniteReplayDecompressor.Test
{
    public class NetExportGuidsTest
    {
        [Fact]
        public void ReadNetExportGuidsTest()
        {
            var packet = @"NetExportGuids/NetExportGuid0.dump";
            using (var stream = File.Open(packet, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new FortniteBinaryDecompressor(stream))
                {
                    //reader.InternalLoadObject();
                    Assert.Equal(reader.BaseStream.Position, reader.BaseStream.Length);
                }
            }
        }

        [Fact]
        public void ReadNetExportGuids1Test()
        {
            var packet = @"NetExportGuids/NetExportGuid1.dump";
            using (var stream = File.Open(packet, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new FortniteBinaryDecompressor(stream))
                {
                    //reader.InternalLoadObject();
                    Assert.Equal(reader.BaseStream.Position, reader.BaseStream.Length);
                }
            }
        }

        [Fact]
        public void ReadNetExportGuids2Test()
        {
            var packet = @"NetExportGuids/NetExportGuid2.dump";
            using (var stream = File.Open(packet, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new FortniteBinaryDecompressor(stream))
                {
                    //reader.InternalLoadObject();
                    Assert.Equal(reader.BaseStream.Position, reader.BaseStream.Length);
                }
            }
        }
    }
}
