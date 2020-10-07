using System;
using Unreal.Core.Models.Contracts;

namespace Unreal.Core.Models
{
    public class FDateTime : IProperty
    {
        public DateTime Time { get; private set; }

        public void Serialize(INetBitReader reader)
        {
            Time = new DateTime((long)reader.ReadUInt64());
        }
    }
}