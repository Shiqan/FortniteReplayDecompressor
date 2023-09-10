namespace Unreal.Core.Models;

/// <summary>
/// Struct containing common header data that is written / read when serializing Fast Arrays.
/// see https://github.com/EpicGames/UnrealEngine/blob/a097c209d382ada36690612ad8eaf8ee57d0887f/Engine/Source/Runtime/Engine/Classes/Engine/NetSerialization.h#L556
/// </summary>
public class FFastArraySerializerHeader
{
    /// <summary>
    /// The current ArrayReplicationKey.
    /// </summary>
    public int ArrayReplicationKey { get; set; }

    /// <summary>
    /// The previous ArrayReplicationKey. 
    /// </summary>
    public int BaseReplicationKey { get; set; }

    /// <summary>
    /// The number of added elements.
    /// </summary>
    public int NumChanged { get; set; }

    /// <summary>
    /// The number of deleted elements.
    /// </summary>
    public int NumDeletes { get; set; }
}
