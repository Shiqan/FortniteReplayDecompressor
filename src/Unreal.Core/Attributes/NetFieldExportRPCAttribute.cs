using System;

namespace Unreal.Core.Attributes
{
    /// <summary>
    /// Attribute to map a class to the specified path. Used for function RPC property replication.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class NetFieldExportRPCAttribute : Attribute
    {
        public string Path { get; private set; }

        public NetFieldExportRPCAttribute(string path)
        {
            Path = path;
        }
    }
}
