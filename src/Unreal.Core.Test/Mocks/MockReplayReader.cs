using Unreal.Core.Contracts;

namespace Unreal.Core.Test.Mocks
{
    public class MockReplayReader : ReplayReader<MockReplay>
    {
        public MockReplayReader()
        {
            Replay = new MockReplay();
        }

        public MockReplay GetReplay()
        {
            return Replay;
        }

        protected override void OnExportRead(uint channel, INetFieldExportGroup exportGroup)
        {
            // pass
        }
    }
}
