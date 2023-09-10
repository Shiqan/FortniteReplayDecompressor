using System.IO;
using Xunit;

namespace OozSharp.Test;

public class DecompressTest
{
    [Theory]
    [InlineData(@"CompressedChunk/mermaid-fortnite.dump", 405273)]
    [InlineData(@"CompressedChunk/mermaid-fortnite2.dump", 262151)]
    public void MermaidTest(string data, int expectedSize)
    {
        using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        var rawData = ms.ToArray();

        var kraken = new Kraken();
        var result = kraken.Decompress(rawData, expectedSize);
        Assert.Equal(expectedSize, result.Length);
    }
}
