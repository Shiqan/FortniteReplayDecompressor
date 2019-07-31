using FortniteReplayReaderDecompressor;
using System;
using System.IO;
using static System.Environment;

namespace ConsoleReader
{
    class Program
    {
        static void Main(string[] args)
        {
            //var localAppDataFolder = GetFolderPath(SpecialFolder.LocalApplicationData);
            //var replayFilesFolder = Path.Combine(localAppDataFolder, @"FortniteGame\Saved\Demos");
            ////var replayFilesFolder = @"D:\Projects\FortniteReplayCollection";
            //var replayFiles = Directory.EnumerateFiles(replayFilesFolder, "*.replay");

            //foreach (var replayFile in replayFiles)
            //{
            //    using (var stream = File.Open(replayFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //    {
            //        using (var reader = new FortniteBinaryDecompressor(stream))
            //        {
            //            var replay = reader.ReadFile();
            //        }
            //    }
            //}

            //var replayFile = "Replays/UnsavedReplay-2018.10.06-22.00.32.replay";
            //var replayFile = "Replays/UnsavedReplay-2019.04.05-20.22.58.replay";
            //var replayFile = "Replays/UnsavedReplay-2019.05.03-21.24.46.replay";
            //var replayFile = "Replays/UnsavedReplay-2019.05.22-16.58.41.replay";
            //var replayFile = "Replays/UnsavedReplay-2019.06.14-19.49.16.replay";
            var replayFile = "Replays/UnsavedReplay-2019.06.30-20.39.37.replay";
            //var replayFile = "Replays/00769AB3D5F45A5ED7B01553227A8A82E07CC592.replay";
            using (var stream = File.Open(replayFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var reader = new ReplayReader();
                reader.Read(stream);
            }

            Console.WriteLine("---- done ----");
            Console.ReadLine();
        }
    }
}
