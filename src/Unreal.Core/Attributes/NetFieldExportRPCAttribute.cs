using System;

namespace Unreal.Core.Attributes;

/// <summary>
/// Attribute to map a property to the specified path. Used for function RPC property replication.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class NetFieldExportRPCAttribute : Attribute
{
    public string Name { get; private set; }
    public string PathName { get; private set; }

    /// <summary>
    /// Indicates whether or not this RPC property should be parsed as a function.
    /// </summary>
    public bool IsFunction { get; private set; }

    /// <summary>
    /// Indicates whether or not this struct should be parsed directly when being read (instead of going through <see cref="ReplayReader{T}.ReceiveProperties(FBitArchive, Models.NetFieldExportGroup, uint, bool)"/>.
    /// </summary>
    public bool CustomStruct { get; private set; }

    /// <summary>
    /// Indicates whether or not the checksum bit should be read when receiving properties.
    /// </summary>
    public bool EnablePropertyChecksum { get; private set; }

    public NetFieldExportRPCAttribute(string name, string pathName, bool isFunction = false, bool enablePropertyChecksum = true, bool customStruct = false)
    {
        Name = name;
        PathName = pathName;
        IsFunction = isFunction;
        EnablePropertyChecksum = enablePropertyChecksum;
        CustomStruct = customStruct;
    }
}
