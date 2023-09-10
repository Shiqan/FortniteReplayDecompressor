using OozSharp.Exceptions;
using Xunit;

namespace OozSharp.Test;

public unsafe class QuantumHeaderTest
{
    [Theory]
    [InlineData(new byte[] { 0x01, 0x89, 0x3A }, false, 100667u, 0u, 3)]
    [InlineData(new byte[] { 0x01, 0xDA, 0xDE }, false, 121567u, 0u, 3)]
    [InlineData(new byte[] { 0x07, 0xFF, 0xFF, 0x89 }, true, 0u, 137u, 4)]
    [InlineData(new byte[] { 0x01, 0xDA, 0xDE, 0x01, 0xDA, 0xDE }, true, 121567u, 121566u, 6)]
    public void KrakenQuantumHeaderTest(byte[] rawData, bool useChecksums, uint compressedSize, uint checksum, int read)
    {
        fixed (byte* source = rawData)
        {
            var header = new KrakenQuantumHeader(source, useChecksums, out var bytesRead);
            Assert.Equal(compressedSize, header.CompressedSize);
            Assert.Equal(checksum, header.Checksum);
            Assert.Equal(0, header.Flag1);
            Assert.Equal(0, header.Flag2);
            Assert.Equal(read, bytesRead);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x03, 0xFF, 0xFF }, 393294, "Failed to parse KrakenQuantumHeader")]
    public void KrakenQuantumHeaderThrowsTest(byte[] rawData, int decompressedSize, string message)
    {
        fixed (byte* source = rawData)
        {
            var local = source;
            var exception = Assert.Throws<DecoderException>(() => new KrakenQuantumHeader(local, false, out var bytesRead));
            Assert.Equal(message, exception.Message);
        }
    }
}
