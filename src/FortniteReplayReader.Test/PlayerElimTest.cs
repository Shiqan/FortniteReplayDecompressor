using FortniteReplayReader.Exceptions;
using FortniteReplayReader.Extensions;
using FortniteReplayReader.Models;
using System;
using System.IO;
using Unreal.Core.Models;
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
            using var archive = new Unreal.Core.BinaryReader(stream);
            var reader = new ReplayReader()
            {
                Branch = "++Fortnite+Release-4.0"
            };
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
            Assert.False(archive.IsError);
        }

        [Fact]
        public void Season42Test()
        {
            var data = $"PlayerElims/season4.2.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream);
            var reader = new ReplayReader()
            {
                Branch = "++Fortnite+Release-4.2"
            };
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
            Assert.False(archive.IsError);
        }

        [Fact]
        public void Season43Test()
        {
            var data = $"PlayerElims/season4.3.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream);
            var reader = new ReplayReader()
            {
                Branch = "++Fortnite+Release-4.3"
            };
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
            Assert.False(archive.IsError);
        }

        [Fact]
        public void Season5Test()
        {
            var data = $"PlayerElims/season5.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream);
            var reader = new ReplayReader()
            {
                Branch = "++Fortnite+Release-5.41"
            };
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
            Assert.False(archive.IsError);
        }

        [Fact]
        public void Season6Test()
        {
            var data = $"PlayerElims/season6.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream);
            var reader = new ReplayReader()
            {
                Branch = "++Fortnite+Release-6.00"
            };
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
            Assert.False(archive.IsError);
        }

        [Fact]
        public void Season7Test()
        {
            var data = $"PlayerElims/season7.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream);
            var reader = new ReplayReader()
            {
                Branch = "++Fortnite+Release-7.10"
            };
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
            Assert.False(archive.IsError);
        }

        [Fact]
        public void Season8Test()
        {
            var data = $"PlayerElims/season8.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream);
            var reader = new ReplayReader()
            {
                Branch = "++Fortnite+Release-8.20"
            };
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
            Assert.False(archive.IsError);
        }

        [Fact]
        public void Season9Test()
        {
            var data = $"PlayerElims/season9.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream)
            {
                EngineNetworkVersion = EngineNetworkVersionHistory.HISTORY_FAST_ARRAY_DELTA_STRUCT
            };
            var reader = new ReplayReader()
            {
                Branch = "++Fortnite+Release-9.10"
            };
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
            Assert.False(archive.IsError);
        }
        
        [Fact]
        public void Season11BotTest()
        {
            var data = $"PlayerElims/season11-bot.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream)
            {
                EngineNetworkVersion = EngineNetworkVersionHistory.HISTORY_OPTIONALLY_QUANTIZE_SPAWN_INFO
            };
            var reader = new ReplayReader()
            {
                Branch = "++Fortnite+Release-11.00"
            };
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
            Assert.False(archive.IsError);
        }
        
        [Fact]
        public void Season11Test()
        {
            var data = $"PlayerElims/season11.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream)
            {
                EngineNetworkVersion = EngineNetworkVersionHistory.HISTORY_OPTIONALLY_QUANTIZE_SPAWN_INFO
            };
            var reader = new ReplayReader()
            {
                Branch = "++Fortnite+Release-11.00"
            };
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
            Assert.False(archive.IsError);
        }
        
        [Fact]
        public void Season11update11Test()
        {
            var data = $"PlayerElims/season11.11.dump";
            using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var archive = new Unreal.Core.BinaryReader(stream)
            {
                EngineNetworkVersion = EngineNetworkVersionHistory.HISTORY_OPTIONALLY_QUANTIZE_SPAWN_INFO
            };
            var reader = new ReplayReader()
            {
                Branch = "++Fortnite+Release-11.11"
            };
            var result = reader.ParseElimination(archive, null);

            Assert.True(archive.AtEnd());
            Assert.False(archive.IsError);
        }

        [Fact]
        public void Season1131Test()
        {
            byte[] rawData = {
                0x09, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x80, 0xB2, 0x0A, 0xC9, 0x3C, 0x43, 0xEC, 0x7F, 0x3F, 0x42, 0xB8, 0x90,
                0x47, 0xC1, 0x50, 0x82, 0xC7, 0xFD, 0x61, 0x88, 0x47, 0x00, 0x00, 0x80,
                0x3F, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x80, 0x11, 0xFB, 0x7F, 0x3F, 0xC0, 0x0E, 0x49,
                0x3C, 0xA4, 0x4A, 0xA1, 0x47, 0xE1, 0x92, 0x91, 0xC7, 0x93, 0xF3, 0x82,
                0x47, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80,
                0x3F, 0x11, 0x10, 0x1F, 0x39, 0x0E, 0x4F, 0x62, 0x11, 0x47, 0xD7, 0x8D,
                0x1B, 0xE4, 0xFE, 0xE3, 0x35, 0xE5, 0x9F, 0x11, 0x10, 0x74, 0x94, 0xBB,
                0xA6, 0x39, 0x20, 0x4A, 0xDC, 0x8E, 0xC2, 0x63, 0x53, 0xA2, 0xD5, 0xB8,
                0x39, 0x08, 0x00, 0x00, 0x00, 0x00
            };

            using var stream = new MemoryStream(rawData);
            using var archive = new Unreal.Core.BinaryReader(stream)
            {
                EngineNetworkVersion = EngineNetworkVersionHistory.HISTORY_JITTER_IN_HEADER
            };
            var reader = new ReplayReader()
            {
                Branch = "++Fortnite+Release-11.31"
            };
            var result = reader.ParseElimination(archive, null);
            Assert.True(archive.AtEnd());
            Assert.False(archive.IsError);
        }

        [Fact]
        public void PlayerElimThrows()
        {
            byte[] rawData = {
                0x09, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x80, 0xB2, 0x0A, 0xC9, 0x3C, 0x43, 0xEC, 0x7F, 0x3F, 0x42, 0xB8, 0x90,
                0x47, 0xC1, 0x50, 0x82, 0xC7, 0xFD, 0x61, 0x88, 0x47, 0x00, 0x00, 0x80,
                0x3F, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x80, 0x11, 0xFB, 0x7F, 0x3F, 0xC0, 0x0E, 0x49,
                0x3C, 0xA4, 0x4A, 0xA1, 0x47, 0xE1, 0x92, 0x91, 0xC7, 0x93, 0xF3, 0x82
            };

            using var stream = new MemoryStream(rawData);
            using var archive = new Unreal.Core.BinaryReader(stream)
            {
                EngineNetworkVersion = EngineNetworkVersionHistory.HISTORY_JITTER_IN_HEADER
            };
            var reader = new ReplayReader()
            {
                Branch = "++Fortnite+Release-11.31"
            };

            Assert.Throws<PlayerEliminationException>(() => reader.ParseElimination(archive, null));
        }
    }
}
