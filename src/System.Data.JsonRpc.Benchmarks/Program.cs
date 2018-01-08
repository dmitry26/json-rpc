using System.Linq;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Validators;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

namespace System.Data.JsonRpc.Benchmarks
{
    public static class Program
    {
        public static void Main()
        {
            var configuration = ManualConfig.CreateEmpty();

            configuration.Add(DefaultConfig.Instance.GetColumnProviders().ToArray());
            configuration.Add(Job.Dry.WithTargetCount(05));
            configuration.Add(ConsoleLogger.Default);
            configuration.Add(MemoryDiagnoser.Default);
            configuration.Add(JitOptimizationsValidator.DontFailOnError);

            BenchmarkRunner.Run<JsonRpcSerializerBenchmarks>(configuration);
        }
    }
}