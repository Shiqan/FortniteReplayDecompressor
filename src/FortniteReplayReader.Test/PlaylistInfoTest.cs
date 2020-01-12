using FortniteReplayReader.Models.NetFieldExports;
using Unreal.Core;
using Xunit;

namespace FortniteReplayReader.Test
{
    public class PlaylistInfoTest
    {
        [Fact]
        public void PlaylistInfoTest1()
        {
            byte[] rawData = {
                0x5F, 0xA2, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            var reader = new NetBitReader(rawData, 49)
            {
                NetworkVersion = Unreal.Core.Models.Enums.NetworkVersionHistory.HISTORY_CHARACTER_MOVEMENT_NOINTERP,
                EngineNetworkVersion = Unreal.Core.Models.Enums.EngineNetworkVersionHistory.HISTORY_ENGINENETVERSION_LATEST // TODO upload to proper version once it is known (15)...
            };
            var playlist = new PlaylistInfo();
            playlist.Serialize(reader);

            Assert.Equal(2635u, playlist.Id); // replay 11.31
            Assert.True(reader.AtEnd());
            Assert.False(reader.IsError);

        }

        [Fact]
        public void PlaylistInfoTest2()
        {
            byte[] rawData = {
                0xDF, 0x3B, 0x00 ,0x00 ,0x00 ,0x00, 0x00
            };

            var reader = new NetBitReader(rawData, 49)
            {
                NetworkVersion = Unreal.Core.Models.Enums.NetworkVersionHistory.HISTORY_CHARACTER_MOVEMENT_NOINTERP,
                EngineNetworkVersion = Unreal.Core.Models.Enums.EngineNetworkVersionHistory.HISTORY_OPTIONALLY_QUANTIZE_SPAWN_INFO
            };
            var playlist = new PlaylistInfo();
            playlist.Serialize(reader);

            Assert.Equal(1019u, playlist.Id); // replay 11.11
            Assert.True(reader.AtEnd());
            Assert.False(reader.IsError);
        }

        [Fact]
        public void PlaylistInfoTest3()
        {
            byte[] rawData = {
                0xDF, 0x2C, 0x00 ,0x00 ,0x00 ,0x00
            };

            var reader = new NetBitReader(rawData, 48)
            {
                NetworkVersion = Unreal.Core.Models.Enums.NetworkVersionHistory.HISTORY_CHARACTER_MOVEMENT,
                EngineNetworkVersion = Unreal.Core.Models.Enums.EngineNetworkVersionHistory.HISTORY_CHANNEL_NAMES
            };
            var playlist = new PlaylistInfo();
            playlist.Serialize(reader);

            Assert.Equal(1463u, playlist.Id); // replay 6.30
            Assert.True(reader.AtEnd());
            Assert.False(reader.IsError);
        }
    }
}
