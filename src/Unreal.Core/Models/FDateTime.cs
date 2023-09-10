using System;
using Unreal.Core.Contracts;

namespace Unreal.Core.Models;

public class FDateTime : IProperty
{
    public DateTime Time { get; private set; }

    public void Serialize(NetBitReader reader) => Time = new DateTime((long) reader.ReadUInt64());
}