using BenchmarkDotNet.Attributes;
using System;
using Unreal.Core.Models;

namespace BenchmarkReader;

[MemoryDiagnoser]
public class BenchmarkNetGuidCache
{
    private readonly NetGuidCache netGuidCache;

    private const int iterations = 10000;

    private readonly Random random = new();

    public BenchmarkNetGuidCache()
    {
        netGuidCache = new NetGuidCache();

        for (var i = 0; i < iterations; i++)
        {
            netGuidCache.AddToExportGroupMap(i.ToString(), new NetFieldExportGroup() { PathNameIndex = (uint) i, PathName = i.ToString() });
        }
    }

    [Benchmark]
    public void GetNetFieldExportGroupFromIndex() => netGuidCache.GetNetFieldExportGroupFromIndex((uint) random.Next(iterations));

    [Benchmark]
    public void GetNetFieldExportGroup() => netGuidCache.GetNetFieldExportGroup(random.Next(iterations).ToString());

    [Benchmark]
    public void TryGetClassNetCache() => netGuidCache.TryGetClassNetCache(random.Next(iterations).ToString(), out var group, true);


}
