using OozSharp.Exceptions;
using System.IO;
using Xunit;

namespace Unreal.Encryption.Test;

public class OodleDecompressTest
{
    [Theory]
    [InlineData(@"CompressedChunk/compressed-chunk-0.dump", 405273)]
    [InlineData(@"CompressedChunk/compressed-chunk-1.dump", 393295)]
    public void DecompressTest(string data, int decompressedSize)
    {
        using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        var compressedBuffer = ms.ToArray();
        var result = Oodle.DecompressReplayData(compressedBuffer, decompressedSize);
        Assert.Equal(decompressedSize, result.Length);
    }

    [Theory]
    [InlineData(@"CompressedChunk/compressed-chunk-0.dump", 305273)]
    [InlineData(@"CompressedChunk/compressed-chunk-1.dump", 393294)]
    public void DecompressThrows(string data, int decompressedSize)
    {
        using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        var compressedBuffer = ms.ToArray();

        Assert.Throws<DecoderException>(() => Oodle.DecompressReplayData(compressedBuffer, decompressedSize));
    }
}
