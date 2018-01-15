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
            _serializer.RequestContracts["req_nam"] = new JsonRpcRequestContract(new Dictionary<string, Type> { ["arg1"] = typeof(string), ["arg2"] = typeof(string) });
            _serializer.RequestContracts["req_pos"] = new JsonRpcRequestContract(new[] { typeof(string), typeof(string) });

            foreach (var i in new JsonRpcId[] { 100, 200, "37f67ce5-a2ce-4c9a-aa48-82b898958be8", "a846b75a-0c4b-4750-b505-644c94ecc7c0" })
            {
                _serializer.DynamicResponseBindings[i] = new JsonRpcResponseContract(typeof(string), typeof(string));
            }

            foreach (var s in new[] { "req_num_nam", "req_num_pos", "req_str_nam", "req_str_pos" })
            {
                _resources[s] = EmbeddedResourceManager.GetString($"Assets.{s}.json");
                _requests[s] = _serializer.DeserializeRequestData(_resources[s]).BatchItems.Select(i => i.Message).ToArray();
            }

            foreach (var s in new[] { "res_num_err", "res_num_suc", "res_str_err", "res_str_suc" })
            {
                _resources[s] = EmbeddedResourceManager.GetString($"Assets.{s}.json");
                _responses[s] = _serializer.DeserializeResponseData(_resources[s]).BatchItems.Select(i => i.Message).ToArray();
            }
        }

        [Benchmark(Description = "req: json -> .net [num, nam]")]
        public void DeserializeRequestsWhenIdIsNumberAndParametersAreByName()
        {
            _serializer.DeserializeRequestData(_resources["req_num_nam"]);
        }

        [Benchmark(Description = "req: json -> .net [num, pos]")]
        public void DeserializeRequestsWhenIdIsNumberAndParametersAreByPosition()
        {
            _serializer.DeserializeRequestData(_resources["req_num_pos"]);
        }

        [Benchmark(Description = "req: json -> .net [str, nam]")]
        public void DeserializeRequestsWhenIdIsStringAndParametersAreByName()
        {
            _serializer.DeserializeRequestData(_resources["req_str_nam"]);
        }

        [Benchmark(Description = "req: json -> .net [str, pos]")]
        public void DeserializeRequestsWhenIdIsStringAndParametersAreByPosition()
        {
            _serializer.DeserializeRequestData(_resources["req_str_pos"]);
        }

        [Benchmark(Description = "res: json -> .net [num, err]")]
        public void DeserializeResponsesWhenIdIsNumberAndParametersAreByName()
        {
            _serializer.DeserializeResponseData(_resources["res_num_err"]);
        }

        [Benchmark(Description = "res: json -> .net [num, suc]")]
        public void DeserializeResponsesWhenIdIsNumberAndParametersAreByPosition()
        {
            _serializer.DeserializeResponseData(_resources["res_num_suc"]);
        }

        [Benchmark(Description = "res: json -> .net [str, err]")]
        public void DeserializeResponsesWhenIdIsStringAndParametersAreByName()
        {
            _serializer.DeserializeResponseData(_resources["res_str_err"]);
        }

        [Benchmark(Description = "res: json -> .net [str, suc]")]
        public void DeserializeResponsesWhenIdIsStringAndParametersAreByPosition()
        {
            _serializer.DeserializeResponseData(_resources["res_str_suc"]);
        }

        [Benchmark(Description = "req: .net -> json [num, nam]")]
        public void SerializeRequestsWhenIdIsNumberAndParametersAreByName()
        {
            _serializer.SerializeRequests(_requests["req_num_nam"]);
        }

        [Benchmark(Description = "req: .net -> json [num, pos]")]
        public void SerializeRequestsWhenIdIsNumberAndParametersAreByPosition()
        {
            _serializer.SerializeRequests(_requests["req_num_pos"]);
        }

        [Benchmark(Description = "req: .net -> json [str, nam]")]
        public void SerializeRequestsWhenIdIsStringAndParametersAreByName()
        {
            _serializer.SerializeRequests(_requests["req_str_nam"]);
        }

        [Benchmark(Description = "req: .net -> json [str, pos]")]
        public void SerializeRequestsWhenIdIsStringAndParametersAreByPosition()
        {
            _serializer.SerializeRequests(_requests["req_str_pos"]);
        }

        [Benchmark(Description = "res: .net -> json [num, err]")]
        public void SerializeResponsesWhenIdIsNumberAndParametersAreByName()
        {
            _serializer.SerializeResponses(_responses["res_num_err"]);
        }

        [Benchmark(Description = "res: .net -> json [num, suc]")]
        public void SerializeResponsesWhenIdIsNumberAndParametersAreByPosition()
        {
            _serializer.SerializeResponses(_responses["res_num_suc"]);
        }

        [Benchmark(Description = "res: .net -> json [str, err]")]
        public void SerializeResponsesWhenIdIsStringAndParametersAreByName()
        {
            _serializer.SerializeResponses(_responses["res_str_err"]);
        }

        [Benchmark(Description = "res: .net -> json [str, suc]")]
        public void SerializeResponsesWhenIdIsStringAndParametersAreByPosition()
        {
            _serializer.SerializeResponses(_responses["res_str_suc"]);
        }
    }
}