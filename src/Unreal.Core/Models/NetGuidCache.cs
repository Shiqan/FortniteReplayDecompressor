using System;
using System.Collections.Generic;

namespace Unreal.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class NetGuidCache
    {
        public Dictionary<uint, NetGuidCacheObject> ObjectLookup { get; private set; } = new Dictionary<uint, NetGuidCacheObject>();

        /// <summary>
        /// Maps net field export group name to the respective FNetFieldExportGroup
        /// </summary>
        public Dictionary<string, NetFieldExportGroup> NetFieldExportGroupMap { get; private set; } = new Dictionary<string, NetFieldExportGroup>();

        /// <summary>
        /// Maps assigned net field export group index to the respective FNetFieldExportGroup name.
        /// </summary>
        public Dictionary<uint, string> NetFieldExportGroupIndexToGroup { get; private set; } = new Dictionary<uint, string>();

        /// <summary>
        /// Maps netguid to the respective FNetFieldExportGroup name.
        /// </summary>
        public Dictionary<uint, string> NetGuidToPathName { get; private set; } = new Dictionary<uint, string>();

        /// <summary>
        /// Maps assigned net field export group index to the respective FNetFieldExportGroup name.
        /// </summary>
        public Dictionary<uint, NetFieldExportGroup> NetFieldExportGroupMapPathFixed { get; private set; } = new Dictionary<uint, NetFieldExportGroup>();

        /// <summary>
        /// Holds data about the tag dictionary
        /// </summary>
        public NetFieldExportGroup NetworkGameplayTagNodeIndex // TODO make this a UGameplayTagsManager
        {
            get
            {
                if (_networkGameplayTagNodeIndex == null)
                {
                    if (NetFieldExportGroupMap.TryGetValue("NetworkGameplayTagNodeIndex", out var nodeIndex))
                    {
                        _networkGameplayTagNodeIndex = nodeIndex;
                    }
                }
                return _networkGameplayTagNodeIndex;
            }
        }

        private Dictionary<uint, NetFieldExportGroup> _archTypeToExportGroup = new Dictionary<uint, NetFieldExportGroup>();
        private Dictionary<uint, string> _cleanedPaths = new Dictionary<uint, string>();
        private Dictionary<string, string> _cleanedClassNetCache = new Dictionary<string, string>();
        private NetFieldExportGroup _networkGameplayTagNodeIndex { get; set; }

        public UObject GetObjectFromNetGUID(NetworkGUID netGuid, bool ignoreMustBeMapped)
        {
            if (!netGuid.IsValid())
            {
                return null;
            }

            if (!netGuid.IsDefault())
            {
                return null;
            }

            if (!ObjectLookup.TryGetValue(netGuid.Value, out var cacheObject))
            {
                return null;
            }

            if (cacheObject.IsBroken)
            {
                return null;
            }

            if (cacheObject.IsPending)
            {
                return null;
            }

            if (string.IsNullOrEmpty(cacheObject.PathName))
            {
                return null;
            }

            if (cacheObject.OuterGuid.IsValid())
            {
                if (!ObjectLookup.TryGetValue(cacheObject.OuterGuid.Value, out var outerCacheObject))
                {
                    return null;
                }

                if (outerCacheObject.IsBroken)
                {
                    cacheObject.IsBroken = true;

                    return null;
                }

                var objOuter = GetObjectFromNetGUID(outerCacheObject.OuterGuid, ignoreMustBeMapped);

                if (objOuter == null)
                {
                    return null;
                }
            }

            return null;
        }

        public void AddToExportGroupMap(string group, NetFieldExportGroup exportGroup)
        {
            NetFieldExportGroupMap[group] = exportGroup;
            NetFieldExportGroupIndexToGroup[exportGroup.PathNameIndex] = group;
        }

        public NetFieldExportGroup GetNetFieldExportGroupFromIndex(uint index)
        {
            if (!NetFieldExportGroupIndexToGroup.TryGetValue(index, out var group))
            {
                return null;
            }
            return NetFieldExportGroupMap[group];
        }

        public NetFieldExportGroup GetNetFieldExportGroup(string path)
        {
            if (path == null || !NetFieldExportGroupMap.TryGetValue(path, out var group))
            {
                return null;
            }
            return group;
        }

        public void Cleanup()
        {
            //InReliable.Clear();
            //Channels.Clear();
            NetFieldExportGroupIndexToGroup.Clear();
            NetFieldExportGroupMap.Clear();
            NetGuidToPathName.Clear();
            ObjectLookup.Clear();
            NetFieldExportGroupMapPathFixed.Clear();
        }

        public NetFieldExportGroup GetNetFieldExportGroup(Actor actor)
        {
            var guid = actor.Archetype;
            var isActor = false;

            if (guid == null)
            {
                guid = actor.ActorNetGUID;
                isActor = true;
            }

            if (!_archTypeToExportGroup.ContainsKey(guid.Value))
            {
                var path = NetGuidToPathName[guid.Value];

                if (isActor)
                {
                    //The default types never end up here.
                    return null;

                    var tempPath = CoreRedirects.GetRedirect(path);

                    if (!string.IsNullOrEmpty(tempPath))
                    {
                        path = tempPath;
                    }
                }

                if (NetFieldExportGroupMapPathFixed.ContainsKey(guid.Value))
                {
                    _archTypeToExportGroup[guid.Value] = NetFieldExportGroupMapPathFixed[guid.Value];

                    return NetFieldExportGroupMapPathFixed[guid.Value];
                }

                foreach (var groupPathKvp in NetFieldExportGroupMap)
                {
                    var groupPath = groupPathKvp.Key;

                    if (!_cleanedPaths.TryGetValue(groupPathKvp.Value.PathNameIndex, out var groupPathFixed))
                    {
                        groupPathFixed = RemoveAllPathPrefixes(groupPath);
                        _cleanedPaths[groupPathKvp.Value.PathNameIndex] = groupPathFixed;
                    }

                    if (path.Contains(groupPathFixed, StringComparison.Ordinal))
                    {
                        NetFieldExportGroupMapPathFixed[guid.Value] = NetFieldExportGroupMap[groupPath];
                        _archTypeToExportGroup[guid.Value] = NetFieldExportGroupMap[groupPath];

                        return NetFieldExportGroupMap[groupPath];
                    }
                }

                return null;
            }

            return _archTypeToExportGroup[guid.Value];
        }

        public NetFieldExportGroup GetNetFieldExportGroupForClassNetCache(string group)
        {
            if (group == null)
            {
                return null;
            }

            if (!_cleanedClassNetCache.TryGetValue(group, out var classNetCachePath))
            {
                classNetCachePath = $"{RemoveAllPathPrefixes(group)}_ClassNetCache";
                _cleanedClassNetCache[group] = classNetCachePath;
            }

            if (!NetFieldExportGroupMap.TryGetValue(classNetCachePath, out var netFieldExportGroup))
            {
                return null;
            }

            return netFieldExportGroup;
        }

        public bool TryGetTagName(uint tagIndex, out string tagName)
        {
            tagName = "";
            if (tagIndex < NetworkGameplayTagNodeIndex.NetFieldExportsLength)
            {
                if (NetworkGameplayTagNodeIndex.NetFieldExports[tagIndex] != null)
                {
                    tagName = NetworkGameplayTagNodeIndex.NetFieldExports[tagIndex]?.Name;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// see UObjectBaseUtility
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string RemoveAllPathPrefixes(string path)
        {
            for (var i = path.Length - 1; i >= 0; i--)
            {
                switch (path[i])
                {
                    case '.':
                        return path.Substring(i + 1);
                    case '/':
                        return path;
                }
            }

            path = RemovePathPrefix(path, "Default__");
            return path;
        }

        private string RemovePathPrefix(string path, string toRemove)
        {
            if (toRemove.Length > path.Length)
            {
                return path;
            }

            for (var i = 0; i < toRemove.Length; i++)
            {
                if (path[i] != toRemove[i])
                {
                    return path;
                }
            }

            return path.Substring(toRemove.Length);
        }
    }
}