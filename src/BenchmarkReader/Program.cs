using BenchmarkDotNet.Running;

namespace BenchmarkReader;

internal class Program
{
    private static void Main(string[] args) => BenchmarkRunner.Run<BenchmarkBitReader>();//BenchmarkRunner.Run<BenchmarkBinaryReader>();//BenchmarkRunner.Run<BenchmarkNetGuidCache>();//BenchmarkRunner.Run<BenchmarkReader>();
}
