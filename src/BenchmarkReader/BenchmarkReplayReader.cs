using BenchmarkDotNet.Attributes;
using FortniteReplayReader.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using Unreal.Core.Models.Enums;

namespace BenchmarkReader
{
    public class BenchmarkReplayReader : FortniteReplayReader.ReplayReader
    {
        public BenchmarkReplayReader(ILogger logger = null)
        {
            Replay = new FortniteReplay();
            _logger = logger;
        }

        [Benchmark]
        [Arguments("Replays/UnsavedReplay-2019.09.12-21.39.37.replay", ParseMode.Normal)]
        [Arguments("Replays/season6.10.replay", ParseMode.Normal)]
        [Arguments("Replays/season11.replay", ParseMode.Normal)]
        [Arguments("Replays/season12.replay", ParseMode.Normal)]
        public new FortniteReplay ReadReplay(string fileName, ParseMode mode = ParseMode.Minimal)
        {
            using var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream);
            return ReadReplay(archive, mode);
        }
    }
}
