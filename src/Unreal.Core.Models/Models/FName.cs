using Unreal.Core.Models.Contracts;

namespace Unreal.Core.Models
{
    public class FName : IProperty
    {
        public string Name { get; private set; }

        public void Serialize(INetBitReader reader)
        {
            Name = reader.SerializePropertyName();
        }
    }
}