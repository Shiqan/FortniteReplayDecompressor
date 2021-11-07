using System;

namespace Unreal.Core.Attributes;

/// <summary>
/// Attribute to mark PlayerControllers, which require an additional byte to be parsed in 
/// <see cref="ReplayReader{T}.ProcessBunch(Models.DataBunch)"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class PlayerControllerAttribute : Attribute
{
    public string Path { get; private set; }

    public PlayerControllerAttribute(string path)
    {
        Path = path;
    }
}
