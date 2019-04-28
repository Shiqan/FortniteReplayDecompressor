using System;
using System.Collections.Generic;
using System.Text;

namespace FortniteReplayReaderDecompressor.Core.Models
{
    /// <summary>
    /// https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Misc/NetworkGuid.h#L14
    /// </summary>
    public class NetworkGUID
    {
        public uint Value { get; set; }
    }
}
