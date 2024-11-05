namespace Unreal.Core.Models.Enums;

/// <summary>
/// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/Core/Public/Misc/NetworkVersion.h#L33
/// </summary>
public enum EngineNetworkVersionHistory
{
    HISTORY_INITIAL = 1,
    HISTORY_REPLAY_BACKWARDS_COMPAT = 2,                // Bump version to get rid of older replays before backwards compat was turned on officially
    HISTORY_MAX_ACTOR_CHANNELS_CUSTOMIZATION = 3,       // Bump version because serialization of the actor channels changed
    HISTORY_REPCMD_CHECKSUM_REMOVE_PRINTF = 4,          // Bump version since the way FRepLayoutCmd::CompatibleChecksum was calculated changed due to an optimization
    HISTORY_NEW_ACTOR_OVERRIDE_LEVEL = 5,               // Bump version since a level reference was added to the new actor information
    HISTORY_CHANNEL_NAMES = 6,                          // Bump version since channel type is now an fname
    HISTORY_CHANNEL_CLOSE_REASON = 7,                   // Bump version to serialize a channel close reason in bunches instead of bDormant
    HISTORY_ACKS_INCLUDED_IN_HEADER = 8,                // Bump version since acks are now sent as part of the header
    HISTORY_NETEXPORT_SERIALIZATION = 9,                // Bump version due to serialization change to FNetFieldExport
    HISTORY_NETEXPORT_SERIALIZE_FIX = 10,               // Bump version to fix net field export name serialization
    HISTORY_FAST_ARRAY_DELTA_STRUCT = 11,               // Bump version to allow fast array serialization, delta struct serialization.
    HISTORY_FIX_ENUM_SERIALIZATION = 12,                // Bump version to fix enum net serialization issues.
    HISTORY_OPTIONALLY_QUANTIZE_SPAWN_INFO = 13,        // Bump version to conditionally disable quantization for Scale, Location, and Velocity when spawning network actors.
    HISTORY_JITTER_IN_HEADER = 14,                      // Bump version since we added jitter clock time to packet headers and removed remote saturation
    HISTORY_CLASSNETCACHE_FULLNAME = 15,                // Bump version to use full paths in GetNetFieldExportGroupForClassNetCache
    HISTORY_REPLAY_DORMANCY = 16,                       // Bump version to support dormancy properly in replays
    HISTORY_ENUM_SERIALIZATION_COMPAT = 17,             // Bump version to include enum bits required for serialization into compat checksums, as well as unify enum and byte property enum serialization (TODO)
    HISTORY_SUBOBJECT_OUTER_CHAIN = 18,                 // Bump version to support subobject outer chains matching on client and server
    HISTORY_HITRESULT_INSTANCEHANDLE = 19,              // Bump version to support FHitResult change of Actor to HitObjectHandle. This change was made in CL 14369221 but a net version wasn't added at the time.
    HISTORY_INTERFACE_PROPERTY_SERIALIZATION = 20,	    // Bump version to support net serialization of FInterfaceProperty
    HISTORY_MONTAGE_PLAY_INST_ID_SERIALIZATION = 21,    // Bump version to support net serialization of FGameplayAbilityRepAnimMontage, addition of PlayInstanceId and removal of bForcePlayBit
    HISTORY_SERIALIZE_DOUBLE_VECTORS_AS_DOUBLES = 22,   // Bump version to support net serialization of double vector types
    HISTORY_PACKED_VECTOR_LWC_SUPPORT = 23,             // Bump version to support quantized LWC FVector net serialization
    HISTORY_PAWN_REMOTEVIEWPITCH = 24,                  // Bump version to support serialization changes to RemoteViewPitch
    HISTORY_REPMOVE_SERVERFRAME_AND_HANDLE = 25,        // Bump version to support serialization changes to RepMove so we can get the serverframe and physics handle associated with the object
    HISTORY_21_AND_VIEWPITCH_ONLY_DO_NOT_USE = 26,      // Bump version to support up to history 21 + HISTORY_PAWN_REMOTEVIEWPITCH.  DO NOT USE!!!
    HISTORY_PLACEHOLDER = 27,                           // Bump version to a placeholder.  This version is the same as HISTORY_REPMOVE_SERVERFRAME_AND_HANDLE
    HISTORY_RUNTIME_FEATURES_COMPATIBILITY = 28,        // Bump version to add network runtime feature compatibility test to handshake (hello/upgrade) control messages
    HISTORY_SOFTOBJECTPTR_NETGUIDS = 29,                // Bump version to support replicating SoftObjectPtrs by NetGuid instead of raw strings.
    HISTORY_SUBOBJECT_DESTROY_FLAG = 30,                // Bump version to support subobject destruction message flags
    HISTORY_GAMESTATE_REPLCIATED_TIME_AS_DOUBLE = 31,   // Bump version to support AGameStateBase::ReplicatedWorldTimeSeconds as double instead of float.
    HISTORY_CUSTOMVERION = 32,                          // Bump version to switch to using custom versions
    DynamicMontageSerialization = 33,                   // Bump version to support dynamic montage serialization in the Gameplay Ability System
    PredictionKeyBaseNotReplicated = 34,                // Bump version to stop FPredictionKey::Base from being replicated (it was unused).
    RepMoveOptionalAcceleration = 35,                   // Bump version to support serialization changes to RepMove for optional Acceleration
    CustomExports = 36,                                 // Bump version to support CustomExports (such as NetTokens used for supporting "Dynamic Replication" of GameplayTags -- see FGameplayTag::NetSerialize_Packed)		// -----<new versions can be added above this line>-------------------------------------------------

    HISTORY_ENGINENETVERSION_PLUS_ONE,
    LATEST = HISTORY_ENGINENETVERSION_PLUS_ONE - 1,
}
