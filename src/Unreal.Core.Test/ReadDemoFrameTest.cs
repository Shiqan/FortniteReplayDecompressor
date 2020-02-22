using System.IO;
using Unreal.Core.Models.Enums;
using Unreal.Core.Test.Mocks;
using Xunit;

namespace Unreal.Core.Test
{
    public class ReadDemoFrameTest
    {
        [Theory]
        [InlineData("DemoFrames/demopackets-0.dump")]
        [InlineData("DemoFrames/demopackets-1.dump")]
        [InlineData("DemoFrames/demopackets-2.dump")]
        [InlineData("DemoFrames/demopackets-3.dump")]
        public void ReadDemoFrameIntoPlaybackPacketsTest(string data)
        {
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
