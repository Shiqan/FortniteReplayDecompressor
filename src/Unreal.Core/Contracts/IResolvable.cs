using Unreal.Core.Models;

namespace Unreal.Core.Contracts
{
    public interface IResolvable
    {
        void Resolve(NetGuidCache cache);
    }
}