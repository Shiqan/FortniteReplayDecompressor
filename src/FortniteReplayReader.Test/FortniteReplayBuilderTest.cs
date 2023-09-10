using FortniteReplayReader.Models;
using FortniteReplayReader.Models.NetFieldExports;
using System.Linq;
using Unreal.Core.Models;
using Xunit;

namespace FortniteReplayReader.Test;

public class FortniteReplayBuilderTest
{
    private readonly FortniteReplayBuilder builder;
    private readonly FortniteReplay replay;

    public FortniteReplayBuilderTest()
    {
        builder = new FortniteReplayBuilder();
        replay = new FortniteReplay();
    }

    [Fact]
    public void GameStateTest()
    {
        var state = new GameState()
        {
            GameSessionId = "123",
            WinningTeam = 99,
            ReplicatedWorldTimeSeconds = 1,
            ReplicatedWorldTimeSecondsDouble = 1,
        };
        builder.UpdateGameState(state);

        builder.Build(replay);
        Assert.Equal(state.GameSessionId, replay.GameData.GameSessionId);
        Assert.Equal(state.WinningTeam, replay.GameData.WinningTeam);

        builder.UpdateGameState(new Models.NetFieldExports.GameState());
        Assert.Equal(state.GameSessionId, replay.GameData.GameSessionId);
        Assert.Equal(state.WinningTeam, replay.GameData.WinningTeam);

        state.WinningTeam = 100;
        builder.UpdateGameState(state);
        Assert.Equal(99u, replay.GameData.WinningTeam);
    }

    [Fact]
    public void PlaylistInfoTest()
    {
        var playlist = new PlaylistInfo()
        {
            Name = "Duo"
        };
        builder.UpdatePlaylistInfo(playlist);

        builder.Build(replay);
        Assert.Equal(playlist.Name, replay.GameData.CurrentPlaylist);
    }

    [Fact]
    public void GameplayModifierTests()
    {
        var modifier = new ActiveGameplayModifier()
        {
            ModifierDef = new ItemDefinition()
            {
                Name = "Respawn"
            }
        };
        builder.UpdateGameplayModifiers(modifier);

        builder.Build(replay);
        Assert.Equal(modifier.ModifierDef.Name, replay.GameData.ActiveGameplayModifiers[0]);
    }

    [Fact]
    public void SafeZoneTest()
    {
        var zone = new SafeZoneIndicator()
        {
            SafeZoneStartShrinkTime = 1,
            Radius = 5000
        };
        builder.UpdateSafeZones(zone);

        builder.Build(replay);
        Assert.Equal(zone.Radius, replay.MapData.SafeZones[0].Radius);
    }

    [Fact]
    public void SafeZoneShouldBeIgnoredIfMovingTest()
    {
        var zone = new SafeZoneIndicator()
        {
            Radius = 5000
        };
        builder.UpdateSafeZones(zone);

        builder.Build(replay);
        Assert.Empty(replay.MapData.SafeZones);
    }

    [Fact]
    public void LlamaTest()
    {
        var llama = new SupplyDropLlama()
        {
        };
        builder.UpdateLlama(1, llama);

        builder.Build(replay);
        var addedLlama = replay.MapData.Llamas.First();
        Assert.Equal(1u, addedLlama.Id);
        Assert.False(addedLlama.Looted);

        var newLlama = new SupplyDropLlama()
        {
            Looted = true,
            FinalDestination = new FVector(3, 2, 1)
        };
        builder.UpdateLlama(1, newLlama);
        Assert.True(addedLlama.Looted);
        Assert.Equal(newLlama.FinalDestination, addedLlama.LandingLocation);

        builder.UpdateLlama(2, newLlama);
        builder.Build(replay);
        Assert.Equal(2, replay.MapData.Llamas.Count);
    }

