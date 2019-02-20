using FortniteReplayReaderDecompressor;
using System.IO;
using static System.Environment;

namespace ConsoleReader
{
    class Program
    {
        static void Main(string[] args)
        {
            var localAppDataFolder = GetFolderPath(SpecialFolder.LocalApplicationData);
            var replayFilesFolder = Path.Combine(localAppDataFolder, @"FortniteGame\Saved\Demos");

            var replayFiles = Directory.EnumerateFiles(replayFilesFolder, "UnsavedReplay-2019.02.18-20.56.20.replay");

            foreach (var replayFile in replayFiles)
            {

                using (var stream = File.Open(replayFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new FortniteReplayDecompressor(stream))
                    {
                        var replay = reader.ReadFile();
                    }
                }
            }
        }
    }
}
