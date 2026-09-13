using BenchmarkDotNet.Attributes;
using Unreal.Core.Models;

namespace BenchmarkReader;

[MemoryDiagnoser]
[ShortRunJob]
public class BenchmarkPacketAndBunch
{
    private readonly Unreal.Core.BinaryReader _archive;
    private readonly byte[] _raw = new byte[64 * 1024];
    private readonly DataBunch _reusableBunch = new();

    public BenchmarkPacketAndBunch()
    {
        Random.Shared.NextBytes(_raw);
        _archive = new Unreal.Core.BinaryReader(_raw);
    }

    [Benchmark(Baseline = true)]
    public int Baseline_AllocatingPacketAndBunch()
    {
        var count = 0;
        _archive.Seek(0);
        for (var i = 0; i < 200; i++)
        {
            // Old: ReadBytes allocates new byte[] per packet
            var packetBytes = _archive.ReadBytes(256).ToArray();
            // Old: new DataBunch allocated per bunch
            var bunch = new DataBunch
            {
                PacketId = i,
                ChIndex = (uint)i
            };
            count += packetBytes.Length + (int)bunch.ChIndex;
        }
        return count;
    }

    [Benchmark]
    public int Optimized_ZeroCopyPacketAndReusableBunch()
    {
        var count = 0;
        _archive.Seek(0);
        for (var i = 0; i < 200; i++)
        {
            // Optimized: ReadMemory zero-copy slice
            var packetMemory = _archive.ReadMemory(256);
            // Optimized: Reuse _reusableBunch
            var bunch = _reusableBunch;
            bunch.PacketId = i;
            bunch.ChIndex = (uint)i;
            count += packetMemory.Length + (int)bunch.ChIndex;
        }
        return count;
    }
}
