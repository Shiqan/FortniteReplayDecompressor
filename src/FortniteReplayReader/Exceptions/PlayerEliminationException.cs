using System;

namespace FortniteReplayReader.Exceptions;

public class PlayerEliminationException : Exception
{
    public PlayerEliminationException() : base() { }
    public PlayerEliminationException(string msg) : base(msg) { }
    public PlayerEliminationException(string msg, Exception exception) : base(msg, exception) { }
}
