using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace Unreal.Core;

/// <summary>
/// Responsible for parsing received properties to the correct <see cref="Type"/> and setting the parsed value on the created object.
/// Only parses the properties marked with <see cref="NetFieldExportAttribute"/>.
/// </summary>
public class NetFieldParser
{
    private readonly NetGuidCache GuidCache;
    public HashSet<string> PlayerControllerGroups { get; private set; } = new();

    //private readonly Dictionary<string, NetFieldGroupInfo> _netFieldGroups = new();
    private readonly KeyList<string, NetFieldGroupInfo> NetFieldGroups = new();
    private readonly SingleInstanceExport[] _objects;
    private readonly Dictionary<Type, RepLayoutCmdType> _primitiveTypeLayout = new();
    private readonly Dictionary<string, ClassNetCacheInfo> _classNetCacheToNetFieldGroup = new();
    private readonly CompiledLinqCache _linqCache = new();

    /// <summary>
    /// Create a NetFieldParser, which will load all <see cref="NetFieldExportGroup"/> in the <see cref="AppDomain.CurrentDomain"/>.
    /// </summary>
    /// <param name="cache">Instance of NetGuidCache, used to resolve netguids to their string value.</param>
    /// <param name="mode"></param>
    /// <param name="assemblyNameFilter">Found assemblies should contain this string.</param>
    public NetFieldParser(NetGuidCache cache, ParseMode mode, string assemblyNameFilter = "ReplayReader")
    {
        GuidCache = cache;

        var types = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName.Contains(assemblyNameFilter) || a.FullName.Contains("Unreal.Core"))
            .SelectMany(i => i.GetTypes())
            .ToList();

        // Add all netfieldexportgroups
        var netFields = types.Where(c => c.GetCustomAttribute<NetFieldExportGroupAttribute>() != null);
        foreach (var type in netFields)
        {
            var attribute = type.GetCustomAttribute<NetFieldExportGroupAttribute>();
            if (attribute.MinimalParseMode <= mode)
            {
                var info = new NetFieldGroupInfo
                {
                    Type = type,
                    TypeId = _linqCache.AddExportType(type),
                };

                NetFieldGroups.Add(attribute.Path, info);
                AddNetFieldInfo(type, info, mode);
            }
        }

        // Add all subgroups
        var netSubFields = types.Where(c => c.GetCustomAttribute<NetFieldExportSubGroupAttribute>() != null);
        foreach (var type in netSubFields)
        {
            var attribute = type.GetCustomAttribute<NetFieldExportSubGroupAttribute>();
            if (attribute.MinimalParseMode <= mode)
            {
                NetFieldGroups.TryGetValue(attribute.Path, out var info);
                AddNetFieldInfo(type, info, mode);
            }
        }

        // ClassNetCaches
        var classNetCaches = types.Where(c => c.GetCustomAttribute<NetFieldExportClassNetCacheAttribute>() != null);
        foreach (var type in classNetCaches)
        {
            var attribute = type.GetCustomAttribute<NetFieldExportClassNetCacheAttribute>();
            if (attribute.MinimalParseMode <= mode)
            {
                var info = new ClassNetCacheInfo();
                AddClassNetInfo(type, info);
                _classNetCacheToNetFieldGroup[attribute.Path] = info;
            }
        }

        // PlayerControllers
        var controllers = types.Where(c => c.GetCustomAttribute<PlayerControllerAttribute>() != null);
        foreach (var type in controllers)
        {
            var attribute = type.GetCustomAttribute<PlayerControllerAttribute>();
            PlayerControllerGroups.Add(attribute.Path);
        }

        // Type layout for dynamic arrays
        _primitiveTypeLayout.Add(typeof(bool), RepLayoutCmdType.PropertyBool);
        _primitiveTypeLayout.Add(typeof(byte), RepLayoutCmdType.PropertyByte);
        _primitiveTypeLayout.Add(typeof(ushort), RepLayoutCmdType.PropertyUInt16);
        _primitiveTypeLayout.Add(typeof(int), RepLayoutCmdType.PropertyInt);
        _primitiveTypeLayout.Add(typeof(uint), RepLayoutCmdType.PropertyUInt32);
        _primitiveTypeLayout.Add(typeof(ulong), RepLayoutCmdType.PropertyUInt64);
        _primitiveTypeLayout.Add(typeof(float), RepLayoutCmdType.PropertyFloat);
        _primitiveTypeLayout.Add(typeof(string), RepLayoutCmdType.PropertyString);
        _primitiveTypeLayout.Add(typeof(object), RepLayoutCmdType.Ignore);

