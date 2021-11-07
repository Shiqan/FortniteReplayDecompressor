using System;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Attributes;

/// <summary>
/// Attribute to map a class to the specified path as a subgroup of that path. Used for generic property replication.
/// The specified path should exist as a <see cref="NetFieldExportGroupAttribute"/> on another class.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class NetFieldExportSubGroupAttribute : Attribute
{
    public string Path { get; private set; }
    public ParseMode MinimalParseMode { get; private set; }

    public NetFieldExportSubGroupAttribute(string path, ParseMode minimalParseMode = ParseMode.Normal)
    {
        Path = path;
        MinimalParseMode = minimalParseMode;
    }
}
