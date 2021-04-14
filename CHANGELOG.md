# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [2.0.0] - 2021-
### Added
- Player Elim information on event chunk [SL-x-TnT](https://github.com/SL-x-TnT)

### Changed
- .Net 5.0
- Performance improvements [razfriman](https://github.com/razfriman/)
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