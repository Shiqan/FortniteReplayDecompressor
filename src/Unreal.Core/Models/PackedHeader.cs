namespace Unreal.Core.Models
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/27e7396206a1b3778d357cd67b93ec718ffb0dad/Engine/Source/Runtime/Engine/Private/Net/NetPacketNotify.cpp#L80
    /// </summary>
    public static class FPackedHeader
    {
        // see https://github.com/EpicGames/UnrealEngine/blob/27e7396206a1b3778d357cd67b93ec718ffb0dad/Engine/Source/Runtime/Engine/Public/Net/NetPacketNotify.h#L36
        public const int SequenceNumberBits = 14;
        public const uint MaxSequenceHistoryLength = 256;

        const int HistoryWordCountBits = 4;
        const int SeqMask = (1 << SequenceNumberBits) - 1;
        const int HistoryWordCountMask = (1 << HistoryWordCountBits) - 1;
        const int AckSeqShift = HistoryWordCountBits;
        const int SeqShift = AckSeqShift + SequenceNumberBits;

        public static uint GetSeq(uint packed)
        {
            return packed >> (SeqShift & SeqMask);
        }

        public static uint GetAckedSeq(uint packed)
        {
            return packed >> (AckSeqShift & SeqMask);
        }

        public static uint GetHistoryWordCount(uint packed)
        {
            return packed & HistoryWordCountMask;
        }
    }
}