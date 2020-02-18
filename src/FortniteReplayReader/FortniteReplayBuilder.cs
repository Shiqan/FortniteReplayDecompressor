using FortniteReplayReader.Models;
using FortniteReplayReader.Models.NetFieldExports;
using FortniteReplayReader.Models.NetFieldExports.RPC;
using FortniteReplayReader.Models.TelemetryEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using Unreal.Core.Contracts;

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

        private Dictionary<uint, FortInventory> _inventories = new Dictionary<uint, FortInventory>();

        private float? ReplicatedWorldTimeSeconds = 0;

        public void AddActorChannel(uint channelIndex, uint guid)
        {
            _actorToChannel[guid] = channelIndex;
        }

        public void RemoveChannel(uint channelIndex, uint guid)
        {
            _actorToChannel.Remove(guid);
            _pawnChannelToStateChannel.Remove(channelIndex);
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

            if (state.ReplicatedWorldTimeSeconds != null)
            {
                ReplicatedWorldTimeSeconds = state.ReplicatedWorldTimeSeconds;
            }

            GameData.WinningPlayerIds ??= state.WinningPlayerList?.Select(i => i);
            GameData.WinningTeam ??= state.WinningTeam;
        }

        public void CreateGameStateEvent(GameState state)
        {
            var e = new GameStateEvent();
            e.ReplicatedWorldTimeSeconds ??= ReplicatedWorldTimeSeconds;
            e.TeamsLeft ??= state.TeamsLeft;
            e.SafeZonePhase ??= state.SafeZonePhase;
            e.PlayerBotsLeft ??= state.PlayerBotsLeft;
            e.PlayersLeft ??= state.PlayersLeft;
            e.GamePhase ??= state.GamePhase;
            e.GameplayState ??= state.GameplayState;
            _events.Add(e);
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
                        PlayerIds = new List<int?>() { playerData.Id },
                        PlayerNames = new List<string>() { playerData.PlayerId },
                        Placement = playerData.Placement,
                        PartyOwnerId = playerData.IsPartyLeader ? playerData.Id : null
                    };
                    continue;
                }

                teamData.PlayerIds.Add(playerData.Id);
                teamData.PlayerNames.Add(playerData.PlayerId);
                if (playerData.IsPartyLeader)
                {
                    teamData.PartyOwnerId = playerData.Id;
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

            if (damage.HitActor < 0)
            {
                return;
            }

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
            var entry = new KillFeedEntry()
            {
                ReplicatedWorldTimeSeconds = ReplicatedWorldTimeSeconds
            };

            if (state.RebootCounter != null)
            {
                entry.IsRevived = true;
            }

            if (state.bDBNO == true)
            {
                entry.IsDowned = true;
            }

            if (_actorToChannel.TryGetValue(state.FinisherOrDowner.GetValueOrDefault(), out var actorChannelIndex))
            {
                if (_players.TryGetValue(actorChannelIndex, out var finisherOrDownerData))
                {
                    entry.FinisherOrDowner = finisherOrDownerData.PlayerId;
                    entry.FinisherOrDownerIsBot = finisherOrDownerData.IsBot == true;
                }
            }

            entry.PlayerId = data.Id;
            entry.PlayerIsBot = data.IsBot == true;

            entry.Distance ??= state.Distance;
            entry.DeathCause ??= state.DeathCause;
            entry.DeathCircumstance ??= state.DeathCircumstance;
            entry.DeathTags ??= state.DeathTags?.Tags?.Select(i => i.TagName);

            KillFeed.Add(entry);
        }

        public void UpdatePlayerPawn(uint channelIndex, PlayerPawn pawn)
        {
            if (!_pawnChannelToStateChannel.TryGetValue(channelIndex, out var stateChannelIndex))
            {
                if (pawn.PlayerState == null)
                {
                    return;
                }

                var actorId = pawn.PlayerState.Value;
                if (_actorToChannel.TryGetValue(actorId, out stateChannelIndex))
                {
                    _pawnChannelToStateChannel[channelIndex] = stateChannelIndex;
                }
                else
                {
                    // no player state channel?
                    return;
                }
            }

            var playerState = _players[stateChannelIndex];

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

        public void CreatePawnEvent(uint channelIndex, PlayerPawn pawn)
        {
            var e = new PlayerMovementEvent()
            {
                ReplicatedWorldTimeSeconds = ReplicatedWorldTimeSeconds,
                ReplicatedMovement = pawn.ReplicatedMovement
            };
            e.bCanBeDamaged ??= pawn.bCanBeDamaged;
            e.LocationOffset ??= pawn.LocationOffset;
            e.RelativeScale3D ??= pawn.RelativeScale3D;
            e.RotationOffset ??= pawn.RotationOffset;
            e.RemoteViewPitch ??= pawn.RemoteViewPitch;
            e.Location ??= pawn.Location;
            e.Rotation ??= pawn.Rotation;
            e.ReplayLastTransformUpdateTimeStamp ??= pawn.ReplayLastTransformUpdateTimeStamp;
            e.ReplicatedMovementMode ??= pawn.ReplicatedMovementMode;
            e.bIsCrouched ??= pawn.bIsCrouched;
            e.Position ??= pawn.Position;
            e.LinearVelocity ??= pawn.LinearVelocity;
            e.CurrentMovementStyle ??= pawn.CurrentMovementStyle;
            e.bIsDying ??= pawn.bIsDying;
            e.CurrentWeapon ??= pawn.CurrentWeapon;
            e.bIsInvulnerable ??= pawn.bIsInvulnerable;
            e.bMovingEmote ??= pawn.bMovingEmote;
            e.bWeaponActivated ??= pawn.bWeaponActivated;
            e.bIsDBNO ??= pawn.bIsDBNO;
            e.bWasDBNOOnDeath ??= pawn.bWasDBNOOnDeath;
            e.bWeaponHolstered ??= pawn.bWeaponHolstered;
            e.LastReplicatedEmoteExecuted ??= pawn.LastReplicatedEmoteExecuted;
            e.ForwardAlpha ??= pawn.ForwardAlpha;
            e.RightAlpha ??= pawn.RightAlpha;
            e.TurnDelta ??= pawn.TurnDelta;
            e.SteerAlpha ??= pawn.SteerAlpha;
            e.GravityScale ??= pawn.GravityScale;
            e.WorldLookDir ??= pawn.WorldLookDir;
            e.bIsHonking ??= pawn.bIsHonking;
            e.bIsJumping ??= pawn.bIsJumping;
            e.bIsSprinting ??= pawn.bIsSprinting;
            e.Vehicle ??= pawn.Vehicle;
            e.VehicleApexZ ??= pawn.VehicleApexZ;
            e.SeatIndex ??= pawn.SeatIndex;
            e.bIsWaterJump ??= pawn.bIsWaterJump;
            e.bIsWaterSprintBoost ??= pawn.bIsWaterSprintBoost;
            e.bIsWaterSprintBoostPending ??= pawn.bIsWaterSprintBoostPending;
            e.BuildingState ??= pawn.BuildingState;
            e.bIsTargeting ??= pawn.bIsTargeting;
            e.bIsPlayingEmote ??= pawn.bIsPlayingEmote;
            e.bStartedInteractSearch ??= pawn.bStartedInteractSearch;
            e.AccelerationPack ??= pawn.AccelerationPack;
            e.AccelerationZPack ??= pawn.AccelerationZPack;
            e.bIsWaitingForEmoteInteraction ??= pawn.bIsWaitingForEmoteInteraction;
            e.GroupEmoteLookTarget ??= pawn.GroupEmoteLookTarget;
            e.bIsSkydiving ??= pawn.bIsSkydiving;
            e.bIsParachuteOpen ??= pawn.bIsParachuteOpen;
            e.bIsSkydivingFromBus ??= pawn.bIsSkydivingFromBus;
            e.bIsInAnyStorm ??= pawn.bIsInAnyStorm;
            e.bIsSlopeSliding ??= pawn.bIsSlopeSliding;
            e.bIsInsideSafeZone ??= pawn.bIsInsideSafeZone;
            e.bIsOutsideSafeZone ??= pawn.bIsOutsideSafeZone;
            e.Zipline ??= pawn.Zipline;
            e.bIsZiplining ??= pawn.bIsZiplining;
            e.bJumped ??= pawn.bJumped;
            e.RemoteViewData32 ??= pawn.RemoteViewData32;
            e.EntryTime ??= pawn.EntryTime;
            e.WalkSpeed ??= pawn.WalkSpeed;
            e.RunSpeed ??= pawn.RunSpeed;
            e.CrouchedRunSpeed ??= pawn.CrouchedRunSpeed;
            e.CrouchedSprintSpeed ??= pawn.CrouchedSprintSpeed;
            e.WeaponActivated ??= pawn.WeaponActivated;
            e.bIsInWaterVolume ??= pawn.bIsInWaterVolume;
            e.DBNOHoister ??= pawn.DBNOHoister;
            e.GravityFloorAltitude ??= pawn.GravityFloorAltitude;
            e.GravityFloorWidth ??= pawn.GravityFloorWidth;
            e.GravityFloorGravityScalar ??= pawn.GravityFloorGravityScalar;
            e.FlySpeed ??= pawn.FlySpeed;
            e.bIsSkydivingFromLaunchPad ??= pawn.bIsSkydivingFromLaunchPad;
            e.bInGliderRedeploy ??= pawn.bInGliderRedeploy;
            _events.Add(e);
        }

        public void CreatePickupEvent(uint channelIndex, FortPickup pickup)
        {
            if (ReplicatedWorldTimeSeconds <= 0) return;

            if (pickup.TossState != null)
            {
                var e = new LootEvent
                {
                    ReplicatedWorldTimeSeconds = ReplicatedWorldTimeSeconds,
                    Count = pickup.Count,
                    ItemDefinition = pickup.ItemDefinition?.Name,
                    Durability = pickup.Durability,
                    LoadedAmmo = pickup.LoadedAmmo,
                    A = pickup.A,
                    B = pickup.B,
                    C = pickup.C,
                    D = pickup.D,
                    CombineTarget = pickup.CombineTarget?.Name,
                    PickupTarget = pickup.PickupTarget,
                    ItemOwner = pickup.ItemOwner,
                    LootInitialPosition = pickup.LootInitialPosition,
                    LootFinalPosition = pickup.LootFinalPosition,
                    FlyTime = pickup.FlyTime,
                    StartDirection = pickup.StartDirection,
                    FinalTossRestLocation = pickup.FinalTossRestLocation,
                    TossState = pickup.TossState,
                    OptionalOwnerID = pickup.OptionalOwnerID,
                    IsPickedUp = pickup.bPickedUp,
                    DroppedBy = pickup.PawnWhoDroppedPickup,
                    OrderIndex = pickup.OrderIndex,
                };
                _events.Add(e);
            }
        }

        public void UpdateInventory(uint channelIndex, FortInventory inventory)
        {
            if (inventory.ReplayPawn > 0)
            {
                //Normal replays only have your inventory. Every time you die, there's a new player pawn.
                _inventories.TryAdd(channelIndex, inventory);
            }
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

        public void CreateLlamaEvent(uint channelIndex, SupplyDropLlama supplyDropLlama)
        {
            var e = new LlamaEvent()
            {
                ReplicatedWorldTimeSeconds = ReplicatedWorldTimeSeconds,
                Id = channelIndex,
                Looted = supplyDropLlama.Looted,
                FinalDestination = supplyDropLlama.FinalDestination,
                ReplicatedMovement = supplyDropLlama.ReplicatedMovement,
                HasSpawnedPickups = supplyDropLlama.bHasSpawnedPickups,
            };
            _events.Add(e);
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

        public void CreateSupplyDropEvent(uint channelIndex, Models.NetFieldExports.SupplyDrop supplyDrop)
        {
            var e = new SupplyDropEvent()
            {
                ReplicatedWorldTimeSeconds = ReplicatedWorldTimeSeconds,
                Id = channelIndex,
                ReplicatedMovement = supplyDrop.ReplicatedMovement,
                BalloonPopped = supplyDrop.BalloonPopped,
                HasSpawnedPickups = supplyDrop.bHasSpawnedPickups,
                LandingLocation = supplyDrop.LandingLocation,
                Opened = supplyDrop.Opened,
                FallHeight = supplyDrop.FallHeight,
                FallSpeed = supplyDrop.FallSpeed,
            };
            _events.Add(e);
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
        }

        public void CreateRebootVanEvent(uint channelIndex, SpawnMachineRepData spawnMachine)
        {
            var e = new RebootVanEvent()
            {
                ReplicatedWorldTimeSeconds = ReplicatedWorldTimeSeconds,
                Id = spawnMachine.SpawnMachineRepDataHandle,
                Location = spawnMachine.Location,
                CooldownEndTime = spawnMachine.SpawnMachineCooldownEndTime,
                CooldownStartTime = spawnMachine.SpawnMachineCooldownStartTime,
                State = spawnMachine.SpawnMachineState
            };
            _events.Add(e);
        }

        public void CreateHealthEvent(uint channelIndex, HealthSet health)
        {
            if (_players.TryGetValue(channelIndex, out var playerData))
            {
                var e = new HealthEvent()
                {
                    ReplicatedWorldTimeSeconds = ReplicatedWorldTimeSeconds,
                    PlayerId = playerData.Id,
                    HealthBaseValue = health.HealthBaseValue,
                    HealthCurrentValue = health.HealthCurrentValue,
                    HealthMaxValue = health.HealthMaxValue,
                    HealthUnclampedBaseValue = health.HealthUnclampedBaseValue,
                    HealthUnclampedCurrentValue = health.HealthUnclampedCurrentValue,
                    ShieldBaseValue = health.ShieldBaseValue,
                    ShieldCurrentValue = health.ShieldCurrentValue,
                    ShieldMaxValue = health.ShieldMaxValue
                };
                _events.Add(e);
            }
        }

        //public void UpdateExplosion(BroadcastExplosion explosion)
        //{
        //    // ¯\_(ツ)_/¯
        //}

        public void UpdatePoiManager(FortPoiManager poiManager)
        {
            MapData.GridCountX ??= poiManager.GridCountX;
            MapData.GridCountY ??= poiManager.GridCountY;
            MapData.WorldGridStart ??= poiManager.WorldGridStart;
            MapData.WorldGridEnd ??= poiManager.WorldGridEnd;
            MapData.WorldGridSpacing ??= poiManager.WorldGridSpacing;
            MapData.WorldGridTotalSize ??= poiManager.WorldGridTotalSize;

            // ignore PoiTagContainerTable since it is just a list of all POI...
        }

        //public void UpdateGameplayCue(uint channelIndex, GameplayCue gameplayCue)
        //{
        //    // ¯\_(ツ)_/¯
        //}
    }
}
