using Unreal.Core.Models.Contracts;

namespace Unreal.Core.Models
{
    public abstract class NetFieldExportGroupAdapter<T> where T : INetFieldExportGroup
    {
        public T Data { get; set; }
        public abstract void ReadField(string field, INetBitReader netBitReader);
    }
}
