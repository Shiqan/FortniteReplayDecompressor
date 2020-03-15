using System;
using System.Collections.Generic;
using System.Text;

namespace OozSharp
{
    public class KrakenDecoder
    {
        internal int SourceUsed { get; set; }
        internal int DestinationUsed { get; set; }
        internal KrakenHeader Header { get; set; }
        internal int ScratchSize => Scratch.Length;
        internal byte[] Scratch { get; set; } = new byte[0x6C000];
    }
}
