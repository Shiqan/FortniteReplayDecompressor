# FortniteReplayDecompressor

![FortniteReplayReader Nuget](https://img.shields.io/nuget/v/FortniteReplayReader?color=brightgreen)
[![Chat](https://img.shields.io/badge/chat-on%20discord-7289da.svg)](https://discord.gg/p5CMqJC)
![Build](https://github.com/Shiqan/FortniteReplayDecompressor/workflows/Build/badge.svg)
[![Documentation Status](https://readthedocs.org/projects/fortnitereplaydecompressor/badge/?version=latest)](https://fortnitereplaydecompressor.readthedocs.io/en/latest/?badge=latest)

C# parser for your Fortnite replays.

## Getting Started

.NET 8.0 or .NET 9.0 is required.

```powershell
dotnet add package FortniteReplayReader
```

```csharp
var replayFile = "your-amazing-fortnite.replay";
var reader = new ReplayReader();
var replay = reader.ReadReplay(replayFile);
```

## Documentation
Available at [readthedocs.org](https://fortnitereplaydecompressor.readthedocs.io/en/latest/?badge=latest). For any other question you can join our [Discord server](https://discord.gg/p5CMqJC)!

## Alternatives
- [xNocken](https://github.com/xNocken/replay-reader) created a [node version](https://www.npmjs.com/package/fortnite-replay-parser).

## Special thanks
Special thanks to [Kuinox](https://github.com/Kuinox/ChartsNite) for the collaboration to figure out the compression and structure of the replay file.

Special thanks to [ApertureC](https://github.com/ApertureC/) for the collaboration to figure out the UE4 network packets.

Special thanks to [AlpaGit](https://github.com/AlpaGit) for seeing the bits I did not see.

Special thanks to [SL-x-TnT](https://github.com/SL-x-TnT) for his amazing work on the NetFieldParser (among other things).

Special thanks to [SL-x-TnT](https://github.com/SL-x-TnT) because he deserves to be mentioned twice.

## License
Licensed under the [MIT License](LICENSE).
