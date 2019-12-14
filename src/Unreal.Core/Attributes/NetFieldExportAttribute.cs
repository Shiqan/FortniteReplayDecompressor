using System;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class NetFieldExportAttribute : Attribute
    {
        public string Name { get; set; }
        public RepLayoutCmdType Type { get; set; }
        public int Handle { get; set; }
        public UnknownFieldInfo Info { get; set; }

        public NetFieldExportAttribute(string name, RepLayoutCmdType type, int handle, string propertyNameUKInfo, string typeUKInfo, int bitCountUKInfo)
        {
            Name = name;
            Type = type;
            Handle = handle;
            Info = new UnknownFieldInfo(propertyNameUKInfo, typeUKInfo, bitCountUKInfo, (uint)handle);
        }
    }
}