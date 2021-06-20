using BenchmarkDotNet.Running;

namespace BenchmarkReader
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkBitReader>();
            //BenchmarkRunner.Run<BenchmarkBinaryReader>();
            //BenchmarkRunner.Run<BenchmarkNetGuidCache>();
            //BenchmarkRunner.Run<BenchmarkReader>();
        }
    }
}
