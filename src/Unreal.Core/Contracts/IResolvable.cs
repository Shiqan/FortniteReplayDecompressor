using Unreal.Core.Models;

namespace Unreal.Core.Contracts;

/// <summary>
/// Interface to indicate that property can be resolved using <see cref="NetGuidCache"/>. 
/// For example <see cref="ItemDefinition"/> or <see cref="ActorGuid"/>.
/// </summary>
public interface IResolvable
{
    void Resolve(NetGuidCache cache);
}