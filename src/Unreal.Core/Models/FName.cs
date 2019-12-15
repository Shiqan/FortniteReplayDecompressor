using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Models
{
    public class FName : IProperty
    {
        public string Name { get; private set; }

        public void Serialize(NetBitReader reader)
        {
            Name = reader.SerializePropertyName();
        }
    }
}