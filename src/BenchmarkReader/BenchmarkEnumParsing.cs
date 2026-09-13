using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Unreal.Core.Models.Enums;

namespace BenchmarkReader;

[MemoryDiagnoser]
[ShortRunJob]
public class BenchmarkEnumParsing
{
    private readonly byte[] _bytes = [1, 2, 3, 0, 2, 1, 3, 0];

    [Benchmark(Baseline = true)]
    public int Baseline_EnumToObject()
    {
        var sum = 0;
        for (var i = 0; i < _bytes.Length; i++)
        {
            var val = _bytes[i];
            var reason = (ChannelCloseReason)Enum.ToObject(typeof(ChannelCloseReason), val);
            sum += (int)reason;
        }
        return sum;
    }

    [Benchmark]
    public int Optimized_UnsafeAs()
    {
        var sum = 0;
        for (var i = 0; i < _bytes.Length; i++)
        {
            ref var val = ref _bytes[i];
            var reason = Unsafe.As<byte, ChannelCloseReason>(ref val);
            sum += (int)reason;
        }
        return sum;
    }
}
