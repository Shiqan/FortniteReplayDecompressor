using System.Diagnostics.CodeAnalysis;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Contracts;

/// <summary>
/// Responsible for parsing received properties to the correct <see cref="Type"/> and setting the parsed value on the created object.
/// Only parses the properties marked with <see cref="Unreal.Core.Attributes.NetFieldExportAttribute"/>.
/// </summary>
public interface INetFieldParser
{
    /// <summary>
    /// Create the object associated with the property that should be read.
    /// Used as a workaround for RPC structs.
    /// </summary>
    IProperty? CreatePropertyType(string group, string propertyName);

    /// <summary>
    /// Create the object associated with the NetFieldExportGroup. 
    /// </summary>
    INetFieldExportGroup? CreateType(string group);

    /// <summary>
    /// Tries to read the property and update the value accordingly.
    /// </summary>
    bool ReadField(INetFieldExportGroup obj, NetFieldExport export, uint handle, NetFieldExportGroup exportGroup, NetBitReader netBitReader, bool singleInstance = true);

    /// <summary>
    /// Returns whether or not the property of this classnetcache was found.
    /// </summary>
    bool TryGetClassNetCacheProperty(string property, string group, [NotNullWhen(true)] out NetFieldParser.ClassNetCachePropertyInfo? info);

    /// <summary>
    /// Returns whether or not this <paramref name="group"/> is marked to be parsed given the <see cref="ParseMode"/>.
    /// </summary>
    bool WillReadClassNetCache(string group, ParseMode mode);

    /// <summary>
    /// Returns whether or not this <paramref name="group"/> is marked to be parsed given the <see cref="ParseMode"/>.
    /// </summary>
    bool WillReadType(string group, ParseMode mode);

    /// <summary>
    /// Returns whether or not this <paramref name="group"/> is a PlayerController.<br/>
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/6c20d9831a968ad3cb156442bebb41a883e62152/Engine/Source/Runtime/Engine/Private/PlayerController.cpp#L1338"/>
    /// </summary>
    bool IsPlayerController(string group);

    /// <summary>
    /// Register types to be used for the parsing.
    /// For example <see cref="Unreal.Core.Attributes.NetFieldExportAttribute"/> or <see cref="Unreal.Core.Attributes.NetFieldExportClassNetCacheAttribute"/>.
    /// </summary>
    void RegisterTypes(IEnumerable<Type> types);

    /// <summary>
    /// Register a type to be used for the parsing.
    /// For example <see cref="Unreal.Core.Attributes.NetFieldExportAttribute"/> or <see cref="Unreal.Core.Attributes.NetFieldExportClassNetCacheAttribute"/>.
    /// </summary>
    void RegisterType(Type type);
}