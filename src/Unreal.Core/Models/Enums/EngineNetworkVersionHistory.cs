namespace Unreal.Core.Models.Enums
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/7d9919ac7bfd80b7483012eab342cb427d60e8c9/Engine/Source/Runtime/Core/Public/Misc/NetworkVersion.h#L33
    /// </summary>
    public enum EngineNetworkVersionHistory
    {
        HISTORY_INITIAL = 1,
        HISTORY_REPLAY_BACKWARDS_COMPAT = 2,            // Bump version to get rid of older replays before backwards compat was turned on officially
        HISTORY_MAX_ACTOR_CHANNELS_CUSTOMIZATION = 3,   // Bump version because serialization of the actor channels changed
        HISTORY_REPCMD_CHECKSUM_REMOVE_PRINTF = 4,      // Bump version since the way FRepLayoutCmd::CompatibleChecksum was calculated changed due to an optimization
        HISTORY_NEW_ACTOR_OVERRIDE_LEVEL = 5,           // Bump version since a level reference was added to the new actor information
        HISTORY_CHANNEL_NAMES = 6,                      // Bump version since channel type is now an fname
        HISTORY_CHANNEL_CLOSE_REASON = 7,               // Bump version to serialize a channel close reason in bunches instead of bDormant
        HISTORY_ACKS_INCLUDED_IN_HEADER = 8,            // Bump version since acks are now sent as part of the header
        HISTORY_NETEXPORT_SERIALIZATION = 9,            // Bump version due to serialization change to FNetFieldExport
        HISTORY_NETEXPORT_SERIALIZE_FIX = 10,           // Bump version to fix net field export name serialization
        HISTORY_UPDATE9 = 11,                           // Update 9 (not in unreal engine source code yet)
    }
}
