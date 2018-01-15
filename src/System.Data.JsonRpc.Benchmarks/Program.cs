using System.Data.JsonRpc.Benchmarks.Framework;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Validators;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

namespace System.Data.JsonRpc.Benchmarks
{
    public static class Program
    {
        public static void Main()
        {
            var configuration = CreateConfiguration();

            var suites = typeof(Program).Assembly.GetExportedTypes()
                .Select(x => (Type: x, x.GetCustomAttribute<BenchmarkSuiteAttribute>()?.Name))
                .Where(x => x.Name != null)
                .OrderBy(x => x.Name)
                .ToArray();

            foreach (var suite in suites)
            {
                Console.WriteLine($"Running benchmark suite \"{suite.Name}\"...");
                BenchmarkRunner.Run(suite.Type, configuration);
            }
        }

        private static IConfig CreateConfiguration()
        {
            var result = ManualConfig.CreateEmpty();

            result.Add(DefaultConfig.Instance.GetColumnProviders().ToArray());
            result.Add(Job.Dry.With(RunStrategy.Throughput).WithTargetCount(05));
            result.Add(ConsoleLogger.Default);
            result.Add(MemoryDiagnoser.Default);
            result.Add(JitOptimizationsValidator.DontFailOnError);

            return result;
        }
    }
}