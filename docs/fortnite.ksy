meta:
  id: fortnite
  file-extension: replay
  endian: le
seq:
  - id: magic
    contents: [0x7f, 0xe2, 0xa2, 0x1c]
    doc: Unreal Replay magic number.
  - id: file_version
    type: u4
    enum: version_history
    doc: File versioning, used for backward compatibility.
  - id: length_in_ms
    type: u4
    doc: Length of the replay in ms.
  - id: network_version
    type: u4
    doc: Network version used by the engine. Used for backward compatibility.
  - id: change_list
    type: u4
    doc: Always increasing number. Can show as "build number".
  - id: friendly_name
    type: ustring
  - id: is_live
    type: u4
  - id: timestamp
    type: u8
    if: file_version.to_i > version_history::recorded_timestamp.to_i
  - id: compressed
    type: u4
    if: file_version.to_i > version_history::compression.to_i  
  - id: encrypted
    type: u4
    if: file_version.to_i > version_history::encrypted.to_i
  - id: chunk_header
    type: chunk
  - id: chunk
    type: chunk
    repeat: eos
types:
  ustring:
    seq:
      - id: len
        type: s4
        doc: the length of the string. if the length is negative, it's an Unicode string
      - id: content
        type: str
        size: len < 0 ? (-len)*2 : len
        encoding: UTF-8
  chunk:
    seq:
      - id: chunk_header
        type: chunk_header
      - id: chunk_content
        size: chunk_header.chunk_size
        type:
          switch-on: chunk_header.chunk_type
          cases:
            'chunk_type::header' : chunk_content_header  
            'chunk_type::replay_data' : chunk_replay_data
            'chunk_type::checkpoint' : chunk_checkpoint
            'chunk_type::event' : chunk_event
  
  chunk_content_header:
    seq:
      - id: magic
        contents: [0x3d, 0xa1, 0xf5, 0x2c]
      - id: version
        type: u4
        enum: network_version_history
      - id: network_checksum
        type: u4
      - id: engine_network_protocol_version
        type: u4
        enum: engine_network_version_history
      - id: game_network_protocol_version
        type: u4
      - id: guid
        size: 16
        if: version.to_i >= network_version_history::guid_demo_header.to_i
      - id: engine_version
        type: engine_version_base
        if: version.to_i >= network_version_history::save_full_engine_version.to_i
      - id: changelist
        type: u4
        if: 0==0 #todo
      - id: level_name_and_times_count
        type: u4
      
  chunk_replay_data:
    seq:
      - id: time1
        type: u4
  
  chunk_checkpoint:
    seq:
      - id: id
        type: ustring
  
  chunk_event:
    seq:
      - id: id
        type: ustring
  
  chunk_header:
    seq:
      - id: chunk_type
        type: u4
        enum: chunk_type
      - id: chunk_size
        type: u4
  engine_version_base:
    seq:
      - id: major
        type: u2
      - id: minor
        type: u2
      - id: patch
        type: u2
      - id: change_list
        type: u4
      - id: branch
        type: ustring
enums:
  chunk_type:
    0: header
    1: replay_data
    2: checkpoint
    3: event
  version_history:
    0: initial
    1: fixed_size_friendly_name
    2: compression
    3: recorded_timestamp
    4: stream_chunk_times
    5: friendly_name_encoding
    6: encrypted
    7: new_version
  network_version_history:
    1: initial
    2: absolute_time
    3: increased_buffer
    4: engine_version
    5: extra_version
    6: multi_level
    7: multi_level_time_change
    8: deleted_startups_actors
    9: demo_header_enum_flags
    10: level_streaming_fixes
    11: save_full_engine_version
    12: guid_demo_header
    13: history_character_movement
    14: new_version
  engine_network_version_history:
    1: history_initial
    2: history_replay_backward_compat
    3: history_max_actor_channels_customization
    4: history_repcmd_checksum_remove_printf
    5: history_new_actor_override_level
    6: history_channel_names
    7: history_channel_close_reason
    8: history_acks_included_in_header
    9: history_netexport_serialization
    10: history_netexport_serialize_fix