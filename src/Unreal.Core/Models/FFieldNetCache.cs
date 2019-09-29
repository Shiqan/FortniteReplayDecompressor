using System;
using System.Collections.Generic;
using System.Text;

namespace Unreal.Core.Models
{
    /// <summary>
    /// Information about a field.
    /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/CoreUObject/Public/UObject/CoreNet.h#L26
    /// </summary>
    public class FFieldNetCache
    {
        public UField Field { get; set; }
        public int FieldNetIndex { get; set; }
        public uint FieldChecksum { get; set; }
        public bool Incompatible { get; set; }
    }
}
