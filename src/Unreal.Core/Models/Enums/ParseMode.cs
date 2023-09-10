using System;

namespace Unreal.Core.Models.Enums;

/// <summary>
/// Setting to determines how much should be parsed.
/// </summary>
[Flags]
public enum ParseMode
{
    /// <summary>
    /// Parses only the custom text based events.
    /// </summary>
    EventsOnly,

    /// <summary>
    /// Parses events and partial useful data. 
    /// </summary>
    Minimal,

    /// <summary>
    /// Parses events and all useful data.
    /// </summary>
    Normal,

    /// <summary>
    /// Parses all available data.
    /// </summary>
    Full,

    /// <summary>
    /// Use with caution :)
    /// </summary>
    Debug,

    /// <summary>
    /// Ignore all groups with this attribute.
    /// </summary>
    Ignore
}
