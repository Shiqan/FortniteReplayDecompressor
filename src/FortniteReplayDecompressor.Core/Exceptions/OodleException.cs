using System;

namespace FortniteReplayReaderDecompressor.Core.Exceptions
{
    public class OodleException : Exception
    {
        public OodleException() : base() { }
        public OodleException(string msg) : base(msg) { }
    }
}
