using System;

namespace Unreal.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class NetFieldExportSubGroupAttribute : Attribute
    {
        public string Path { get; private set; }

        public NetFieldExportSubGroupAttribute(string path)
        {
            Path = path;
        }
    }
}
