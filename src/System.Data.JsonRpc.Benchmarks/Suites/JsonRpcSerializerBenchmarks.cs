using System.Collections.Generic;
using System.Data.JsonRpc.Benchmarks.Framework;
using System.Data.JsonRpc.Benchmarks.Resources;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace System.Data.JsonRpc.Benchmarks.Suites
{
    [BenchmarkSuite(nameof(JsonRpcSerializer))]
    public abstract class JsonRpcSerializerBenchmarks
    {
        private readonly JsonRpcSerializer _serializer = new JsonRpcSerializer();
        private readonly IDictionary<string, string> _resources = new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly IDictionary<string, JsonRpcRequest[]> _requests = new Dictionary<string, JsonRpcRequest[]>(StringComparer.Ordinal);
        private readonly IDictionary<string, JsonRpcResponse[]> _responses = new Dictionary<string, JsonRpcResponse[]>(StringComparer.Ordinal);

        protected JsonRpcSerializerBenchmarks()
        {
            _serializer.RequestContracts["mn"] = new JsonRpcRequestContract(new Dictionary<string, Type> { ["p1"] = typeof(long), ["p2"] = typeof(long) });
            _serializer.RequestContracts["mp"] = new JsonRpcRequestContract(new[] { typeof(long), typeof(long) });

            foreach (var identifier in new JsonRpcId[] { "1", "2", 1L, 2L, 1D, 2D })
            {
                _serializer.DynamicResponseBindings[identifier] = new JsonRpcResponseContract(typeof(long), typeof(long));
            }

            foreach (var name in new[] { "req_nul_nam", "req_nul_pos", "req_str_nam", "req_str_pos", "req_int_nam", "req_int_pos", "req_flt_nam", "req_flt_pos" })
            {
                _resources[name] = EmbeddedResourceManager.GetString($"Assets.{name}.json");
                _requests[name] = _serializer.DeserializeRequestData(_resources[name]).Items.Select(i => i.Message).ToArray();
            }

            foreach (var name in new[] { "res_nul_err", "res_str_err", "res_str_scs", "res_int_err", "res_int_scs", "res_flt_err", "res_flt_scs" })
            {
                _resources[name] = EmbeddedResourceManager.GetString($"Assets.{name}.json");
                _responses[name] = _serializer.DeserializeResponseData(_resources[name]).Items.Select(i => i.Message).ToArray();
            }
        }

        [Benchmark(Description = "json -> clro / req-nul-nam")]
        public void DeserializeRequestsWhenIdIsEmptyAndParametersAreByName()
        {
            _serializer.DeserializeRequestData(_resources["req_nul_nam"]);
        }

        [Benchmark(Description = "json -> clro / req-nul-pos")]
        public void DeserializeRequestsWhenIdIsEmptyAndParametersAreByPosition()
        {
            _serializer.DeserializeRequestData(_resources["req_nul_pos"]);
        }

        [Benchmark(Description = "json -> clro / req-str-nam")]
        public void DeserializeRequestsWhenIdIsStringAndParametersAreByName()
        {
            _serializer.DeserializeRequestData(_resources["req_str_nam"]);
        }

        [Benchmark(Description = "json -> clro / req-str-pos")]
        public void DeserializeRequestsWhenIdIsStringAndParametersAreByPosition()
        {
            _serializer.DeserializeRequestData(_resources["req_str_pos"]);
        }

        [Benchmark(Description = "json -> clro / req-int-nam")]
        public void DeserializeRequestsWhenIdIsIntegerAndParametersAreByName()
        {
            _serializer.DeserializeRequestData(_resources["req_int_nam"]);
        }

        [Benchmark(Description = "json -> clro / req-int-pos")]
        public void DeserializeRequestsWhenIdIsIntegerAndParametersAreByPosition()
        {
            _serializer.DeserializeRequestData(_resources["req_int_pos"]);
        }

        [Benchmark(Description = "json -> clro / req-flt-nam")]
        public void DeserializeRequestsWhenIdIsFloatAndParametersAreByName()
        {
            _serializer.DeserializeRequestData(_resources["req_flt_nam"]);
        }

        [Benchmark(Description = "json -> clro / req-flt-pos")]
        public void DeserializeRequestsWhenIdIsFloatAndParametersAreByPosition()
        {
            _serializer.DeserializeRequestData(_resources["req_flt_pos"]);
        }

        [Benchmark(Description = "json -> clro / res-nul-err")]
        public void DeserializeResponsesWhenIdIsEmptyAndSuccessIsFalse()
        {
            _serializer.DeserializeResponseData(_resources["res_nul_err"]);
        }

        [Benchmark(Description = "json -> clro / res-str-err")]
        public void DeserializeResponsesWhenIdIsStringAndSuccessIsFalse()
        {
            _serializer.DeserializeResponseData(_resources["res_str_err"]);
        }

        [Benchmark(Description = "json -> clro / res-str-scs")]
        public void DeserializeResponsesWhenIdIsStringAndSuccessIsTrue()
        {
            _serializer.DeserializeResponseData(_resources["res_str_scs"]);
        }

        [Benchmark(Description = "json -> clro / res-int-err")]
        public void DeserializeResponsesWhenIdIsIntegerAndSuccessIsFalse()
        {
            _serializer.DeserializeResponseData(_resources["res_int_err"]);
        }

        [Benchmark(Description = "json -> clro / res-int-scs")]
        public void DeserializeResponsesWhenIdIsIntegerAndSuccessIsTrue()
        {
            _serializer.DeserializeResponseData(_resources["res_int_scs"]);
        }

        [Benchmark(Description = "json -> clro / res-flt-err")]
        public void DeserializeResponsesWhenIdIsFloatAndSuccessIsFalse()
        {
            _serializer.DeserializeResponseData(_resources["res_flt_err"]);
        }

        [Benchmark(Description = "json -> clro / res-flt-scs")]
        public void DeserializeResponsesWhenIdIsFloatAndSuccessIsTrue()
        {
            _serializer.DeserializeResponseData(_resources["res_flt_scs"]);
        }

        [Benchmark(Description = "clro -> json / req-nul-nam")]
        public void SerializeRequestsWhenIdIsEmptyAndParametersAreByName()
        {
            _serializer.SerializeRequests(_requests["req_nul_nam"]);
        }

        [Benchmark(Description = "clro -> json / req-nul-pos")]
        public void SerializeRequestsWhenIdIsEmptyAndParametersAreByPosition()
        {
            _serializer.SerializeRequests(_requests["req_nul_pos"]);
        }

        [Benchmark(Description = "clro -> json / req-str-nam")]
        public void SerializeRequestsWhenIdIsStringAndParametersAreByName()
        {
            _serializer.SerializeRequests(_requests["req_str_nam"]);
        }

        [Benchmark(Description = "clro -> json / req-str-pos")]
        public void SerializeRequestsWhenIdIsStringAndParametersAreByPosition()
        {
            _serializer.SerializeRequests(_requests["req_str_pos"]);
        }

        [Benchmark(Description = "clro -> json / req-int-nam")]
        public void SerializeRequestsWhenIdIsIntegerAndParametersAreByName()
        {
            _serializer.SerializeRequests(_requests["req_int_nam"]);
        }

        [Benchmark(Description = "clro -> json / req-int-pos")]
        public void SerializeRequestsWhenIdIsIntegerAndParametersAreByPosition()
        {
            _serializer.SerializeRequests(_requests["req_int_pos"]);
        }

        [Benchmark(Description = "clro -> json / req-flt-nam")]
        public void SerializeRequestsWhenIdIsFloatAndParametersAreByName()
        {
            _serializer.SerializeRequests(_requests["req_flt_nam"]);
        }

        [Benchmark(Description = "clro -> json / req-flt-pos")]
        public void SerializeRequestsWhenIdIsFloatAndParametersAreByPosition()
        {
            _serializer.SerializeRequests(_requests["req_flt_pos"]);
        }

        [Benchmark(Description = "clro -> json / res-nul-err")]
        public void SerializeResponsesWhenIdIsEmptyAndSuccessIsFalse()
        {
            _serializer.SerializeResponses(_responses["res_nul_err"]);
        }

        [Benchmark(Description = "clro -> json / res-str-err")]
        public void SerializeResponsesWhenIdIsStringAndSuccessIsFalse()
        {
            _serializer.SerializeResponses(_responses["res_str_err"]);
        }

        [Benchmark(Description = "clro -> json / res-str-scs")]
        public void SerializeResponsesWhenIdIsStringAndSuccessIsTrue()
        {
            _serializer.SerializeResponses(_responses["res_str_scs"]);
        }

        [Benchmark(Description = "clro -> json / res-int-err")]
        public void SerializeResponsesWhenIdIsIntegerAndSuccessIsFalse()
        {
            _serializer.SerializeResponses(_responses["res_int_err"]);
        }

        [Benchmark(Description = "clro -> json / res-int-scs")]
        public void SerializeResponsesWhenIdIsIntegerAndSuccessIsTrue()
        {
            _serializer.SerializeResponses(_responses["res_int_scs"]);
        }

        [Benchmark(Description = "clro -> json / res-flt-err")]
        public void SerializeResponsesWhenIdIsFloatAndSuccessIsFalse()
        {
            _serializer.SerializeResponses(_responses["res_flt_err"]);
        }

        [Benchmark(Description = "clro -> json / res-flt-scs")]
        public void SerializeResponsesWhenIdIsFloatAndSuccessIsTrue()
        {
            _serializer.SerializeResponses(_responses["res_flt_scs"]);
        }
    }
}