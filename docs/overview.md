Overview
=====

![](images/replay-overview.png)

## Unreal Engine Replay System
Unreal Engine 4 (UE4) features a [Replay system](https://docs.unrealengine.com/en-US/Engine/Replay/index.html) which can record gameplay for later viewing. This feature is available in all games, from live, multiplayer games played on dedicated servers, to single-player games, and even including Play-In-Editor sessions. At a high level, the Replay system works by using a DemoNetDriver to read data drawn from the built-in replication system, similar to how a NetDriver operates in a live, networked gameplay environment. Even if a project doesn't have a multiplayer mode, any project set up to replicate data is compatible with the Replay System without the need for further modification. The [DemoNetDriver](https://docs.unrealengine.com/en-US/Engine/Replay/Streamers/index.html) is a specialized network driver that passes replicated data to Streamers, which record the information needed to create replays. There are several different Streamers included with the Engine that can be attached to the DemoNetDriver depending on how the replay data is intended to be viewed. Fortnite uses the Local File Streamer. The Local File Streamer records replay data asynchronously to a single file on local storage (such as a hard drive). This is the default streamer as of its introduction in Engine version 4.20, replacing the Null Streamer. The Local File Streamer's single-file output makes distribution and management of saved replays easier, and its asynchronous recording improves performance on systems with lower hard drive speeds, such as consoles. Replay data files are saved to the "Saved/Demos/" folder in your project, and have the extension ".replay".

## Replay format
In terms of data, a replay starts with [metadata](https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L183). The [rest of the replay](https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Private/LocalFileNetworkReplayStreaming.cpp#L243) consists of so called [chunks](https://github.com/EpicGames/UnrealEngine/blob/27e7396206a1b3778d357cd67b93ec718ffb0dad/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Public/LocalFileNetworkReplayStreaming.h#L19). 
These chunks contains three types of game-state information, as well as some additional text data. The first chunk of the replay is baseline data describing the starting state of the game world. Checkpoints that act as snapshots of the net changes to the world (from the baseline) appear at regular, user-defined intervals. The space between checkpoints is then filled with incremental changes to individual objects in the world. Any moment in the game can be reconstructed by the engine quickly and accurately by initializing the world to the starting state, making the changes described in the checkpoints before the chosen time, and then applying each incremental change after the most recent checkpoint leading up to the desired point in time. The text data contained in a replay consists of a display name, which can be used when making a player-facing list, and user-defined text tags (HTTP Streamer only), which can be used when searching or filtering through lists of games. Some of the events included in Fortnite replays are statistics for the player, general statistics for the match and kill feed events.


## Compression
The checkpoints and replay data are compressed using Oodle. [Oodle](http://www.radgametools.com/oodle.htm) is an SDK for high performance lossless data compression and is used to compress the network data to reduce bandwith. As a result, [that data in the replay is compressed as well](https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Plugins/Runtime/PacketHandlers/CompressionComponents/Oodle/Source/OodleHandlerComponent/Private/OodleArchives.cpp#L21). You can uncompress this data by using the Oodle.dll included in the Fortnite folder.

## ReplayData
The incremental changes between checkpoints is called Replaydata. These incremental changes contain the (slightly changed) network packets that are send over the wire along with extra information for each packet, such as required objects to load. Each packet itself contains bunches, which are small updates for actor channels. In these bunches additional objects are loaded, and are the properties that should be replicated in the game world. Replication is done via property replication and function call replication (rpc).

## Kaitai Struct
The information above can be quite daunting, to actually visualze the explanation above we can use the [Kaitai Struct WebIDE](https://ide.kaitai.io/) (source code of Kaitai available [here](https://github.com/kaitai-io/kaitai_struct_webide)). The Kaitai struct uses a `.ksy` file, and [Kuinox](https://github.com/Kuinox) created one for Fortnite.
So download the [fortnite.ksy](fortnite.ksy) and drag & drop it in the web ide, after that just drag any fortnite replay to the hex viewer and see the structure for yourself. Make sure to drop by our Discord and thank Kuinox personally!


---

## Further Reading

If you are looking for more information around this area, the [Unreal Networking Overview](https://docs.unrealengine.com/udk/Three/NetworkingOverview.html) page is a great place to get started.