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
    }
}
