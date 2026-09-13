using System.Buffers.Binary;
using BenchmarkDotNet.Attributes;
using Unreal.Core;

namespace BenchmarkReader;

[MemoryDiagnoser]
[ShortRunJob]
public class BenchmarkBitReaderPrimitives
{
    private readonly BitReader _reader;
    private readonly byte[] _data = new byte[1024];

    public BenchmarkBitReaderPrimitives()
    {
        Random.Shared.NextBytes(_data);
        _reader = new BitReader(_data);
    }

    [Benchmark(Baseline = true)]
    public int Baseline_BitByBitLoop()
    {
        var result = 0;
        var bitCount = 24;
        for (var run = 0; run < 100; run++)
        {
            var val = 0;
            for (var i = 0; i < bitCount; i++)
            {
                var bit = (_data[(run * 3) + (i >> 3)] & (1 << (i & 7))) != 0;
                if (bit) val |= (1 << i);
            }
            result ^= val;
        }
        return result;
    }

    [Benchmark]
    public int Optimized_UInt64Shift()
    {
        var result = 0;
        var bitCount = 24;
        var mask = (1UL << bitCount) - 1UL;
        var span = _data.AsSpan();
        for (var run = 0; run < 100; run++)
        {
            var val = BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(run * 3, 8));
            var res = (int)(val & mask);
            result ^= res;
        }
        return result;
    }
}
