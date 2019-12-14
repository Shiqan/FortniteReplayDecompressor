namespace Unreal.Core.Models.Enums
{
    /// <summary>
    /// Setting to determines how much should be parsed.
    /// </summary>
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
        /// Use with caution :)
        /// </summary>
        Debug
    }
}