        // Allows deserializing type arrays
        var iPropertyTypes = types.Where(x => typeof(IProperty).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
        foreach (var iPropertyType in iPropertyTypes)
        {
            _primitiveTypeLayout.Add(iPropertyType, RepLayoutCmdType.Property);
        }

        //Load object instances
        _objects = new SingleInstanceExport[_linqCache.TotalTypes];

        for (var i = 0; i < NetFieldGroups.Length; i++)
        {
            var group = NetFieldGroups[i];

            _objects[group.TypeId] = new SingleInstanceExport
            {
                Instance = (INetFieldExportGroup) Activator.CreateInstance(group.Type),
                ChangedProperties = new List<NetFieldInfo>(group.Properties.Length),
            };
        }
    }

    /// <summary>
    /// https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/
    /// </summary>
    private static Action<INetFieldExportGroup, object> CreateSetter(PropertyInfo propertyInfo)
    {
        var field = GetBackingField(propertyInfo);

        var methodName = field.ReflectedType.FullName + ".set_" + field.Name;
        var setterMethod = new DynamicMethod(methodName, null, new Type[2] { typeof(INetFieldExportGroup), typeof(object) }, true);
        var gen = setterMethod.GetILGenerator();
        if (field.IsStatic)
        {
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stsfld, field);
        }
        else
        {
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Unbox_Any, field.FieldType);
            gen.Emit(OpCodes.Stfld, field);
        }
        gen.Emit(OpCodes.Ret);

        return (Action<INetFieldExportGroup, object>) setterMethod.CreateDelegate(typeof(Action<INetFieldExportGroup, object>));

        FieldInfo GetBackingField(PropertyInfo property)
        {
            return property.DeclaringType.GetField($"<{property.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
        }
    }

    private void AddNetFieldInfo(Type type, NetFieldGroupInfo info, ParseMode mode)
    {
        foreach (var property in type.GetProperties())
        {
            var netFieldExportAttribute = property.GetCustomAttribute<NetFieldExportAttribute>();
            var handleAttribute = property.GetCustomAttribute<NetFieldExportHandleAttribute>();

            if (netFieldExportAttribute == null && handleAttribute == null)
            {
                continue;
            }

            if (netFieldExportAttribute != null && netFieldExportAttribute.MinimalParseMode != null && netFieldExportAttribute.MinimalParseMode > mode)
            {
                continue;
            }

            if (handleAttribute != null && handleAttribute.MinimalParseMode != null && handleAttribute.MinimalParseMode > mode)
            {
                continue;
            }

            int? elementTypeId = null;
            if (property.PropertyType.IsArray)
            {
                var elementType = property.PropertyType.GetElementType();

                if (typeof(INetFieldExportGroup).IsAssignableFrom(elementType))
                {
                    elementTypeId = _linqCache.AddExportType(elementType);
                }
            }

            var netFieldInfo = new NetFieldInfo
            {
                MovementAttribute = property.GetCustomAttribute<RepMovementAttribute>(),
                PropertyInfo = property,
                ElementTypeId = elementTypeId
            };

            if (property.PropertyType.IsEnum)
            {
                netFieldInfo.DefaultValue = Enum.GetValues(property.PropertyType).Cast<int>().Max();
            }
            else if (property.PropertyType.IsValueType)
            {
                netFieldInfo.DefaultValue = Activator.CreateInstance(property.PropertyType);
            }

            if (netFieldExportAttribute != null)
            {
                netFieldInfo.Attribute = netFieldExportAttribute;
                info.Properties.Add(netFieldExportAttribute.Name, netFieldInfo);
            }
            else
            {
                info.UsesHandles = true;
                netFieldInfo.Attribute = handleAttribute;
                info.Handles.Add(handleAttribute.Handle, netFieldInfo);
            }
        }
    }

    private void AddClassNetInfo(Type type, ClassNetCacheInfo info)
    {
        foreach (var property in type.GetProperties())
        {
            var structAttribute = property.GetCustomAttribute<NetFieldExportRPCAttribute>();
            if (structAttribute != null)
            {
                info.Properties[structAttribute.Name] = new ClassNetCachePropertyInfo()
                {
                    PropertyInfo = property,
                    Name = structAttribute.Name,
                    PathName = structAttribute.PathName,
                    IsFunction = structAttribute.IsFunction,
                    EnablePropertyChecksum = structAttribute.EnablePropertyChecksum,
                    IsCustomStruct = structAttribute.CustomStruct
                };
            }
        }
    }

    /// <summary>
    /// Returns whether or not this <paramref name="group"/> is marked to be parsed.
    /// </summary>
    /// <param name="group"></param>
    /// <returns>true if group should be parsed further, false otherwise</returns>
    public bool WillReadType(string group) => NetFieldGroups.TryGetValue(group, out var _);

    /// <summary>
    /// Returns whether or not this <paramref name="group"/> is marked to be parsed.
    /// </summary>
    /// <param name="group"></param>
    /// <returns>true if group should be parsed further, false otherwise</returns>
    public bool WillReadClassNetCache(string group) => _classNetCacheToNetFieldGroup.ContainsKey(group);

    /// <summary>
    /// Returns whether or not the property of this classnetcache was found.
    /// </summary>
    /// <param name="group"></param>
    /// <returns>true if classnetcache property was found, false otherwise</returns>
    public bool TryGetClassNetCacheProperty(string property, string group, [NotNullWhen(returnValue: true)] out ClassNetCachePropertyInfo? info)
    {
        info = null;
        if (_classNetCacheToNetFieldGroup.TryGetValue(group, out var groupInfo))
        {
            return groupInfo.Properties.TryGetValue(property, out info);
        }
        return false;
    }

    /// <summary>
    /// Tries to read the property and update the value accordingly.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="export"></param>
    /// <param name="handle"></param>
    /// <param name="exportGroup"></param>
    /// <param name="netBitReader"></param>
    public bool ReadField(INetFieldExportGroup obj, NetFieldExport export, uint handle, NetFieldExportGroup exportGroup, NetBitReader netBitReader, bool singleInstance = true)
    {
        if (export.PropertyId == -2 || exportGroup.GroupId == -2)
        {
            return false;
        }

        if (exportGroup.GroupId == -1)
        {
            if (!NetFieldGroups.TryGetIndex(exportGroup.PathName, out var groupIndex))
            {
                exportGroup.GroupId = -2;
                return false;
            }
            exportGroup.GroupId = groupIndex;
        }

        var netGroupInfo = NetFieldGroups[exportGroup.GroupId];
        NetFieldInfo netFieldInfo;
        if (netGroupInfo.UsesHandles)
        {
            if (!netGroupInfo.Handles.TryGetValue(handle, out netFieldInfo))
            {
                return false;
            }
        }
        else
        {
            var propertyIndex = export.PropertyId;
            if (export.PropertyId == -1)
            {
                if (!netGroupInfo.Properties.TryGetIndex(export.Name, out propertyIndex))
                {
                    export.PropertyId = -2;
                    return false;
                }

                export.PropertyId = propertyIndex;
            }

            netFieldInfo = netGroupInfo.Properties[propertyIndex];
        }

        SetType(obj, netFieldInfo, netGroupInfo, exportGroup, netBitReader, singleInstance);
        return true;
    }

    private void SetType(INetFieldExportGroup obj, NetFieldInfo netFieldInfo, NetFieldGroupInfo groupInfo, NetFieldExportGroup exportGroup, NetBitReader netBitReader, bool singleInstance)
    {
        var data = netFieldInfo.Attribute.Type switch
        {
            RepLayoutCmdType.DynamicArray => ReadArrayField(exportGroup, netFieldInfo, groupInfo, netBitReader),
            RepLayoutCmdType.RepMovement => netFieldInfo.MovementAttribute != null ? netBitReader.SerializeRepMovement(
                locationQuantizationLevel: netFieldInfo.MovementAttribute.LocationQuantizationLevel,
                rotationQuantizationLevel: netFieldInfo.MovementAttribute.RotationQuantizationLevel,
                velocityQuantizationLevel: netFieldInfo.MovementAttribute.VelocityQuantizationLevel) : netBitReader.SerializeRepMovement(),
            _ => ReadDataType(netFieldInfo.Attribute.Type, netBitReader, netFieldInfo.PropertyInfo.PropertyType),
        };

        if (data != null && !netBitReader.IsError)
        {
            if (singleInstance)
            {
                _objects[groupInfo.TypeId].ChangedProperties.Add(netFieldInfo);
            }

            if (netFieldInfo.SetMethod == null)
            {
                netFieldInfo.SetMethod = CreateSetter(netFieldInfo.PropertyInfo);
            }

            netFieldInfo.SetMethod(obj, data);
        }
    }

    private object? ReadDataType(RepLayoutCmdType replayout, NetBitReader netBitReader, Type objectType)
    {
        object? data = null;

        switch (replayout)
        {
            case RepLayoutCmdType.Property:
                data = _linqCache.CreatePropertyObject(objectType);
                (data as IProperty)?.Serialize(netBitReader);
                (data as IResolvable)?.Resolve(GuidCache);
                break;
            case RepLayoutCmdType.PropertyBool:
                data = netBitReader.SerializePropertyBool();
                break;
            case RepLayoutCmdType.PropertyName:
                data = netBitReader.SerializePropertyName();
                break;
            case RepLayoutCmdType.PropertyFloat:
                data = netBitReader.SerializePropertyFloat();
                break;
            case RepLayoutCmdType.PropertyDouble:
                data = netBitReader.SerializePropertyDouble();
                break;
            case RepLayoutCmdType.PropertyNativeBool:
                data = netBitReader.SerializePropertyNativeBool();
                break;
            case RepLayoutCmdType.PropertyNetId:
                data = netBitReader.SerializePropertyNetId();
                break;
            case RepLayoutCmdType.PropertyObject:
                data = netBitReader.SerializePropertyObject();
                break;
            case RepLayoutCmdType.PropertyPlane:
                throw new NotImplementedException("Plane RepLayoutCmdType not implemented");
            case RepLayoutCmdType.PropertyRotator:
                data = netBitReader.SerializePropertyRotator();
                break;
            case RepLayoutCmdType.PropertyString:
                data = netBitReader.SerializePropertyString();
                break;
            case RepLayoutCmdType.PropertyVector10:
                data = netBitReader.SerializePropertyVector10();
                break;
            case RepLayoutCmdType.PropertyVector100:
                data = netBitReader.SerializePropertyVector100();
                break;
            case RepLayoutCmdType.PropertyVectorNormal:
                data = netBitReader.SerializePropertyVectorNormal();
                break;
            case RepLayoutCmdType.PropertyVectorQ:
                data = netBitReader.SerializePropertyQuantizedVector(VectorQuantization.RoundWholeNumber);
                break;
            case RepLayoutCmdType.RepMovement:
                data = netBitReader.SerializeRepMovement();
                break;
            case RepLayoutCmdType.Enum:
                data = netBitReader.SerializePropertyEnum();
                break;
            // TODO Auto generation fix to handle 1-8 bits
            case RepLayoutCmdType.PropertyByte:
                data = netBitReader.ReadByte();
                break;
            case RepLayoutCmdType.PropertyInt:
                data = netBitReader.ReadInt32();
                break;
            case RepLayoutCmdType.PropertyInt16:
                data = netBitReader.ReadInt16();
                break;
            case RepLayoutCmdType.PropertyUInt64:
                data = netBitReader.ReadUInt64();
                break;
            case RepLayoutCmdType.PropertyUInt16:
                data = netBitReader.ReadUInt16();
                break;
            case RepLayoutCmdType.PropertyUInt32:
                data = netBitReader.ReadUInt32();
                break;
            case RepLayoutCmdType.PropertyVector:
                data = netBitReader.SerializePropertyVector();
                break;
            case RepLayoutCmdType.PropertyVector2D:
                data = netBitReader.SerializePropertyVector2D();
                break;
            default:
                netBitReader.Seek(netBitReader.GetBitsLeft(), System.IO.SeekOrigin.Current);
                break;
        }

        return data;
    }

    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/Engine/Private/RepLayout.cpp#L3141
    /// </summary>
    private Array ReadArrayField(NetFieldExportGroup netfieldExportGroup, NetFieldInfo fieldInfo, NetFieldGroupInfo groupInfo, NetBitReader netBitReader)
    {
        var arrayIndexes = netBitReader.ReadIntPacked();

        var elementType = fieldInfo.PropertyInfo.PropertyType.GetElementType();
        var replayout = RepLayoutCmdType.Ignore;
        var isGroupType = elementType == groupInfo.Type || elementType == groupInfo.Type.BaseType;

        if (!isGroupType)
        {
            groupInfo = null;

            if (!_primitiveTypeLayout.TryGetValue(elementType, out replayout))
            {
                replayout = RepLayoutCmdType.Ignore;
            }
        }

        if (replayout == RepLayoutCmdType.Ignore)
        {
            return null;
        }

        var arr = Array.CreateInstance(elementType, arrayIndexes);

        while (true)
        {
            var index = netBitReader.ReadIntPacked();

            if (index == 0)
            {
                // At this point, the 0 either signifies:
                // An array terminator, at which point we're done.
                // An array element terminator, which could happen if the array had tailing elements removed.
                if (netBitReader.GetBitsLeft() == 8)
                {
                    // We have bits left over, so see if its the Array Terminator.
                    // This should be 0
                    var terminator = netBitReader.ReadIntPacked();

                    if (terminator != 0x00)
                    {
                        //Log error

                        return arr;
                    }
                }

                return arr;
            }

            // Shift all indexes down since 0 represents null handle
            index--;

            if (index >= arrayIndexes)
            {
                //Log error
                return arr;
            }

            object? data = null;

            if (isGroupType)
            {
                data = _linqCache.CreateObject(fieldInfo.ElementTypeId.Value);
            }

            while (true)
            {
                var handle = netBitReader.ReadIntPacked();

                if (handle == 0)
                {
                    break;
                }

                handle--;

                if (netfieldExportGroup.NetFieldExportsLength < handle)
                {
                    return arr;
                }

                var export = netfieldExportGroup.NetFieldExports[handle];
                var numBits = netBitReader.ReadIntPacked();

                if (numBits == 0)
                {
                    continue;
                }

                if (export == null)
                {
                    netBitReader.SkipBits((int) numBits);
                    continue;
                }

                try
                {
                    netBitReader.SetTempEnd((int) numBits, FBitArchiveEndIndex.READ_ARRAY_FIELD);

                    if (groupInfo != null)
                    {
                        ReadField((INetFieldExportGroup) data, export, handle, netfieldExportGroup, netBitReader, singleInstance: false);
                    }
                    else
                    {
                        data = ReadDataType(replayout, netBitReader, elementType);
                    }
                }
                finally
                {
                    netBitReader.RestoreTempEnd(FBitArchiveEndIndex.READ_ARRAY_FIELD);
                }
            }

            arr.SetValue(data, index);
        }
    }

    /// <summary>
    /// Create the object associated with the NetFieldExportGroup. 
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public INetFieldExportGroup? CreateType(string group)
    {
        if (!NetFieldGroups.TryGetValue(group, out var exportGroup))
        {
            return null;
        }

        var cachedEntry = _objects[exportGroup.TypeId];

        for (var i = 0; i < cachedEntry.ChangedProperties.Count; i++)
        {
            var fieldInfo = cachedEntry.ChangedProperties[i];
            fieldInfo.SetMethod(cachedEntry.Instance, fieldInfo.DefaultValue);
        }

        cachedEntry.ChangedProperties.Clear();
        return cachedEntry.Instance;
    }

    /// <summary>
    /// Create the object associated with the property that should be read.
    /// Used as a workaround for RPC structs.
    /// </summary>
    public IProperty? CreatePropertyType(string group, string propertyName)
    {
        if (_classNetCacheToNetFieldGroup.TryGetValue(group, out var groupInfo))
        {
            if (groupInfo.Properties.TryGetValue(propertyName, out var fieldInfo))
            {
                return _linqCache.CreatePropertyObject(fieldInfo.PropertyInfo.PropertyType);
            }
        }
        return null;
    }

    private sealed class NetFieldGroupInfo
    {
        public Type Type { get; set; }
        public int TypeId { get; set; }
        public bool UsesHandles { get; set; }
        public KeyList<string, NetFieldInfo> Properties { get; set; } = new();
        public Dictionary<uint, NetFieldInfo> Handles { get; set; } = new();
    }

    private sealed class NetFieldInfo
    {
        public RepMovementAttribute? MovementAttribute { get; set; }
        public NetFieldAttribute Attribute { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public int? ElementTypeId { get; set; }
        public object DefaultValue { get; set; }
        public Action<INetFieldExportGroup, object> SetMethod { get; set; }
    }

    // TODO make private
    public sealed class ClassNetCachePropertyInfo
    {
        public PropertyInfo PropertyInfo { get; set; }
        public string Name { get; set; }
        public string PathName { get; set; }
        public bool EnablePropertyChecksum { get; set; }
        public bool IsFunction { get; set; }
        public bool IsCustomStruct { get; set; }
    }

    private sealed class ClassNetCacheInfo
    {
        public Dictionary<string, ClassNetCachePropertyInfo> Properties { get; set; } = new();
    }

    private sealed class SingleInstanceExport
    {
        //Single instance
        internal INetFieldExportGroup Instance { get; set; }
        internal List<NetFieldInfo> ChangedProperties { get; set; }
    }
}