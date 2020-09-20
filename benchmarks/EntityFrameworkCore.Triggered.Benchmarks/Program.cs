using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace EntityFrameworkCore.Triggered.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, DefaultConfig.Instance.WithOption(ConfigOptions.DisableOptimizationsValidator, true));
        }
    }
}