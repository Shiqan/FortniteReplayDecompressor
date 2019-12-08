using FortniteReplayReader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ConsoleReader
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging(loggingBuilder => loggingBuilder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Warning));
            var provider = serviceCollection.BuildServiceProvider();
            var logger = provider.GetService<ILogger<Program>>();

            //var localAppDataFolder = GetFolderPath(SpecialFolder.LocalApplicationData);
            ////var replayFilesFolder = Path.Combine(localAppDataFolder, @"FortniteGame\Saved\Demos");
            //var replayFilesFolder = @"D:\Projects\FortniteReplayCollection";
            //var replayFiles = Directory.EnumerateFiles(replayFilesFolder, "*.replay");

            //foreach (var replayFile in replayFiles)
            //{
            //    var reader = new ReplayReader(logger);
            //    var replay = reader.ReadReplay(replayFile);
            //}

            //var replayFile = "Replays/shootergame.replay";
            var replayFile = "Replays/season6.10.replay";
            //var replayFile = "Replays/season11.11.replay";
            //var replayFile = "Replays/season11.replay";
            //var replayFile = "Replays/UnsavedReplay-2018.10.06-22.00.32.replay";
            //var replayFile = "Replays/UnsavedReplay-2019.04.05-20.22.58.replay";
            //var replayFile = "Replays/UnsavedReplay-2019.05.03-21.24.46.replay";
            //var replayFile = "Replays/UnsavedReplay-2019.05.22-16.58.41.replay";
            //var replayFile = "Replays/UnsavedReplay-2019.06.30-20.39.37.replay";
            //var replayFile = "Replays/UnsavedReplay-2019.09.12-21.39.37.replay";
            //var replayFile = "Replays/00769AB3D5F45A5ED7B01553227A8A82E07CC592.replay";

            var reader = new ReplayReader(logger);
            var replay = reader.ReadReplay(replayFile);

            Console.WriteLine("---- done ----");
            Console.ReadLine();
        }
    }
}
