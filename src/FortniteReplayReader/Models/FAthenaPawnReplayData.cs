using System;
using Unreal.Core;
using Unreal.Core.Contracts;

namespace FortniteReplayReader.Models;

public class FAthenaPawnReplayData : IProperty
{
    public ReadOnlyMemory<byte> EncryptedReplayData { get; private set; }

    public void Serialize(NetBitReader reader) => reader.Seek(0, System.IO.SeekOrigin.End);
}