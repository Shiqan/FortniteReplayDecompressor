using FortniteReplayReader.Contracts;

namespace FortniteReplayReader.Models.TelemetryEvents
{
    public class GameStateEvent : ITelemetryEvent
    {
        public float? ReplicatedWorldTimeSeconds { get; set; }

        public int? GameplayState { get; set; }
        public int? GamePhase { get; set; }
        public int? TeamsLeft { get; set; }
        public int? PlayersLeft { get; set; }
        public int? PlayerBotsLeft { get; set; }
        public int? SafeZonePhase { get; set; }
    }
}
