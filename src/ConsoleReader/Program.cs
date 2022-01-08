using FortniteReplayReader;
using FortniteReplayReader.Models.NetFieldExports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Unreal.Core;
using Unreal.Core.Contracts;
using Unreal.Core.Extensions;
using Unreal.Core.Models.Enums;

namespace ConsoleReader;

internal class Program
{
    private static void Main(string[] args)
    {
        var serviceCollection = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Warning));
        serviceCollection.AddSingleton<INetFieldParser, NetFieldParser>();
        serviceCollection.AddNetFieldExportGroup<Aircraft>();
        var provider = serviceCollection.BuildServiceProvider();
        var logger = provider.GetService<ILogger<Program>>();
        //var localAppDataFolder = GetFolderPath(SpecialFolder.LocalApplicationData);
        //var replayFilesFolder = Path.Combine(localAppDataFolder, @"FortniteGame\Saved\Demos");
        var replayFilesFolder = @"H:\Projects\FortniteReplayCollection\_upload\season 11\";
        var replayFiles = Directory.EnumerateFiles(replayFilesFolder, "*.replay");

        var sw = new Stopwatch();
#if DEBUG
        var reader = new ReplayReader(logger, ParseMode.Minimal);
#else
            var reader = new ReplayReader(null, ParseMode.Minimal);
#endif
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
    }
}
