using System;
using Unreal.Core;
using Unreal.Core.Contracts;

namespace FortniteReplayReader.Models
{
    public class FAthenaPawnReplayData : IProperty
    {
        public ReadOnlyMemory<byte> EncryptedReplayData { get; private set; }

        public void Serialize(NetBitReader reader)
        {
            //EncryptedReplayData = reader.ReadBytes(reader.GetBitsLeft() / 8);
        }
    }
}