using BenchmarkDotNet.Attributes;
using Unreal.Core;
using Unreal.Core.Models;

namespace BenchmarkReader;

[MemoryDiagnoser]
[ShortRunJob]
public class BenchmarkVectorSerialization
{
    private readonly BitReader _reader;
    private readonly byte[] _data;

    public BenchmarkVectorSerialization()
    {
        // Construct bit stream where serialized bit = 1, packed vector follows
        _data = new byte[1000];
        for (var i = 0; i < _data.Length; i++) _data[i] = 0xFF;
        _reader = new BitReader(_data);
    }

    [Benchmark(Baseline = true)]
    public int Baseline_EagerDefaultVectorAllocation()
    {
        var sum = 0;
        _reader.Seek(0);
        for (var i = 0; i < 50; i++)
        {
            // Eagerly allocates new FVector(0, 0, 0)
            var defaultVec = new FVector(0, 0, 0);
            var bWasSerialized = _reader.ReadBit();
            var vec = bWasSerialized ? _reader.ReadPackedVector(10, 24) : defaultVec;
            sum += (int)vec.X;
        }
        return sum;
    }

    [Benchmark]
    public int Optimized_LazyVectorAllocation()
    {
        var sum = 0;
        _reader.Seek(0);
        for (var i = 0; i < 50; i++)
        {
            // Only allocates if NOT serialized
            var bWasSerialized = _reader.ReadBit();
            var vec = bWasSerialized ? _reader.ReadPackedVector(10, 24) : new FVector(0, 0, 0);
            sum += (int)vec.X;
        }
        return sum;
    }
}
