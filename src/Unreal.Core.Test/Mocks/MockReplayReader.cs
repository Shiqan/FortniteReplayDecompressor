using Microsoft.Extensions.Logging;
using Unreal.Core.Contracts;
using Unreal.Core.Models;

namespace Unreal.Core.Test.Mocks
{
    public class MockReplayReader : ReplayReader<MockReplay>
    {
        public MockReplayReader(ILogger logger = null) : base(logger)
        {
            Replay = new MockReplay();
            GuidCache = new NetGuidCache();
        }

        public void SetReplay(MockReplay replay)
        {
            Replay = replay;
        }

        public MockReplay GetReplay()
        {
            return Replay;
        }

        protected override void OnChannelClosed(uint channelIndex, NetworkGUID actor)
        {
            // pass
        }

        protected override void OnChannelOpened(uint channelIndex, NetworkGUID actor)
        {
            // pass
        }

        protected override void OnExportRead(uint channelIndex, INetFieldExportGroup exportGroup)
        {
            // pass
        }
    }
}
