using FortniteReplayReader.Models;
using FortniteReplayReader.Models.NetFieldExports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FortniteReplayReader
{
    /// <summary>
    /// Responsible for constructing the <see cref="FortniteReplay"/> out of the received exports.
    /// </summary>
    public class FortniteReplayBuilder
    {
        private readonly FortniteReplay Replay;

        private Dictionary<uint, PlayerData> _players = new Dictionary<uint, PlayerData>();


        public FortniteReplayBuilder(FortniteReplay replay)
        {
            Replay = replay;
        }

        public void UpdateGameState(GameState state)
        {
            Replay.GameData.GameSessionId = state?.GameSessionId;
            Replay.GameData.UtcTimeStartedMatch = state.UtcTimeStartedMatch?.Time;
            Replay.GameData.MapInfo = state.MapInfo?.Name;

            Replay.GameData.IsLargeTeamGame = state.bIsLargeTeamGame;
            Replay.GameData.TournamentRound = state.EventTournamentRound;

            Replay.GameData.AdditionalPlaylistLevels = state.AdditionalPlaylistLevelsStreamed?.Select(i => i.Name);

            Replay.GameData.TeamCount = state.TeamCount;
            Replay.GameData.TeamSize = state.TeamSize;
            Replay.GameData.TotalPlayerStructures = state.TotalPlayerStructures;

            Replay.GameData.AircraftStartTime = state.AircraftStartTime;
            Replay.GameData.SafeZonesStartTime = state.SafeZonesStartTime;

            Replay.MapData.BattleBusFlightPaths = state.TeamFlightPaths.Select(i => new BattleBus(i) { Skin = state.DefaultBattleBus?.Name });
        }

        public void UpdatePlaylistInfo(PlaylistInfo playlist)
        {
            Replay.GameData.CurrentPlaylist = playlist.Name;
        }

        public void UpdateGameplayModifiers(ActiveGameplayModifier modifier)
        {
            Replay.GameData.ActiveGameplayModifiers.Add(modifier.ModifierDef?.Name);
        }

        public void UpdatePlayerState(uint channelId, FortPlayerState state)
        {
            if (state.bOnlySpectator) return;

            if (!_players.TryGetValue(channelId, out var playerData))
            {
                _players[channelId] = new PlayerData(state);
                return;
            }
            else
            {
                // TODO
                return;
            }
        }

        public void UpdateSafeZones(SafeZoneIndicator safeZone)
        {
            if (safeZone.SafeZoneStartShrinkTime <= 0 && safeZone.SafeZoneFinishShrinkTime <= 0) return;

            Replay.MapData.SafeZones.Add(new SafeZone(safeZone));
        }
    }
}
