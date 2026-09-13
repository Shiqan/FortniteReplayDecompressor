using BenchmarkDotNet.Attributes;
using Unreal.Core.Extensions;

namespace BenchmarkReader;

[MemoryDiagnoser]
[ShortRunJob]
public class BenchmarkStringExtensions
{
    private static readonly string[] TestPaths =
    [
        "Default__FortPlayerPawn_Athena_C",
        "ROOT_Default__AthenaGameState_C",
        "DIRTY_ROOT_BuildingSMActor",
        "PlayerController_Athena",
        "FortPickupAthena_C"
    ];

    private static readonly string[] RemovePathPrefixes = ["Default__", "ROOT_", "DIRTY_"];

    [Benchmark(Baseline = true)]
    public int Baseline_LINQ()
    {
        var count = 0;
        for (var i = 0; i < TestPaths.Length; i++)
        {
            var path = TestPaths[i];
            while (RemovePathPrefixes.Where(toRemove => path.StartsWith(toRemove, StringComparison.Ordinal)).Any())
            {
                path = path.Substring(path.IndexOf('_') + 1);
            }
            count += path.Length;
        }
        return count;
    }

    [Benchmark]
    public int Optimized_StartsWithLoop()
    {
        var count = 0;
        for (var i = 0; i < TestPaths.Length; i++)
        {
            var path = TestPaths[i].RemoveAllPathPrefixes();
            count += path.Length;
        }
        return count;
    }
}
