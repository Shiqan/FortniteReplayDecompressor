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

        private Dictionary<uint, uint> _actorToChannel = new Dictionary<uint, uint>();
        private Dictionary<uint, PlayerData> _players = new Dictionary<uint, PlayerData>();
        private Dictionary<uint, Llama> _llamas = new Dictionary<uint, Llama>();
        private Dictionary<uint, Models.SupplyDrop> _drops = new Dictionary<uint, Models.SupplyDrop>();

        private Dictionary<uint, PlayerPawn> _playerPawns = new Dictionary<uint, PlayerPawn>();
        private Dictionary<uint, uint> _pawnChannelToStateChannel = new Dictionary<uint, uint>();
        

        public FortniteReplayBuilder(FortniteReplay replay)
        {
            Replay = replay;
        }

        public void AddActorChannel(uint channelIndex, uint guid)
        {
            _actorToChannel[guid] = channelIndex;
        }

        public void RemoveChannel(uint channelIndex, uint guid)
        {
            _actorToChannel.Remove(guid);
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
            if (state.bOnlySpectator == true) return;

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
            playerData.HasThankedBusDriver ??= state.bThankedBusDriver;

            playerData.DeathCause ??= state.DeathCause;
            playerData.DeathTags ??= state.DeathTags?.Tags?.Select(i => i.TagName);

            playerData.Cosmetics.Parts ??= state.Parts?.Name;
            playerData.Cosmetics.VariantRequiredCharacterParts ??= state.VariantRequiredCharacterParts?.Select(i => i.Name);
        }

        public void UpdatePlayerPawn(uint channelIndex, PlayerPawn pawn)
        {
            if (!_playerPawns.TryGetValue(channelIndex, out var p))
            {
                if (pawn.PlayerState == null) return;

                var actorId = pawn.PlayerState.Value;
                if (_actorToChannel.TryGetValue(actorId, out var stateChannelIndex))
                {
                    _pawnChannelToStateChannel[channelIndex] = stateChannelIndex;
                    _playerPawns[channelIndex] = pawn;
                }
                else
                {
                    // no player state channel?
                    return;
                }
            }

            var playerState = _players[_pawnChannelToStateChannel[channelIndex]];
            
            playerState.Cosmetics.Character ??= pawn.Character?.Name;
            playerState.Cosmetics.BannerColorId ??= pawn.BannerColorId;
            playerState.Cosmetics.BannerIconId ??= pawn.BannerIconId;
            playerState.Cosmetics.IsDefaultCharacter ??= pawn.bIsDefaultCharacter;
            playerState.Cosmetics.Backpack ??= pawn.Backpack?.Name;
            playerState.Cosmetics.PetSkin ??= pawn.PetSkin?.Name;
            playerState.Cosmetics.Glider ??= pawn.Glider?.Name;
            playerState.Cosmetics.LoadingScreen ??= pawn.LoadingScreen?.Name;
            playerState.Cosmetics.MusicPack ??= pawn.MusicPack?.Name;
            playerState.Cosmetics.Pickaxe ??= pawn.Pickaxe?.Name;
            playerState.Cosmetics.SkyDiveContrail ??= pawn.SkyDiveContrail?.Name;
            playerState.Cosmetics.Dances ??= pawn.Dances?.Select(i => i.Name);
            playerState.Cosmetics.ItemWraps ??= pawn.ItemWraps?.Select(i => i.Name);
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