    [Fact]
    public void SupplyDropTest()
    {
        var drop = new Models.NetFieldExports.SupplyDrop()
        {
            LandingLocation = new FVector(1, 2, 3),
            Opened = true
        };
        builder.UpdateSupplyDrop(1, drop);

        builder.Build(replay);
        var addedDrop = replay.MapData.SupplyDrops.First();
        Assert.Equal(1u, addedDrop.Id);
        Assert.False(addedDrop.Looted);
        Assert.Null(addedDrop.LandingLocation);

        var newDrop = new Models.NetFieldExports.SupplyDrop()
        {
            Opened = true,
            LandingLocation = new FVector(1, 2, 3),
        };
        builder.UpdateSupplyDrop(1, newDrop);
        Assert.Equal(drop.LandingLocation, addedDrop.LandingLocation);
        Assert.True(addedDrop.Looted);

        builder.UpdateSupplyDrop(2, newDrop);
        builder.Build(replay);
        Assert.Equal(2, replay.MapData.SupplyDrops.Count);
    }

    [Fact]
    public void PoiManagerTest()
    {
        var manager = new FortPoiManager()
        {
            GridCountX = 1,
            GridCountY = 2,
            WorldGridStart = new FVector2D(1, 2),
            WorldGridEnd = new FVector2D(3, 4),
            WorldGridSpacing = new FVector2D(4, 5),
            WorldGridTotalSize = new FVector2D(6, 7)
        };
        builder.UpdatePoiManager(manager);
        builder.Build(replay);

        Assert.Equal(manager.GridCountX, replay.MapData.GridCountX);
        Assert.Equal(manager.GridCountY, replay.MapData.GridCountY);
        Assert.Equal(manager.WorldGridStart, replay.MapData.WorldGridStart);
        Assert.Equal(manager.WorldGridEnd, replay.MapData.WorldGridEnd);
        Assert.Equal(manager.WorldGridSpacing, replay.MapData.WorldGridSpacing);
        Assert.Equal(manager.WorldGridTotalSize, replay.MapData.WorldGridTotalSize);
    }

    [Fact]
    public void PlayerStateTest()
    {
        var state = new FortPlayerState()
        {
            PlayerID = 1,
            UniqueId = "abc-123",
            BotUniqueId = "",
            bIsABot = false,
            TeamIndex = 1,
            HeroType = new ItemDefinition() { Name = "bandolier" }
        };
        builder.UpdatePlayerState(1, state);
        builder.Build(replay);
        Assert.Equal(1, replay.PlayerData.First().Id);
        Assert.Equal("abc-123", replay.PlayerData.First().PlayerId);
        Assert.Equal("bandolier", replay.PlayerData.First().Cosmetics.HeroType);
    }

    [Fact]
    public void PlayerStateMarksReplayOwnerTest()
    {
        var gameState = new GameState()
        {
            RecorderPlayerState = new ActorGuid { Value = 1 }
        };

        builder.UpdateGameState(gameState);

        var state = new FortPlayerState()
        {
            PlayerID = 1,
            UniqueId = "abc-123",
            BotUniqueId = "",
            bIsABot = false,
            TeamIndex = 1,
            HeroType = new ItemDefinition() { Name = "bandolier" }
        };
        builder.AddActorChannel(1, 1);
        builder.UpdatePlayerState(1, state);
        builder.Build(replay);

        Assert.Contains(replay.PlayerData, i => i.IsReplayOwner);
    }

    [Fact]
    public void PlayerStateHandlesBotsTest()
    {
        var state = new FortPlayerState()
        {
            PlayerID = 1,
            UniqueId = "",
            BotUniqueId = "NotABot",
            bIsABot = true,
            TeamIndex = 2,
            HeroType = new ItemDefinition() { Name = "default" }
        };
        builder.UpdatePlayerState(1, state);
        builder.Build(replay);
        Assert.Equal(1, replay.PlayerData.First().Id);
        Assert.Equal("NotABot", replay.PlayerData.First().PlayerId);
        Assert.Equal("default", replay.PlayerData.First().Cosmetics.HeroType);
    }

