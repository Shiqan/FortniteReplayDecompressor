# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
- More changes and improvements by [SL-x-TnT](https://github.com/SL-x-TnT)
- Interfaces to support proper DI
- Register types to be parsed with DI

## [2.2.3] - 2024-11-05
### Changed
- add support for Fortnite v32 (see [issue 58](https://github.com/Shiqan/FortniteReplayDecompressor/issues/58)) (by [SL-x-TnT](https://github.com/SL-x-TnT))

## [2.2.2] - 2024-03-31
### Changed
- fix `ParseElimination` (see [issue 55](https://github.com/Shiqan/FortniteReplayDecompressor/issues/55))

## [2.2.1] - 2024-03-09
### Changed
- fix possible infinite loop while parsing broken replays (by [diopolgg](https://github.com/diopolgg))

## [2.2.0] - 2023-11-18
### Changed
- support .net8

## [2.1.0] - 2023-09-10
### Changed
- support .net6

## [2.0.8] - 2023-09-10
### Changed
- add support for Fortnite v26

## [2.0.7] - 2023-09-10
### Changed
- add HISTORY_GAMESTATE_REPLCIATED_TIME_AS_DOUBLE (by [chrisai-dev](https://github.com/chrisai-dev))

## [2.0.6] - 2023-06-10
### Changed
- adding support for Fortnite v25

## [2.0.5] - 2022-12-04
### Changed
- support new network version

## [2.0.4] - 2022-10-04
### Changed
- fix issue in Oodle decompress (by [SL-x-TnT](https://github.com/SL-x-TnT))

## [2.0.3] - 2022-09-19
### Changed
- Fix new Large World Vectors in new Fortnite season (thanks to [bassner](https://github.com/bassner/) and [xNocken](https://github.com/xNocken/))

## [2.0.2] - 2022-01-28
### Changed
- Fix for NetFieldGroups that uses handles

## [2.0.1] - 2021-11-13
### Changed
- Fix issue where bActorIsOuter bit was read twice

## [2.0.0] - 2021-02-11
### Added
- Additional information on Player Elim event (by [SL-x-TnT](https://github.com/SL-x-TnT))
- PlayerNames on PlayerData (by [SL-x-TnT](https://github.com/SL-x-TnT) and [xNocken](https://github.com/xNocken/))

### Changed
- .Net 5.0
- Performance improvements (by [razfriman](https://github.com/razfriman/) and [SL-x-TnT](https://github.com/SL-x-TnT))
- IL delegates (by [SL-x-TnT](https://github.com/SL-x-TnT)))
- Moved Unreal.Encryption dependency to FortniteReplayReader

## [1.1.2] - 2020-08-09
### Added
- MatchEndTime to GameData

### Changed
- Small performance improvement by skipping already failed paths
- Fix bug with random cosmetics


## [1.1.1] - 2020-07-01
### Added
- Player locations on ParseMode.Full

## [1.1.0] - 2020-06-12
### Changed
- Performance improvements
- Add additional properties to PlayerData and TeamData

## [1.0.4] - 2020-04-26
### Changed
- Number of kills wasnt updating properly

## [1.0.3] - 2020-04-31
### Changed
- Nuget package dependencies

## [1.0.2] - 2020-03-31
### Changed
- Nuget package dependencies

## [1.0.1] - 2020-03-31
### Changed
- Winning Player Ids should be int

## [1.0.0] - 2020-03-29
### Added
- Unreal: Full support for Unreal Engine 4 replay parsing, written in .net core 3.1 with support for both Windows and Linux.
- Fortnite: Retrieve game data, player data, team data and some additional map information such as llama and supply drop locations.
- Fortnite: Knocks, eliminations and revives available in killfeed.


[Unreleased]: https://github.com/Shiqan/FortniteReplayDecompressor/branches
[1.0.0]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/1.0.0
[1.0.1]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/1.0.1
[1.0.2]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/1.0.2
[1.0.3]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/1.0.3
[1.0.4]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/1.0.4
[1.1.0]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/1.1.0
[1.1.1]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/1.1.1
[1.1.2]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/1.1.2
[2.0.0]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/2.0.0
[2.0.1]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/2.0.1
[2.0.2]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/2.0.2
[2.0.3]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/2.0.3
[2.0.4]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/2.0.4
[2.0.5]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/2.0.5
[2.0.6]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/2.0.6
[2.0.7]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/2.0.7
[2.0.8]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/2.0.8
[2.1.0]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/2.1.0
[2.2.0]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/2.2.0
[2.2.1]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/2.2.1
[2.2.2]: https://github.com/Shiqan/FortniteReplayDecompressor/releases/tag/2.2.2
