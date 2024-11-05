using System;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Models;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Public/Net/DataBunch.h#L112
/// </summary>
public class DataBunch
{
    public DataBunch()
    {

    }

    public DataBunch(DataBunch InBunch)
    {
        Archive = InBunch.Archive;
        PacketId = InBunch.PacketId;
        ChIndex = InBunch.ChIndex;
        ChType = InBunch.ChType;
        ChName = InBunch.ChName;
        ChSequence = InBunch.ChSequence;
        bOpen = InBunch.bOpen;
        bClose = InBunch.bClose;
        bDormant = InBunch.bDormant;
        bIsReplicationPaused = InBunch.bIsReplicationPaused;
        bReliable = InBunch.bReliable;
        bPartial = InBunch.bPartial;
        bPartialInitial = InBunch.bPartialInitial;
        bPartialFinal = InBunch.bPartialFinal;
        bHasPackageMapExports = InBunch.bHasPackageMapExports;
        bHasMustBeMappedGUIDs = InBunch.bHasMustBeMappedGUIDs;
        bIgnoreRPCs = InBunch.bIgnoreRPCs;
        CloseReason = InBunch.CloseReason;
    }
    public FBitArchive Archive { get; set; }
    public int PacketId { get; set; }
    public uint ChIndex { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Obsolete("UE_DEPRECATED(4.22, \"ChType deprecated in favor of ChName.\")")]
    public ChannelType ChType { get; set; }
    public ChannelName ChName { get; set; }
    public int ChSequence { get; set; }
    public bool bOpen { get; set; }
    public bool bClose { get; set; }

    /// <summary>
    /// Close, but go dormant
    /// </summary>
    public bool bDormant { get; set; }

    /// <summary>
    /// Replication on this channel is being paused by the server
    /// </summary>
    public bool bIsReplicationPaused { get; set; }
    public bool bReliable { get; set; }

    /// <summary>
    /// Not a complete bunch
    /// </summary>
    public bool bPartial { get; set; }

    /// <summary>
    /// The first bunch of a partial bunch
    /// </summary>
    public bool bPartialInitial { get; set; }

    /// <summary>
    ///  This bunch marks the end of the CustomExports data that needs to be processed immediately (not queued)
    /// </summary>
    public bool bHasPartialCustomExportsFinalBit { get; set; }

    /// <summary>
    /// The final bunch of a partial bunch
    /// </summary>
    public bool bPartialFinal { get; set; }

    /// <summary>
    /// This bunch has networkGUID name/id pairs
    /// </summary>
    public bool bHasPackageMapExports { get; set; }  // T

    /// <summary>
    /// This bunch has guids that must be mapped before we can process this bunch
    /// </summary>
    public bool bHasMustBeMappedGUIDs { get; set; }
    public bool bIgnoreRPCs { get; set; }
    public ChannelCloseReason CloseReason { get; set; }
}
