using System.Buffers.Binary;
using BenchmarkDotNet.Attributes;

namespace BenchmarkReader;

[MemoryDiagnoser]
[ShortRunJob]
public class BenchmarkBinaryPrimitives
{
    private readonly ReadOnlyMemory<byte> _bytes;
    private readonly byte[] _raw = new byte[2048];

    public BenchmarkBinaryPrimitives()
    {
        Random.Shared.NextBytes(_raw);
        _bytes = _raw;
    }

    [Benchmark(Baseline = true)]
    public int Baseline_SliceMemoryAndBitConverter()
    {
        var sum = 0;
        var pos = 0;
        for (var i = 0; i < 50; i++)
        {
            // Old BinaryReader: Bytes.Slice(_position, N).Span + BitConverter
            var s = BitConverter.ToInt16(_bytes.Slice(pos, 2).Span);
            pos += 2;
            var val = BitConverter.ToInt32(_bytes.Slice(pos, 4).Span);
            pos += 4;
            var u = BitConverter.ToUInt32(_bytes.Slice(pos, 4).Span);
            pos += 4;
            var l = BitConverter.ToInt64(_bytes.Slice(pos, 8).Span);
            pos += 8;
            var b = _bytes.Slice(pos, 1).Span[0];
            pos += 1;
            var f = BitConverter.ToSingle(_bytes.Slice(pos, 4).Span);
            pos += 4;
            sum += s + val + (int)u + (int)l + b + (int)f;
        }
        return sum;
    }

    [Benchmark]
    public int Optimized_CachedSpanAndBinaryPrimitives()
    {
        var sum = 0;
        var pos = 0;
        var span = _bytes.Span;
        for (var i = 0; i < 50; i++)
        {
            // Optimized BinaryReader: Bytes.Span.Slice(_position, N) + BinaryPrimitives
            var s = BinaryPrimitives.ReadInt16LittleEndian(span.Slice(pos, 2));
            pos += 2;
            var val = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(pos, 4));
            pos += 4;
            var u = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(pos, 4));
            pos += 4;
            var l = BinaryPrimitives.ReadInt64LittleEndian(span.Slice(pos, 8));
            pos += 8;
            var b = span[pos++];
            var f = BinaryPrimitives.ReadSingleLittleEndian(span.Slice(pos, 4));
            pos += 4;
            sum += s + val + (int)u + (int)l + b + (int)f;
        }
        return sum;
    }
}
