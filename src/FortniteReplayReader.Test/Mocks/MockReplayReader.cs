using FortniteReplayReader.Models;
using Unreal.Core;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Test.Mocks;

public class MockReplayReader : ReplayReader
{
    public MockReplayReader() : base()
    {
        _netGuidCache = new NetGuidCache();
    }

    public void SetMode(ParseMode mode) => _parseMode = mode;

    public void SetReplay(FortniteReplay replay) => Replay = replay;

    public FortniteReplay GetReplay() => Replay;

    internal FArchive DecryptBuffer(BinaryReader archive, int v) => base.DecryptBuffer(archive, v);
}
