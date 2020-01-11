using Unreal.Core;
using Unreal.Core.Contracts;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models.NetFieldExports
{
    public class PlaylistInfo : IProperty, IResolvable
    {
        public uint Id { get; set; }
        public string Name { get; set; }

        public void Serialize(NetBitReader reader)
        {
            reader.SkipBits(2);
            Id = reader.ReadIntPacked();
        }

        public void Resolve(NetGuidCache cache)
        {
            if (cache.TryGetPathName(Id, out var name))
            {
                Name = name;
            }
        }
    }
}
