Introduction
=====

So you are looking for a way to parse your Fortnite replays and found this open source project.
So yes, we can parse your replays, zero cost (it's free) and fully extensible.

## Getting the library

You cannot get the library yet, but it will be available on Nuget "soon". 

Until then you'll have to get the source code from Github. Once you have the code locally, you need to do just one little thing: add the `oo2core_5_win64.dll` (or your OS specific file extension) to the project.
You can find it in the Fornite installation directory.

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

+ `Minimal`
+ `Normal`
+ `Extended`
+ `Full`

For example: 

``` csharp
var replayFile = "your-amazing-fortnite.replay";
var reader = new ReplayReader();
var replay = reader.ReadReplay(replayFile, ParseMode.Full);
```