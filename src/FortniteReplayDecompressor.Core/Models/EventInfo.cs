using FortniteReplayReaderDecompressor.Core.Contracts;

namespace FortniteReplayReaderDecompressor.Core.Models
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/NetworkReplayStreaming/LocalFileNetworkReplayStreaming/Public/LocalFileNetworkReplayStreaming.h#L65
    /// </summary>
    public class EventInfo : IVisitable<EventInfo>
    {
        public string Id { get; set; }
        public string Group { get; set; }
        public string Metadata { get; set; }
        public uint StartTime { get; set; }
        public uint EndTime { get; set; }
        public int SizeInBytes { get; set; }

        public EventInfo Accept(ReplayVisitor visitor, FArchive archive)
        {
            return visitor.ReadEvent(archive);
        }
    }
}
