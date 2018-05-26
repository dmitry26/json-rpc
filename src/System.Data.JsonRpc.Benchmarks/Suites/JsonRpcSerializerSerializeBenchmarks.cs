using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace System.Data.JsonRpc.Benchmarks.Suites
{
    public abstract class JsonRpcSerializerSerializeBenchmarks
    {
        private static readonly IReadOnlyDictionary<string, JsonRpcRequest> _requests = CreateRequestDictionary();
        private static readonly IReadOnlyDictionary<string, JsonRpcResponse> _responses = CreateResponseDictionary();
        private static readonly IReadOnlyDictionary<string, IReadOnlyList<JsonRpcRequest>> _requestBatches = CreateRequestBatchesDictionary();
        private static readonly IReadOnlyDictionary<string, IReadOnlyList<JsonRpcResponse>> _responseBatches = CreateResponseBatchesDictionary();

        private readonly JsonRpcSerializer _serializer = new JsonRpcSerializer();

        private static IReadOnlyDictionary<string, JsonRpcRequest> CreateRequestDictionary()
        {
            return new Dictionary<string, JsonRpcRequest>
            {
                ["request_params_by_name"] = CreateRequestParamsByName(),
                ["request_params_by_position"] = CreateRequestParamsByPosition(),
                ["request_params_none"] = CreateRequestParamsNone(),
            };
        }

        private static IReadOnlyDictionary<string, IReadOnlyList<JsonRpcRequest>> CreateRequestBatchesDictionary()
        {
            return new Dictionary<string, IReadOnlyList<JsonRpcRequest>>
            {
                ["request_params_by_name"] = new[] { CreateRequestParamsByName() },
                ["request_params_by_position"] = new[] { CreateRequestParamsByPosition() },
                ["request_params_none"] = new[] { CreateRequestParamsNone() },
            };
        }

        private static IReadOnlyDictionary<string, JsonRpcResponse> CreateResponseDictionary()
        {
            return new Dictionary<string, JsonRpcResponse>
            {
                ["response_error"] = CreateResponseError(),
                ["response_error_with_data"] = CreateResponseErrorWithData(),
                ["response_success"] = CreateResponseSuccess(),
            };
        }

        private static IReadOnlyDictionary<string, IReadOnlyList<JsonRpcResponse>> CreateResponseBatchesDictionary()
        {
            return new Dictionary<string, IReadOnlyList<JsonRpcResponse>>
            {
                ["response_error"] = new[] { CreateResponseError() },
                ["response_error_with_data"] = new[] { CreateResponseErrorWithData() },
                ["response_success"] = new[] { CreateResponseSuccess() },
            };
        }

        private static JsonRpcRequest CreateRequestParamsNone()
        {
            return new JsonRpcRequest("m", 0L);
        }

        private static JsonRpcRequest CreateRequestParamsByName()
        {
            var parameters = new Dictionary<string, object>
            {
                ["p"] = 0L
            };

            return new JsonRpcRequest("m", 0L, parameters);
        }

        private static JsonRpcRequest CreateRequestParamsByPosition()
        {
            var parameters = new object[]
            {
                0L
            };

            return new JsonRpcRequest("m", 0L, parameters);
        }

        private static JsonRpcResponse CreateResponseSuccess()
        {
            return new JsonRpcResponse(0L, 0L);
        }

        private static JsonRpcResponse CreateResponseError()
        {
            return new JsonRpcResponse(new JsonRpcError(0L, "m"), 0L);
        }

        private static JsonRpcResponse CreateResponseErrorWithData()
        {
            return new JsonRpcResponse(new JsonRpcError(0L, "m", 0L), 0L);
        }

        [Benchmark]
        public object ParamsNoneSerializeRequest()
        {
            return _serializer.SerializeRequest(_requests["request_params_none"]);
        }

        [Benchmark]
        public object BatchParamsNoneSerializeRequest()
        {
            return _serializer.SerializeRequests(_requestBatches["request_params_none"]);
        }

        [Benchmark]
        public object ParamsByNameSerializeRequest()
        {
            return _serializer.SerializeRequest(_requests["request_params_by_name"]);
        }

        [Benchmark]
        public object BatchParamsByNameSerializeRequest()
        {
            return _serializer.SerializeRequests(_requestBatches["request_params_by_name"]);
        }

        [Benchmark]
        public object ParamsByPositionSerializeRequest()
        {
            return _serializer.SerializeRequest(_requests["request_params_by_position"]);
        }

        [Benchmark]
        public object BatchParamsByPositionSerializeRequest()
        {
            return _serializer.SerializeRequests(_requestBatches["request_params_by_position"]);
        }

        [Benchmark]
        public object SuccessSerializeResponse()
        {
            return _serializer.SerializeResponse(_responses["response_success"]);
        }

        [Benchmark]
        public object BatchSuccessSerializeResponse()
        {
            return _serializer.SerializeResponses(_responseBatches["response_success"]);
        }

        [Benchmark]
        public object ErrorSerializeResponse()
        {
            return _serializer.SerializeResponse(_responses["response_error"]);
        }

        [Benchmark]
        public object BatchErrorSerializeResponse()
        {
            return _serializer.SerializeResponses(_responseBatches["response_error"]);
        }

        [Benchmark]
        public object ErrorWithDataSerializeResponse()
        {
            return _serializer.SerializeResponse(_responses["response_error_with_data"]);
        }

        [Benchmark]
        public object BatchErrorWithDataSerializeResponse()
        {
            return _serializer.SerializeResponses(_responseBatches["response_error_with_data"]);
        }
    }
}