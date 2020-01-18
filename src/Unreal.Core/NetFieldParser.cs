using FastMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace Unreal.Core
{
    /// <summary>
    /// Responsible for parsing received properties to the correct <see cref="Type"/> and setting the parsed value on the created object.
    /// Only parses the properties marked with <see cref="NetFieldExportAttribute"/>.
    /// </summary>
    public partial class NetFieldParser
    {
        private readonly ParseMode Mode;
        private readonly NetGuidCache GuidCache;
        public HashSet<string> PlayerControllerGroups { get; private set; } = new HashSet<string>();

        private Dictionary<string, NetFieldGroupInfo> _netFieldGroups = new Dictionary<string, NetFieldGroupInfo>();
        private Dictionary<Type, RepLayoutCmdType> _primitiveTypeLayout = new Dictionary<Type, RepLayoutCmdType>();
        private Dictionary<string, string> _functionToNetFieldGroup = new Dictionary<string, string>();
        private Dictionary<string, ClassNetCacheInfo> _classNetCacheToNetFieldGroup = new Dictionary<string, ClassNetCacheInfo>();
        private CompiledLinqCache _linqCache = new CompiledLinqCache();

        public NetFieldParser(NetGuidCache cache, ParseMode mode)
        {
            Mode = mode;
            GuidCache = cache;

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(i => i.GetTypes());
            var netFields = types.Where(c => c.GetCustomAttribute<NetFieldExportGroupAttribute>() != null);

            foreach (var type in netFields)
            {
                var attribute = type.GetCustomAttribute<NetFieldExportGroupAttribute>();
                if (attribute.MinimalParseMode <= mode)
                {
                    var info = new NetFieldGroupInfo
                    {
                        Type = type
                    };

                    _netFieldGroups[attribute.Path] = info;
                    AddNetFieldInfo(type, info);
                }
            }

            // Allows deserializing type arrays
            var netSubFields = types.Where(c => c.GetCustomAttribute<NetFieldExportSubGroupAttribute>() != null);
            foreach (var type in netSubFields)
            {
                var attribute = type.GetCustomAttribute<NetFieldExportSubGroupAttribute>();
                if (attribute.MinimalParseMode <= mode)
                {
                    var info = _netFieldGroups[attribute.Path];
                    AddNetFieldInfo(type, info);
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

            //Type layout for dynamic arrays
            _primitiveTypeLayout.Add(typeof(bool), RepLayoutCmdType.PropertyBool);
            _primitiveTypeLayout.Add(typeof(byte), RepLayoutCmdType.PropertyByte);
            _primitiveTypeLayout.Add(typeof(ushort), RepLayoutCmdType.PropertyUInt16);
            _primitiveTypeLayout.Add(typeof(int), RepLayoutCmdType.PropertyInt);
            _primitiveTypeLayout.Add(typeof(uint), RepLayoutCmdType.PropertyUInt32);
            _primitiveTypeLayout.Add(typeof(ulong), RepLayoutCmdType.PropertyUInt64);
            _primitiveTypeLayout.Add(typeof(float), RepLayoutCmdType.PropertyFloat);
            _primitiveTypeLayout.Add(typeof(string), RepLayoutCmdType.PropertyString);

            // Allows deserializing type arrays
            var iPropertyTypes = types.Where(x => typeof(IProperty).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
            foreach (var iPropertyType in iPropertyTypes)
            {
                _primitiveTypeLayout.Add(iPropertyType, RepLayoutCmdType.Property);
            }

            _primitiveTypeLayout.Add(typeof(object), RepLayoutCmdType.Ignore);

        }

        private void AddNetFieldInfo(Type type, NetFieldGroupInfo info)
        {
            foreach (var property in type.GetProperties())
            {
                var netFieldExportAttribute = property.GetCustomAttribute<NetFieldExportAttribute>();
                var handleAttribute = property.GetCustomAttribute<NetFieldExportHandleAttribute>();

                if (netFieldExportAttribute == null && handleAttribute == null)
                {
                    continue;
                }

                if (netFieldExportAttribute != null)
                {
                    info.Properties[netFieldExportAttribute.Name] = new NetFieldInfo
                    {
                        MovementAttribute = property.GetCustomAttribute<RepMovementAttribute>(),
                        Attribute = netFieldExportAttribute,
                        PropertyInfo = property
                    };
                }
                else
                {
                    info.UsesHandles = true;
                    info.Handles[handleAttribute.Handle] = new NetFieldInfo
                    {
                        MovementAttribute = property.GetCustomAttribute<RepMovementAttribute>(),
                        Attribute = handleAttribute,
                        PropertyInfo = property
                    };
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
        public bool WillReadType(string group)
        {
            return _netFieldGroups.ContainsKey(group);
        }

        /// <summary>
        /// Returns whether or not this <paramref name="group"/> is marked to be parsed.
        /// </summary>
        /// <param name="group"></param>
        /// <returns>true if group should be parsed further, false otherwise</returns>
        public bool WillReadClassNetCache(string group)
        {
            return _classNetCacheToNetFieldGroup.ContainsKey(group);
        }

        /// <summary>
        /// Returns whether or not the property of this classnetcache was found.
        /// </summary>
        /// <param name="group"></param>
        /// <returns>true if classnetcache property was found, false otherwise</returns>
        public bool TryGetClassNetCacheProperty(string property, string group, out ClassNetCachePropertyInfo info)
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
        public void ReadField(object obj, NetFieldExport export, uint handle, NetFieldExportGroup exportGroup, NetBitReader netBitReader)
        {
            var group = exportGroup.PathName;

            if (!_netFieldGroups.TryGetValue(group, out var netGroupInfo))
            {
                return;
            }

            NetFieldInfo netFieldInfo;
            if (netGroupInfo.UsesHandles)
            {
                if (!netGroupInfo.Handles.TryGetValue(handle, out netFieldInfo))
                {
                    return;
                }
            }
            else
            {
                if (!netGroupInfo.Properties.TryGetValue(export.Name, out netFieldInfo))
                {
                    return;
                }
            }

            SetType(obj, exportGroup, netFieldInfo, netBitReader);
        }

        private void SetType(object obj, NetFieldExportGroup exportGroup, NetFieldInfo netFieldInfo, NetBitReader netBitReader)
        {
            var data = netFieldInfo.Attribute.Type switch
            {
                RepLayoutCmdType.DynamicArray => ReadArrayField(exportGroup, netFieldInfo, netBitReader),
                RepLayoutCmdType.RepMovement => netFieldInfo.MovementAttribute != null ? netBitReader.SerializeRepMovement(
                    locationQuantizationLevel: netFieldInfo.MovementAttribute.LocationQuantizationLevel,
                    rotationQuantizationLevel: netFieldInfo.MovementAttribute.RotationQuantizationLevel,
                    velocityQuantizationLevel: netFieldInfo.MovementAttribute.VelocityQuantizationLevel) : netBitReader.SerializeRepMovement(),
                _ => ReadDataType(netFieldInfo.Attribute.Type, netBitReader, netFieldInfo.PropertyInfo.PropertyType),
            };

            if (data != null)
            {
                var typeAccessor = TypeAccessor.Create(obj.GetType());
                typeAccessor[obj, netFieldInfo.PropertyInfo.Name] = data;
            }
        }

        private object ReadDataType(RepLayoutCmdType replayout, NetBitReader netBitReader, Type objectType = null)
        {
            object data = null;

            switch (replayout)
            {
                case RepLayoutCmdType.Property:
                    data = _linqCache.CreateObject(objectType);
                    (data as IProperty).Serialize(netBitReader);
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
                    data = (byte)netBitReader.ReadBitsToInt(netBitReader.GetBitsLeft());
                    break;
                // TODO Auto generation fix to handle 1-32 bits.
                case RepLayoutCmdType.PropertyInt:
                    data = netBitReader.ReadBitsToInt(netBitReader.GetBitsLeft());
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
        private Array ReadArrayField(NetFieldExportGroup netfieldExportGroup, NetFieldInfo fieldInfo, NetBitReader netBitReader)
        {
            var arrayIndexes = netBitReader.ReadIntPacked();

            var elementType = fieldInfo.PropertyInfo.PropertyType.GetElementType();
            var isPrimitveType = _primitiveTypeLayout.TryGetValue(elementType, out var replayout);

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

                object data = null;

                if (!isPrimitveType)
                {
                    data = _linqCache.CreateObject(elementType);
                }

                while (true)
                {
                    var handle = netBitReader.ReadIntPacked();

                    if (handle == 0)
                    {
                        break;
                    }

                    handle--;

                    if (netfieldExportGroup.NetFieldExports.Length < handle)
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
                        netBitReader.SkipBits((int)numBits);
                        continue;
                    }

                    var cmdReader = new NetBitReader(netBitReader.ReadBits(numBits))
                    {
                        EngineNetworkVersion = netBitReader.EngineNetworkVersion,
                        NetworkVersion = netBitReader.NetworkVersion
                    };

                    if (!isPrimitveType)
                    {
                        ReadField(data, export, handle, netfieldExportGroup, cmdReader);
                    }
                    else
                    {
                        data = ReadDataType(replayout, cmdReader, elementType);
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
        public INetFieldExportGroup CreateType(string group)
        {
            if (group == null || !_netFieldGroups.TryGetValue(group, out var netfieldGroup))
            {
                return null;
            }

            return (INetFieldExportGroup)_linqCache.CreateObject(netfieldGroup.Type);
        }

        /// <summary>
        /// Create the object associated with the property that should be read.
        /// Used as a workaround for RPC structs.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IProperty CreatePropertyType(Type type)
        {
            return (IProperty)_linqCache.CreateObject(type);
        }

        private class NetFieldGroupInfo
        {
            public Type Type { get; set; }
            public bool UsesHandles { get; set; }
            public Dictionary<string, NetFieldInfo> Properties { get; set; } = new Dictionary<string, NetFieldInfo>();
            public Dictionary<uint, NetFieldInfo> Handles { get; set; } = new Dictionary<uint, NetFieldInfo>();
        }

        private class NetFieldInfo
        {
            public RepMovementAttribute MovementAttribute { get; set; }
            public NetFieldAttribute Attribute { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
        }

        public class ClassNetCachePropertyInfo
        {
            public PropertyInfo PropertyInfo { get; set; }
            public string Name { get; set; }
            public string PathName { get; set; }
            public bool EnablePropertyChecksum { get; set; }
            public bool IsFunction { get; set; }
            public bool IsCustomStruct { get; set; }
        }

        private class ClassNetCacheInfo
        {
            public Dictionary<string, ClassNetCachePropertyInfo> Properties { get; set; } = new Dictionary<string, ClassNetCachePropertyInfo>();
        }
    }
}