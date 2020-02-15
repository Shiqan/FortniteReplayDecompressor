using System.IO;
using Unreal.Core.Models.Enums;
using Unreal.Core.Test.Mocks;
using Xunit;

namespace Unreal.Core.Test
{
    public class ReadDemoFrameTest
    {
        [Fact]
        public void ReadDemoFrameIntoPlaybackPacketsTest()
        {
            for (var i = 0; i < 4; i++)
            {
                var data = $"DemoFrames/demopackets-{i}.dump";
                using var stream = File.Open(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var archive = new Unreal.Core.BinaryReader(stream);
                archive.EngineNetworkVersion = EngineNetworkVersionHistory.HISTORY_OPTIONALLY_QUANTIZE_SPAWN_INFO;
                archive.NetworkVersion = NetworkVersionHistory.HISTORY_CHARACTER_MOVEMENT_NOINTERP;
                archive.ReplayHeaderFlags = ReplayHeaderFlags.HasStreamingFixes;

                var reader = new MockReplayReader();
                reader.ReadDemoFrameIntoPlaybackPackets(archive);
                Assert.True(archive.AtEnd());
                Assert.False(archive.IsError);
            }
        }
    }
}
