using Unreal.Core.Models;

namespace Unreal.Core.Contracts
{
    public interface IEvent
    {
        EventInfo Info { get; set; }
    }
}
