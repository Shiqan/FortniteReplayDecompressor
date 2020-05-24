using BenchmarkDotNet.Running;

namespace BenchmarkReader
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkBitReader>();
        }
    }
}
