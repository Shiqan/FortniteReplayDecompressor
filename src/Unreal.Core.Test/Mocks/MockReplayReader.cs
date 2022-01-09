using Microsoft.Extensions.Logging;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Test.Mocks;

public class MockReplayReader : ReplayReader<MockReplay>
{
    public MockReplayReader(INetGuidCache guidCache = null, INetFieldParser parser = null, ILogger logger = null) : base(guidCache, parser, logger)
    {
        Replay = new MockReplay();
    }

    public void SetReplay(MockReplay replay) => Replay = replay;

    public MockReplay GetReplay() => Replay;

    public void ReadNetExportGuids(FArchive archive) => base.ReadNetExportGuids(archive);
    public void ReadNetFieldExport(FArchive archive) => base.ReadNetFieldExport(archive);
    public void ReadNetFieldExports(FArchive archive) => base.ReadNetFieldExports(archive);
    public void ReadExternalData(FArchive archive) => base.ReadExternalData(archive);
    public void InternalLoadObject(FArchive archive, bool isExportingNetGUIDBunch, int internalLoadObjectRecursionCount = 0) => base.InternalLoadObject(archive, isExportingNetGUIDBunch, internalLoadObjectRecursionCount);
    public void ReadReplayInfo(FArchive archive) => base.ReadReplayInfo(archive);
    public void ReadReplayHeader(FArchive archive) => base.ReadReplayHeader(archive);
    public void ReadReplayData(FArchive archive, int fallbackChunkSize) => base.ReadReplayData(archive, fallbackChunkSize);
    public void ReadReplayChunks(FArchive archive) => base.ReadReplayChunks(archive);
    public PacketState ReadPacket(FArchive archive) => base.ReadPacket(archive);
    public void ReadEvent(FArchive archive) => base.ReadEvent(archive);
    public void ReadDemoFrameIntoPlaybackPackets(FArchive archive) => base.ReadDemoFrameIntoPlaybackPackets(archive);

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

