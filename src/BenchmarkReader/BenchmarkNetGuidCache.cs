using BenchmarkDotNet.Attributes;
using Unreal.Core.Models;

namespace BenchmarkReader;

[MemoryDiagnoser]
[ShortRunJob]
public class BenchmarkNetGuidCache
{
    private readonly NetGuidCache netGuidCache;
    private const int iterations = 1000;
    private readonly uint[] _testIndices = new uint[iterations];

    public BenchmarkNetGuidCache()
    {
        netGuidCache = new NetGuidCache();

        for (var i = 0; i < iterations; i++)
        {
            var name = $"Group_{i}";
            netGuidCache.AddToExportGroupMap(name, new NetFieldExportGroup() { PathNameIndex = (uint)i, PathName = name });
            _testIndices[i] = (uint)i;
        }
    }

    [Benchmark(Baseline = true)]
    public int Baseline_TwoStepLookup()
    {
        var sum = 0;
        for (var i = 0; i < iterations; i++)
        {
            var idx = _testIndices[i];
            if (netGuidCache.NetFieldExportGroupIndexToGroup.TryGetValue(idx, out var groupName) &&
                netGuidCache.NetFieldExportGroupMap.TryGetValue(groupName, out var group))
            {
                sum += (int)group.PathNameIndex;
            }
        }
        return sum;
    }

    [Benchmark]
    public int Optimized_DirectSingleLookup()
    {
        var sum = 0;
        for (var i = 0; i < iterations; i++)
        {
            var idx = _testIndices[i];
            var group = netGuidCache.GetNetFieldExportGroupFromIndex(idx);
            if (group != null)
            {
                sum += (int)group.PathNameIndex;
            }
        }
        return sum;
    }
}
