namespace Unreal.Core.Models;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/PackageMapClient.h#L124
/// </summary>
public class NetFieldExportGroup
{
    public string PathName { get; set; }
    public uint PathNameIndex { get; set; }
    public uint NetFieldExportsLength { get; set; }
    public NetFieldExport?[] NetFieldExports { get; set; }

    /// <summary>
    /// Index of the group for the corresponding type in NetFieldParser
    /// -1 if index is unknown.
    /// -2 if index is not found.
    /// </summary>
    internal int GroupId { get; set; } = -1;

    public bool IsValidIndex(uint handle) => handle >= 0 && handle < NetFieldExportsLength;
}
