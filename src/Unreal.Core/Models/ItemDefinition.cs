using Unreal.Core.Contracts;

namespace Unreal.Core.Models;

/// <summary>
/// Just a wrapper class for <see cref="NetworkGUID"/>
/// </summary>
public class ItemDefinition : NetworkGUID, IResolvable
{
    public string Name { get; set; }

    public void Resolve(INetGuidCache cache)
    {
        if (IsValid())
        {
            if (cache.TryGetPathName(Value, out var name))
            {
                Name = name;
            }
        }
    }
}
