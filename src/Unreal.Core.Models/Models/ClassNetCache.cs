using System.Collections.Generic;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ClassNetCache
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ParseMode Mode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, ClassNetCacheProperty> Properties { get; set; }
    }
}