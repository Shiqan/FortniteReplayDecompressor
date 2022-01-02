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

    public string Branch
    {
        get => base.Branch;
        set => base.Branch = value;
    }
    public int Minor => base.Minor;
    public int Major => base.Major;


    public void SetMode(ParseMode mode) => _parseMode = mode;

    public void SetReplay(FortniteReplay replay) => Replay = replay;

    public FortniteReplay GetReplay() => Replay;

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


    public Models.Events.Stats ParseMatchStats(FArchive archive, EventInfo eventInfo) => base.ParseMatchStats(archive, eventInfo);
    public Models.Events.TeamStats ParseTeamStats(FArchive archive, EventInfo eventInfo) => base.ParseTeamStats(archive, eventInfo);
    public Models.Events.PlayerElimination ParseElimination(FArchive archive, EventInfo eventInfo) => base.ParseElimination(archive, eventInfo);

    public FArchive DecryptBuffer(Unreal.Core.BinaryReader archive, int v) => base.DecryptBuffer(archive, v);
}
