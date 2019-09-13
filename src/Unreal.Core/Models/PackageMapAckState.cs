using System.Collections.Generic;

namespace Unreal.Core.Models
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/PackageMapClient.h#L289
    /// </summary>
    class PackageMapAckState
    {
        Dictionary<NetworkGUID, int> NetGUIDAckStatus;  // Map that represents the ack state of each net guid for this connection
        HashSet<uint> NetFieldExportGroupPathAcked;     // Map that represents whether or not a net field export group has been ack'd by the client
        HashSet<uint> NetFieldExportAcked;
    }
}