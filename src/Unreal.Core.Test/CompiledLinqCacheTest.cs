using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;
using Xunit;

namespace Unreal.Core.Test;

[NetFieldExportGroup("group1", minimalParseMode: ParseMode.Ignore)]
public class NetFieldGroupLinqExample : INetFieldExportGroup
{
    [NetFieldExport("VectorField", RepLayoutCmdType.PropertyVector)]
    public FVector VectorField { get; set; }
}

public class CompiledLinqCacheTest
{
    [Fact]
    public void CreateObjectTest()
    {
        var cache = new CompiledLinqCache();
        var typeId = cache.AddExportType(typeof(NetFieldGroupLinqExample));
        var result = cache.CreateObject(typeId);
        Assert.IsType<NetFieldGroupLinqExample>(result);
    }
}
