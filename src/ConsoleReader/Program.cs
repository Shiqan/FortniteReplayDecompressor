using FortniteReplayReaderDecompressor;
using System;
using System.IO;

namespace ConsoleReader
{
    class Program
    {
        static void Main(string[] args)
        {
            //var replayFile = "Replays/UnsavedReplay-2018.10.06-22.00.32.replay";
            //var replayFile = "Replays/UnsavedReplay-2019.04.05-20.22.58.replay";
            var replayFile = "Replays/UnsavedReplay-2019.05.03-21.24.46.replay";
            //var replayFile = "Replays/UnsavedReplay-2019.05.22-16.58.41.replay";
            using (var stream = File.Open(replayFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new FortniteBinaryDecompressor(stream))
                {
                    var replay = reader.ReadFile();
                }
            }
            Console.WriteLine("---- done ----");
            Console.ReadLine();
        }
    }
}
