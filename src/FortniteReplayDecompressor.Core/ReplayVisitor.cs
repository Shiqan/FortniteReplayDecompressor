using FortniteReplayReaderDecompressor.Core.Models;

namespace FortniteReplayReaderDecompressor.Core
{
    public abstract class ReplayVisitor
    {
        public abstract void ReadReplay(FArchive archive);
        public abstract ReplayInfo ReadReplayInfo(FArchive archive);
        public abstract ReplayHeader ReadReplayHeader(FArchive archive);
        public abstract void ReadReplayChunks(FArchive archive);
        public abstract CheckpointInfo ReadCheckpoint(FArchive archive);
        public abstract ReplayDataInfo ReadReplayData(FArchive archive);
        public abstract void ReadEvent(FArchive archive);
    }
}
