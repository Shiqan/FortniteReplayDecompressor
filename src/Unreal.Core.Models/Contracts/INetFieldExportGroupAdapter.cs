namespace Unreal.Core.Models.Contracts
{
    public interface INetFieldExportGroupAdapter
    {

    }
    
    public interface INetFieldExportGroupAdapter<T> : INetFieldExportGroupAdapter where T : INetFieldExportGroup
    {
        T Data { get; set; }
        void ReadField(string field, INetBitReader netBitReader);
    }
}
