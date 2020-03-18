using FortniteReplayReader.Models;
using FortniteReplayReader.Models.NetFieldExports;
using System.Linq;
using Unreal.Core.Models;
using Xunit;

namespace FortniteReplayReader.Test
{
    public class FortniteReplayBuilderTest
    {
        FortniteReplayBuilder builder;
        FortniteReplay replay;

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
                FinalDestination = new FVector(1, 2, 3),
                Looted = true
            };
            builder.UpdateLlama(1, llama);

            builder.Build(replay);
            var addedLlama = replay.MapData.Llamas.First();
            Assert.Equal(1u, addedLlama.Id);
            Assert.Equal(llama.Looted, addedLlama.Looted);
            Assert.Equal(llama.FinalDestination, addedLlama.LandingLocation);

            var newLlama = new SupplyDropLlama()
            {
                FinalDestination = new FVector(3, 2, 1)
            };
            builder.UpdateLlama(1, newLlama);
            Assert.Equal(llama.FinalDestination, addedLlama.LandingLocation);

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
                WorldGridStart = new FVector2D(1,2),
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
    }
}
