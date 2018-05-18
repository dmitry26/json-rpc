using System.Collections.Generic;
using System.Data.JsonRpc.Benchmarks.Resources;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace System.Data.JsonRpc.Benchmarks.Suites
{
    public abstract class JsonRpcSerializerBenchmarks
    {
        private static readonly IReadOnlyDictionary<string, string> _resources = CreateResourceDictionary();

        private readonly JsonRpcSerializer _serializer = new JsonRpcSerializer();
        private readonly IDictionary<string, JsonRpcRequest[]> _requests = new Dictionary<string, JsonRpcRequest[]>(StringComparer.Ordinal);
        private readonly IDictionary<string, JsonRpcResponse[]> _responses = new Dictionary<string, JsonRpcResponse[]>(StringComparer.Ordinal);

        protected JsonRpcSerializerBenchmarks()
        {
            foreach (var contract in CreateRequestContracts())
            {
                _serializer.RequestContracts[contract.Key] = contract.Value;
            }
            foreach (var contract in CreateResponseContracts())
            {
                _serializer.DynamicResponseBindings[contract.Key] = contract.Value;
            }
            foreach (var code in GetRequestCodes())
            {
                _requests[code] = _serializer.DeserializeRequestData(_resources[code]).Items.Select(i => i.Message).ToArray();
            }
            foreach (var code in GetResponseCodes())
            {
                _responses[code] = _serializer.DeserializeResponseData(_resources[code]).Items.Select(i => i.Message).ToArray();
            }
        }

        private static IReadOnlyDictionary<string, string> CreateResourceDictionary()
        {
            var resources = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var code in GetRequestCodes())
            {
                resources[code] = EmbeddedResourceManager.GetString($"Assets.{code}.json");
            }
            foreach (var code in GetResponseCodes())
            {
                resources[code] = EmbeddedResourceManager.GetString($"Assets.{code}.json");
            }

            return resources;
        }

        private static IReadOnlyDictionary<string, JsonRpcRequestContract> CreateRequestContracts()
        {
            var parameters1 = new Dictionary<string, Type>
            {
                ["p1"] = typeof(long),
                ["p2"] = typeof(long)
            };

            var parameters2 = new[]
            {
                typeof(long),
                typeof(long)
            };

            return new Dictionary<string, JsonRpcRequestContract>()
            {
                ["mn"] = new JsonRpcRequestContract(parameters1),
                ["mp"] = new JsonRpcRequestContract(parameters2)
            };
        }

        private static IReadOnlyDictionary<JsonRpcId, JsonRpcResponseContract> CreateResponseContracts()
        {
            var contracts = new Dictionary<JsonRpcId, JsonRpcResponseContract>();
            var contract = new JsonRpcResponseContract(typeof(long), typeof(long));

            foreach (var id in new JsonRpcId[] { "1", "2", 1L, 2L, 1D, 2D })
            {
                contracts[id] = contract;
            }

            return contracts;
        }

        private static IEnumerable<string> GetRequestCodes()
        {
            return new[] { "req_nul_nam", "req_nul_pos", "req_str_nam", "req_str_pos", "req_int_nam", "req_int_pos", "req_flt_nam", "req_flt_pos" };
        }

        private static IEnumerable<string> GetResponseCodes()
        {
            return new[] { "res_nul_err", "res_str_err", "res_str_scs", "res_int_err", "res_int_scs", "res_flt_err", "res_flt_scs" };
        }

        [Benchmark(Description = "json -> clro / req-nul-nam")]
        public object DeserializeRequestsWhenIdIsEmptyAndParametersAreByName()
        {
            return _serializer.DeserializeRequestData(_resources["req_nul_nam"]);
        }

        [Benchmark(Description = "json -> clro / req-nul-pos")]
        public object DeserializeRequestsWhenIdIsEmptyAndParametersAreByPosition()
        {
            return _serializer.DeserializeRequestData(_resources["req_nul_pos"]);
        }

        [Benchmark(Description = "json -> clro / req-str-nam")]
        public object DeserializeRequestsWhenIdIsStringAndParametersAreByName()
        {
            return _serializer.DeserializeRequestData(_resources["req_str_nam"]);
        }

        [Benchmark(Description = "json -> clro / req-str-pos")]
        public object DeserializeRequestsWhenIdIsStringAndParametersAreByPosition()
        {
            return _serializer.DeserializeRequestData(_resources["req_str_pos"]);
        }

        [Benchmark(Description = "json -> clro / req-int-nam")]
        public object DeserializeRequestsWhenIdIsIntegerAndParametersAreByName()
        {
            return _serializer.DeserializeRequestData(_resources["req_int_nam"]);
        }

        [Benchmark(Description = "json -> clro / req-int-pos")]
        public object DeserializeRequestsWhenIdIsIntegerAndParametersAreByPosition()
        {
            return _serializer.DeserializeRequestData(_resources["req_int_pos"]);
        }

        [Benchmark(Description = "json -> clro / req-flt-nam")]
        public object DeserializeRequestsWhenIdIsFloatAndParametersAreByName()
        {
            return _serializer.DeserializeRequestData(_resources["req_flt_nam"]);
        }

        [Benchmark(Description = "json -> clro / req-flt-pos")]
        public object DeserializeRequestsWhenIdIsFloatAndParametersAreByPosition()
        {
            return _serializer.DeserializeRequestData(_resources["req_flt_pos"]);
        }

        [Benchmark(Description = "json -> clro / res-nul-err")]
        public object DeserializeResponsesWhenIdIsEmptyAndSuccessIsFalse()
        {
            return _serializer.DeserializeResponseData(_resources["res_nul_err"]);
        }

        [Benchmark(Description = "json -> clro / res-str-err")]
        public object DeserializeResponsesWhenIdIsStringAndSuccessIsFalse()
        {
            return _serializer.DeserializeResponseData(_resources["res_str_err"]);
        }

        [Benchmark(Description = "json -> clro / res-str-scs")]
        public object DeserializeResponsesWhenIdIsStringAndSuccessIsTrue()
        {
            return _serializer.DeserializeResponseData(_resources["res_str_scs"]);
        }

        [Benchmark(Description = "json -> clro / res-int-err")]
        public object DeserializeResponsesWhenIdIsIntegerAndSuccessIsFalse()
        {
            return _serializer.DeserializeResponseData(_resources["res_int_err"]);
        }

        [Benchmark(Description = "json -> clro / res-int-scs")]
        public object DeserializeResponsesWhenIdIsIntegerAndSuccessIsTrue()
        {
            return _serializer.DeserializeResponseData(_resources["res_int_scs"]);
        }

        [Benchmark(Description = "json -> clro / res-flt-err")]
        public object DeserializeResponsesWhenIdIsFloatAndSuccessIsFalse()
        {
            return _serializer.DeserializeResponseData(_resources["res_flt_err"]);
        }

        [Benchmark(Description = "json -> clro / res-flt-scs")]
        public object DeserializeResponsesWhenIdIsFloatAndSuccessIsTrue()
        {
            return _serializer.DeserializeResponseData(_resources["res_flt_scs"]);
        }

        [Benchmark(Description = "clro -> json / req-nul-nam")]
        public object SerializeRequestsWhenIdIsEmptyAndParametersAreByName()
        {
            return _serializer.SerializeRequests(_requests["req_nul_nam"]);
        }

        [Benchmark(Description = "clro -> json / req-nul-pos")]
        public object SerializeRequestsWhenIdIsEmptyAndParametersAreByPosition()
        {
            return _serializer.SerializeRequests(_requests["req_nul_pos"]);
        }

        [Benchmark(Description = "clro -> json / req-str-nam")]
        public object SerializeRequestsWhenIdIsStringAndParametersAreByName()
        {
            return _serializer.SerializeRequests(_requests["req_str_nam"]);
        }

        [Benchmark(Description = "clro -> json / req-str-pos")]
        public object SerializeRequestsWhenIdIsStringAndParametersAreByPosition()
        {
            return _serializer.SerializeRequests(_requests["req_str_pos"]);
        }

        [Benchmark(Description = "clro -> json / req-int-nam")]
        public object SerializeRequestsWhenIdIsIntegerAndParametersAreByName()
        {
            return _serializer.SerializeRequests(_requests["req_int_nam"]);
        }

        [Benchmark(Description = "clro -> json / req-int-pos")]
        public object SerializeRequestsWhenIdIsIntegerAndParametersAreByPosition()
        {
            return _serializer.SerializeRequests(_requests["req_int_pos"]);
        }

        [Benchmark(Description = "clro -> json / req-flt-nam")]
        public object SerializeRequestsWhenIdIsFloatAndParametersAreByName()
        {
            return _serializer.SerializeRequests(_requests["req_flt_nam"]);
        }

        [Benchmark(Description = "clro -> json / req-flt-pos")]
        public object SerializeRequestsWhenIdIsFloatAndParametersAreByPosition()
        {
            return _serializer.SerializeRequests(_requests["req_flt_pos"]);
        }

        [Benchmark(Description = "clro -> json / res-nul-err")]
        public object SerializeResponsesWhenIdIsEmptyAndSuccessIsFalse()
        {
            return _serializer.SerializeResponses(_responses["res_nul_err"]);
        }

        [Benchmark(Description = "clro -> json / res-str-err")]
        public object SerializeResponsesWhenIdIsStringAndSuccessIsFalse()
        {
            return _serializer.SerializeResponses(_responses["res_str_err"]);
        }

        [Benchmark(Description = "clro -> json / res-str-scs")]
        public object SerializeResponsesWhenIdIsStringAndSuccessIsTrue()
        {
            return _serializer.SerializeResponses(_responses["res_str_scs"]);
        }

        [Benchmark(Description = "clro -> json / res-int-err")]
        public object SerializeResponsesWhenIdIsIntegerAndSuccessIsFalse()
        {
            return _serializer.SerializeResponses(_responses["res_int_err"]);
        }

        [Benchmark(Description = "clro -> json / res-int-scs")]
        public object SerializeResponsesWhenIdIsIntegerAndSuccessIsTrue()
        {
            return _serializer.SerializeResponses(_responses["res_int_scs"]);
        }

        [Benchmark(Description = "clro -> json / res-flt-err")]
        public object SerializeResponsesWhenIdIsFloatAndSuccessIsFalse()
        {
            return _serializer.SerializeResponses(_responses["res_flt_err"]);
        }

        [Benchmark(Description = "clro -> json / res-flt-scs")]
        public object SerializeResponsesWhenIdIsFloatAndSuccessIsTrue()
        {
            return _serializer.SerializeResponses(_responses["res_flt_scs"]);
        }
    }
}