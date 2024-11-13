﻿using FortniteReplayReader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using Unreal.Core.Models.Enums;
using static System.Environment;

// Set up dependency injection and logging services
var serviceCollection = new ServiceCollection()
    .AddLogging(loggingBuilder => loggingBuilder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Warning));
var provider = serviceCollection.BuildServiceProvider();
var logger = provider.GetService<ILogger<Program>>();

// Define the folder containing replay files
var localAppDataFolder = GetFolderPath(SpecialFolder.LocalApplicationData);
// var replayFilesFolder = Path.Combine(localAppDataFolder, @"FortniteGame\Saved\Demos");
var replayFilesFolder = @"C:\Users\ferro\Downloads\";
var replayFiles = Directory.EnumerateFiles(replayFilesFolder, "*.replay");

var sw = new Stopwatch();
long total = 0;

#if DEBUG
var reader = new ReplayReader(logger, ParseMode.Minimal);
#else
var reader = new ReplayReader(null, ParseMode.Minimal);
#endif

// Process each replay file
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
