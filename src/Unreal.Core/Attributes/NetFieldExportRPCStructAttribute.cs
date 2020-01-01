using System;

namespace Unreal.Core.Attributes
{
    /// <summary>
    /// Attribute to map a property to the specified path. Used for function RPC property replication.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class NetFieldExportRPCStructAttribute : Attribute
    {
        public string Name { get; private set; }
        public string PathName { get; private set; }

        /// <summary>
        /// Indicates whether or not the checksum bit should be read when receiving properties.
        /// </summary>
        public bool EnablePropertyChecksum { get; private set; }

        public NetFieldExportRPCStructAttribute(string name, string pathName, bool enablePropertyChecksum = true)
        {
            Name = name;
            PathName = pathName;
            EnablePropertyChecksum = enablePropertyChecksum;
        }
    }
}
