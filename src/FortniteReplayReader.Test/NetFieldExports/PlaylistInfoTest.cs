using FortniteReplayReader.Models.NetFieldExports;
using Unreal.Core;
using Unreal.Core.Models.Enums;
using Xunit;

namespace FortniteReplayReader.Test;

public class PlaylistInfoTest
{
    [Theory]
    [InlineData(new byte[] { 0x5F, 0xA2, 0x00, 0x00, 0x00, 0x00, 0x00 }, 49,
        NetworkVersionHistory.HISTORY_CHARACTER_MOVEMENT_NOINTERP,
        EngineNetworkVersionHistory.HISTORY_CLASSNETCACHE_FULLNAME,
        2635u)] // replay 11.31
    [InlineData(new byte[] { 0xDF, 0x3B, 0x00, 0x00, 0x00, 0x00, 0x00 }, 49,
        NetworkVersionHistory.HISTORY_CHARACTER_MOVEMENT_NOINTERP,
        EngineNetworkVersionHistory.HISTORY_OPTIONALLY_QUANTIZE_SPAWN_INFO,
        1019u)] // replay 11.11
    [InlineData(new byte[] { 0xDF, 0x2C, 0x00, 0x00, 0x00, 0x00 }, 48,
        NetworkVersionHistory.HISTORY_CHARACTER_MOVEMENT,
        EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES,
        1463u)] // replay 6.30
    public void PlaylistInfoTest1(byte[] rawData, int bitCount,
        NetworkVersionHistory networkVersion, EngineNetworkVersionHistory engineNetworkVersion, uint id)
    {
        var reader = new NetBitReader(rawData, bitCount)
        {
            NetworkVersion = networkVersion,
            EngineNetworkVersion = engineNetworkVersion
        };
        var playlist = new PlaylistInfo();
        playlist.Serialize(reader);

        Assert.Equal(id, playlist.Id);
        Assert.True(reader.AtEnd());
        Assert.False(reader.IsError);
    }
}
