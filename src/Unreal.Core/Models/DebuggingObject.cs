using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Models;

//Purely for debugging. It's not optimized
public class DebuggingObject : IProperty
{
    private enum VectorType { Normal, Vector10, Vector100, Quantize };
    private enum RotatorType { Byte, Short };

    public byte[] Bytes => AsByteArray();
    public int TotalBits { get; private set; }
    public bool? BoolValue => TotalBits == 1 ? _reader.PeekBit() : new bool?();
    public int? ShortValue => Bytes.Length == 2 ? BitConverter.ToInt16(Bytes, 0) : new short?();
    public float? FloatValue => Bytes.Length == 4 ? BitConverter.ToSingle(Bytes, 0) : new float?();
    public int? IntValue => Bytes.Length == 4 ? BitConverter.ToInt32(Bytes, 0) : new int?();
    public long? LongValue => Bytes.Length == 8 ? BitConverter.ToInt64(Bytes, 0) : new long?();
    public int? ByteValue => Bytes.Length == 1 ? Bytes[0] : new byte?();
    public uint IntPacked => AsIntPacked();
    public string NetId => AsNetId();
    public List<IProperty> PotentialProperties => AsPotentialPropeties();
    public List<DebuggingHandle> PossibleExport => AsExportHandle();

    public FVector QuantizeVector => AsVector(VectorType.Quantize);
    public FVector VectorNormal => AsVector(VectorType.Normal);
    public FVector Vector10 => AsVector(VectorType.Vector10);
    public FVector Vector100 => AsVector(VectorType.Vector100);
    public FRotator RotatorByte => AsRotator(RotatorType.Byte);
    public FRotator RotatorShort => AsRotator(RotatorType.Short);

    public FRepMovement RepMovement => AsRepMovement();
    public string AsciiString => Encoding.ASCII.GetString(Bytes);
    public string UnicodeString => Encoding.ASCII.GetString(Bytes);

    public string FString => AsFString();
    public string StaticName => AsStaticName();
    public object[] Array => AsArray();
    public bool DidError => _reader.IsError;

