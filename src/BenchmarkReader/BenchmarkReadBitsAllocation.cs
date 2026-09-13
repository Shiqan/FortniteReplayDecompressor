using BenchmarkDotNet.Attributes;
using Unreal.Core;

namespace BenchmarkReader;

[MemoryDiagnoser]
[ShortRunJob]
public class BenchmarkReadBitsAllocation
{
    private readonly BitReader _sourceReader;
    private readonly NetBitReader _destReader;
    private readonly byte[] _data = new byte[4096];

    public BenchmarkReadBitsAllocation()
    {
        Random.Shared.NextBytes(_data);
        _sourceReader = new BitReader(_data);
        _destReader = new NetBitReader();
    }

    [Benchmark(Baseline = true)]
    public int Baseline_ReadBits_AllocatingArray()
    {
        var sum = 0;
        _sourceReader.Seek(3); // Start unaligned
        for (var i = 0; i < 50; i++)
        {
            // Old: archive.ReadBits(27) allocates new byte[4] per property
            var bits = _sourceReader.ReadBits(27);
            sum += bits.Length;
        }
        return sum;
    }

    [Benchmark]
    public int Optimized_FillBitsFrom_ZeroAlloc()
    {
        var sum = 0;
        _sourceReader.Seek(3); // Start unaligned
        for (var i = 0; i < 50; i++)
        {
            // Optimized: _cmdReader.FillBitsFrom(archive, 27) into reusable buffer (0 alloc)
            _destReader.FillBitsFrom(_sourceReader, 27);
            sum += _destReader.GetBitsLeft();
        }
        return sum;
    }
}
