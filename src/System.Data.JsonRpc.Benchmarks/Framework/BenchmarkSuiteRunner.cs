using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System.Linq;
using System.Reflection;

namespace System.Data.JsonRpc.Benchmarks.Framework
{
    /// <summary>Benchmark suite runner.</summary>
    internal static class BenchmarkSuiteRunner
    {
        /// <summary>Runs benchmark suites from the specified assembly.</summary>
        /// <param name="assembly">Assembly to search benchmark suites in.</param>
        /// <param name="configuration">Benchmark runninng configuration.</param>
        /// <exception cref="ArgumentNullException"><paramref name="assembly" /> is <see langword="null" />.</exception>
        public static void Run(Assembly assembly, IConfig configuration = null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var suites = typeof(Program).Assembly.GetExportedTypes()
                .Select(x => (Type: x, x.GetCustomAttribute<BenchmarkSuiteAttribute>()?.Name))
                .Where(x => x.Name != null)
                .OrderBy(x => x.Name)
                .ToArray();

            Console.WriteLine($"Found {suites.Length} benchmark suite(s)");

            foreach (var suite in suites)
            {
                Console.WriteLine($"Running benchmark suite \"{suite.Name}\"...");
                BenchmarkRunner.Run(suite.Type, configuration);
            }
        }
    }
}