    [Fact]
    public void PlayerStateSkipsSpectatorsTest()
    {
        var state = new FortPlayerState()
        {
            PlayerID = 1,
            bOnlySpectator = true
        };
        builder.UpdatePlayerState(1, state);
        builder.Build(replay);
        Assert.Empty(replay.PlayerData);
    }

    [Fact]
    public void PlayerStateUpdatesTotalKillsTest()
    {
        var state = new FortPlayerState()
        {
            PlayerID = 1,
            UniqueId = "abc-123",
            BotUniqueId = "",
            bIsABot = false,
            TeamIndex = 1,
            KillScore = 1,
            TeamKillScore = 1,
            HeroType = new ItemDefinition() { Name = "bandolier" }
        };
        builder.UpdatePlayerState(1, state);
        builder.Build(replay);

        Assert.Equal(1u, replay.PlayerData.First().Kills);
        Assert.Equal(1u, replay.PlayerData.First().TeamKills);

        state = new FortPlayerState()
        {
            KillScore = 2,
            TeamKillScore = 3,
        };
        builder.UpdatePlayerState(1, state);
        builder.Build(replay);

        Assert.Equal(2u, replay.PlayerData.First().Kills);
        Assert.Equal(3u, replay.PlayerData.First().TeamKills);
    }

    [Fact]
    public void PlayerStateUpdateKillFeedTest()
    {
        var state = new FortPlayerState()
        {
            PlayerID = 1,
            UniqueId = "abc-123",
            BotUniqueId = "",
            bIsABot = false,
            TeamIndex = 1,
            HeroType = new ItemDefinition() { Name = "bandolier" }
        };
        builder.UpdatePlayerState(1, state);
        builder.Build(replay);

        state = new FortPlayerState()
        {
            DeathCause = 1
        };
        builder.UpdatePlayerState(1, state);
        builder.Build(replay);
        Assert.NotEmpty(replay.KillFeed);
        Assert.Equal(1, replay.KillFeed.First().PlayerId);
    }

    [Fact]
    public void PlayerPawnTest()
    {
        var state = new FortPlayerState()
        {
            PlayerID = 1,
            UniqueId = "abc-123",
            BotUniqueId = "",
            bIsABot = false,
            TeamIndex = 1,
            HeroType = new ItemDefinition() { Name = "bandolier" }
        };
        builder.UpdatePlayerState(1, state);
        builder.AddActorChannel(1, 100);

        var pawn = new PlayerPawn()
        {
            PlayerState = 100,
            Pickaxe = new ItemDefinition() { Name = "raiders revenge" }
        };
        builder.UpdatePlayerPawn(2, pawn);
        builder.Build(replay);

        Assert.Equal("raiders revenge", replay.PlayerData.First().Cosmetics.Pickaxe);
    }

    [Fact]
    public void PlayerPawnDontAddWhenMissingPlayerStateTest()
    {
        var state = new FortPlayerState()
        {
            PlayerID = 1,
            UniqueId = "abc-123",
            BotUniqueId = "",
            bIsABot = false,
            TeamIndex = 1,
            HeroType = new ItemDefinition() { Name = "bandolier" }
        };
        builder.UpdatePlayerState(1, state);
        builder.AddActorChannel(1, 100);

        var pawn = new PlayerPawn()
        {
            Pickaxe = new ItemDefinition() { Name = "raiders revenge" }
        };
        builder.UpdatePlayerPawn(2, pawn);
        builder.Build(replay);

        Assert.Null(replay.PlayerData.First().Cosmetics.Pickaxe);
    }

