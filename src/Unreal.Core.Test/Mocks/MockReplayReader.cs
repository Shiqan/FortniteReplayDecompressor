using Microsoft.Extensions.Logging;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Test.Mocks;

public class MockReplayReader : ReplayReader<MockReplay>
{
    public MockReplayReader(ILogger logger = null, ParseMode parseMode = ParseMode.Minimal) : base(logger, parseMode)
    {
        Replay = new MockReplay();
        _netGuidCache = new NetGuidCache();
    }

    public void SetReplay(MockReplay replay) => Replay = replay;

    public MockReplay GetReplay() => Replay;

    protected override void OnExportRead(uint channelIndex, INetFieldExportGroup exportGroup)
    {
        // pass
    }

    protected override void OnExternalDataRead(uint channelIndex, IExternalData update)
    {
        // pass
    }

    protected override void OnNetDeltaRead(uint channelIndex, NetDeltaUpdate update)
    {
        // pass
    }
}
