﻿using System.IO;
using Unreal.Core.Test.Mocks;
using Xunit;

namespace Unreal.Core.Test;

public class NetExportGuidsTest
{
    [Theory]
    [InlineData(new byte[] {
        0x3F, 0x02, 0x07, 0x06, 0x07, 0x0A, 0x07, 0x0E, 0x07, 0x00, 0x39, 0x00,
        0x00, 0x00, 0x2F, 0x54, 0x65, 0x6D, 0x70, 0x2F, 0x47, 0x61, 0x6D, 0x65,
        0x2F, 0x41, 0x74, 0x68, 0x65, 0x6E, 0x61, 0x2F, 0x4D, 0x61, 0x70, 0x73,
        0x2F, 0x50, 0x4F, 0x49, 0x2F, 0x41, 0x74, 0x68, 0x65, 0x6E, 0x61, 0x5F,
        0x50, 0x4F, 0x49, 0x5F, 0x4C, 0x6F, 0x62, 0x62, 0x79, 0x5F, 0x30, 0x30,
        0x32, 0x5F, 0x33, 0x66, 0x35, 0x61, 0x62, 0x34, 0x35, 0x63, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x15, 0x00, 0x00, 0x00, 0x41, 0x74, 0x68, 0x65, 0x6E,
        0x61, 0x5F, 0x50, 0x4F, 0x49, 0x5F, 0x4C, 0x6F, 0x62, 0x62, 0x79, 0x5F,
        0x30, 0x30, 0x32, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00,
        0x50, 0x65, 0x72, 0x73, 0x69, 0x73, 0x74, 0x65, 0x6E, 0x74, 0x4C, 0x65,
        0x76, 0x65, 0x6C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x25, 0x00, 0x00, 0x00,
        0x54, 0x69, 0x65, 0x72, 0x65, 0x64, 0x5F, 0x41, 0x74, 0x68, 0x65, 0x6E,
        0x61, 0x5F, 0x46, 0x6C, 0x6F, 0x6F, 0x72, 0x4C, 0x6F, 0x6F, 0x74, 0x5F,
        0x57, 0x61, 0x72, 0x6D, 0x75, 0x70, 0x35, 0x33, 0x5F, 0x31, 0x33, 0x34,
        0x00, 0x00, 0x00, 0x00, 0x00
    })]
    [InlineData(new byte[] {
        0x6B, 0x04, 0x05, 0x6F, 0x04, 0x05, 0x00, 0x15, 0x00, 0x00, 0x00, 0x2F,
        0x53, 0x63, 0x72, 0x69, 0x70, 0x74, 0x2F, 0x46, 0x6F, 0x72, 0x74, 0x6E,
        0x69, 0x74, 0x65, 0x47, 0x61, 0x6D, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x1A, 0x00, 0x00, 0x00, 0x44, 0x65, 0x66, 0x61, 0x75, 0x6C, 0x74, 0x5F,
        0x5F, 0x46, 0x6F, 0x72, 0x74, 0x50, 0x69, 0x63, 0x6B, 0x75, 0x70, 0x41,
        0x74, 0x68, 0x65, 0x6E, 0x61, 0x00, 0x61, 0x70, 0xB7, 0x51
    })]
    [InlineData(new byte[] {
        0x8B, 0x04, 0x07, 0x06, 0x06, 0x19, 0x00, 0x00, 0x00, 0x41, 0x74, 0x68,
        0x65, 0x6E, 0x61, 0x5F, 0x50, 0x4F, 0x49, 0x5F, 0x4C, 0x6F, 0x62, 0x62,
        0x79, 0x5F, 0x30, 0x30, 0x32, 0x5F, 0x43, 0x5F, 0x32, 0x00, 0x55, 0xAC,
        0xF5, 0x87
    })]
    public void InternalLoadObjectTest(byte[] rawData)
    {
        using var stream = new MemoryStream(rawData);
        using var archive = new Unreal.Core.BinaryReader(stream);
        var reader = new MockReplayReader();
        reader.InternalLoadObject(archive, true);
        Assert.True(archive.AtEnd());
        Assert.False(archive.IsError);
    }

