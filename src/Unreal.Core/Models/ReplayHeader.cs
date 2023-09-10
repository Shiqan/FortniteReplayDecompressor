using Unreal.Core.Models.Enums;

namespace Unreal.Core.Models;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/811c1ce579564fa92ecc22d9b70cbe9c8a8e4b9a/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L151
/// </summary>
public class ReplayHeader
{
    /// <summary>
    /// Version number to detect version mismatches.
    /// </summary>
    public NetworkVersionHistory NetworkVersion { get; set; }

    /// <summary>
    /// Network checksum.
    /// </summary>
    public uint NetworkChecksum { get; set; }

    /// <summary>
    /// Version of the engine internal network format.
    /// </summary>
    public EngineNetworkVersionHistory EngineNetworkVersion { get; set; }

    /// <summary>
    /// Version of the game internal network format.
    /// </summary>
    public uint GameNetworkProtocolVersion { get; set; }

    /// <summary>
    /// Unique identifier of the replay.
    /// </summary>
    public string Guid { get; set; }

    /// <summary>
    /// Major engine version on which the replay was recorded.
    /// </summary>
    public ushort Major { get; set; }

    /// <summary>
    /// Minor engine version on which the replay was recorded.
    /// </summary>
    public ushort Minor { get; set; }

    /// <summary>
    /// Patch engine version on which the replay was recorded.
    /// </summary>
    public ushort Patch { get; set; }

    /// <summary>
    /// Full engine version on which the replay was recorded.
    /// </summary>
    public uint Changelist { get; set; }

    /// <summary>
    /// Full engine version on which the replay was recorded.
    /// </summary>
    public string Branch { get; set; }

    /// <summary>
    /// Engine package version on which the replay was recorded.
    /// </summary>
    public uint UE4Version { get; set; }

    /// <summary>
    /// Engine package version on which the replay was recorded.
    /// </summary>
    public uint UE5Version { get; set; }

    /// <summary>
    /// Licensee package version on which the replay was recorded.
    /// </summary>
    public uint PackageVersionLicenseeUE { get; set; }

    /// <summary>
    /// Name and time changes of levels loaded.
    /// </summary>
    public (string, uint)[] LevelNamesAndTimes { get; set; }

    /// <summary>
    /// Replay flags.
    /// </summary>
    public ReplayHeaderFlags Flags { get; set; }

    /// <summary>
    /// Area for subclasses to write stuff.
    /// </summary>
    public string[] GameSpecificData { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Platform { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public BuildTargetType? BuildTargetType { get; set; }

    public bool HasLevelStreamingFixes() => Flags >= ReplayHeaderFlags.HasStreamingFixes;
}
