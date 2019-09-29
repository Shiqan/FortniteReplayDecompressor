using System;
using System.Collections.Generic;

namespace Unreal.Core.Models
{
    /// <summary>
    /// Information about a class, cached for network coordination.
    /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/CoreUObject/Public/UObject/CoreNet.h#L44
    /// </summary>
    public class FClassNetCache
    {
        public FFieldNetCache[] Fields { get; set; }
        //public Dictionary<UObject, FFieldNetCache> FieldMap { get; set; }
        public Dictionary<int, FFieldNetCache> FieldChecksumMap { get; set; }

        public FFieldNetCache GetFromField(string field)
        {
            throw new NotImplementedException();
        }

        public FFieldNetCache GetFromChecksum(int checksum)
        {
            throw new NotImplementedException();
        }

        public FFieldNetCache GetFromIndex(int checksum)
        {
            throw new NotImplementedException();
        }
    }
}
