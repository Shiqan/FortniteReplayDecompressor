using BenchmarkDotNet.Attributes;
using FortniteReplayReader;
using FortniteReplayReader.Models;
using Unreal.Core;

namespace BenchmarkReader;

[MemoryDiagnoser]
[SimpleJob]
public class BenchmarkReader
{
    private readonly ReplayReader _reader;

    public BenchmarkReader()
    {
        var guidCache = new NetGuidCache();
        var parser = new NetFieldParser(guidCache);
        _reader = new ReplayReader(guidCache, parser);
    }

    [Benchmark]
    public FortniteReplay ReadOldReplay() => _reader.ReadReplay(@"F:\Projects\FortniteReplayCollection\_upload\season 11\UnsavedReplay-2019.10.16-19.40.49.replay", Unreal.Core.Models.Enums.ParseMode.Full);
}
