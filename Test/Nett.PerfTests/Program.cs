using BenchmarkDotNet.Running;

namespace Nett.PerfTests
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Benchmarks>();
        }
    }
}
