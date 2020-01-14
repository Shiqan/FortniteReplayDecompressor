using System;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Attributes
{
    /// <summary>
    /// Attribute to map a property to the name used in the property replication.
    /// <see cref="RepLayoutCmdType"/> is used to specify the parsing method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class NetFieldExportAttribute : Attribute
    {
        public string Name { get; set; }
        public RepLayoutCmdType Type { get; set; }

        public NetFieldExportAttribute(string name, RepLayoutCmdType type)
        {
            Name = name;
            Type = type;
        }
    }
}