namespace Unreal.Core.Contracts
{
    public interface IProperty
    {
        void Serialize(NetBitReader reader);
    }
}