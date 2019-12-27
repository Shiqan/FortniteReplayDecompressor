using Unreal.Core.Contracts;

namespace Unreal.Core.Models
{
    public class FGameplayTag : IProperty
    {
        public string TagName { get; private set; }

        public void Serialize(NetBitReader reader)
        {
            //TagName = reader.ReadFString();
            var netIndex = reader.ReadIntPacked();
            TagName = netIndex.ToString();
        }
    }
}