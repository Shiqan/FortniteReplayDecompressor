namespace Unreal.Core.Models.Enums
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/Core/Public/Misc/NetworkVersion.h#L33
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
        HISTORY_FAST_ARRAY_DELTA_STRUCT = 11,           // Bump version to allow fast array serialization, delta struct serialization.
        HISTORY_FIX_ENUM_SERIALIZATION = 12,            // Bump version to fix enum net serialization issues.
        HISTORY_OPTIONALLY_QUANTIZE_SPAWN_INFO = 13,    // Bump version to conditionally disable quantization for Scale, Location, and Velocity when spawning network actors.
        HISTORY_JITTER_IN_HEADER = 14,                  // Bump version since we added jitter clock time to packet headers and removed remote saturation
        HISTORY_CLASSNETCACHE_FULLNAME = 15,            // Bump version to use full paths in GetNetFieldExportGroupForClassNetCache
        HISTORY_REPLAY_DORMANCY = 16,                   // Bump version to support dormancy properly in replays
        HISTORY_ENUM_SERIALIZATION_COMPAT = 17,         // Bump version to include enum bits required for serialization into compat checksums, as well as unify enum and byte property enum serialization (TODO)
        HISTORY_SUBOBJECT_OUTER_CHAIN = 18,             // Bump version to support subobject outer chains matching on client and server
        HISTORY_HITRESULT_INSTANCEHANDLE = 19,          // Bump version to support FHitResult change of Actor to HitObjectHandle. This change was made in CL 14369221 but a net version wasn't added at the time.
        HISTORY_INTERFACE_PROPERTY_SERIALIZATION = 20,	// Bump version to support net serialization of FInterfaceProperty

        HISTORY_ENGINENETVERSION_PLUS_ONE,
        LATEST = HISTORY_ENGINENETVERSION_PLUS_ONE - 1,
    }
}
