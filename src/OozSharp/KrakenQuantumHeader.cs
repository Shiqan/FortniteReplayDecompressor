using OozSharp.Exceptions;

namespace OozSharp;

public unsafe class KrakenQuantumHeader
{
    public uint CompressedSize { get; set; }
    public uint Checksum { get; set; }
    public byte Flag1 { get; set; }
    public byte Flag2 { get; set; }

    // Whether the whole block matched a previous block
    public uint WholeMatchDistance { get; set; }

    public KrakenQuantumHeader(byte* source, bool useChecksums, out int bytesRead)
    {
        var v = (uint) ((source[0] << 16) | (source[1] << 8) | source[2]);
        var size = v & 0x3FFFF;

        if (size != 0x3FFFF)
        {
            CompressedSize = size + 1;
            Flag1 = (byte) ((v >> 18) & 1);
            Flag2 = (byte) ((v >> 19) & 1);

            if (useChecksums)
            {
                Checksum = (uint) ((source[3] << 16) | (source[4] << 8) | source[5]);

                bytesRead = 6;
            }
            else
            {
                bytesRead = 3;
            }

            return;
        }

        v >>= 18;

        if (v == 1)
        {
            Checksum = source[3];
            CompressedSize = 0;
            WholeMatchDistance = 0;

            bytesRead = 4;

            return;
        }

        throw new DecoderException($"Failed to parse KrakenQuantumHeader");
    }
}
