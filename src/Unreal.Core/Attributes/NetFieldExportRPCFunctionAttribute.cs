using System;

namespace Unreal.Core.Attributes
{
    /// <summary>
    /// Attribute to map a property to the specified path. Used for function RPC property replication.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class NetFieldExportRPCFunctionAttribute : Attribute
    {
        public string Name { get; private set; }
        public string PathName { get; private set; }

        public NetFieldExportRPCFunctionAttribute(string name, string pathName)
        {
            Name = name;
            PathName = pathName;
        }
    }
}
