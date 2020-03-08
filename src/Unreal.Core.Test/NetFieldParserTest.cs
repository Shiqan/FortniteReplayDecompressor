using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;
using Xunit;

namespace Unreal.Core.Test
{
    [NetFieldExportGroup("group1", minimalParseMode: ParseMode.Minimal)]
    public class NetFieldGroup1 : INetFieldExportGroup
    {
    }

    [NetFieldExportGroup("group2", minimalParseMode: ParseMode.Full)]
    public class NetFieldGroup2 : INetFieldExportGroup
    {
    }

    [NetFieldExportGroup("ignore", minimalParseMode: ParseMode.Ignore)]
    public class NetFieldGroupIgnore : INetFieldExportGroup
    {
    }

    [NetFieldExportClassNetCache("classnetcache1", minimalParseMode: ParseMode.Minimal)]
    public class ClassNetCache1
    {
        [NetFieldExportRPC("DefaultRPCProperty", "path-DefaultRPCProperty")]
        public object DefaultRPCProperty { get; set; }

        [NetFieldExportRPC("RPCPropertyWithChecksum", "path-RPCPropertyWithChecksum", enablePropertyChecksum: false)]
        public object RPCPropertyWithoutChecksum { get; set; }

        [NetFieldExportRPC("FunctionRPCProperty", "path-FunctionRPCProperty", isFunction: true)]
        public object FunctionRPCProperty { get; set; }

        [NetFieldExportRPC("CustomRPCProperty", "path-CustomRPCProperty", customStruct: true)]
        public int CustomRPCProperty { get; set; }
    }

    [NetFieldExportClassNetCache("classnetcache2", minimalParseMode: ParseMode.Full)]
    public class ClassNetCache2
    {
    }

    [NetFieldExportClassNetCache("classnetcacheignore", minimalParseMode: ParseMode.Ignore)]
    public class ClassNetCacheIgnore
    {
    }

    public class NetFieldParserTest
    {
        [Fact]
        public void WillReadTypesOnMinimalTest()
        {
            var guidCache = new NetGuidCache();
            var parser = new NetFieldParser(guidCache, ParseMode.Minimal, "Unreal.Core.Test");

            Assert.True(parser.WillReadType("group1"));
            Assert.False(parser.WillReadType("group2"));
            Assert.False(parser.WillReadType("ignore"));
        }

        [Fact]
        public void WillReadTypesOnFullTest()
        {
            var guidCache = new NetGuidCache();
            var parser = new NetFieldParser(guidCache, ParseMode.Full, "Unreal.Core.Test");

            Assert.True(parser.WillReadType("group1"));
            Assert.True(parser.WillReadType("group2"));
            Assert.False(parser.WillReadType("ignore"));
        }

        [Fact]
        public void WillReadClassNetCacheOnMinimalTest()
        {
            var guidCache = new NetGuidCache();
            var parser = new NetFieldParser(guidCache, ParseMode.Minimal, "Unreal.Core.Test");

            Assert.True(parser.WillReadClassNetCache("classnetcache1"));
            Assert.False(parser.WillReadClassNetCache("classnetcache2"));
            Assert.False(parser.WillReadClassNetCache("classnetcacheignore"));
        }

        [Fact]
        public void WillReadClassNetCacheOnFullTest()
        {
            var guidCache = new NetGuidCache();
            var parser = new NetFieldParser(guidCache, ParseMode.Full, "Unreal.Core.Test");

            Assert.True(parser.WillReadClassNetCache("classnetcache1"));
            Assert.True(parser.WillReadClassNetCache("classnetcache2"));
            Assert.False(parser.WillReadClassNetCache("classnetcacheignore"));
        }

        [Fact]
        public void TryGetClassNetCachePropertyTest()
        {
            var guidCache = new NetGuidCache();
            var parser = new NetFieldParser(guidCache, ParseMode.Full, "Unreal.Core.Test");

            var result = parser.TryGetClassNetCacheProperty("DefaultRPCProperty", "classnetcache1", out var info);
            Assert.True(result);
            Assert.Equal("DefaultRPCProperty", info.Name);
            Assert.Equal("path-DefaultRPCProperty", info.PathName);
            Assert.True(info.EnablePropertyChecksum);
            Assert.False(info.IsCustomStruct);
            Assert.False(info.IsFunction);

            result = parser.TryGetClassNetCacheProperty("RPCPropertyWithChecksum", "classnetcache1", out info);
            Assert.True(result);
            Assert.Equal("RPCPropertyWithChecksum", info.Name);
            Assert.Equal("path-RPCPropertyWithChecksum", info.PathName);
            Assert.False(info.EnablePropertyChecksum);
            Assert.False(info.IsCustomStruct);
            Assert.False(info.IsFunction);

            result = parser.TryGetClassNetCacheProperty("FunctionRPCProperty", "classnetcache1", out info);
            Assert.True(result);
            Assert.Equal("FunctionRPCProperty", info.Name);
            Assert.Equal("path-FunctionRPCProperty", info.PathName);
            Assert.True(info.EnablePropertyChecksum);
            Assert.False(info.IsCustomStruct);
            Assert.True(info.IsFunction);

            result = parser.TryGetClassNetCacheProperty("CustomRPCProperty", "classnetcache1", out info);
            Assert.True(result);
            Assert.Equal("CustomRPCProperty", info.Name);
            Assert.Equal("path-CustomRPCProperty", info.PathName);
            Assert.True(info.EnablePropertyChecksum);
            Assert.True(info.IsCustomStruct);
            Assert.False(info.IsFunction);
            Assert.True(typeof(int) == info.PropertyInfo.PropertyType);
        }

        [Fact]
        public void TryGetClassNetCachePropertyDoesNotThrowTest()
        {
            var guidCache = new NetGuidCache();
            var parser = new NetFieldParser(guidCache, ParseMode.Full, "Unreal.Core.Test");

            var result = parser.TryGetClassNetCacheProperty("doesnotexist", "classnetcache1", out var info);
            Assert.False(result);

            result = parser.TryGetClassNetCacheProperty("doesnotexist", "classnetcache3", out info);
            Assert.False(result);
        }
    }
}
