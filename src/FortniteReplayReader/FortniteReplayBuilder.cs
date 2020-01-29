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
        private Dictionary<uint, Llama> _llamas = new Dictionary<uint, Llama>();
        private Dictionary<uint, Models.SupplyDrop> _drops = new Dictionary<uint, Models.SupplyDrop>();

        public FortniteReplayBuilder(FortniteReplay replay)
        {
            Replay = replay;
        }

        public void UpdateGameState(GameState state)
        {
            Replay.GameData.GameSessionId ??= state?.GameSessionId;
            Replay.GameData.UtcTimeStartedMatch ??= state.UtcTimeStartedMatch?.Time;
            Replay.GameData.MapInfo ??= state.MapInfo?.Name;

            Replay.GameData.IsLargeTeamGame ??= state.bIsLargeTeamGame;
            Replay.GameData.TournamentRound ??= state.EventTournamentRound;

            Replay.GameData.AdditionalPlaylistLevels ??= state.AdditionalPlaylistLevelsStreamed?.Select(i => i.Name);

            Replay.GameData.TeamCount ??= state.TeamCount;
            Replay.GameData.TeamSize ??= state.TeamSize;
            Replay.GameData.TotalPlayerStructures ??= state.TotalPlayerStructures;

            Replay.GameData.AircraftStartTime ??= state.AircraftStartTime;
            Replay.GameData.SafeZonesStartTime ??= state.SafeZonesStartTime;

            Replay.MapData.BattleBusFlightPaths ??= state.TeamFlightPaths?.Select(i => new BattleBus(i) { Skin = state.DefaultBattleBus?.Name });
        }

        public void UpdatePlaylistInfo(PlaylistInfo playlist)
        {
            Replay.GameData.CurrentPlaylist ??= playlist.Name;
        }

        public void UpdateGameplayModifiers(ActiveGameplayModifier modifier)
        {
            Replay.GameData.ActiveGameplayModifiers.Add(modifier.ModifierDef?.Name);
        }

        public void UpdatePlayerState(uint channelIndex, FortPlayerState state)
        {
            if (state.bOnlySpectator) return;

            if (!_players.TryGetValue(channelIndex, out var playerData))
            {
                _players[channelIndex] = new PlayerData(state);
                return;
            }

            if (state.RebootCounter > 0 && state.RebootCounter > playerData.RebootCounter)
            {
                playerData.RebootCounter = state.RebootCounter;
            }

            playerData.Placement ??= state.Place;
            playerData.TeamKills ??= state.TeamKillScore;
            playerData.Kills ??= state.KillScore;
            playerData.DeathCause ??= state.DeathCause;
            playerData.HasThankedBusDriver ??= state.bThankedBusDriver;
        }

        public void UpdateSafeZones(SafeZoneIndicator safeZone)
        {
            if (safeZone.SafeZoneStartShrinkTime <= 0 && safeZone.SafeZoneFinishShrinkTime <= 0) return;

            Replay.MapData.SafeZones.Add(new SafeZone(safeZone));
        }

        public void UpdateLlama(uint channelIndex, SupplyDropLlama supplyDropLlama)
        {
            if (!_llamas.TryGetValue(channelIndex, out var llama))
            {
                llama = new Llama(channelIndex, supplyDropLlama);
                Replay.MapData.Llamas.Add(llama);
                _llamas.Add(channelIndex, llama);
                return;
            }

            if (supplyDropLlama.Looted)
            {
                llama.Looted = true;
            }

            if (supplyDropLlama.bHasSpawnedPickups)
            {
                llama.HasSpawnedPickups = true;
            }
        }
        
        public void UpdateSupplyDrop(uint channelIndex, Models.NetFieldExports.SupplyDrop supplyDrop)
        {
            if (!_drops.TryGetValue(channelIndex, out var drop))
            {
                drop = new Models.SupplyDrop(channelIndex, supplyDrop);
                Replay.MapData.SupplyDrops.Add(drop);
                _drops.Add(channelIndex, drop);
                return;
            }

            if (supplyDrop.Opened)
            {
                drop.Opened = true;
            }

            if (supplyDrop.BalloonPopped)
            {
                drop.BalloonPopped = true;
            }

            if (supplyDrop.bHasSpawnedPickups)
            {
                drop.HasSpawnedPickups = true;
            }

            if (supplyDrop.LandingLocation != null)
            {
                drop.LandingLocation = supplyDrop.LandingLocation;
            }
        }
    }
}
