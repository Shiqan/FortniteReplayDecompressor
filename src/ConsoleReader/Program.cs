using FortniteReplayReader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

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
            //var replayFile = "Replays/season6.10.replay";
            //var replayFile = "Replays/season11.11.replay";
            //var replayFile = "Replays/season11.31.replay";
            var replayFile = "Replays/season11.replay";
            //var replayFile = "Replays/season12.replay";
            //var replayFile = "Replays/collectPickup.replay";

            var sw = new Stopwatch();
            sw.Start();
            var reader = new ReplayReader(logger);
            var replay = reader.ReadReplay(replayFile, Unreal.Core.Models.Enums.ParseMode.Debug);
            sw.Stop();

            Console.WriteLine($"---- done in {(sw.ElapsedMilliseconds / 1000)} seconds ----");
            Console.ReadLine();
        }
    }
}
