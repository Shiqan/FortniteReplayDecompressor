using FortniteReplayReader.Models.Events;
using System.Collections.Generic;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models;

public class FortniteReplay : Replay
{
    /// <summary>
    /// Eliminations found in the event chunks. See <see cref="KillFeed"/> for a much more detailed list of eliminations.
    /// </summary>
    public IList<PlayerElimination> Eliminations { get; internal set; } = new List<PlayerElimination>();

    /// <summary>
    /// Personal stats found in the event chunk.
    /// </summary>
    public Stats Stats { get; internal set; }

    /// <summary>
    /// Team stats found in the event chunk.
    /// </summary>
    public TeamStats TeamStats { get; internal set; }

    /// <summary>
    /// Generic game information found in the network chunks.
    /// </summary>
    public GameData GameData { get; internal set; } = new GameData();

    /// <summary>
    /// Team information
    /// </summary>
    public IEnumerable<TeamData> TeamData { get; internal set; }

    /// <summary>
    /// Player information
    /// </summary>
    public IEnumerable<PlayerData> PlayerData { get; internal set; }

    /// <summary>
    /// Eliminations based on the network chunks.
    /// </summary>
    public IList<KillFeedEntry> KillFeed { get; internal set; } = new List<KillFeedEntry>();

    /// <summary>
    /// Map information
    /// </summary>
    public MapData MapData { get; internal set; } = new MapData();
}
