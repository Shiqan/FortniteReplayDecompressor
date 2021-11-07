﻿using System.IO;
using Unreal.Core.Models.Enums;
using Unreal.Core.Test.Mocks;
using Xunit;

namespace Unreal.Core.Test;

public class NetFieldExportTest
{
    [Theory]
    [InlineData(new byte[] {
                0x01, 0x95, 0xAE, 0x00, 0x00, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00,
                0x49, 0x74, 0x65, 0x6D, 0x2E, 0x57, 0x65, 0x61, 0x70, 0x6F, 0x6E, 0x2E,
                0x52, 0x61, 0x6E, 0x67, 0x65, 0x64, 0x2E, 0x53, 0x68, 0x6F, 0x74, 0x67,
                0x75, 0x6E, 0x2E, 0x54, 0x61, 0x63, 0x74, 0x69, 0x63, 0x61, 0x6C, 0x00,
                0x00, 0x00, 0x00, 0x00
            })]
    [InlineData(new byte[] {
                0x01, 0x8F, 0xDC, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1E, 0x00, 0x00, 0x00,
                0x57, 0x65, 0x61, 0x70, 0x6F, 0x6E, 0x2E, 0x52, 0x61, 0x6E, 0x67, 0x65,
                0x64, 0x2E, 0x50, 0x69, 0x73, 0x74, 0x6F, 0x6C, 0x2E, 0x53, 0x74, 0x61,
                0x6E, 0x64, 0x61, 0x72, 0x64, 0x00, 0x00, 0x00, 0x00, 0x00
            })]
    [InlineData(new byte[] {
                0x01, 0x51, 0x3A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x31, 0x00, 0x00, 0x00,
                0x41, 0x74, 0x68, 0x65, 0x6E, 0x61, 0x2E, 0x4C, 0x6F, 0x63, 0x61, 0x74,
                0x69, 0x6F, 0x6E, 0x2E, 0x55, 0x6E, 0x6E, 0x61, 0x6D, 0x65, 0x64, 0x50,
                0x4F, 0x49, 0x2E, 0x43, 0x6C, 0x69, 0x66, 0x66, 0x73, 0x69, 0x64, 0x65,
                0x52, 0x75, 0x69, 0x6E, 0x65, 0x64, 0x48, 0x6F, 0x75, 0x73, 0x65, 0x73,
                0x00, 0x00, 0x00, 0x00, 0x00
            })]
    [InlineData(new byte[] {
                0x01, 0x0A, 0xE4, 0x01, 0xDC, 0xA3, 0x00, 0x13, 0x00, 0x00, 0x00, 0x52,
                0x65, 0x70, 0x6C, 0x69, 0x63, 0x61, 0x74, 0x65, 0x64, 0x4D, 0x6F, 0x76,
                0x65, 0x6D, 0x65, 0x6E, 0x74, 0x00, 0x00, 0x00, 0x00, 0x00
            })]
    public void ReadNetFieldExportTest(byte[] rawData)
    {
        using var ms = new MemoryStream(rawData);
        using var archive = new Unreal.Core.BinaryReader(ms)
        {
            EngineNetworkVersion = EngineNetworkVersionHistory.HISTORY_OPTIONALLY_QUANTIZE_SPAWN_INFO,
            NetworkVersion = NetworkVersionHistory.HISTORY_CHARACTER_MOVEMENT_NOINTERP,
            ReplayHeaderFlags = ReplayHeaderFlags.HasStreamingFixes
        };

        var reader = new MockReplayReader();
        reader.ReadNetFieldExport(archive);
        Assert.True(archive.AtEnd());
        Assert.False(archive.IsError);
    }



