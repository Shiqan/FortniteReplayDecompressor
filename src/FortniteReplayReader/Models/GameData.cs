using System;
using System.Collections.Generic;

namespace FortniteReplayReader.Models;

public class GameData
{
    public string GameSessionId { get; set; }
    public DateTime? UtcTimeStartedMatch { get; set; }
    public float? MatchEndTime { get; set; }
    public string MapInfo { get; set; }
    public string CurrentPlaylist { get; set; }
    public IEnumerable<string> AdditionalPlaylistLevels { get; set; }
    public IList<string> ActiveGameplayModifiers { get; set; } = new List<string>();

    /// <summary>
    /// Actor Id of recording player.
    /// </summary>
    public uint? RecorderId { get; set; }

    public int? MaxPlayers { get; set; }
    public int? TotalTeams { get; set; }
    public int? TotalBots { get; set; }
    public int? TeamSize { get; set; }
    public int? TotalPlayerStructures { get; set; }

    public bool IsTournamentRound => TournamentRound > 0;
    public int? TournamentRound { get; set; }
    public bool? IsLargeTeamGame { get; set; }

    public float? AircraftStartTime { get; set; }
    public float? SafeZonesStartTime { get; set; }

    public uint? WinningTeam { get; set; }
    public IEnumerable<int> WinningPlayerIds { get; set; }
}
