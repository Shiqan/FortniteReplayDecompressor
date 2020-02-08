using FortniteReplayReader.Contracts;
using FortniteReplayReader.Models;
using FortniteReplayReader.Models.Events;
using FortniteReplayReader.Models.NetFieldExports;
using FortniteReplayReader.Models.NetFieldExports.RPC;
using FortniteReplayReader.Models.TelemetryEvents;
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
        private GameData GameData = new GameData();
        private MapData MapData = new MapData();
        private List<KillFeedEntry> KillFeed = new List<KillFeedEntry>();

        private Dictionary<uint, uint> _actorToChannel = new Dictionary<uint, uint>();
        private Dictionary<uint, uint> _pawnChannelToStateChannel = new Dictionary<uint, uint>();

        private IList<ITelemetryEvent> _events = new List<ITelemetryEvent>();
        private HashSet<uint> _onlySpectatingPlayers = new HashSet<uint>();
        private Dictionary<uint, PlayerData> _players = new Dictionary<uint, PlayerData>();
        private Dictionary<int?, TeamData> _teams = new Dictionary<int?, TeamData>();
        private Dictionary<uint, Llama> _llamas = new Dictionary<uint, Llama>();
        private Dictionary<int, RebootVan> _rebootVans = new Dictionary<int, RebootVan>();
        private Dictionary<uint, Models.SupplyDrop> _drops = new Dictionary<uint, Models.SupplyDrop>();

        private float? ReplicatedWorldTimeSeconds = 0;
        private int? TeamsLeft = 100;
        private int? SafeZonePhase = 0;

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
            GameData.GameSessionId ??= state?.GameSessionId;
            GameData.UtcTimeStartedMatch ??= state.UtcTimeStartedMatch?.Time;
            GameData.MapInfo ??= state.MapInfo?.Name;

            GameData.IsLargeTeamGame ??= state.bIsLargeTeamGame;
            GameData.TournamentRound ??= state.EventTournamentRound;

            GameData.AdditionalPlaylistLevels ??= state.AdditionalPlaylistLevelsStreamed?.Select(i => i.Name);

            GameData.TeamCount ??= state.TeamCount;
            GameData.TeamSize ??= state.TeamSize;
            GameData.TotalPlayerStructures ??= state.TotalPlayerStructures;

            GameData.AircraftStartTime ??= state.AircraftStartTime;
            GameData.SafeZonesStartTime ??= state.SafeZonesStartTime;

            MapData.BattleBusFlightPaths ??= state.TeamFlightPaths?.Select(i => new BattleBus(i) { Skin = state.DefaultBattleBus?.Name });

            if (state.TeamsLeft != null)
            {
                TeamsLeft = state.TeamsLeft;
            }

            if (state.SafeZonePhase != null)
            {
                SafeZonePhase = state.SafeZonePhase;
            }

            if (state.ReplicatedWorldTimeSeconds != null)
            {
                ReplicatedWorldTimeSeconds = state.ReplicatedWorldTimeSeconds;
            }
        }

        public void UpdatePlaylistInfo(PlaylistInfo playlist)
        {
            GameData.CurrentPlaylist ??= playlist.Name;
        }

        public void UpdateGameplayModifiers(ActiveGameplayModifier modifier)
        {
            GameData.ActiveGameplayModifiers.Add(modifier.ModifierDef?.Name);
        }

        public void UpdateTeamData()
        {
            foreach (var playerData in _players.Values)
            {
                if (!_teams.TryGetValue(playerData.TeamIndex, out var teamData))
                {
                    _teams[playerData.TeamIndex] = new TeamData()
                    {
                        TeamIndex = playerData.TeamIndex,
                        PlayerIds = new List<string>() { playerData.PlayerId },
                        Placement = playerData.Placement,
                        PartyOwnerId = playerData.IsPartyLeader ? playerData.PlayerId : ""
                    };
                    continue;
                }

                teamData.PlayerIds.Add(playerData.PlayerId);
                if (playerData.IsPartyLeader)
                {
                    teamData.PartyOwnerId = playerData.PlayerId;
                }
            }
        }

        public void UpdateBatchedDamge(uint channelIndex, BatchedDamageCues damage)
        {
            if (!_pawnChannelToStateChannel.TryGetValue(channelIndex, out var stateChannel))
            {
                return;
            }

            if (!_players.TryGetValue(stateChannel, out var playerData))
            {
                return;
            }

            var e = new DamageEvent
            {
                ReplicatedWorldTimeSeconds = ReplicatedWorldTimeSeconds,
                ShotPlayerId = playerData.Id,
                Location = damage.Location,
                Damage = damage.Magnitude,
                Normal = damage.Normal,
                IsCritical = damage.bIsCritical,
                CriticalHitNonPlayer = damage.NonPlayerbIsCritical,
                IsFatal = damage.bIsFatal,
                FatalHitNonPlayer = damage.NonPlayerbIsFatal,
                WeaponActivate = damage.bWeaponActivate,
                IsBallistic = damage.bIsBallistic,
                IsShield = damage.bIsShield,
                IsShieldDestroyed = damage.bIsShieldDestroyed,
            };

            if (damage.HitActor < 0) return;

            if (!_actorToChannel.TryGetValue(damage.HitActor.GetValueOrDefault(), out var hitActorStateChannel))
            {
                return;
            }

            if (!_players.TryGetValue(hitActorStateChannel, out var hitPlayerData))
            {
                // hitting non players not interesting?
                return;
            }
            e.HitPlayerId = hitPlayerData.Id;

            _events.Add(e);
        }

        public void UpdatePlayerState(uint channelIndex, FortPlayerState state)
        {
            if (state.bOnlySpectator == true)
            {
                _onlySpectatingPlayers.Add(channelIndex);
                return;
            }

            if (_onlySpectatingPlayers.Contains(channelIndex))
            {
                return;
            }

            if (!_players.TryGetValue(channelIndex, out var playerData))
            {
                _players[channelIndex] = new PlayerData(state);
                return;
            }

            if (state.RebootCounter > 0 && state.RebootCounter > playerData.RebootCounter)
            {
                playerData.RebootCounter = state.RebootCounter;
            }

            if (state.RebootCounter > 0 || state.bDBNO != null || state.DeathCause != null)
            {
                UpdateKillFeed(channelIndex, playerData, state);
            }

            if (state.Ping > 0)
            {
                // workaround
                playerData.IsReplayOwner = true;
            }

            playerData.Placement ??= state.Place;
            playerData.TeamKills ??= state.TeamKillScore;
            playerData.Kills ??= state.KillScore;
            playerData.HasThankedBusDriver ??= state.bThankedBusDriver;

            playerData.DeathCause ??= state.DeathCause;
            playerData.DeathCircumstance ??= state.DeathCircumstance;
            playerData.DeathTags ??= state.DeathTags?.Tags?.Select(i => i.TagName);

            playerData.Cosmetics.Parts ??= state.Parts?.Name;
            playerData.Cosmetics.VariantRequiredCharacterParts ??= state.VariantRequiredCharacterParts?.Select(i => i.Name);
        }

        public void UpdateKillFeed(uint channelIndex, PlayerData data, FortPlayerState state)
        {
            var entry = new KillFeedEntry();

            if (state.FinisherOrDowner == null)
            {
                // TODO
                return;
            }

            if (state.RebootCounter != null)
            {
                entry.IsRevived = true;
            }

            if (state.bDBNO == true)
            {
                entry.IsDowned = true;
            }

            if (!_actorToChannel.TryGetValue(state.FinisherOrDowner.GetValueOrDefault(), out var actorChannelIndex))
            {
                // TODO
                return;
            }

            if (!_players.TryGetValue(actorChannelIndex, out var finisherOrDownerData))
            {
                return;
            }

            entry.PlayerId = data.PlayerId;
            entry.PlayerIsBot = data.IsBot == true;
            entry.FinisherOrDowner = finisherOrDownerData.PlayerId;
            entry.FinisherOrDownerIsBot = finisherOrDownerData.IsBot == true;
            entry.TimeSeconds = ReplicatedWorldTimeSeconds;

            entry.Distance ??= state.Distance;
            entry.DeathCause ??= state.DeathCause;
            entry.DeathCircumstance ??= state.DeathCircumstance;
            entry.DeathTags ??= state.DeathTags?.Tags?.Select(i => i.TagName);

            KillFeed.Add(entry);
        }

        public void UpdatePlayerPawn(uint channelIndex, PlayerPawn pawn)
        {
            if (pawn.PlayerState == null)
            {
                return;
            }

            var actorId = pawn.PlayerState.Value;
            if (_actorToChannel.TryGetValue(actorId, out var stateChannelIndex))
            {
                _pawnChannelToStateChannel[channelIndex] = stateChannelIndex;
            }
            else
            {
                // no player state channel?
                return;
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
            if (safeZone.SafeZoneStartShrinkTime <= 0 && safeZone.SafeZoneFinishShrinkTime <= 0)
            {
                return;
            }

            MapData.SafeZones.Add(new SafeZone(safeZone));
        }

        public void UpdateLlama(uint channelIndex, SupplyDropLlama supplyDropLlama)
        {
            if (!_llamas.TryGetValue(channelIndex, out var llama))
            {
                llama = new Llama(channelIndex, supplyDropLlama);
                MapData.Llamas.Add(llama);
                _llamas.Add(channelIndex, llama);
                return;
            }

            if (supplyDropLlama.Looted)
            {
                llama.Looted = true;
                llama.LootedTime = ReplicatedWorldTimeSeconds;
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
                MapData.SupplyDrops.Add(drop);
                _drops.Add(channelIndex, drop);
                return;
            }

            if (supplyDrop.Opened)
            {
                drop.Opened = true;
                drop.OpenedTime = ReplicatedWorldTimeSeconds;
            }

            if (supplyDrop.BalloonPopped)
            {
                drop.BalloonPopped = true;
                drop.BalloonPoppedTime = ReplicatedWorldTimeSeconds;
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

        public void UpdateRebootVan(uint channelIndex, SpawnMachineRepData spawnMachine)
        {
            if (!_rebootVans.TryGetValue(spawnMachine.SpawnMachineRepDataHandle, out var rebootVan))
            {
                rebootVan = new RebootVan(spawnMachine);
                MapData.RebootVans.Add(rebootVan);
                _rebootVans.Add(spawnMachine.SpawnMachineRepDataHandle, rebootVan);
                return;
            }
            else
            {
                return;
            }
        }
    }
}
