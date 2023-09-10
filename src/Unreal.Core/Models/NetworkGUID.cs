using Unreal.Core.Contracts;

namespace Unreal.Core.Models;

/// <summary>
/// https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Misc/NetworkGuid.h#L14
/// </summary>
public class NetworkGUID : IProperty
{
    public uint Value { get; set; }

    public bool IsValid() => Value > 0;

    public bool IsDynamic() => Value > 0 && (Value & 1) != 1;

    public bool IsDefault() => Value == 1;

    public void Serialize(NetBitReader reader) => Value = reader.ReadIntPacked();
}
