using FortniteReplayReaderDecompressor.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FortniteReplayReaderDecompressor.Core.Models
{
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

        public int PacketId { get; set; }
        //FInBunch* Next;
        //UNetConnection* Connection;
        public uint ChIndex { get; set; }
        // UE_DEPRECATED(4.22, "ChType deprecated in favor of ChName.")
        public ChannelType ChType { get; set; }
        // FName
        public ChannelName ChName { get; set; }
        public int ChSequence { get; set; }
        public bool bOpen { get; set; }
        public bool bClose { get; set; }
        // UE_DEPRECATED(4.22, "bDormant is deprecated in favor of CloseReason")
        public bool bDormant { get; set; }                 // Close, but go dormant
        public bool bIsReplicationPaused { get; set; }     // Replication on this channel is being paused by the server
        public bool bReliable { get; set; }
        public bool bPartial { get; set; }                // Not a complete bunch
        public bool bPartialInitial { get; set; }           // The first bunch of a partial bunch
        public bool bPartialFinal { get; set; }      // The final bunch of a partial bunch
        public bool bHasPackageMapExports { get; set; }  // This bunch has networkGUID name/id pairs
        public bool bHasMustBeMappedGUIDs { get; set; }  // This bunch has guids that must be mapped before we can process this bunch
        public bool bIgnoreRPCs { get; set; }
        public ChannelCloseReason CloseReason { get; set; }
    }
}
