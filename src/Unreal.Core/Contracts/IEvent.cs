using Unreal.Core.Models;

namespace Unreal.Core.Contracts;

/// <summary>
/// Interface for all text based events in a replay.
/// </summary>
public interface IEvent
{
    EventInfo Info { get; set; }
}