    [Fact]
    public void PlayerPawnDontAddWhenMissingChannelTest()
    {
        var state = new FortPlayerState()
        {
            PlayerID = 1,
            UniqueId = "abc-123",
            BotUniqueId = "",
            bIsABot = false,
            TeamIndex = 1,
            HeroType = new ItemDefinition() { Name = "bandolier" }
        };
        builder.UpdatePlayerState(1, state);

        var pawn = new PlayerPawn()
        {
            PlayerState = 100,
            Pickaxe = new ItemDefinition() { Name = "raiders revenge" }
        };
        builder.UpdatePlayerPawn(2, pawn);
        builder.Build(replay);

        Assert.Null(replay.PlayerData.First().Cosmetics.Pickaxe);
    }

    [Fact]
    public void PlayerPawnUpdatesStateChannel()
    {
        var state = new FortPlayerState()
        {
            PlayerID = 1,
            UniqueId = "abc-123",
            BotUniqueId = "",
            bIsABot = false,
            TeamIndex = 1,
            HeroType = new ItemDefinition() { Name = "bandolier" }
        };
        builder.UpdatePlayerState(1, state);
        builder.AddActorChannel(1, 100);

        var pawn = new PlayerPawn()
        {
            PlayerState = 100,
            Pickaxe = new ItemDefinition() { Name = "raiders revenge" }
        };
        builder.UpdatePlayerPawn(2, pawn);

        var state2 = new FortPlayerState()
        {
            PlayerID = 2,
            UniqueId = "123-abc",
            BotUniqueId = "",
            bIsABot = false,
            TeamIndex = 1,
            HeroType = new ItemDefinition() { Name = "bandolier" }
        };
        builder.UpdatePlayerState(3, state2);
        builder.AddActorChannel(3, 101);
        var pawn2 = new PlayerPawn()
        {
            PlayerState = 101,
            Glider = new ItemDefinition() { Name = "umbrella" }
        };
        builder.AddActorChannel(3, 101);
        builder.UpdatePlayerPawn(2, pawn2);

        builder.Build(replay);

        Assert.Equal("raiders revenge", replay.PlayerData.First().Cosmetics.Pickaxe);
        Assert.Null(replay.PlayerData.First().Cosmetics.Glider);
        Assert.Equal("umbrella", replay.PlayerData.Last().Cosmetics.Glider);
    }

    [Fact]
    public void TeamDataTest()
    {
        void CreatePlayer(uint channel, int id, int team)
        {
            builder.UpdatePlayerState(channel, new FortPlayerState()
            {
                PlayerID = id,
                UniqueId = $"player{id}",
                BotUniqueId = "",
                bIsABot = false,
                TeamIndex = team
            });
        }

        CreatePlayer(1, 1, 1);
        CreatePlayer(2, 2, 1);
        CreatePlayer(3, 3, 2);
        CreatePlayer(4, 4, 2);
        CreatePlayer(5, 5, 3);

        builder.UpdatePlayerState(6, new FortPlayerState()
        {
            PlayerID = 6,
            UniqueId = $"player{6}",
            BotUniqueId = "",
            bIsABot = false,
            TeamIndex = 3,
            PartyOwnerUniqueId = "player6"
        });

        builder.UpdatePlayerState(1, new FortPlayerState()
        {
            Place = 1
        });

        builder.Build(replay);

        Assert.Equal(3, replay.TeamData.Count());
        Assert.Equal(2, replay.TeamData.First(i => i.TeamIndex == 1).PlayerIds.Count);
        Assert.Contains(1, replay.TeamData.First(i => i.TeamIndex == 1).PlayerIds);
        Assert.Contains(2, replay.TeamData.First(i => i.TeamIndex == 1).PlayerIds);
        Assert.Contains(5, replay.TeamData.First(i => i.TeamIndex == 3).PlayerIds);
        Assert.Equal(6, replay.TeamData.First(i => i.TeamIndex == 3).PartyOwnerId);
        Assert.Equal(1, replay.TeamData.First(i => i.TeamIndex == 1).Placement);
    }
}
