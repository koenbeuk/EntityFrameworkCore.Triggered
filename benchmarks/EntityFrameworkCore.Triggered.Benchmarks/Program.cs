using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace EntityFrameworkCore.Triggered.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<PlainOverheadBenchmarks>();
        }
    }
}