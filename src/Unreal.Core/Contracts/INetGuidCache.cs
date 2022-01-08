using System.Diagnostics.CodeAnalysis;
using Unreal.Core.Models;

namespace Unreal.Core.Contracts;

/// <summary>
/// Track all NetGuids being loaded during a replay.
/// </summary>
public interface INetGuidCache
{
    /// <summary>
    /// Add a <see cref="NetFieldExportGroup"/> to the GuidCache.
    /// </summary>
    void AddToExportGroupMap(NetFieldExportGroup exportGroup);

    /// <summary>
    /// Add a NetGuid to PathName mapping to the GuidCache.
    /// </summary>
    void AddNetGuidToPathName(uint netguid, string pathName);

    /// <summary>
    /// Add a <see cref="IExternalData"/> to the ExternalData.
    /// </summary>
    void AddToExternalData(IExternalData data);

    /// <summary>
    /// Empty the NetGuidCache
    /// </summary>
    void Cleanup(bool cleanForCheckpoint = false);

    /// <summary>
    /// Get the <see cref="NetFieldExportGroup"/> by the path.
    /// </summary>
    NetFieldExportGroup? GetNetFieldExportGroup(string path);

    /// <summary>
    /// Get the <see cref="NetFieldExportGroup"/> by the Actor guid.
    /// </summary>
    NetFieldExportGroup? GetNetFieldExportGroup(uint? netguid);

    /// <summary>
    /// Get the <see cref="NetFieldExportGroup"/> by the index.
    /// </summary>
    NetFieldExportGroup? GetNetFieldExportGroupFromIndex(uint? index);

    /// <summary>
    /// Tries to find the ClassNetCache for the given group path.
    /// </summary>
    bool TryGetClassNetCache(string? group, [NotNullWhen(true)] out NetFieldExportGroup? netFieldExportGroup, bool useFullName);

    /// <summary>
    /// Tries to resolve the netguid using the <see cref="NetGuidToPathName"/>
    /// </summary>
    bool TryGetExternalData(uint? netguid, [NotNullWhen(true)] out IExternalData? externalData);

    /// <summary>
    /// Tries to resolve the netguid using the <see cref="NetGuidToPathName"/>
    /// </summary>
    bool TryGetPathName(uint netguid, [NotNullWhen(true)] out string? pathName);

    /// <summary>
    /// Tries to resolve the tagIndex using the <see cref="NetworkGameplayTagNodeIndex"/>
    /// </summary>
    bool TryGetTagName(uint tagIndex, [NotNullWhen(true)] out string? tagName);
}