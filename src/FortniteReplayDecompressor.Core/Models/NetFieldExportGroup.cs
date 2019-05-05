using System.Collections.Generic;

namespace FortniteReplayReaderDecompressor.Core.Models
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/PackageMapClient.h#L124
    /// </summary>
    public class NetFieldExportGroup
    {
        public string PathName { get; set; }
        public uint PathNameIndex { get; set; }
        public uint NetFieldExportsLenght { get; set; }
        public IList<NetFieldExport> NetFieldExports { get; set; }
    }
}
