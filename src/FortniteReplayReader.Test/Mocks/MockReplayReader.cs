using FortniteReplayReader;
using FortniteReplayReader.Models;
using Microsoft.Extensions.Logging;
using System;
using Unreal.Core;
using Unreal.Core.Contracts;
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
            this._parseMode = mode;
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
