using Unreal.Core.Contracts;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models.Events;

public abstract class BaseEvent : IEvent
{
    public EventInfo Info { get; set; }
}