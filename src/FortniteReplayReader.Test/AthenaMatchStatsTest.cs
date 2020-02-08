using FortniteReplayReader.Extensions;
using FortniteReplayReader.Models;
using FortniteReplayReader.Models.Events;
using System;
using System.IO;
using Xunit;

namespace FortniteReplayReader.Test
{
    public class TestAthenaMatchStats
    {
        private void AssertEqual(Stats expected, Stats actual)
        {
            Assert.Equal(expected.Eliminations, actual.Eliminations);
            Assert.Equal(expected.Assists, actual.Assists);
            Assert.Equal(expected.Revives, actual.Revives);
            Assert.Equal(expected.Accuracy, (float) Math.Round(actual.Accuracy * 100));
            Assert.Equal(expected.MaterialsUsed, actual.MaterialsUsed);
            Assert.Equal(expected.MaterialsGathered, actual.MaterialsGathered);
            Assert.Equal(expected.DamageTaken, actual.DamageTaken);
            Assert.Equal(expected.WeaponDamage, actual.WeaponDamage);
            Assert.Equal(expected.OtherDamage, actual.OtherDamage);
            Assert.Equal(expected.DamageToPlayers, actual.DamageToPlayers);
            Assert.Equal(expected.DamageToStructures, actual.DamageToStructures);
            Assert.Equal(expected.TotalTraveled, (uint) actual.TotalTraveled.CentimetersToDistance());
        }

        [Fact]
        public void TestAthenaMatchStats0()
        {
            var data = $"AthenaMatchStats/matchstats0.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream);
            var reader = new ReplayReader();
            var result = reader.ParseMatchStats(archive, null);

            Assert.True(archive.AtEnd());
            Assert.False(archive.IsError);

            var expected = new Stats
            {
                Info = null,
                Unknown = 0,
                Accuracy = 24f,
                Eliminations = 0,
                Assists = 2,
                WeaponDamage = 314,
                OtherDamage = 14,
                Revives = 0,
                DamageTaken = 338,
                DamageToStructures = 1026,
                MaterialsGathered = 28,
                MaterialsUsed = 10,
                TotalTraveled = 1,
            };

            AssertEqual(expected, result);
        }

        [Fact]
        public void TestAthenaMatchStats1()
        {
            var data = $"AthenaMatchStats/matchstats1.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream);
            var reader = new ReplayReader();
            var result = reader.ParseMatchStats(archive, null);

            Assert.True(archive.AtEnd());
            Assert.False(archive.IsError);

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

            AssertEqual(expected, result);
        }

        [Fact]
        public void TestAthenaMatchStats2()
        {
            var data = $"AthenaMatchStats/matchstats2.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream);
            var reader = new ReplayReader();
            var result = reader.ParseMatchStats(archive, null);

            Assert.True(archive.AtEnd());
            Assert.False(archive.IsError);

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

            AssertEqual(expected, result);
        }
    }
}
