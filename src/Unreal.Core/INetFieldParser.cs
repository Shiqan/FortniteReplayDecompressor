using Unreal.Core.Models;
using Unreal.Core.Models.Contracts;

namespace Unreal.Core
{
    public interface INetFieldParser
    {
        /// <summary>
        /// Returns whether or not this <paramref name="group"/> is marked as a player controller.
        /// </summary>
        /// <param name="group"></param>
        /// <returns>true if group is a player controller, false otherwise</returns>
        public bool IsPlayerController(string group);

        /// <summary>
        /// Returns whether or not this <paramref name="group"/> is marked to be parsed.
        /// </summary>
        /// <param name="group"></param>
        /// <returns>true if group should be parsed further, false otherwise</returns>
        public bool WillReadClassNetCache(string group);

        /// <summary>
        /// Returns whether or not this <paramref name="group"/> is marked to be parsed.
        /// </summary>
        /// <param name="group"></param>
        /// <returns>true if group should be parsed further, false otherwise</returns>
        public bool WillReadType(string group);

        /// <summary>
        /// Create a new instance of the INetFieldExportGroup associated with this <paramref name="group"/>.
        /// </summary>
        /// <param name="group"></param>
        public INetFieldExportGroupAdapter CreateType(string group);
    }
}