using System.Collections.Generic;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Models;

/// <summary>
/// Base class of communication channels.
/// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/Engine/Classes/Engine/Channel.h
/// </summary>
public class UChannel
{
    private readonly HashSet<string> _ignore = new();
    public ChannelName ChannelName { get; set; }
    public uint ChannelIndex { get; set; }
    public ChannelType ChannelType { get; set; }

    /// <summary>
    /// Add the <paramref name="group"/> to the list of groups to ignore for this channel.
    /// </summary>
    /// <param name="group"></param>
    public void IgnoreGroup(string group) => _ignore.Add(group);

    /// <summary>
    /// Whether or not this <paramref name="group"/> is ignored by this channel.
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public bool IsIgnoringGroup(string group) => _ignore.Contains(group);

    public Actor? Actor { get; set; }
    public uint? ArchetypeId => Actor?.Archetype?.Value;
    public uint? ActorId => Actor?.ActorNetGUID.Value;
}
