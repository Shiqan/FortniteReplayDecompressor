﻿using System.IO;
using Unreal.Core.Exceptions;
using Unreal.Core.Test.Mocks;
using Xunit;

namespace Unreal.Core.Test;

public class ReplayHeaderTest
{
    [Theory]
    [InlineData(new byte[] {
            0x3D, 0xA1, 0xF5, 0x2C, 0x0D, 0x00, 0x00, 0x00, 0xF2, 0xEB, 0xED, 0x11,
            0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x6B, 0xAE, 0xDA, 0xF0,
            0x70, 0x40, 0x1D, 0x4E, 0x8D, 0xE8, 0x21, 0xBD, 0x28, 0xB6, 0x20, 0x04,
            0x04, 0x00, 0x16, 0x00, 0x00, 0x00, 0xF6, 0xC4, 0x49, 0x00, 0x18, 0x00,
            0x00, 0x00, 0x2B, 0x2B, 0x46, 0x6F, 0x72, 0x74, 0x6E, 0x69, 0x74, 0x65,
            0x2B, 0x52, 0x65, 0x6C, 0x65, 0x61, 0x73, 0x65, 0x2D, 0x37, 0x2E, 0x33,
            0x30, 0x00, 0x01, 0x00, 0x00, 0x00, 0x21, 0x00, 0x00, 0x00, 0x2F, 0x47,
            0x61, 0x6D, 0x65, 0x2F, 0x41, 0x74, 0x68, 0x65, 0x6E, 0x61, 0x2F, 0x4D,
            0x61, 0x70, 0x73, 0x2F, 0x41, 0x74, 0x68, 0x65, 0x6E, 0x61, 0x5F, 0x54,
            0x65, 0x72, 0x72, 0x61, 0x69, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
            0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x53,
            0x75, 0x62, 0x47, 0x61, 0x6D, 0x65, 0x3D, 0x41, 0x74, 0x68, 0x65, 0x6E,
            0x61, 0x00
        })]
    [InlineData(new byte[] {
            0x3D, 0xA1, 0xF5, 0x2C, 0x0E, 0x00, 0x00, 0x00, 0xB0, 0x9F, 0x83, 0xDE,
            0x0B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xEC, 0x54, 0x16, 0x37,
            0xA5, 0xE2, 0x00, 0x47, 0xAC, 0x7A, 0xF5, 0xE5, 0x69, 0x55, 0xA9, 0x10,
            0x04, 0x00, 0x17, 0x00, 0x00, 0x00, 0xCC, 0x1B, 0x74, 0x00, 0x18, 0x00,
            0x00, 0x00, 0x2B, 0x2B, 0x46, 0x6F, 0x72, 0x74, 0x6E, 0x69, 0x74, 0x65,
            0x2B, 0x52, 0x65, 0x6C, 0x65, 0x61, 0x73, 0x65, 0x2D, 0x39, 0x2E, 0x34,
            0x31, 0x00, 0x01, 0x00, 0x00, 0x00, 0x21, 0x00, 0x00, 0x00, 0x2F, 0x47,
            0x61, 0x6D, 0x65, 0x2F, 0x41, 0x74, 0x68, 0x65, 0x6E, 0x61, 0x2F, 0x4D,
            0x61, 0x70, 0x73, 0x2F, 0x41, 0x74, 0x68, 0x65, 0x6E, 0x61, 0x5F, 0x54,
            0x65, 0x72, 0x72, 0x61, 0x69, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
            0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x53,
            0x75, 0x62, 0x47, 0x61, 0x6D, 0x65, 0x3D, 0x41, 0x74, 0x68, 0x65, 0x6E,
            0x61, 0x00
        })]
    [InlineData(new byte[] {
            0x3D, 0xA1, 0xF5, 0x2C, 0x0E, 0x00, 0x00, 0x00, 0x13, 0x62, 0x03, 0x7F,
            0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x41, 0x9F, 0x4D,
            0x23, 0x73, 0xAF, 0x49, 0x93, 0xD3, 0x9C, 0xA5, 0xAC, 0x4B, 0xF8, 0x61,
            0x04, 0x00, 0x19, 0x00, 0x00, 0x00, 0x5A, 0x56, 0xB0, 0x00, 0x19, 0x00,
            0x00, 0x00, 0x2B, 0x2B, 0x46, 0x6F, 0x72, 0x74, 0x6E, 0x69, 0x74, 0x65,
            0x2B, 0x52, 0x65, 0x6C, 0x65, 0x61, 0x73, 0x65, 0x2D, 0x31, 0x32, 0x2E,
            0x30, 0x30, 0x00, 0x01, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x2F,
            0x47, 0x61, 0x6D, 0x65, 0x2F, 0x41, 0x74, 0x68, 0x65, 0x6E, 0x61, 0x2F,
            0x41, 0x70, 0x6F, 0x6C, 0x6C, 0x6F, 0x2F, 0x4D, 0x61, 0x70, 0x73, 0x2F,
            0x41, 0x70, 0x6F, 0x6C, 0x6C, 0x6F, 0x5F, 0x54, 0x65, 0x72, 0x72, 0x61,
            0x69, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x00, 0x00, 0x01,
            0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x53, 0x75, 0x62, 0x47, 0x61,
            0x6D, 0x65, 0x3D, 0x41, 0x74, 0x68, 0x65, 0x6E, 0x61, 0x00
        })]
    public void ReadReplayHeader(byte[] rawData)
    {
        using var stream = new MemoryStream(rawData);
        using var archive = new Unreal.Core.BinaryReader(stream);
        var reader = new MockReplayReader();
        reader.ReadReplayHeader(archive);
        Assert.True(archive.AtEnd());
        Assert.False(archive.IsError);
    }

    [Fact]
    public void ReadReplayHeaderThrowsOnWrongMagicTest()
    {
        byte[] rawData = {
                0x7F, 0xE2, 0xA2, 0x00, 0x06, 0x00, 0x00, 0x00, 0x62, 0xD6, 0x01, 0x00
            };
        using var stream = new MemoryStream(rawData);
        using var archive = new Unreal.Core.BinaryReader(stream);
        var reader = new MockReplayReader();

        var exception = Assert.Throws<InvalidReplayException>(() => reader.ReadReplayHeader(archive));
        Assert.Equal("Header.Magic != NETWORK_DEMO_MAGIC. Header.Magic: 10674815, NETWORK_DEMO_MAGIC: 754295101", exception.Message);
    }

    [Fact]
    public void ReadReplayHeaderThrowsOnWrongVersionTest()
    {
        byte[] rawData = {
                0x3D, 0xA1, 0xF5, 0x2C, 0x04, 0x00, 0x00, 0x00, 0x62, 0xD6, 0x01, 0x00
            };
        using var stream = new MemoryStream(rawData);
        using var archive = new Unreal.Core.BinaryReader(stream);
        var reader = new MockReplayReader();

        var exception = Assert.Throws<InvalidReplayException>(() => reader.ReadReplayHeader(archive));
        Assert.Equal("Header.Version < MIN_NETWORK_DEMO_VERSION. Header.Version: HISTORY_SAVE_ENGINE_VERSION, MIN_NETWORK_DEMO_VERSION: HISTORY_EXTRA_VERSION", exception.Message);
    }
}
