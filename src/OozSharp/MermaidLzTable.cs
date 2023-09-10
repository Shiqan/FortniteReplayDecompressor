namespace OozSharp;

public unsafe struct MermaidLzTable
{
    // Flag stream. Format of flags:
    // Read flagbyte from |cmd_stream|
    // If flagbyte >= 24:
    //   flagbyte & 0x80 == 0 : Read from |off16_stream| into |recent_offs|.
    //                   != 0 : Don't read offset.
    //   flagbyte & 7 = Number of literals to copy first from |lit_stream|.
    //   (flagbyte >> 3) & 0xF = Number of bytes to copy from |recent_offs|.
    //
    //  If flagbyte == 0 :
    //    Read byte L from |length_stream|
    //    If L > 251: L += 4 * Read word from |length_stream|
    //    L += 64
    //    Copy L bytes from |lit_stream|.
    //
    //  If flagbyte == 1 :
    //    Read byte L from |length_stream|
    //    If L > 251: L += 4 * Read word from |length_stream|
    //    L += 91
    //    Copy L bytes from match pointed by next offset from |off16_stream|
    //
    //  If flagbyte == 2 :
    //    Read byte L from |length_stream|
    //    If L > 251: L += 4 * Read word from |length_stream|
    //    L += 29
    //    Copy L bytes from match pointed by next offset from |off32_stream|, 
    //    relative to start of block.
    //    Then prefetch |off32_stream[3]|
    //
    //  If flagbyte > 2:
    //    L = flagbyte + 5
    //    Copy L bytes from match pointed by next offset from |off32_stream|,
    //    relative to start of block.
    //    Then prefetch |off32_stream[3]|

    internal byte* CmdStream { get; set; }
    internal byte* CmdStreamEnd { get; set; }

    internal byte* LengthStream { get; set; }

    internal byte* LitStream { get; set; }
    internal byte* LitStreamEnd { get; set; }

    internal ushort* Offset16Stream { get; set; }
    internal ushort* Offset16StreamEnd { get; set; }

    internal uint* Offset32Stream { get; set; }
    internal uint* Offset32StreamEnd { get; set; }

    internal uint* Offset32Stream1 { get; set; }
    internal uint* Offset32Stream2 { get; set; }
    internal uint Offset32Stream1Size { get; set; }
    internal uint Offset32Stream2Size { get; set; }

    internal uint CmdStream2Offsets { get; set; }
    internal uint CmdStream2OffsetsEnd { get; set; }
}
