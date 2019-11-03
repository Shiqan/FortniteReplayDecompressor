using Unreal.Core.Models.Enums;

namespace Unreal.Core.Models
{
    /// <summary>
    /// Represents a single property, which could be either a Top Level Property, a Nested Struct Property, or an element in a Dynamic Array.
    /// see https://github.com/EpicGames/UnrealEngine/blob/bf95c2cbc703123e08ab54e3ceccdd47e48d224a/Engine/Source/Runtime/Engine/Public/Net/RepLayout.h#L810
    /// </summary>
    public class RepLayoutCmd
    {
        /** Pointer back to property, used for NetSerialize calls, etc. */
        UProperty Property;

        /** For arrays, this is the cmd index to jump to, to skip this arrays inner elements. */
        ushort EndCmd;

        /** For arrays, element size of data. */
        ushort ElementSize;

        /** Absolute offset of property in Object Memory. */
        int Offset;

        /** Absolute offset of property in Shadow Memory. */
        int ShadowOffset;

        /** Handle relative to start of array, or top list. */
        ushort RelativeHandle;

        /** Index into Parents. */
        ushort ParentIndex;

        /** Used to determine if property is still compatible */
        uint CompatibleChecksum;

        RepLayoutCmdType Type;

        RepLayoutFlags Flags;
    }
}
