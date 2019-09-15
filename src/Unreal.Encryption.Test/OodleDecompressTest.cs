using System.IO;
using Xunit;

namespace Unreal.Encryption.Test
{
    public class OodleDecompressTest
    {
        [Fact]
        public void DecompressTest()
        {
            var data = @"CompressedChunk/compressed.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            var compressedBuffer = ms.ToArray();
            var decompressedSize = 441993;
            var compressedSize = 210703;
            var result = Oodle.DecompressReplayData(compressedBuffer, compressedSize, decompressedSize);
            Assert.Equal(decompressedSize, result.Length);
        }
    }
}
