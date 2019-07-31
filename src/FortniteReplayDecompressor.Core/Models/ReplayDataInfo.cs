using FortniteReplayReaderDecompressor.Core.Contracts;

namespace FortniteReplayReaderDecompressor.Core.Models
{
    /// <summary>
    /// see 
    /// </summary>
    public class ReplayDataInfo : IVisitable<ReplayDataInfo>
    {
        public uint Start { get; set; }
        public uint End { get; set; }
        public uint Length { get; set; }

        public ReplayDataInfo Accept(ReplayVisitor visitor, FArchive archive)
        {
            return visitor.ReadReplayData(archive);
        }
    }
}
