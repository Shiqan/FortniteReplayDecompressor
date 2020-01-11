using System;

namespace Unreal.Core.Models.Enums
{
    /// <summary>
    /// Setting to determines how much should be parsed.
    /// </summary>
    [Flags]
    public enum ParseMode
    {
        /// <summary>
        /// Parses only events.
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
        /// Parses events and all useful data, including weapons and vehicles.
        /// </summary>
        Extended,

        /// <summary>
        /// Parses events and all useful data, including damage events, weapons and RPC structs and functions.
        /// </summary>
        Full,

        /// <summary>
        /// Use with caution :)
        /// </summary>
        Debug,

        /// <summary>
        /// Ignore
        /// </summary>
        Ignore
    }
}
