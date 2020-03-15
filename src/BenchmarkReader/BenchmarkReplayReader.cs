using BenchmarkDotNet.Attributes;
using FortniteReplayReader.Models;
using System.IO;

namespace BenchmarkReader
{
    public class BenchmarkReplayReader : FortniteReplayReader.ReplayReader
    {
        public BenchmarkReplayReader()
        {
            Replay = new FortniteReplay();
        }

        [Benchmark]
        [Arguments("Replays/season6.10.replay")]
        [Arguments("Replays/season11.replay")]
        [Arguments("Replays/season12.replay")]
        public new FortniteReplay ReadReplay(string fileName)
        {
            using var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream);
            return ReadReplay(archive);
        }
    }
}
