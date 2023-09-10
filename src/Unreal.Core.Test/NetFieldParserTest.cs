using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;
using Xunit;

namespace Unreal.Core.Test;

[NetFieldExportGroup("group1", minimalParseMode: ParseMode.Minimal)]
public class NetFieldGroup1 : INetFieldExportGroup
{
    [NetFieldExport("bField", RepLayoutCmdType.PropertyBool)]
    public bool bField { get; set; }

    [NetFieldExport("ItemDefinitionField", RepLayoutCmdType.Property)]
    public ItemDefinition ItemDefinitionField { get; set; }

    [NetFieldExport("VectorField", RepLayoutCmdType.PropertyVector)]
    public FVector VectorField { get; set; }

    [NetFieldExport("ArrayField", RepLayoutCmdType.DynamicArray)]
    public ItemDefinition[] ArrayField { get; set; }
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

    [Fact]
    public void ReadBooleanFieldTest()
    {
        var reader = new NetBitReader(new byte[] { 0x01 }, 1);
        var export = new NetFieldExport()
        {
            Handle = 0,
            Name = "bField"

        };
        var group = new NetFieldExportGroup()
        {
            PathName = "group1",
            NetFieldExportsLength = 1,
            NetFieldExports = new NetFieldExport[] { export },
            PathNameIndex = 1
        };

        var guidCache = new NetGuidCache();
        var parser = new NetFieldParser(guidCache, ParseMode.Full, "Unreal.Core.Test");

        var data = parser.CreateType(group.PathName);
        parser.ReadField(data, export, export.Handle, group, reader);
        Assert.True(reader.AtEnd());
        Assert.False(reader.IsError);
        Assert.True(((NetFieldGroup1) data).bField);
    }

    [Fact]
    public void ReadGuidFieldTest()
    {
        var reader = new NetBitReader(new byte[] { 0x87, 0x04 });
        var export = new NetFieldExport()
        {
            Handle = 0,
            Name = "ItemDefinitionField"
        };

        var group = new NetFieldExportGroup()
        {
            PathName = "group1",
            NetFieldExportsLength = 1,
            NetFieldExports = new NetFieldExport[] { export },
            PathNameIndex = 1
        };

        var guidCache = new NetGuidCache();
        var parser = new NetFieldParser(guidCache, ParseMode.Full, "Unreal.Core.Test");

        var data = parser.CreateType(group.PathName);
        parser.ReadField(data, export, export.Handle, group, reader);
        Assert.True(reader.AtEnd());
        Assert.False(reader.IsError);
        Assert.True(((NetFieldGroup1) data).ItemDefinitionField.IsValid());
        Assert.Equal(323u, ((NetFieldGroup1) data).ItemDefinitionField.Value);
    }

    [Fact]
    public void ReadVectorFieldTest()
    {
        var reader = new NetBitReader(new byte[] { 0x01, 0x0B, 0xC7, 0x47, 0x8A, 0x26, 0xA7, 0xC7, 0x00, 0x80, 0x71, 0xC5 }, 96)
        {
            NetworkVersion = NetworkVersionHistory.HISTORY_CHARACTER_MOVEMENT_NOINTERP,
            EngineNetworkVersion = EngineNetworkVersionHistory.HISTORY_CLASSNETCACHE_FULLNAME
        };

        var export = new NetFieldExport()
        {
            Handle = 0,
            Name = "VectorField"
        };

        var group = new NetFieldExportGroup()
        {
            PathName = "group1",
            NetFieldExportsLength = 1,
            NetFieldExports = new NetFieldExport[] { export },
            PathNameIndex = 1
        };

        var guidCache = new NetGuidCache();
        var parser = new NetFieldParser(guidCache, ParseMode.Full, "Unreal.Core.Test");

        var data = parser.CreateType(group.PathName);
        parser.ReadField(data, export, export.Handle, group, reader);
        Assert.True(reader.AtEnd());
        Assert.False(reader.IsError);
        Assert.Equal(-3864, ((NetFieldGroup1) data).VectorField.Z);
    }


    [Fact]
    public void ReadArrayFieldTest()
    {
        var reader = new NetBitReader(new byte[] {
            0x0C, 0x02, 0x6F, 0x02, 0x20, 0xD7, 0x08, 0x00, 0x04, 0x6F, 0x02, 0x20, 0xDF, 0x08, 0x00,
            0x06, 0x6F, 0x02, 0x20, 0xE7, 0x08, 0x00, 0x08, 0x6F, 0x02, 0x20, 0xEF, 0x08, 0x00, 0x0A,
            0x6F, 0x02, 0x20, 0x8F, 0x06, 0x00, 0x0C, 0x6F, 0x02, 0x20, 0xF7, 0x08, 0x00, 0x00 }, 352)
        {
            NetworkVersion = NetworkVersionHistory.HISTORY_CHARACTER_MOVEMENT_NOINTERP,
            EngineNetworkVersion = EngineNetworkVersionHistory.HISTORY_CLASSNETCACHE_FULLNAME
        };

        var export = new NetFieldExport()
        {
            Handle = 0,
            Name = "ArrayField"
        };

        var group = new NetFieldExportGroup()
        {
            PathName = "group1",
            NetFieldExportsLength = 183,
            NetFieldExports = new NetFieldExport[183],
            PathNameIndex = 1
        };
        group.NetFieldExports[182] = export;

        var guidCache = new NetGuidCache();
        var parser = new NetFieldParser(guidCache, ParseMode.Full, "Unreal.Core.Test");

        var data = parser.CreateType(group.PathName);
        parser.ReadField(data, export, export.Handle, group, reader);
        Assert.True(reader.AtEnd());
        Assert.False(reader.IsError);
        Assert.Equal(6, ((NetFieldGroup1) data).ArrayField.Length);
    }
}
