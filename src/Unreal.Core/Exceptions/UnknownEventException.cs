using System;

namespace Unreal.Core.Exceptions;

public class UnknownEventException : Exception
{
    public UnknownEventException() : base() { }
    public UnknownEventException(string msg) : base(msg) { }
}
