using System;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Attributes;

/// <summary>
/// Attribute to map a class to the specified path. Used for generic property replication.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class NetFieldExportGroupAttribute : Attribute
{
    public string Path { get; private set; }
    public ParseMode MinimalParseMode { get; private set; }

    public NetFieldExportGroupAttribute(string path, ParseMode minimalParseMode = ParseMode.Normal)
    {
        Path = path;
        MinimalParseMode = minimalParseMode;
    }
}
