using System.Collections.Generic;

namespace Unreal.Core.Models.Contracts
{
    public interface INetGuidCache
    {
        Dictionary<uint, string> NetFieldExportGroupIndexToGroup { get; }
        Dictionary<string, NetFieldExportGroup> NetFieldExportGroupMap { get; }
        Dictionary<uint, NetFieldExportGroup> NetFieldExportGroupMapPathFixed { get; }
        Dictionary<uint, string> NetGuidToPathName { get; }
        NetFieldExportGroup NetworkGameplayTagNodeIndex { get; }

        void AddToExportGroupMap(string group, NetFieldExportGroup exportGroup);
        void Cleanup();
        NetFieldExportGroup GetNetFieldExportGroup(string path);
        NetFieldExportGroup GetNetFieldExportGroup(uint netguid);
        NetFieldExportGroup GetNetFieldExportGroupFromIndex(uint index);
        bool TryGetClassNetCache(string group, out NetFieldExportGroup netFieldExportGroup, bool useFullName);
        bool TryGetPathName(uint netguid, out string pathName);
        bool TryGetTagName(uint tagIndex, out string tagName);
    }
}