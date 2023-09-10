using OozSharp.Exceptions;

namespace OozSharp;

public unsafe class KrakenHeader
{
    public DecoderTypes DecoderType { get; set; }
    public bool RestartDecoder { get; set; }
    public bool Uncompressed { get; set; }
    public bool UseChecksums { get; set; }

    public KrakenHeader(byte* source)
    {
        var firstByte = source[0];
        var secondByte = source[1];

        if ((firstByte & 0xF) == 0xC)
        {
            if (((firstByte >> 4) & 3) != 0)
            {
                throw new DecoderException($"Failed to decode header. ((source[0] >> 4) & 3) != 0");
            }

            RestartDecoder = ((firstByte >> 7) & 0x1) == 0x01;
            Uncompressed = ((firstByte >> 6) & 0x1) == 0x01;

            DecoderType = (DecoderTypes) (secondByte & 0x7F);
            UseChecksums = ((secondByte >> 7) & 0x1) == 0x01;
        }
        else
        {
            throw new DecoderException($"Failed to decode header. (source[0] & 0xF) != 0xC");
        }
    }
}