    [Theory]
    [InlineData(new byte[] {
        0x08, 0x4D, 0x00, 0x00, 0x00, 0x93, 0x22, 0x05, 0x97, 0x22, 0x05, 0x00,
        0x29, 0x00, 0x00, 0x00, 0x2F, 0x47, 0x61, 0x6D, 0x65, 0x2F, 0x49, 0x74,
        0x65, 0x6D, 0x73, 0x2F, 0x52, 0x65, 0x73, 0x6F, 0x75, 0x72, 0x63, 0x65,
        0x50, 0x69, 0x63, 0x6B, 0x75, 0x70, 0x73, 0x2F, 0x57, 0x6F, 0x6F, 0x64,
        0x49, 0x74, 0x65, 0x6D, 0x44, 0x61, 0x74, 0x61, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x0D, 0x00, 0x00, 0x00, 0x57, 0x6F, 0x6F, 0x64, 0x49, 0x74, 0x65,
        0x6D, 0x44, 0x61, 0x74, 0x61, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7E, 0x00,
        0x00, 0x00, 0x9B, 0x22, 0x05, 0x9F, 0x22, 0x05, 0x00, 0x41, 0x00, 0x00,
        0x00, 0x2F, 0x47, 0x61, 0x6D, 0x65, 0x2F, 0x41, 0x74, 0x68, 0x65, 0x6E,
        0x61, 0x2F, 0x49, 0x74, 0x65, 0x6D, 0x73, 0x2F, 0x57, 0x65, 0x61, 0x70,
        0x6F, 0x6E, 0x73, 0x2F, 0x57, 0x49, 0x44, 0x5F, 0x41, 0x73, 0x73, 0x61,
        0x75, 0x6C, 0x74, 0x5F, 0x53, 0x65, 0x6D, 0x69, 0x41, 0x75, 0x74, 0x6F,
        0x5F, 0x41, 0x74, 0x68, 0x65, 0x6E, 0x61, 0x5F, 0x52, 0x5F, 0x4F, 0x72,
        0x65, 0x5F, 0x54, 0x30, 0x33, 0x00, 0x00, 0x00, 0x00, 0x00, 0x26, 0x00,
        0x00, 0x00, 0x57, 0x49, 0x44, 0x5F, 0x41, 0x73, 0x73, 0x61, 0x75, 0x6C,
        0x74, 0x5F, 0x53, 0x65, 0x6D, 0x69, 0x41, 0x75, 0x74, 0x6F, 0x5F, 0x41,
        0x74, 0x68, 0x65, 0x6E, 0x61, 0x5F, 0x52, 0x5F, 0x4F, 0x72, 0x65, 0x5F,
        0x54, 0x30, 0x33, 0x00, 0x00, 0x00, 0x00, 0x00, 0x4F, 0x00, 0x00, 0x00,
        0xA3, 0x22, 0x05, 0xA7, 0x22, 0x05, 0x00, 0x2A, 0x00, 0x00, 0x00, 0x2F,
        0x47, 0x61, 0x6D, 0x65, 0x2F, 0x49, 0x74, 0x65, 0x6D, 0x73, 0x2F, 0x52,
        0x65, 0x73, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x50, 0x69, 0x63, 0x6B, 0x75,
        0x70, 0x73, 0x2F, 0x53, 0x74, 0x6F, 0x6E, 0x65, 0x49, 0x74, 0x65, 0x6D,
        0x44, 0x61, 0x74, 0x61, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00,
        0x00, 0x53, 0x74, 0x6F, 0x6E, 0x65, 0x49, 0x74, 0x65, 0x6D, 0x44, 0x61,
        0x74, 0x61, 0x00, 0x00, 0x00, 0x00, 0x00, 0x4F, 0x00, 0x00, 0x00, 0xAB,
        0x22, 0x05, 0xAF, 0x22, 0x05, 0x00, 0x2A, 0x00, 0x00, 0x00, 0x2F, 0x47,
        0x61, 0x6D, 0x65, 0x2F, 0x49, 0x74, 0x65, 0x6D, 0x73, 0x2F, 0x52, 0x65,
        0x73, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x50, 0x69, 0x63, 0x6B, 0x75, 0x70,
        0x73, 0x2F, 0x4D, 0x65, 0x74, 0x61, 0x6C, 0x49, 0x74, 0x65, 0x6D, 0x44,
        0x61, 0x74, 0x61, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x00,
        0x4D, 0x65, 0x74, 0x61, 0x6C, 0x49, 0x74, 0x65, 0x6D, 0x44, 0x61, 0x74,
        0x61, 0x00, 0x00, 0x00, 0x00, 0x00
    })]
    [InlineData(new byte[] {
        0x04, 0x1D, 0x00, 0x00, 0x00, 0xB3, 0x22, 0x07, 0x06, 0x06, 0x10, 0x00,
        0x00, 0x00, 0x4C, 0x46, 0x5F, 0x37, 0x78, 0x31, 0x32, 0x5F, 0x50, 0x61,
        0x72, 0x65, 0x6E, 0x74, 0x33, 0x00, 0x1F, 0xCC, 0xBA, 0x28, 0x76, 0x00,
        0x00, 0x00, 0xB7, 0x22, 0x05, 0xBB, 0x22, 0x05, 0x00, 0x3D, 0x00, 0x00,
        0x00, 0x2F, 0x47, 0x61, 0x6D, 0x65, 0x2F, 0x41, 0x74, 0x68, 0x65, 0x6E,
        0x61, 0x2F, 0x49, 0x74, 0x65, 0x6D, 0x73, 0x2F, 0x57, 0x65, 0x61, 0x70,
        0x6F, 0x6E, 0x73, 0x2F, 0x57, 0x49, 0x44, 0x5F, 0x41, 0x73, 0x73, 0x61,
        0x75, 0x6C, 0x74, 0x5F, 0x41, 0x75, 0x74, 0x6F, 0x5F, 0x41, 0x74, 0x68,
        0x65, 0x6E, 0x61, 0x5F, 0x52, 0x5F, 0x4F, 0x72, 0x65, 0x5F, 0x54, 0x30,
        0x33, 0x00, 0x00, 0x00, 0x00, 0x00, 0x22, 0x00, 0x00, 0x00, 0x57, 0x49,
        0x44, 0x5F, 0x41, 0x73, 0x73, 0x61, 0x75, 0x6C, 0x74, 0x5F, 0x41, 0x75,
        0x74, 0x6F, 0x5F, 0x41, 0x74, 0x68, 0x65, 0x6E, 0x61, 0x5F, 0x52, 0x5F,
        0x4F, 0x72, 0x65, 0x5F, 0x54, 0x30, 0x33, 0x00, 0x00, 0x00, 0x00, 0x00
    })]
    [InlineData(new byte[] {
        0x00
    })]
    public void ReadNetExprtGuidTest(byte[] rawData)
    {
        using var stream = new MemoryStream(rawData);
        using var archive = new Unreal.Core.BinaryReader(stream);
        var reader = new MockReplayReader();
        reader.ReadNetExportGuids(archive);
        Assert.True(archive.AtEnd());
        Assert.False(archive.IsError);
    }
}
