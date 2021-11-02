Introduction
=====

So you are looking for a way to parse your Fortnite replays and found this open source project.
So yes, we can parse your replays, zero cost (it's free) and fully extensible.

## Getting the library

.net 5.0 is required.

In order to get started, you'll have to add the [FortniteReplayReader](https://www.nuget.org/packages/FortniteReplayReader) Nuget package to your project.
```powershell
dotnet add package FortniteReplayReader
```

*Note* If you are looking at parsing other `replay` files than Fortnite, you can add [Unreal.Core](https://www.nuget.org/packages/Unreal.Core).


## First example

So let's start with some code, the "Hello world" example (of course).

``` csharp
var replayFile = "your-amazing-fortnite.replay";
var reader = new ReplayReader();
var replay = reader.ReadReplay(replayFile);
```

That's it.

## Parse Mode

The FortniteReplayDecompressor comes with a parse mode option that will effect how things work.

Depending on the type of information you are after, you may find that you need to pass along the `ParseMode` argument.

It is hard to say exactly which of these values you may need to set, as that obviously depends entirely on what you are trying to achieve. But for most people `Normal` should be sufficient.

+ `EventsOnly`
+ `Minimal`
+ `Normal`
+ `Full`

For example: 

``` csharp
var replayFile = "your-amazing-fortnite.replay";
var reader = new ReplayReader(parseMode: ParseMode.Full);
var replay = reader.ReadReplay(replayFile);
```
