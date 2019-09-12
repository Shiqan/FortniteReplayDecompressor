using System;

namespace Unreal.Encryption.Exceptions
{
    public class OodleException : Exception
    {
        public OodleException() : base() { }
        public OodleException(string msg) : base(msg) { }
    }
}
