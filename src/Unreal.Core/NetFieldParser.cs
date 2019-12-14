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
    public class NetFieldParser
    {
        public static bool IncludeOnlyMode { get; set; } = true;

        private static Dictionary<string, Type> _netFieldGroups = new Dictionary<string, Type>();
        private static Dictionary<Type, NetFieldGroupInfo> _netFieldGroupInfo = new Dictionary<Type, NetFieldGroupInfo>();
        private static Dictionary<Type, RepLayoutCmdType> _primitiveTypeLayout = new Dictionary<Type, RepLayoutCmdType>();
        public static Dictionary<string, HashSet<UnknownFieldInfo>> UnknownNetFields { get; private set; } = new Dictionary<string, HashSet<UnknownFieldInfo>>();

        public static bool HasNewNetFields => UnknownNetFields.Count > 0;

        private static CompiledLinqCache _linqCache = new CompiledLinqCache();

        static NetFieldParser()
        {
            var netFields = AppDomain.CurrentDomain.GetAssemblies().SelectMany(i => i.GetTypes()).Where(c => c.GetCustomAttribute<NetFieldExportGroupAttribute>() != null);

            foreach (var type in netFields)
            {
                var attribute = type.GetCustomAttribute<NetFieldExportGroupAttribute>();

                if (attribute != null)
                {
                    _netFieldGroups[attribute.Path] = type;
                }

                var info = new NetFieldGroupInfo();
                _netFieldGroupInfo[type] = info;

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

            //Type layout for dynamic arrays
            _primitiveTypeLayout.Add(typeof(bool), RepLayoutCmdType.PropertyBool);
            _primitiveTypeLayout.Add(typeof(byte), RepLayoutCmdType.PropertyByte);
            _primitiveTypeLayout.Add(typeof(ushort), RepLayoutCmdType.PropertyUInt16);
            _primitiveTypeLayout.Add(typeof(int), RepLayoutCmdType.PropertyInt);
            _primitiveTypeLayout.Add(typeof(uint), RepLayoutCmdType.PropertyUInt32);
            _primitiveTypeLayout.Add(typeof(ulong), RepLayoutCmdType.PropertyUInt64);
            _primitiveTypeLayout.Add(typeof(float), RepLayoutCmdType.PropertyFloat);
            _primitiveTypeLayout.Add(typeof(string), RepLayoutCmdType.PropertyString);

            var iPropertyTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(IProperty).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

            //Allows deserializing IProperty type arrays
            foreach (var iPropertyType in iPropertyTypes)
            {
                _primitiveTypeLayout.Add(iPropertyType, RepLayoutCmdType.Property);
            }

            _primitiveTypeLayout.Add(typeof(object), RepLayoutCmdType.Ignore);

            AddDefaultExportGroups();
        }

        private static void AddDefaultExportGroups()
        {
            //Player info
            //IncludedExportGroups.Add(typeof(FortPlayerState));
            //IncludedExportGroups.Add(typeof(PlayerPawnC));
            //IncludedExportGroups.Add(typeof(FortInventory));
            //IncludedExportGroups.Add(typeof(FortPickup));

            //Game state
            //IncludedExportGroups.Add(typeof(GameStateC));
            //IncludedExportGroups.Add(typeof(SafeZoneIndicatorC));
            //IncludedExportGroups.Add(typeof(AircraftC));

            //Supply drops / llamas
            //IncludedExportGroups.Add(typeof(SupplyDropC));
            //IncludedExportGroups.Add(typeof(SupplyDropLlamaC));
            //IncludedExportGroups.Add(typeof(SupplyDropBalloonC));

            //Projectiles
            //IncludedExportGroups.Add(typeof(BPrjBulletSniperC));
            //IncludedExportGroups.Add(typeof(BPrjBulletSniperHeavyC));
            //IncludedExportGroups.Add(typeof(BPrjLotusMustacheC));
            //IncludedExportGroups.Add(typeof(BPrjArrowExplodeOnImpactC));
            //IncludedExportGroups.Add(typeof(BPrjBulletSniperAutoChildC));

            //All weapons
            /*foreach(KeyValuePair<Type, NetFieldGroupInfo> type in _netFieldGroupInfo.Where(x => x.Value.Properties.Any(y => y.Key == "WeaponData")))
            {
                IncludedExportGroups.Add(type.Key);
            }*/
        }

        public static bool WillReadType(string group)
        {
            return _netFieldGroups.ContainsKey(group);
        }

        public static void ReadField(object obj, NetFieldExport export, NetFieldExportGroup exportGroup, uint handle, NetBitReader netBitReader)
        {
            var group = exportGroup.PathName;

            var fixedExportName = FixInvalidNames(export.Name);

            if (!_netFieldGroups.ContainsKey(group))
            {
                AddUnknownField(fixedExportName, export?.Type, group, handle, netBitReader);

                return;
            }

            var netType = _netFieldGroups[group];
            var netGroupInfo = _netFieldGroupInfo[netType];

            if (!netGroupInfo.Properties.ContainsKey(fixedExportName))
            {
                AddUnknownField(fixedExportName, export?.Type, group, handle, netBitReader);

                return;
            }

            var netFieldInfo = netGroupInfo.Properties[fixedExportName];

            //Update if it finds a higher bit count or an actual type
            if (!string.IsNullOrEmpty(export.Type))
            {
                if (string.IsNullOrEmpty(netFieldInfo.Attribute.Info.Type))
                {
                    AddUnknownField(fixedExportName, export?.Type, group, handle, netBitReader);
                }
            }
            /*else if(netFieldInfo.Attribute.Info.BitCount < netBitReader.GetBitsLeft())
            {
                if(String.IsNullOrEmpty(netFieldInfo.Attribute.Info.Type))
                {
                    AddUnknownField(fixedExportName, export?.Type, group, handle, netBitReader);
                }
            }*/

            SetType(obj, netType, netFieldInfo, exportGroup, netBitReader);
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
                    netBitReader.Seek(netBitReader.Position + netBitReader.GetBitsLeft());
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
                case RepLayoutCmdType.Pointer:
                    switch (netBitReader.GetBitsLeft())
                    {
                        case 8:
                            data = (uint)netBitReader.ReadByte();
                            break;
                        case 16:
                            data = (uint)netBitReader.ReadUInt16();
                            break;
                        case 32:
                            data = netBitReader.ReadUInt32();
                            break;
                    }
                    break;
                case RepLayoutCmdType.PropertyVector:
                    data = new FVector(netBitReader.ReadSingle(), netBitReader.ReadSingle(), netBitReader.ReadSingle());
                    break;
                case RepLayoutCmdType.Ignore:
                    netBitReader.Seek(netBitReader.Position + netBitReader.GetBitsLeft());
                    break;
            }

            return data;
        }

        private static void SetType(object obj, Type netType, NetFieldInfo netFieldInfo, NetFieldExportGroup exportGroup, NetBitReader netBitReader)
        {
            object data;

            switch (netFieldInfo.Attribute.Type)
            {
                case RepLayoutCmdType.DynamicArray:
                    data = ReadArrayField(obj, exportGroup, netFieldInfo, netBitReader);
                    break;
                default:
                    data = ReadDataType(netFieldInfo.Attribute.Type, netBitReader, netFieldInfo.PropertyInfo.PropertyType);
                    break;
            }

            if (data != null)
            {
                var typeAccessor = TypeAccessor.Create(netType);
                typeAccessor[obj, netFieldInfo.PropertyInfo.Name] = data;
            }
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/5677c544747daa1efc3b5ede31642176644518a6/Engine/Source/Runtime/Engine/Private/RepLayout.cpp#L3141
        /// </summary>
        private static Array ReadArrayField(object obj, NetFieldExportGroup netfieldExportGroup, NetFieldInfo fieldInfo, NetBitReader netBitReader)
        {
            var arrayIndexes = netBitReader.ReadIntPacked();

            var elementType = fieldInfo.PropertyInfo.PropertyType.GetElementType();
            RepLayoutCmdType replayout = RepLayoutCmdType.Ignore;

            NetFieldGroupInfo groupInfo = null;

            if (_netFieldGroupInfo.ContainsKey(elementType))
            {
                groupInfo = _netFieldGroupInfo[elementType];
            }
            else
            {
                if (!_primitiveTypeLayout.TryGetValue(elementType, out replayout))
                {
                    replayout = RepLayoutCmdType.Ignore;
                }
            }

            var arr = Array.CreateInstance(elementType, arrayIndexes);

            while (true)
            {
                var index = netBitReader.ReadIntPacked();

                if (index == 0)
                {
                    if (netBitReader.GetBitsLeft() == 8)
                    {
                        var terminator = netBitReader.ReadIntPacked();

                        if (terminator != 0x00)
                        {
                            //Log error

                            return arr;
                        }
                    }

                    return arr;
                }

                --index;

                if (index >= arrayIndexes)
                {
                    //Log error

                    return arr;
                }

                object data = null;

                if (groupInfo != null)
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

                    //Uses the same type for the array
                    if (groupInfo != null)
                    {
                        ReadField(data, export, netfieldExportGroup, handle, cmdReader);
                    }
                    else //Probably primitive values
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

            return (INetFieldExportGroup)_linqCache.CreateObject(_netFieldGroups[group]);
        }

        public static void GenerateFiles(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }

            Directory.CreateDirectory(directory);

            foreach (var netFieldGroundKvp in _netFieldGroups)
            {
                if (!_netFieldGroupInfo.TryGetValue(netFieldGroundKvp.Value, out var groupInfo))
                {
                    continue;
                }

                foreach (var netFieldInfo in groupInfo.Properties)
                {
                    AddUnknownField(netFieldGroundKvp.Key, netFieldInfo.Value.Attribute.Info);
                }
            }


            foreach (var kvp in UnknownNetFields)
            {
                var fileName = string.Join("", kvp.Key.Split('/').Last().Split('.').Last().Split("_")).Replace("Athena", "");
                fileName = FixInvalidNames(fileName);

                if (char.IsDigit(fileName[0]))
                {
                    var firstCharacter = fileName.ToList().FindIndex(x => !char.IsDigit(x));

                    fileName = fileName.Substring(firstCharacter);
                }


                var builder = new StringBuilder();
                builder.AppendLine("using System.Collections.Generic;");
                builder.AppendLine("using Unreal.Core.Attributes;");
                builder.AppendLine("using Unreal.Core.Contracts;");
                builder.AppendLine("using Unreal.Core.Models.Enums;\n");
                builder.AppendLine("namespace Unreal.Core.Models");
                builder.AppendLine("{");
                builder.AppendLine($"\t[NetFieldExportGroup(\"{kvp.Key}\")]");
                builder.AppendLine($"\tpublic class {fileName} : INetFieldExportGroup");
                builder.AppendLine("\t{");

                foreach (var unknownField in kvp.Value.OrderBy(x => x.Handle))
                {
                    RepLayoutCmdType commandType = RepLayoutCmdType.Ignore;
                    var type = "object";

                    if (!string.IsNullOrEmpty(unknownField.Type))
                    {
                        //8, 16, or 32
                        if (unknownField.Type.EndsWith("*") || unknownField.Type.StartsWith("TSubclassOf"))
                        {
                            type = "uint?";
                            commandType = RepLayoutCmdType.Pointer;
                        }
                        else if (unknownField.Type.StartsWith("TEnumAsByte"))
                        {
                            type = "int?";
                            commandType = RepLayoutCmdType.Enum;
                        }
                        else if (unknownField.Type.StartsWith("E") && unknownField.Type.Length > 1 && char.IsUpper(unknownField.Type[1]))
                        {
                            type = "int?";
                            commandType = RepLayoutCmdType.Enum;
                        }
                        else
                        {
                            switch (unknownField.Type)
                            {
                                case "TArray":
                                    type = "object[]";
                                    commandType = RepLayoutCmdType.DynamicArray;
                                    break;
                                case "FRotator":
                                    type = "FRotator";
                                    commandType = RepLayoutCmdType.PropertyRotator;
                                    break;
                                case "float":
                                    type = "float?";
                                    commandType = RepLayoutCmdType.PropertyFloat;
                                    break;
                                case "bool":
                                    type = "bool?";
                                    commandType = RepLayoutCmdType.PropertyBool;
                                    break;
                                case "int8":
                                    if (unknownField.BitCount == 1)
                                    {
                                        type = "bool?";
                                        commandType = RepLayoutCmdType.PropertyBool;
                                    }
                                    else
                                    {
                                        type = "byte?";
                                        commandType = RepLayoutCmdType.PropertyByte;
                                    }
                                    break;
                                case "uint8":
                                    if (unknownField.BitCount == 1)
                                    {
                                        type = "bool?";
                                        commandType = RepLayoutCmdType.PropertyBool;
                                    }
                                    else
                                    {
                                        type = "byte?";
                                        commandType = RepLayoutCmdType.PropertyByte;
                                    }
                                    break;
                                case "int16":
                                    type = "ushort?";
                                    commandType = RepLayoutCmdType.PropertyUInt16;
                                    break;
                                case "uint16":
                                    type = "ushort?";
                                    commandType = RepLayoutCmdType.PropertyUInt16;
                                    break;
                                case "uint32":
                                    type = "uint?";
                                    commandType = RepLayoutCmdType.PropertyUInt32;
                                    break;
                                case "int32":
                                    type = "int?";
                                    commandType = RepLayoutCmdType.PropertyInt;
                                    break;
                                case "FUniqueNetIdRepl":
                                    type = "string";
                                    commandType = RepLayoutCmdType.PropertyNetId;
                                    break;
                                case "FHitResult":
                                case "FGameplayTag":
                                case "FText":
                                case "FVector2D":
                                case "FAthenaPawnReplayData":
                                case "FDateTime":
                                case "FName":
                                case "FQuat":
                                case "FVector":
                                case "FQuantizedBuildingAttribute":
                                    type = unknownField.Type;
                                    commandType = RepLayoutCmdType.Property;
                                    break;
                                case "FVector_NetQuantize":
                                    type = "FVector";
                                    commandType = RepLayoutCmdType.PropertyVectorQ;
                                    break;
                                case "FVector_NetQuantize10":
                                    type = "FVector";
                                    commandType = RepLayoutCmdType.PropertyVector10;
                                    break;
                                case "FVector_NetQuantizeNormal":
                                    type = "FVector";
                                    commandType = RepLayoutCmdType.PropertyVectorNormal;
                                    break;
                                case "FVector_NetQuantize100":
                                    type = "FVector";
                                    commandType = RepLayoutCmdType.PropertyVector100;
                                    break;
                                case "FString":
                                    type = "string";
                                    commandType = RepLayoutCmdType.PropertyString;
                                    break;
                                case "FRepMovement":
                                    type = "FRepMovement";
                                    commandType = RepLayoutCmdType.RepMovement;
                                    break;
                                case "FMinimalGameplayCueReplicationProxy":
                                    type = "int?";
                                    commandType = RepLayoutCmdType.Enum;
                                    break;
                                default:
                                    //Console.WriteLine(unknownField.Type);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        switch (unknownField.BitCount)
                        {
                            case 1:
                                type = "bool?";
                                commandType = RepLayoutCmdType.PropertyBool;
                                break;
                            case 8: //Can't determine if it's a pointer that can have 8, 16, or 32 bits
                            case 16:
                            case 32:
                                type = "uint?";
                                commandType = RepLayoutCmdType.PropertyUInt32;
                                break;
                        }
                    }

                    var fixedPropertyName = FixInvalidNames(unknownField.PropertyName);

                    builder.AppendLine($"\t\t[NetFieldExport(\"{unknownField.PropertyName}\", RepLayoutCmdType.{commandType.ToString()}, {unknownField.Handle}, \"{unknownField.PropertyName}\", \"{unknownField.Type}\", {unknownField.BitCount})]");
                    builder.AppendLine($"\t\tpublic {type} {fixedPropertyName} {{ get; set; }} //Type: {unknownField.Type} Bits: {unknownField.BitCount}\n");
                }

                builder.AppendLine("\t}");
                builder.AppendLine("}");

                var cSharpFile = builder.ToString();

                File.WriteAllText(Path.Combine(directory, fileName) + ".cs", cSharpFile);
            }
        }

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

        private static void AddUnknownField(string group, UnknownFieldInfo fieldInfo)
        {
            var fields = new HashSet<UnknownFieldInfo>();

            if (!UnknownNetFields.TryAdd(group, fields))
            {
                UnknownNetFields.TryGetValue(group, out fields);
            }

            fields.Add(fieldInfo);
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