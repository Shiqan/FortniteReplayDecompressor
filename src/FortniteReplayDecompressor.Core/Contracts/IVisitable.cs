namespace FortniteReplayReaderDecompressor.Core.Contracts
{
    public interface IVisitable<T>
    {
        T Accept(ReplayVisitor visitor, FArchive archive);
    }
}
