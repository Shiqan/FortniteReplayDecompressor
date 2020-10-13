using System.Collections.Generic;
using Unreal.Core.Models.Contracts;

namespace FortniteReplayReader.Models
{
    public class FAthenaPawnReplayData : IProperty
    {
        public IEnumerable<byte> EncryptedReplayData { get; private set; }

        public void Serialize(INetBitReader reader)
        {
            //reader.Seek(0, System.IO.SeekOrigin.End);
        }
    }
}