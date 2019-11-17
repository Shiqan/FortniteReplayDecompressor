using System;

namespace Unreal.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class NetFieldExportGroupAttribute : Attribute
    {
        public string Path { get; private set; }

        public NetFieldExportGroupAttribute(string path)
        {
            Path = path;
        }
    }
}
