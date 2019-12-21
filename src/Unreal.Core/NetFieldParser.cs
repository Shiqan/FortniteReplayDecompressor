using FastMember;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace Unreal.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class NetFieldParser
    {
        public static ParseMode Mode { get; set; }
        public static bool IsDebugMode => Mode == ParseMode.Debug;

        private static Dictionary<string, NetFieldGroupInfo> _netFieldGroups = new Dictionary<string, NetFieldGroupInfo>();
        private static Dictionary<Type, RepLayoutCmdType> _primitiveTypeLayout = new Dictionary<Type, RepLayoutCmdType>();
        public static Dictionary<string, HashSet<UnknownFieldInfo>> UnknownNetFields { get; private set; } = new Dictionary<string, HashSet<UnknownFieldInfo>>();

        public static bool HasNewNetFields => UnknownNetFields.Count > 0;

        private static CompiledLinqCache _linqCache = new CompiledLinqCache();

        static NetFieldParser()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(i => i.GetTypes());
            var netFields = types.Where(c => c.GetCustomAttribute<NetFieldExportGroupAttribute>() != null);

            foreach (var type in netFields)
            {
                var attribute = type.GetCustomAttribute<NetFieldExportGroupAttribute>();
                var info = new NetFieldGroupInfo
                {
                    Type = type
                };

                _netFieldGroups[attribute.Path] = info;

                foreach (var property in type.GetProperties())
                {
                    var netFieldExportAttribute = property.GetCustomAttribute<NetFieldExportAttribute>();

                    if (netFieldExportAttribute == null)
                    {
                        continue;
                    }

                    info.Properties[netFieldExportAttribute.Name] = new NetFieldInfo
                    {
                        Attribute = netFieldExportAttribute,
                        PropertyInfo = property
                    };
                }
            }

            var netSubFields = types.Where(c => c.GetCustomAttribute<NetFieldExportSubGroupAttribute>() != null);
            foreach (var type in netSubFields)
            {
                var attribute = type.GetCustomAttribute<NetFieldExportSubGroupAttribute>();
                foreach (var property in type.GetProperties())
                {
                    var netFieldExportAttribute = property.GetCustomAttribute<NetFieldExportAttribute>();

                    if (netFieldExportAttribute == null)
                    {
                        continue;
                    }

                    var info = _netFieldGroups[attribute.Path];

                    info.Properties[netFieldExportAttribute.Name] = new NetFieldInfo
                    {
                        Attribute = netFieldExportAttribute,
                        PropertyInfo = property
                    };
                }
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

            var iPropertyTypes = types.Where(x => typeof(IProperty).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

            //Allows deserializing IProperty type arrays
            foreach (var iPropertyType in iPropertyTypes)
            {
                _primitiveTypeLayout.Add(iPropertyType, RepLayoutCmdType.Property);
            }

            _primitiveTypeLayout.Add(typeof(object), RepLayoutCmdType.Ignore);
        }

        public static bool WillReadType(string group)
        {
            return _netFieldGroups.ContainsKey(group);
        }

        public static void ReadField(object obj, NetFieldExport export, NetFieldExportGroup exportGroup, uint handle, NetBitReader netBitReader)
        {
            var group = exportGroup.PathName;

            var fixedExportName = FixInvalidNames(export.Name);

            if (!_netFieldGroups.TryGetValue(group, out var netGroupInfo))
            {
                if (IsDebugMode)
                {
                    AddUnknownField(fixedExportName, export?.Type, group, handle, netBitReader);
                }

                return;
            }

            if (!netGroupInfo.Properties.ContainsKey(fixedExportName))
            {
                if (IsDebugMode)
                {
                    AddUnknownField(fixedExportName, export?.Type, group, handle, netBitReader);
                }
                return;
            }

            var netFieldInfo = netGroupInfo.Properties[fixedExportName];

            // Update if it finds an actual type
            if (IsDebugMode && !string.IsNullOrEmpty(export.Type) && string.IsNullOrEmpty(netFieldInfo.Attribute.Info.Type))
            {
                AddUnknownField(fixedExportName, export?.Type, group, handle, netBitReader);
            }

            SetType(obj, exportGroup, netGroupInfo, netFieldInfo, netBitReader);
        }

        private static object ReadDataType(RepLayoutCmdType replayout, NetBitReader netBitReader, Type objectType = null)
        {
            object data = null;

            switch (replayout)
            {
                case RepLayoutCmdType.Property:
                    data = _linqCache.CreateObject(objectType);
                    (data as IProperty).Serialize(netBitReader);
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
                //Auto generation fix to handle 1-8 bits
                case RepLayoutCmdType.PropertyByte:
                    data = (byte)netBitReader.ReadBitsToInt(netBitReader.GetBitsLeft());
                    break;
                //Auto generation fix to handle 1-32 bits. 
                case RepLayoutCmdType.PropertyInt:
                    data = netBitReader.ReadBitsToInt(netBitReader.GetBitsLeft());
                    break;
                case RepLayoutCmdType.PropertyUInt64:
                    data = netBitReader.ReadUInt64();
                    break;
                case RepLayoutCmdType.PropertyUInt16:
                    data = (ushort)netBitReader.ReadBitsToInt(netBitReader.GetBitsLeft());
                    break;
                case RepLayoutCmdType.PropertyUInt32:
                    data = netBitReader.ReadUInt32();
                    break;
                case RepLayoutCmdType.PropertyVector:
                    data = netBitReader.SerializePropertyVector();
                    break;
                case RepLayoutCmdType.Ignore:
                    netBitReader.Seek(netBitReader.Position + netBitReader.GetBitsLeft());
                    break;
            }

            return data;
        }

        private static void SetType(object obj, NetFieldExportGroup exportGroup, NetFieldGroupInfo groupInfo, NetFieldInfo netFieldInfo, NetBitReader netBitReader)
        {
            var data = netFieldInfo.Attribute.Type switch
            {
                RepLayoutCmdType.DynamicArray => ReadArrayField(exportGroup, groupInfo, netFieldInfo, netBitReader),
                _ => ReadDataType(netFieldInfo.Attribute.Type, netBitReader, netFieldInfo.PropertyInfo.PropertyType),
            };

            if (data != null)
            {
                var typeAccessor = TypeAccessor.Create(obj.GetType());
                typeAccessor[obj, netFieldInfo.PropertyInfo.Name] = data;
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/Engine/Private/RepLayout.cpp#L3141
        /// </summary>
        private static Array ReadArrayField(NetFieldExportGroup netfieldExportGroup, NetFieldGroupInfo groupInfo, NetFieldInfo fieldInfo, NetBitReader netBitReader)
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
                    //	An array terminator, at which point we're done.
                    //	An array element terminator, which could happen if the array had tailing elements removed.
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
                        ReadField(data, export, netfieldExportGroup, handle, cmdReader);
                    }
                    else
                    {
                        data = ReadDataType(replayout, cmdReader, elementType);
                    }
                }

                arr.SetValue(data, index);
            }
        }

        public static INetFieldExportGroup CreateType(string group)
        {
            if (!_netFieldGroups.ContainsKey(group))
            {
                return null;
            }

            return (INetFieldExportGroup)_linqCache.CreateObject(_netFieldGroups[group].Type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static unsafe string FixInvalidNames(string str)
        {
            var len = str.Length;
            var newChars = stackalloc char[len];
            var currentChar = newChars;

            for (var i = 0; i < len; ++i)
            {
                var c = str[i];

                var isDigit = (c ^ '0') <= 9;

                var val = (byte)((c & 0xDF) - 0x40);
                var isChar = val > 0 && val <= 26;

                if (isDigit || isChar)
                {
                    *currentChar++ = c;
                }
            }

            return new string(newChars, 0, (int)(currentChar - newChars));
        }

        private static void AddUnknownField(string exportName, string exportType, string group, uint handle, NetBitReader netBitReader)
        {
            var fields = new HashSet<UnknownFieldInfo>();

            if (!UnknownNetFields.TryAdd(group, fields))
            {
                UnknownNetFields.TryGetValue(group, out fields);
            }

            fields.Add(new UnknownFieldInfo(exportName, exportType, netBitReader.GetBitsLeft(), handle));

        }

        private class NetFieldGroupInfo
        {
            public Type Type { get; set; }
            public Dictionary<string, NetFieldInfo> Properties { get; set; } = new Dictionary<string, NetFieldInfo>();
        }

        private class NetFieldInfo
        {
            public NetFieldExportAttribute Attribute { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
        }
    }

    public class UnknownFieldInfo
    {
        public string PropertyName { get; set; }
        public string Type { get; set; }
        public int BitCount { get; set; }
        public uint Handle { get; set; }

        public UnknownFieldInfo(string propertyname, string type, int bitCount, uint handle)
        {
            PropertyName = propertyname;
            Type = type;
            BitCount = bitCount;
            Handle = handle;
        }

        public override bool Equals(object obj)
        {
            var fieldInfo = obj as UnknownFieldInfo;

            if (fieldInfo == null)
            {
                return base.Equals(obj);
            }

            return fieldInfo.PropertyName == PropertyName;
        }

        public override int GetHashCode()
        {
            return PropertyName.GetHashCode();
        }
    }
}