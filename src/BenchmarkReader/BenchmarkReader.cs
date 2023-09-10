using BenchmarkDotNet.Attributes;
using FortniteReplayReader;
using FortniteReplayReader.Models;

namespace BenchmarkReader;

[MemoryDiagnoser]
[SimpleJob]
public class BenchmarkReader
{
    public ReplayReader _reader = new(null, Unreal.Core.Models.Enums.ParseMode.Full);

    public BenchmarkReader()
    {

    }

    [Benchmark]
    public FortniteReplay ReadOldReplay() => _reader.ReadReplay(@"F:\Projects\FortniteReplayCollection\_upload\season 11\UnsavedReplay-2019.10.16-19.40.49.replay");
}