    [Theory]
    [InlineData(new byte[] {
            0x22, 0x02, 0x02, 0x26, 0x00, 0x00, 0x00, 0x2F, 0x53, 0x63, 0x72, 0x69, 0x70,
            0x74, 0x2F, 0x46, 0x6F, 0x72, 0x74, 0x6E, 0x69, 0x74, 0x65, 0x47, 0x61, 0x6D,
            0x65, 0x2E, 0x46, 0x6F, 0x72, 0x74, 0x50, 0x69, 0x63, 0x6B, 0x75, 0x70, 0x41,
            0x74, 0x68, 0x65, 0x6E, 0x61, 0x00, 0xBC, 0x01, 0x02, 0x46, 0xA7, 0x6D, 0x6F,
            0x00, 0x13, 0x00, 0x00, 0x00, 0x62, 0x52, 0x65, 0x70, 0x6C, 0x69, 0x63, 0x61,
            0x74, 0x65, 0x4D, 0x6F, 0x76, 0x65, 0x6D, 0x65, 0x6E, 0x74, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x02, 0x00, 0x01, 0x08, 0x6C, 0xCF, 0x68, 0x7E, 0x01, 0xB1, 0x02,
            0x02, 0x00, 0x01, 0x0A, 0xE4, 0x01, 0xDC, 0xA3, 0x00, 0x13, 0x00, 0x00, 0x00,
            0x52, 0x65, 0x70, 0x6C, 0x69, 0x63, 0x61, 0x74, 0x65, 0x64, 0x4D, 0x6F, 0x76,
            0x65, 0x6D, 0x65, 0x6E, 0x74, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x01,
            0x1A, 0x45, 0xB5, 0x46, 0x84, 0x01, 0xAF, 0x02, 0x02, 0x00, 0x01, 0x1E, 0x58,
            0x1B, 0xCC, 0xA3, 0x00, 0x10, 0x00, 0x00, 0x00, 0x62, 0x52, 0x61, 0x6E, 0x64,
            0x6F, 0x6D, 0x52, 0x6F, 0x74, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x02, 0x00, 0x01, 0x22, 0x9D, 0xE5, 0x39, 0x05, 0x00, 0x0F, 0x00,
            0x00, 0x00, 0x49, 0x74, 0x65, 0x6D, 0x44, 0x65, 0x66, 0x69, 0x6E, 0x69, 0x74,
            0x69, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x01, 0x26, 0x6C,
            0x7D, 0x5C, 0xA8, 0x00, 0x0B, 0x00, 0x00, 0x00, 0x44, 0x75, 0x72, 0x61, 0x62,
            0x69, 0x6C, 0x69, 0x74, 0x79, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x01,
            0x28, 0x7F, 0x41, 0x8E, 0xFE, 0x00, 0x06, 0x00, 0x00, 0x00, 0x4C, 0x65, 0x76,
            0x65, 0x6C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x01, 0x3A, 0xF4, 0xE5,
            0x34, 0x82, 0x00, 0x09, 0x00, 0x00, 0x00, 0x62, 0x49, 0x73, 0x44, 0x69, 0x72,
            0x74, 0x79, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x01, 0x3E, 0xD3, 0x5D,
            0x02, 0x7D, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x53, 0x74, 0x61, 0x74, 0x65, 0x56,
            0x61, 0x6C, 0x75, 0x65, 0x73, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x01,
            0x40, 0xB5, 0x46, 0x04, 0x1A, 0x00, 0x09, 0x00, 0x00, 0x00, 0x49, 0x6E, 0x74,
            0x56, 0x61, 0x6C, 0x75, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x01,
            0x42, 0x27, 0x60, 0x0B, 0x21, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x4E, 0x61, 0x6D,
            0x65, 0x56, 0x61, 0x6C, 0x75, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00,
            0x01, 0x44, 0x76, 0x5E, 0xFC, 0xA6, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x53, 0x74,
            0x61, 0x74, 0x65, 0x54, 0x79, 0x70, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02,
            0x00, 0x01, 0xAC, 0x2E, 0x4B, 0xCB, 0x51, 0x00, 0x15, 0x00, 0x00, 0x00, 0x62,
            0x54, 0x6F, 0x73, 0x73, 0x65, 0x64, 0x46, 0x72, 0x6F, 0x6D, 0x43, 0x6F, 0x6E,
            0x74, 0x61, 0x69, 0x6E, 0x65, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00,
            0x01, 0xB2, 0x17, 0x9B, 0xDC, 0x6A, 0x00, 0x19, 0x00, 0x00, 0x00, 0x62, 0x53,
            0x65, 0x72, 0x76, 0x65, 0x72, 0x53, 0x74, 0x6F, 0x70, 0x70, 0x65, 0x64, 0x53,
            0x69, 0x6D, 0x75, 0x6C, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x02, 0x00, 0x01, 0xB4, 0x9B, 0xCE, 0x2E, 0x6C, 0x00, 0x17, 0x00, 0x00,
            0x00, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x49, 0x6D, 0x70, 0x61, 0x63, 0x74,
            0x53, 0x6F, 0x75, 0x6E, 0x64, 0x46, 0x6C, 0x61, 0x73, 0x68, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x02, 0x00, 0x01, 0x20, 0x1D, 0xF0, 0xF6, 0x05, 0x00, 0x06, 0x00,
            0x00, 0x00, 0x43, 0x6F, 0x75, 0x6E, 0x74, 0x00, 0x00, 0x00, 0x00, 0x00
        })]
    [InlineData(new byte[] {
            0x02, 0x2A, 0x00, 0x01, 0xB9, 0x02, 0x74, 0x79, 0x03, 0x8F, 0x00, 0x0D, 0x00,
            0x00, 0x00, 0x46, 0x6F, 0x72, 0x63, 0x65, 0x50, 0x6C, 0x61, 0x79, 0x42, 0x69,
            0x74, 0x00, 0x00, 0x00, 0x00, 0x00
        })]
    public void ReadNetFieldExportsTest(byte[] rawData)
    {
        using var stream = new MemoryStream(rawData);
        using var archive = new Unreal.Core.BinaryReader(stream)
        {
            EngineNetworkVersion = EngineNetworkVersionHistory.HISTORY_OPTIONALLY_QUANTIZE_SPAWN_INFO,
            NetworkVersion = NetworkVersionHistory.HISTORY_CHARACTER_MOVEMENT_NOINTERP,
            ReplayHeaderFlags = ReplayHeaderFlags.HasStreamingFixes
        };

        var reader = new MockReplayReader();
        reader.ReadNetFieldExports(archive);
        Assert.True(archive.AtEnd());
        Assert.False(archive.IsError);
    }
}
