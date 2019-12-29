using System;
using Unreal.Core.Models.Enums;

namespace Unreal.Core.Attributes
{
    /// <summary>
    /// Attribute to map a class to the specified path. Used for function RPC property replication.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class NetFieldExportRPCAttribute : Attribute
    {
        public string Path { get; private set; }
        public string FunctionName { get; private set; }
        public ParseMode MinimalParseMode { get; private set; }

        public NetFieldExportRPCAttribute(string path, string functionName, ParseMode minimalParseMode = ParseMode.Normal)
        {
            Path = path;
            FunctionName = functionName;
            MinimalParseMode = minimalParseMode;
        }
    }
}
