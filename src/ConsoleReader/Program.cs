using FortniteReplayReader;
using FortniteReplayReader.Models.NetFieldExports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Unreal.Core;
using Unreal.Core.Models;
using Unreal.Core.Models.Contracts;
using Unreal.Core.Models.Enums;

namespace ConsoleReader
{
    class Program
    {
        static void Main(string[] args)
        {
            //var test = new HelloWorldGenerated.RandomClass();
            //test.Hello();

            var netFieldParser = new Unreal.Core.NetFieldParserGenerated();
            //var result = netFieldParser.CreateType("/Game/Athena/Athena_GameState.Athena_GameState_C");

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
            var reader = new ReplayReader(netFieldParser, logger, ParseMode.Minimal);
            long total = 0;
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
                Console.WriteLine($"---- {replayFile} : done in {sw.ElapsedMilliseconds} milliseconds ----");
                total += sw.ElapsedMilliseconds;
            }
            Console.WriteLine($"total: {total / 1000} seconds ----");

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

    public interface IAdapter
    {
        INetFieldExportGroup GetData();
        void ReadField(string a, string b);

    }
}
