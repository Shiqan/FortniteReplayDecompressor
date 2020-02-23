using FortniteReplayReader.Models;
using Microsoft.Extensions.Logging;
using Unreal.Core;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Test.Mocks
{
    public class MockReplayReader : ReplayReader
    {
        public MockReplayReader(ILogger logger = null) : base(logger)
        {
            GuidCache = new NetGuidCache();
        }

        public void SetMode(ParseMode mode)
        {
            _parseMode = mode;
        }

        public void SetReplay(FortniteReplay replay)
        {
            Replay = replay;
        }

        public FortniteReplay GetReplay()
        {
            return Replay;
        }

        internal BinaryReader DecryptBuffer(BinaryReader archive, int v)
        {
            return base.DecryptBuffer(archive, v);
        }
    }
}
