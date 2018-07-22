using System.Collections.Generic;
using System.Data.JsonRpc.Benchmarks.Resources;
using BenchmarkDotNet.Attributes;

namespace System.Data.JsonRpc.Benchmarks.Suites
{
    public abstract class Utf8JsonRpcSerializerDeserializeBenchmarks
    {
        private static readonly IReadOnlyDictionary<string, string> _resources = CreateResourceDictionary();
        private static readonly IReadOnlyDictionary<string, IJsonRpcSerializer> _serializers = CreatSerializers();

        private static IReadOnlyDictionary<string, string> CreateResourceDictionary()
        {
            var resources = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var code in GetResourceCodes())
            {
                resources[code] = EmbeddedResourceManager.GetString($"Assets.{code}.json");
            }

            return resources;
        }

        private static IJsonRpcSerializer CreateSerializerRequestParamsNone()
        {
            var serializer = new Utf8JsonRpcSerializer();

            serializer.RequestContracts["m"] = new JsonRpcRequestContract();

            return serializer;
        }

        private static IJsonRpcSerializer CreateSerializerRequestParamsByName()
        {
            var serializer = new Utf8JsonRpcSerializer();

            var parameters = new Dictionary<string, Type>
            {
                ["p"] = typeof(long)
            };

            serializer.RequestContracts["m"] = new JsonRpcRequestContract(parameters);

            return serializer;
        }

        private static IJsonRpcSerializer CreateSerializerRequestParamsByPosition()
        {
            var serializer = new Utf8JsonRpcSerializer();

            var parameters = new[]
            {
                typeof(long)
            };

            serializer.RequestContracts["m"] = new JsonRpcRequestContract(parameters);

            return serializer;
        }

        private static IJsonRpcSerializer CreateSerializerResponse()
        {
            var serializer = new Utf8JsonRpcSerializer();

            var parameters = new[]
            {
                typeof(long)
            };

            serializer.DynamicResponseBindings[0L] = new JsonRpcResponseContract(typeof(long), typeof(long));

            return serializer;
        }

        private static IReadOnlyDictionary<string, IJsonRpcSerializer> CreatSerializers()
        {
            return new Dictionary<string, IJsonRpcSerializer>
            {
                ["request_params_by_name"] = CreateSerializerRequestParamsByName(),
                ["request_params_by_position"] = CreateSerializerRequestParamsByPosition(),
                ["request_params_none"] = CreateSerializerRequestParamsNone(),
                ["response"] = CreateSerializerResponse(),
            };
        }

        private static IEnumerable<string> GetResourceCodes()
        {
            return new[]
            {
                "request_params_by_name",
                "request_params_by_name_batch",
                "request_params_by_position",
                "request_params_by_position_batch",
                "request_params_none",
                "request_params_none_batch",
                "response_error",
                "response_error_batch",
                "response_error_with_data",
                "response_error_with_data_batch",
                "response_success",
                "response_success_batch"
            };
        }

        [Benchmark]
        public object ParamsNoneDeserializeRequestData()
        {
            return _serializers["request_params_none"].DeserializeRequestData(_resources["request_params_none"]);
        }

        [Benchmark]
        public object BatchParamsNoneDeserializeRequestData()
        {
            return _serializers["request_params_none"].DeserializeRequestData(_resources["request_params_none_batch"]);
        }

        [Benchmark]
        public object ParamsByNameDeserializeRequestData()
        {
            return _serializers["request_params_by_name"].DeserializeRequestData(_resources["request_params_by_name"]);
        }

        [Benchmark]
        public object BatchParamsByNameDeserializeRequestData()
        {
            return _serializers["request_params_by_name"].DeserializeRequestData(_resources["request_params_by_name_batch"]);
        }

        [Benchmark]
        public object ParamsByPositionDeserializeRequestData()
        {
            return _serializers["request_params_by_position"].DeserializeRequestData(_resources["request_params_by_position"]);
        }

        [Benchmark]
        public object BatchParamsByPositionDeserializeRequestData()
        {
            return _serializers["request_params_by_position"].DeserializeRequestData(_resources["request_params_by_position_batch"]);
        }

        [Benchmark]
        public object ErrorDeserializeResponseData()
        {
            return _serializers["response"].DeserializeResponseData(_resources["response_error"]);
        }

        [Benchmark]
        public object BatchErrorDeserializeResponseData()
        {
            return _serializers["response"].DeserializeResponseData(_resources["response_error_batch"]);
        }

        [Benchmark]
        public object ErrorWithDataDeserializeResponseData()
        {
            return _serializers["response"].DeserializeResponseData(_resources["response_error_with_data"]);
        }

        [Benchmark]
        public object BatchErrorWithDataDeserializeResponseData()
        {
            return _serializers["response"].DeserializeResponseData(_resources["response_error_with_data_batch"]);
        }

        [Benchmark]
        public object SuccessDeserializeResponseData()
        {
            return _serializers["response"].DeserializeResponseData(_resources["response_success"]);
        }

        [Benchmark]
        public object BatchSuccessDeserializeResponseData()
        {
            return _serializers["response"].DeserializeResponseData(_resources["response_success_batch"]);
        }
    }
}