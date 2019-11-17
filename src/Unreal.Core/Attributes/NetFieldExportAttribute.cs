using System;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Attributes
{
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
