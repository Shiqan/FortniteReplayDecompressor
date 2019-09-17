using FortniteReplayReader.Extensions;
using FortniteReplayReader.Models;
using System;
using System.IO;
using Unreal.Core.Models.Enums;
using Xunit;

namespace FortniteReplayReader.Test
{
    public class PlayerElimTest
    {
        [Fact]
        public void Season40Test()
        {
            var data = $"PlayerElims/season4.0.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream)
            {
                Branch = "++Fortnite+Release-4.0"
            };
            var reader = new ReplayReader();
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
        }

        [Fact]
        public void Season42Test()
        {
            var data = $"PlayerElims/season4.2.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream)
            {
                Branch = "++Fortnite+Release-4.2"
            };
            var reader = new ReplayReader();
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
        }

        [Fact]
        public void Season43Test()
        {
            var data = $"PlayerElims/season4.3.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream)
            {
                Branch = "++Fortnite+Release-4.3"
            };
            var reader = new ReplayReader();
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
        }

        [Fact]
        public void Season5Test()
        {
            var data = $"PlayerElims/season5.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream)
            {
                Branch = "++Fortnite+Release-5.41"
            };
            var reader = new ReplayReader();
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
        }

        [Fact]
        public void Season6Test()
        {
            var data = $"PlayerElims/season6.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream)
            {
                Branch = "++Fortnite+Release-6.00"
            };
            var reader = new ReplayReader();
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
        }

        [Fact]
        public void Season7Test()
        {
            var data = $"PlayerElims/season7.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream)
            {
                Branch = "++Fortnite+Release-7.10"
            };
            var reader = new ReplayReader();
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
        }

        [Fact]
        public void Season8Test()
        {
            var data = $"PlayerElims/season8.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream)
            {
                Branch = "++Fortnite+Release-8.20"
            };
            var reader = new ReplayReader();
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
        }

        [Fact]
        public void Season9Test()
        {
            var data = $"PlayerElims/season9.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream)
            {
                EngineNetworkVersion = EngineNetworkVersionHistory.HISTORY_UPDATE9,
                Branch = "++Fortnite+Release-9.10"
            };

            var reader = new ReplayReader();
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
        }
    }
}
