namespace Unreal.Core.Contracts;

/// <summary>
/// Interface to indicate properties that can be (de)serialized.
/// </summary>
public interface IProperty
{
    void Serialize(NetBitReader reader);
}