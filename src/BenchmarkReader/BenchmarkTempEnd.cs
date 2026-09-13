using BenchmarkDotNet.Attributes;
using Unreal.Core.Models.Enums;

namespace BenchmarkReader;

[MemoryDiagnoser]
[ShortRunJob]
public class BenchmarkTempEnd
{
    private readonly Dictionary<FBitArchiveEndIndex, int> _dict = [];
    private readonly int[] _array = new int[4];

    private static readonly FBitArchiveEndIndex[] Operations =
    [
        FBitArchiveEndIndex.BUNCH,
        FBitArchiveEndIndex.CONTENT_BLOCK_PAYLOAD,
        FBitArchiveEndIndex.FIELD_HEADER_PAYLOAD,
        FBitArchiveEndIndex.READ_ARRAY_FIELD
    ];

    [Benchmark(Baseline = true)]
    public int Baseline_DictionaryLookup()
    {
        var sum = 0;
        for (var i = 0; i < 1000; i++)
        {
            var op = Operations[i & 3];
            _dict[op] = i * 10;
            sum += _dict[op];
        }
        return sum;
    }

    [Benchmark]
    public int Optimized_ArrayLookup()
    {
        var sum = 0;
        for (var i = 0; i < 1000; i++)
        {
            var op = (int)Operations[i & 3];
            _array[op] = i * 10;
            sum += _array[op];
        }
        return sum;
    }
}
