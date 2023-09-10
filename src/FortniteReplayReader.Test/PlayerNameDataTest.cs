using FortniteReplayReader.Models.NetFieldExports;
using System.IO;
using Xunit;

namespace FortniteReplayReader.Test;

public class PlayerNameDataTest
{
    [Theory]
    [InlineData(new byte[] {
        0x19, 0xFB, 0x01, 0x00, 0x00, 0x00, 0x00
    }, true, "")]
    [InlineData(new byte[] {
        0x19, 0xFB, 0x00, 0x00, 0x00, 0x00, 0x00
    }, false, "")]
    [InlineData(new byte[] {
        0x19, 0xFB, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x66,
        0x69, 0x6C, 0x69, 0x70, 0x69, 0x6E, 0x68, 0x30,
        0x73, 0x70, 0x00
    }, false, "filipinh0sp")]
    [InlineData(new byte[] {
        0x19, 0xFB, 0x01, 0x07, 0x00, 0x00, 0x00, 0x4E,
        0x68, 0x66, 0x6B, 0x60, 0x6A, 0x00
    }, true, "Shiqan")]
    public void ParsePlayerNameDataTest(byte[] rawData, bool isPlayer, string playerName)
    {
        using MemoryStream stream = new(rawData);
        using Unreal.Core.BinaryReader archive = new(stream);
        PlayerNameData playerNameData = new(archive);

        Assert.True(archive.AtEnd());
        Assert.False(archive.IsError);

        Assert.Equal(25, playerNameData.Handle);
        Assert.Equal(isPlayer, playerNameData.IsPlayer);
        Assert.Equal(playerName, playerNameData.DecodedName);
    }
}
