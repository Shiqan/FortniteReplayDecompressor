using System;

namespace FortniteReplayReaderDecompressor.Core.Exceptions
{
    public class MalformedPacketException : Exception
    {
        public MalformedPacketException() : base() { }
        public MalformedPacketException(string msg) : base(msg) { }
        public MalformedPacketException(string msg, Exception exception) : base(msg, exception) { }
    }
}
