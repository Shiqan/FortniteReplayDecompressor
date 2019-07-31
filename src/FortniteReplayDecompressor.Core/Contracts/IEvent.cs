using FortniteReplayReaderDecompressor.Core.Models;

namespace FortniteReplayReaderDecompressor.Core.Contracts
{
    public interface IEvent
    {
        EventInfo Info { get; set; }
    }
}
