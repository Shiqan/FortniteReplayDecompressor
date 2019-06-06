using System;
using System.Collections.Generic;
using System.Text;

namespace FortniteReplayReaderDecompressor.Core.Models
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Engine/Classes/Engine/DemoNetDriver.h#L60
    /// </summary>
    public class ExternalData
    {
        public byte[] Data { get; set; }
        public int TimeSeconds { get; set; }
    }
}
