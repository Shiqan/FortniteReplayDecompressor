namespace Unreal.Core.Models.Contracts
{
    /// <summary>
    /// Interface to indicate that property can be resolved using <see cref="INetGuidCache"/>. 
    /// For example <see cref="ItemDefinition"/> or <see cref="ActorGuid"/>.
    /// </summary>
    public interface IResolvable
    {
        void Resolve(INetGuidCache cache);
    }
}