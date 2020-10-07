namespace Unreal.Core.Models.Contracts
{
    /// <summary>
    /// Interface to indicate properties that can be (de)serialized.
    /// </summary>
    public interface IProperty
    {
        void Serialize(INetBitReader reader);
    }
}