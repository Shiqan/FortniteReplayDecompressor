using Unreal.Core.Contracts;

namespace Unreal.Core.Models;

/// <summary>
/// Send on <see cref="ReplayReader{T}.NetDeltaSerialize(FBitArchive, NetFieldExportGroup, uint, bool)"/>
/// </summary>
public class NetDeltaUpdate
{
    public int ElementIndex { get; set; }
    public INetFieldExportGroup? Export { get; set; }
    public bool Deleted { get; set; }
    //public FFastArraySerializerHeader Header { get; set; }
    public uint ChannelIndex { get; set; }
}
