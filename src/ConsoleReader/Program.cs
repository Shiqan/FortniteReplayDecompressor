using FortniteReplayReader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using Unreal.Core.Models.Enums;

namespace ConsoleReader
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging(loggingBuilder => loggingBuilder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Error));
            var provider = serviceCollection.BuildServiceProvider();
            var logger = provider.GetService<ILogger<Program>>();

            //var localAppDataFolder = GetFolderPath(SpecialFolder.LocalApplicationData);
            //var replayFilesFolder = Path.Combine(localAppDataFolder, @"FortniteGame\Saved\Demos");
            var replayFilesFolder = @"F:\Projects\FortniteReplayCollection\_upload\season 11\";
            var replayFiles = Directory.EnumerateFiles(replayFilesFolder, "*.replay");

            var sw = new Stopwatch();
            var reader = new ReplayReader(logger, ParseMode.Minimal);
            foreach (var replayFile in replayFiles)
            {
                sw.Restart();
                try
                {
                    var replay = reader.ReadReplay(replayFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                sw.Stop();
                Console.WriteLine($"---- {replayFile} : done in {(sw.ElapsedMilliseconds / 1000)} seconds ----");
            }

            //var replayFile = "Replays/shootergame.replay";
            //var replayFile = "Replays/season6.10.replay";
            //var replayFile = "Replays/season11.11.replay";
            //var replayFile = "Replays/season11.31.replay";
            //var replayFile = "Replays/season11.replay";
            //var replayFile = "Replays/season12.replay";
            //var replayFile = "Replays/collectPickup.replay";

            //var sw = new Stopwatch();
            //sw.Start();
            //var reader = new ReplayReader(logger, ParseMode.Debug);
            //var replay = reader.ReadReplay(replayFile);
            //sw.Stop();

            //Console.WriteLine($"---- done in {(sw.ElapsedMilliseconds / 1000)} seconds ----");
            Console.ReadLine();
        }
    }
}
