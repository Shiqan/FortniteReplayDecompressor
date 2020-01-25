using FortniteReplayReader.Models.NetFieldExports;
using System;
using System.Collections.Generic;
using Unreal.Core.Models;

namespace FortniteReplayReader.Models
{
    public class GameData
    {
        public string GameSessionId { get; set; }
        public DateTime UtcTimeStartedMatch { get; set; }
        public string MapInfo { get; set; }
        public string CurrentPlaylist { get; set; }
        public IEnumerable<string> AdditionalPlaylistLevels { get; set; }
        public IEnumerable<string> ActiveGameplayModifiers { get; set; }

        public int TeamCount { get; set; }
        public int TeamSize { get; set; }
        public int TotalPlayerStructures { get; set; }

        public bool IsTournamentRound { get; set; }
        public bool IsLargeTeamGame { get; set; }

        public float AircraftStartTime { get; set; }
        public float SafeZonesStartTime { get; set; }

        public Aircraft[] TeamFlightPaths { get; set; }
        public FVector FlightStartLocation { get; set; }
        public FRotator FlightStartRotation { get; set; }
        public float TimeTillFlightEnd { get; set; }
        public float TimeTillDropStart { get; set; }
        public float TimeTillDropEnd { get; set; }
    }
}
