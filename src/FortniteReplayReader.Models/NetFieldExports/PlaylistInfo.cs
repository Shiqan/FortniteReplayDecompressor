using Unreal.Core;
using Unreal.Core.Attributes;
using Unreal.Core.Models;
using Unreal.Core.Models.Contracts;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportGroup("CurrentPlaylistInfo", minimalParseMode: ParseMode.Minimal)]
    public class PlaylistInfo : INetFieldExportGroup, IProperty, IResolvable
    {
        public uint Id { get; set; }
        public string Name { get; set; }

        public void Serialize(INetBitReader reader)
        {
            if (reader.EngineNetworkVersion >= EngineNetworkVersionHistory.HISTORY_FAST_ARRAY_DELTA_STRUCT)
            {
                reader.ReadBit();
            }
            reader.ReadBit();
            Id = reader.ReadIntPacked();
            reader.SkipBits(31);
        }

        public void Resolve(INetGuidCache cache)
        {
            if (cache.TryGetPathName(Id, out var name))
            {
                Name = name;
            }
        }
    }
}
