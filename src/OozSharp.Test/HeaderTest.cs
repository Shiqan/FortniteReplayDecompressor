using OozSharp.Exceptions;
using Xunit;

namespace OozSharp.Test;

public unsafe class HeaderTest
{
    [Theory]
    [InlineData(new byte[] { 0x8C, 0x0A }, DecoderTypes.Mermaid)]
    [InlineData(new byte[] { 0x8C, 0x08 }, DecoderTypes.LZNA)]
    [InlineData(new byte[] { 0x8C, 0x0B }, DecoderTypes.BitKnit)]
    public void KrakenHeaderTest(byte[] rawData, DecoderTypes type)
    {
        fixed (byte* source = rawData)
        {
            var header = new KrakenHeader(source);
            Assert.Equal(type, header.DecoderType);
            Assert.True(header.RestartDecoder);
            Assert.False(header.Uncompressed);
            Assert.False(header.UseChecksums);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x2C, 0x0A }, "Failed to decode header. ((source[0] >> 4) & 3) != 0")]
    [InlineData(new byte[] { 0x8D, 0x0A }, "Failed to decode header. (source[0] & 0xF) != 0xC")]
    public void KrakenHeaderThrowsTest(byte[] rawData, string message)
    {
        fixed (byte* source = rawData)
        {
            var local = source;
            var exception = Assert.Throws<DecoderException>(() => new KrakenHeader(local));
            Assert.Equal(message, exception.Message);
        }
    }
}
