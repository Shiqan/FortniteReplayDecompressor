using System.Collections.Generic;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Models
{
    /// <summary>
    /// Base class of communication channels.
    /// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/Engine/Classes/Engine/Channel.h
    /// </summary>
    public class UChannel
    {
        private HashSet<string> _ignore = new HashSet<string>();
        public string ChannelName { get; set; }
        public uint ChannelIndex { get; set; }
        public ChannelType ChannelType { get; set; }

        public void IgnoreChannel(string group) => _ignore.Add(group);
        public bool IsIgnoringChannel(string group) => _ignore.Contains(group);

        public Actor Actor { get; set; }
        public uint? ArchetypeId => Actor?.Archetype?.Value;
        public uint? ActorId => Actor?.ActorNetGUID.Value;
    }
}
