using BenchmarkDotNet.Attributes;

namespace System.Data.JsonRpc.Benchmarks.Suites
{
    public abstract class JsonRpcIdBenchmarks
    {
        private static readonly JsonRpcId _idNone = new JsonRpcId();
        private static readonly JsonRpcId _idString = new JsonRpcId("1");
        private static readonly JsonRpcId _idInteger = new JsonRpcId(1L);
        private static readonly JsonRpcId _idFloat = new JsonRpcId(1D);

        [Benchmark]
        public int NoneObjectGetHashCode()
        {
            return _idNone.GetHashCode();
        }

        [Benchmark]
        public int StringObjectGetHashCode()
        {
            return _idString.GetHashCode();
        }

        [Benchmark]
        public int IntegerObjectGetHashCode()
        {
            return _idInteger.GetHashCode();
        }

        [Benchmark]
        public int FloatObjectGetHashCode()
        {
            return _idFloat.GetHashCode();
        }

        [Benchmark]
        public bool NoneEquatableEquals()
        {
            return _idNone.Equals(_idNone);
        }

        [Benchmark]
        public bool StringEquatableEquals()
        {
            return _idString.Equals(_idString);
        }

        [Benchmark]
        public bool IntegerEquatableEquals()
        {
            return _idInteger.Equals(_idInteger);
        }

        [Benchmark]
        public bool FloatEquatableEquals()
        {
            return _idFloat.Equals(_idFloat);
        }

        [Benchmark]
        public int NoneComparableCompare()
        {
            return _idNone.CompareTo(_idNone);
        }

        [Benchmark]
        public int StringComparableCompare()
        {
            return _idString.CompareTo(_idString);
        }

        [Benchmark]
        public int IntegerComparableCompare()
        {
            return _idInteger.CompareTo(_idInteger);
        }

        [Benchmark]
        public int FloatComparableCompare()
        {
            return _idFloat.CompareTo(_idFloat);
        }
    }
}