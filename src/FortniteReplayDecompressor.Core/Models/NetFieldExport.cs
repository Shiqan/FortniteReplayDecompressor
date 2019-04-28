namespace FortniteReplayReaderDecompressor.Core.Models
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/PackageMapClient.h#L36
    /// </summary>
    public class NetFieldExport
    {
        public uint InHandle { get; set; }
        public uint InCompatibleChecksum { get; set; }
        public string InName { get; set; }
        public string InType { get; set; }
    }
}
