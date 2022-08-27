using FortniteReplayReader;
using FortniteReplayReader.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Unreal.Core;
using Unreal.Core.Models.Enums;
using Unreal.Core.Options;

namespace ConsoleReader;

internal class Program
{
    private static void Main(string[] args)
    {
        var serviceCollection = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Warning));

        //serviceCollection.UseFortniteReplayReader(netFieldParserOptions: o =>
        //{
        //    o.AddNetFieldExportGroupsFromAssembly<ReplayReader>();
        //});

        serviceCollection.UseFortniteReplayReader<ReplayReader>();

        var provider = serviceCollection.BuildServiceProvider();

        var logger = provider.GetService<ILogger<Program>>();
        var reader = provider.GetRequiredService<ReplayReader>();

        //var localAppDataFolder = GetFolderPath(SpecialFolder.LocalApplicationData);
        //var replayFilesFolder = Path.Combine(localAppDataFolder, @"FortniteGame\Saved\Demos");
        var replayFilesFolder = @"C:\Users\ferro\Downloads";

        var replayFiles = Directory.EnumerateFiles(replayFilesFolder, "*.replay");

        var sw = new Stopwatch();
        long total = 0;
        foreach (var replayFile in replayFiles)
        {
            sw.Restart();
            try
            {
                var replay = reader.ReadReplay(replayFile, ParseMode.Full);
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
