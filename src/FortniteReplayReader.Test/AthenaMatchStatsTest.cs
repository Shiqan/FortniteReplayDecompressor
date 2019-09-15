using FortniteReplayReader.Extensions;
using FortniteReplayReader.Models;
using System;
using Xunit;

namespace FortniteReplayReader.Test
{
    public class TestAthenaMatchStats
    {
        private FortniteReplay LoadReplayInfo(string path)
        {
            var replayReader = new FortniteReplayReader();
            return replayReader.ReadReplay(path);
        }

        private void AssertReplay(Stats expected, Stats actual)
        {
            Assert.Equal(expected.Eliminations, actual.Eliminations);
            Assert.Equal(expected.Assists, actual.Assists);
            Assert.Equal(expected.Revives, actual.Revives);
            Assert.Equal(expected.Accuracy, Math.Round(actual.Accuracy * 100));
            Assert.Equal(expected.MaterialsUsed, actual.MaterialsUsed);
            Assert.Equal(expected.MaterialsGathered, actual.MaterialsGathered);
            Assert.Equal(expected.DamageTaken, actual.DamageTaken);
            Assert.Equal(expected.WeaponDamage, actual.WeaponDamage);
            Assert.Equal(expected.OtherDamage, actual.OtherDamage);
            Assert.Equal(expected.DamageToPlayers, actual.DamageToPlayers);
            Assert.Equal(expected.DamageToStructures, actual.DamageToStructures);
            Assert.Equal((int)expected.TotalTraveled, actual.TotalTraveled.CentimetersToDistance());
        }

        [Fact]
        public void TestAthenaMatchStats1()
        {
            var replayFile = @"Replays/UnsavedReplay-2018.10.06-22.00.32.replay";
            var replay = LoadReplayInfo(replayFile);

            var expected = new Stats
            {
                Eliminations = 0,
                Revives = 0,
                Assists = 0,
                Accuracy = 0,
                MaterialsUsed = 0,
                MaterialsGathered = 27,
                DamageTaken = 221,
                WeaponDamage = 0,
                OtherDamage = 0,
                DamageToStructures = 222,
                TotalTraveled = 1,
            };

            AssertReplay(expected, replay.Stats);
        }

        [Fact]
        public void TestAthenaMatchStats2()
        {
            var replayFile = @"Replays/UnsavedReplay-2018.10.17-20.22.26.replay";
            var replay = LoadReplayInfo(replayFile);

            var expected = new Stats
            {
                Eliminations = 2,
                Revives = 0,
                Assists = 0,
                Accuracy = 53,
                MaterialsUsed = 110,
                MaterialsGathered = 531,
                DamageTaken = 301,
                WeaponDamage = 377,
                OtherDamage = 68,
                DamageToStructures = 6905,
                TotalTraveled = 2,
            };

            AssertReplay(expected, replay.Stats);
        }

        [Fact]
        public void TestAthenaMatchStats3()
        {
            var replayFile = @"Replays/UnsavedReplay-2018.10.17-20.33.41.replay";
            var replay = LoadReplayInfo(replayFile);

            var expected = new Stats
            {
                Eliminations = 3,
                Revives = 0,
                Assists = 4,
                Accuracy = 22,
                MaterialsUsed = 710,
                MaterialsGathered = 2063,
                DamageTaken = 839,
                WeaponDamage = 753,
                OtherDamage = 119,
                DamageToStructures = 43504,
                TotalTraveled = 4,
            };

            AssertReplay(expected, replay.Stats);
        }

        [Fact]
        public void TestAthenaMatchStatsUpdate910()
        {
            var replayFile = @"Replays/UnsavedReplay-2019.05.22-16.58.41.replay";
            var replay = LoadReplayInfo(replayFile);
            Assert.NotEmpty(replay.Eliminations);
        }
    }
}