    private static readonly IEnumerable<Type> _propertyTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
        .Where(x => typeof(IProperty).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

    private NetBitReader _reader;

    public void Serialize(NetBitReader reader)
    {
        _reader = new NetBitReader(reader.ReadBits(reader.GetBitsLeft()))
        {
            EngineNetworkVersion = reader.EngineNetworkVersion
        };

        TotalBits = _reader.GetBitsLeft();
    }

    private byte[] AsByteArray()
    {
        _reader.Reset();

        return _reader.ReadBytes((int) Math.Ceiling(_reader.GetBitsLeft() / 8.0)).ToArray();
    }

    private string AsFString()
    {
        _reader.Reset();

        return _reader.ReadFString();
    }

    private string AsNetId()
    {
        _reader.Reset();

        var netId = _reader.SerializePropertyNetId();

        if (_reader.IsError || !_reader.AtEnd())
        {
            return null;
        }

        return netId;
    }

    private FRotator AsRotator(RotatorType type)
    {
        _reader.Reset();

        var rotator = type switch
        {
            RotatorType.Byte => _reader.ReadRotation(),
            RotatorType.Short => _reader.ReadRotationShort(),
            _ => null
        };

        if (_reader.IsError || !_reader.AtEnd())
        {
            return null;
        }

        return rotator;
    }

    private uint AsIntPacked()
    {
        _reader.Reset();
        return _reader.ReadIntPacked();
    }

    private FVector AsVector(VectorType type)
    {
        _reader.Reset();

        var tVector = type switch
        {
            VectorType.Normal => _reader.SerializePropertyVectorNormal(),
            VectorType.Vector10 => _reader.SerializePropertyVector10(),
            VectorType.Vector100 => _reader.SerializePropertyVector100(),
            VectorType.Quantize => _reader.SerializePropertyQuantizedVector(),
            _ => null
        };

        if (_reader.IsError || !_reader.AtEnd())
        {
            return null;
        }

        return tVector;
    }

    private FRepMovement AsRepMovement()
    {
        _reader.Reset();

        var repMovement = _reader.SerializeRepMovement();

        if (_reader.IsError || !_reader.AtEnd())
        {
            return default;
        }

        return repMovement;
    }

    private string AsStaticName()
    {
        _reader.Reset();

        var isHardcoded = _reader.ReadBoolean();

        if (isHardcoded)
        {
            uint nameIndex;
            if (_reader.EngineNetworkVersion < EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES)
            {
                nameIndex = _reader.ReadUInt32();
            }
            else
            {
                nameIndex = _reader.ReadIntPacked();
            }

            if (_reader.IsError || !_reader.AtEnd())
            {
                return null;
            }

            return ((UnrealNames) nameIndex).ToString();
        }

        var inString = _reader.ReadFString();
        var inNumber = _reader.ReadInt32();

        if (_reader.IsError || !_reader.AtEnd())
        {
            return null;
        }

        return inString;
    }

    private object[] AsArray()
    {
        _reader.Reset();

        var totalElements = _reader.ReadIntPacked();

        var data = new object[totalElements];

        while (true)
        {
            var index = _reader.ReadIntPacked();

            if (index == 0)
            {
                if (_reader.GetBitsLeft() == 8)
                {
                    var terminator = _reader.ReadIntPacked();

                    if (terminator != 0x00)
                    {
                        //Log error
                        return null;
                    }
                }

                if (_reader.IsError || !_reader.AtEnd())
                {
                    return null;
                }

                return data;

            }

            --index;

            if (index >= totalElements)
            {
                return null;
            }

            var handles = new List<DebuggingHandle>();

            while (true)
            {
                var debuggingHandle = new DebuggingHandle();

                var handle = _reader.ReadIntPacked();

                debuggingHandle.Handle = handle;

                if (handle == 0)
                {
                    break;
                }

                --handle;

                var numBits = _reader.ReadIntPacked();

                debuggingHandle.NumBits = numBits;

                var obj = new DebuggingObject();

                var tempReader = new NetBitReader(_reader.ReadBits(numBits))
                {
                    EngineNetworkVersion = _reader.EngineNetworkVersion
                };

                obj.Serialize(tempReader);

                data[index] = obj;

                handles.Add(debuggingHandle);
            }

            //Assume it's an export handle
            if (handles.Count > 0)
            {
                data[index] = handles;
            }
        }
    }

    private List<DebuggingHandle> AsExportHandle()
    {
        _reader.Reset();

        while (true)
        {
            var handle = _reader.ReadIntPacked();

            if (handle == 0)
            {
                if (_reader.IsError || !_reader.AtEnd())
                {
                    return null;
                }

                break;
            }

            handle--;

            var numBits = _reader.ReadIntPacked();

            var debuggingHandle = new DebuggingHandle
            {
                Handle = handle,
                NumBits = numBits
            };

            if (numBits == 0)
            {
                continue;
            }

            _reader.SkipBits((int) numBits);

            if (_reader.IsError)
            {
                return null;
            }
        }

        return null;
    }

    private List<IProperty> AsPotentialPropeties()
    {
        var possibleProperties = new List<IProperty>();

        foreach (var type in _propertyTypes)
        {
            if (type == typeof(DebuggingObject))
            {
                continue;
            }

            _reader.Reset();

            var iProperty = (IProperty) Activator.CreateInstance(type);

            iProperty.Serialize(_reader);

            if (_reader.IsError || !_reader.AtEnd())
            {
                continue;
            }

            possibleProperties.Add(iProperty);
        }

        return possibleProperties;
    }
}

public class DebuggingHandle
{
    public uint NumBits { get; set; }
    public uint Handle { get; set; }

    public override string ToString() => $"Handle: {Handle} NumBits: {NumBits}";
}