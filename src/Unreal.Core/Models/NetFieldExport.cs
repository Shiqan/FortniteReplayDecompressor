namespace Unreal.Core.Models
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/PackageMapClient.h#L36
    /// </summary>
    public class NetFieldExport
    {
        public bool IsExported { get; set; }
        public uint Handle { get; set; }
        public uint CompatibleChecksum { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Incompatible { get; set; }

        /// <summary>
        /// Index of the property for the corresponding type in NetFieldParser
        /// -1 if index is unknown.
        /// -2 if index cannot be found.
        /// </summary>
        internal int PropertyId { get; set; } = -1;
    }
}
