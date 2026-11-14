using Unreal.Core.Models.Enums;

namespace Unreal.Core.Attributes;

/// <summary>
/// Maps a class to every export group whose path starts with <see cref="Prefix"/>, for example all player built pieces
/// (<c>/Game/Building/ActorBlueprints/Player/</c>). An exact <see cref="NetFieldExportGroupAttribute"/> path always wins over a prefix.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class NetFieldExportGroupPrefixAttribute : Attribute
{
    public string Prefix { get; private set; }

    public ParseMode MinimalParseMode { get; private set; }

    public NetFieldExportGroupPrefixAttribute(string prefix, ParseMode minimalParseMode = ParseMode.Normal)
    {
        Prefix = prefix;
        MinimalParseMode = minimalParseMode;
    }
}
