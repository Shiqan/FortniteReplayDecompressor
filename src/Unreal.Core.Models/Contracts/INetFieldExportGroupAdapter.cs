namespace Unreal.Core.Models.Contracts
{
    public interface INetFieldExportGroupAdapter
    {
        INetFieldExportGroup GetData();
        void ReadField(string field, INetBitReader netBitReader);
    }

    public interface INetFieldExportGroupAdapter<T> : INetFieldExportGroupAdapter where T : INetFieldExportGroup
    {
        T Data { get; set; }
    }
}
