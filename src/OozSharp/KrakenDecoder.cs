using System;
using System.Buffers;

namespace OozSharp;

public class KrakenDecoder : IDisposable
{
    internal int SourceUsed { get; set; }
    internal int DestinationUsed { get; set; }
    internal KrakenHeader Header { get; set; }
    internal int ScratchSize { get; set; } = 0x6C000;
    internal byte[] Scratch { get; set; }

    public KrakenDecoder()
    {
        Scratch = ArrayPool<byte>.Shared.Rent(ScratchSize);

    }
    public void Dispose() => ArrayPool<byte>.Shared.Return(Scratch);
}
