using BenchmarkDotNet.Attributes;
using FortniteReplayReader;
using FortniteReplayReader.Models;

namespace BenchmarkReader;

[MemoryDiagnoser]
[ShortRunJob]
public class BenchmarkReader
{
    private readonly ReplayReader _reader = new(null, Unreal.Core.Models.Enums.ParseMode.Full);
    private readonly string _replayPath;

    public BenchmarkReader()
    {
        var primary = @"H:\Projects\FortniteReplayDecompressor\src\ConsoleReader\Replays\chapter2season6_10.replay";
        var relative = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"../../../../src/ConsoleReader/Replays/chapter2season6_10.replay"));

        _replayPath = File.Exists(primary) ? primary : relative;
    }

    [Benchmark]
    public FortniteReplay ReadReplay() => _reader.ReadReplay(_replayPath);
}
