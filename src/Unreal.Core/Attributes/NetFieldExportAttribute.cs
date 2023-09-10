using System;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Attributes;

public abstract class NetFieldAttribute : Attribute
{
    public RepLayoutCmdType Type { get; set; }
    public ParseMode? MinimalParseMode { get; set; }
}

/// <summary>
/// Attribute to map a property to the name used in the property replication.
/// <see cref="RepLayoutCmdType"/> is used to specify the parsing method.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class NetFieldExportAttribute : NetFieldAttribute
{
    public string Name { get; set; }

    public NetFieldExportAttribute(string name, RepLayoutCmdType type)
    {
        Name = name;
        Type = type;
    }

    public NetFieldExportAttribute(string name, RepLayoutCmdType type, ParseMode minimalParseMode)
    {
        Name = name;
        Type = type;
        MinimalParseMode = minimalParseMode;
    }
}

/// <summary>
/// Attribute to map a property to the handle used in the property replication.
/// <see cref="RepLayoutCmdType"/> is used to specify the parsing method.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class NetFieldExportHandleAttribute : NetFieldAttribute
{
    public uint Handle { get; private set; }

    public NetFieldExportHandleAttribute(uint handle, RepLayoutCmdType type)
    {
        Handle = handle;
        Type = type;
    }

    public NetFieldExportHandleAttribute(uint handle, RepLayoutCmdType type, ParseMode minimalParseMode)
    {
        Handle = handle;
        Type = type;
        MinimalParseMode = minimalParseMode;
    }
}