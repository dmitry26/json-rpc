using BenchmarkDotNet.Attributes;

namespace System.Data.JsonRpc.Benchmarks.Suites
{
    public abstract class JsonRpcIdBenchmarks
    {
        private static readonly JsonRpcId _idNone = new JsonRpcId();
        private static readonly JsonRpcId _idString = new JsonRpcId("1");
        private static readonly JsonRpcId _idInteger = new JsonRpcId(1L);
        private static readonly JsonRpcId _idFloat = new JsonRpcId(1D);

        [Benchmark(Description = "none -> get-hash-code")]
        public int ObjectGetHashCodeNone()
        {
            return _idNone.GetHashCode();
        }

        [Benchmark(Description = "string -> get-hash-code")]
        public int ObjectGetHashCodeString()
        {
            return _idString.GetHashCode();
        }

        [Benchmark(Description = "integer -> get-hash-code")]
        public int ObjectGetHashCodeInteger()
        {
            return _idInteger.GetHashCode();
        }

        [Benchmark(Description = "float -> get-hash-code")]
        public int ObjectGetHashCodeFloat()
        {
            return _idFloat.GetHashCode();
        }

        [Benchmark(Description = "none vs none -> equals")]
        public bool EquatableEqualsNoneNone()
        {
            return _idNone.Equals(_idNone);
        }

        [Benchmark(Description = "string vs none -> equals")]
        public bool EquatableEqualsStringNone()
        {
            return _idString.Equals(_idNone);
        }

        [Benchmark(Description = "integer vs none -> equals")]
        public bool EquatableEqualsIntegerNone()
        {
            return _idInteger.Equals(_idNone);
        }

        [Benchmark(Description = "float vs none -> equals")]
        public bool EquatableEqualsFloatNone()
        {
            return _idFloat.Equals(_idNone);
        }

        [Benchmark(Description = "none vs string -> equals")]
        public bool EquatableEqualsNoneString()
        {
            return _idNone.Equals(_idString);
        }

        [Benchmark(Description = "string vs string -> equals")]
        public bool EquatableEqualsStringString()
        {
            return _idString.Equals(_idString);
        }

        [Benchmark(Description = "integer vs string -> equals")]
        public bool EquatableEqualsIntegerString()
        {
            return _idInteger.Equals(_idString);
        }

        [Benchmark(Description = "float vs string -> equals")]
        public bool EquatableEqualsFloatString()
        {
            return _idFloat.Equals(_idString);
        }

        [Benchmark(Description = "none vs integer -> equals")]
        public bool EquatableEqualsNoneInteger()
        {
            return _idNone.Equals(_idInteger);
        }

        [Benchmark(Description = "string vs integer -> equals")]
        public bool EquatableEqualStringInteger()
        {
            return _idString.Equals(_idInteger);
        }

        [Benchmark(Description = "integer vs integer -> equals")]
        public bool EquatableEqualIntegerInteger()
        {
            return _idInteger.Equals(_idInteger);
        }

        [Benchmark(Description = "float vs integer -> equals")]
        public bool EquatableEqualFloatInteger()
        {
            return _idFloat.Equals(_idInteger);
        }

        [Benchmark(Description = "none vs float -> equals")]
        public bool EquatableEqualsNoneFloat()
        {
            return _idNone.Equals(_idFloat);
        }

        [Benchmark(Description = "string vs float -> equals")]
        public bool EquatableEqualsStringFloat()
        {
            return _idString.Equals(_idFloat);
        }

        [Benchmark(Description = "integer vs float -> equals")]
        public bool EquatableEqualsIntegerFloat()
        {
            return _idInteger.Equals(_idFloat);
        }

        [Benchmark(Description = "float vs float -> equals")]
        public bool EquatableEqualsFloatFloat()
        {
            return _idFloat.Equals(_idFloat);
        }
    }
}