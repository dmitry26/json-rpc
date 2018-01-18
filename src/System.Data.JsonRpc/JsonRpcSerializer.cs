using System.Collections.Generic;
using System.Data.JsonRpc.Internal;
using System.Data.JsonRpc.Resources;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace System.Data.JsonRpc
{
    /// <summary>Serializes and deserializes JSON-RPC messages into and from the JSON format.</summary>
    public sealed class JsonRpcSerializer : IDisposable
    {
        private readonly JsonSerializer _jsonSerializer = JsonSerializer.CreateDefault();
        private readonly IArrayPool<char> _jsonBufferPool = new JsonBufferPool();
        private readonly IDictionary<string, JsonRpcRequestContract> _requestContracts = new Dictionary<string, JsonRpcRequestContract>(StringComparer.Ordinal);
        private readonly IDictionary<string, JsonRpcResponseContract> _responseContracts = new Dictionary<string, JsonRpcResponseContract>(StringComparer.Ordinal);
        private readonly IDictionary<JsonRpcId, string> _staticResponseBindings;
        private readonly IDictionary<JsonRpcId, JsonRpcResponseContract> _dynamicResponseBindings;

        /// <summary>Initializes a new instance of the <see cref="JsonRpcSerializer" /> class.</summary>
        /// <param name="staticResponseBindings">Container instance for static response bindings.</param>
        /// <param name="dynamicResponseBindings">Container instance for dynamic response bindings.</param>
        public JsonRpcSerializer(IDictionary<JsonRpcId, string> staticResponseBindings = null, IDictionary<JsonRpcId, JsonRpcResponseContract> dynamicResponseBindings = null)
        {
            _staticResponseBindings = staticResponseBindings ?? new Dictionary<JsonRpcId, string>();
            _dynamicResponseBindings = dynamicResponseBindings ?? new Dictionary<JsonRpcId, JsonRpcResponseContract>();
        }

        /// <summary>Deserializes the JSON string to the request data.</summary>
        /// <param name="jsonString">The JSON string to deserialize.</param>
        /// <returns>RPC information about requests.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="jsonString" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during message processing.</exception>
        public JsonRpcData<JsonRpcRequest> DeserializeRequestData(string jsonString)
        {
            if (jsonString == null)
            {
                throw new ArgumentNullException(nameof(jsonString));
            }

            var jsonToken = default(JToken);

            try
            {
                using (var stringReader = new StringReader(jsonString))
                {
                    using (var jsonReader = new JsonTextReader(stringReader))
                    {
                        jsonReader.ArrayPool = _jsonBufferPool;
                        jsonToken = JToken.ReadFrom(jsonReader);
                    }
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcExceptionType.Parsing, Strings.GetString("core.deserialize.json_issue"), e);
            }

            switch (jsonToken.Type)
            {
                case JTokenType.Object:
                    {
                        var jsonObject = (JObject)jsonToken;
                        var item = default(JsonRpcItem<JsonRpcRequest>);

                        try
                        {
                            item = new JsonRpcItem<JsonRpcRequest>(ConvertTokenToRequest(jsonObject));
                        }
                        catch (JsonRpcException e)
                            when (e.Type != default)
                        {
                            item = new JsonRpcItem<JsonRpcRequest>(e);
                        }

                        return new JsonRpcData<JsonRpcRequest>(item);
                    }
                case JTokenType.Array:
                    {
                        var jsonArray = (JArray)jsonToken;

                        if (jsonArray.Count == 0)
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.batch.empty"));
                        }

                        var items = new JsonRpcItem<JsonRpcRequest>[jsonArray.Count];
                        var identifiers = default(HashSet<JsonRpcId>);

                        for (var i = 0; i < jsonArray.Count; i++)
                        {
                            var jsonObject = jsonArray[i];

                            if (jsonObject.Type != JTokenType.Object)
                            {
                                var exception = new JsonRpcException(JsonRpcExceptionType.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.batch.invalid_item"), i));

                                items[i] = new JsonRpcItem<JsonRpcRequest>(exception);

                                continue;
                            }

                            var request = default(JsonRpcRequest);

                            try
                            {
                                request = ConvertTokenToRequest((JObject)jsonObject);
                            }
                            catch (JsonRpcException e)
                                when (e.Type != default)
                            {
                                items[i] = new JsonRpcItem<JsonRpcRequest>(e);

                                continue;
                            }

                            if (request.Id.Type != default)
                            {
                                if ((jsonArray.Count - i > 1) && (identifiers == null))
                                {
                                    identifiers = new HashSet<JsonRpcId>();
                                }
                                if ((identifiers != null) && !identifiers.Add(request.Id))
                                {
                                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.batch.duplicate_id"), request.Id));
                                }
                            }

                            items[i] = new JsonRpcItem<JsonRpcRequest>(request);
                        }

                        return new JsonRpcData<JsonRpcRequest>(items);
                    }
                default:
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.input.invalid_structure"));
            }
        }

        /// <summary>Deserializes the JSON string to the response data.</summary>
        /// <param name="jsonString">The JSON string to deserialize.</param>
        /// <returns>RPC information about responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="jsonString" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during message processing.</exception>
        public JsonRpcData<JsonRpcResponse> DeserializeResponseData(string jsonString)
        {
            if (jsonString == null)
            {
                throw new ArgumentNullException(nameof(jsonString));
            }
            if (jsonString.Length == 0)
            {
                return new JsonRpcData<JsonRpcResponse>();
            }

            var jsonToken = default(JToken);

            try
            {
                using (var stringReader = new StringReader(jsonString))
                {
                    using (var jsonReader = new JsonTextReader(stringReader))
                    {
                        jsonReader.ArrayPool = _jsonBufferPool;
                        jsonToken = JToken.ReadFrom(jsonReader);
                    }
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcExceptionType.Parsing, Strings.GetString("core.deserialize.json_issue"), e);
            }

            switch (jsonToken.Type)
            {
                case JTokenType.Object:
                    {
                        var jsonObject = (JObject)jsonToken;
                        var item = default(JsonRpcItem<JsonRpcResponse>);

                        try
                        {
                            item = new JsonRpcItem<JsonRpcResponse>(ConvertTokenToResponse(jsonObject));
                        }
                        catch (JsonRpcException e)
                            when (e.Type != default)
                        {
                            item = new JsonRpcItem<JsonRpcResponse>(e);
                        }

                        return new JsonRpcData<JsonRpcResponse>(item);
                    }
                case JTokenType.Array:
                    {
                        var jsonArray = (JArray)jsonToken;

                        if (jsonArray.Count == 0)
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.batch.empty"));
                        }

                        var items = new JsonRpcItem<JsonRpcResponse>[jsonArray.Count];
                        var identifiers = default(HashSet<JsonRpcId>);

                        for (var i = 0; i < jsonArray.Count; i++)
                        {
                            var jsonObject = jsonArray[i];

                            if (jsonObject.Type != JTokenType.Object)
                            {
                                var exception = new JsonRpcException(JsonRpcExceptionType.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.batch.invalid_item"), i));

                                items[i] = new JsonRpcItem<JsonRpcResponse>(exception);

                                continue;
                            }

                            var response = default(JsonRpcResponse);

                            try
                            {
                                response = ConvertTokenToResponse((JObject)jsonObject);
                            }
                            catch (JsonRpcException e)
                                when (e.Type != default)
                            {
                                items[i] = new JsonRpcItem<JsonRpcResponse>(e);

                                continue;
                            }

                            if (response.Id.Type != default)
                            {
                                if ((jsonArray.Count - i > 1) && (identifiers == null))
                                {
                                    identifiers = new HashSet<JsonRpcId>();
                                }
                                if ((identifiers != null) && !identifiers.Add(response.Id))
                                {
                                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.batch.duplicate_id"), response.Id));
                                }
                            }

                            items[i] = new JsonRpcItem<JsonRpcResponse>(response);
                        }

                        return new JsonRpcData<JsonRpcResponse>(items);
                    }
                default:
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.input.invalid_structure"));
            }
        }

        /// <summary>Serializes the specified request to a JSON string.</summary>
        /// <param name="request">The specified request to serialize.</param>
        /// <returns>A JSON string representation of the specified request.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during message processing.</exception>
        public string SerializeRequest(JsonRpcRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                var jsonToken = ConvertRequestToToken(request);

                using (var stringWriter = new StringWriter())
                {
                    using (var jsonWriter = new JsonTextWriter(stringWriter))
                    {
                        jsonWriter.ArrayPool = _jsonBufferPool;
                        jsonToken.WriteTo(jsonWriter);
                    }

                    return stringWriter.ToString();
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(Strings.GetString("core.serialize.json_issue"), request.Id, e);
            }
        }

        /// <summary>Serializes the specified collection of requests to a JSON string.</summary>
        /// <param name="requests">The specified collection of requests to serialize.</param>
        /// <returns>A JSON string representation of the specified collection of requests.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="requests" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during message processing.</exception>
        public string SerializeRequests(IReadOnlyCollection<JsonRpcRequest> requests)
        {
            if (requests == null)
            {
                throw new ArgumentNullException(nameof(requests));
            }
            if (requests.Count == 0)
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.serialize.batch.empty"));
            }

            var jsonArray = new JArray();
            var identifiers = default(HashSet<JsonRpcId>);

            void ProcessMessage(JsonRpcRequest request, int index)
            {
                if (request == null)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.serialize.batch.invalid_item"), index));
                }

                if (request.Id.Type != default)
                {
                    if ((requests.Count - index > 1) && (identifiers == null))
                    {
                        identifiers = new HashSet<JsonRpcId>();
                    }
                    if ((identifiers != null) && !identifiers.Add(request.Id))
                    {
                        throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.serialize.batch.duplicate_id"), request.Id));
                    }
                }

                jsonArray.Add(ConvertRequestToToken(request));
            }

            if (requests is IReadOnlyList<JsonRpcRequest> messagesList)
            {
                for (var i = 0; i < messagesList.Count; i++)
                {
                    ProcessMessage(messagesList[i], i);
                }
            }
            else
            {
                var i = 0;

                foreach (var request in requests)
                {
                    ProcessMessage(request, i++);
                }
            }

            try
            {
                using (var stringWriter = new StringWriter())
                {
                    using (var jsonWriter = new JsonTextWriter(stringWriter))
                    {
                        jsonWriter.ArrayPool = _jsonBufferPool;
                        jsonArray.WriteTo(jsonWriter);
                    }

                    return stringWriter.ToString();
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(Strings.GetString("core.serialize.json_issue"), e);
            }
        }

        /// <summary>Serializes the specified response to a JSON string.</summary>
        /// <param name="response">The specified response to serialize.</param>
        /// <returns>A JSON string representation of the specified response.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="response" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during message processing.</exception>
        public string SerializeResponse(JsonRpcResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            try
            {
                var jsonToken = ConvertResponseToToken(response);

                using (var stringWriter = new StringWriter())
                {
                    using (var jsonWriter = new JsonTextWriter(stringWriter))
                    {
                        jsonWriter.ArrayPool = _jsonBufferPool;
                        jsonToken.WriteTo(jsonWriter);
                    }

                    return stringWriter.ToString();
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(Strings.GetString("core.serialize.json_issue"), response.Id, e);
            }
        }

        /// <summary>Serializes the specified collection of responses to a JSON string.</summary>
        /// <param name="responses">The specified collection of responses to serialize.</param>
        /// <returns>A JSON string representation of the specified collection of responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="responses" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during message processing.</exception>
        public string SerializeResponses(IReadOnlyCollection<JsonRpcResponse> responses)
        {
            if (responses == null)
            {
                throw new ArgumentNullException(nameof(responses));
            }
            if (responses.Count == 0)
            {
                return string.Empty;
            }

            var jsonArray = new JArray();
            var identifiers = default(HashSet<JsonRpcId>);

            void ProcessMessage(JsonRpcResponse response, int index)
            {
                if (response == null)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.serialize.batch.invalid_item"), index));
                }

                if (response.Id.Type != default)
                {
                    if ((responses.Count - index > 1) && (identifiers == null))
                    {
                        identifiers = new HashSet<JsonRpcId>();
                    }
                    if ((identifiers != null) && !identifiers.Add(response.Id))
                    {
                        throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.serialize.batch.duplicate_id"), response.Id));
                    }
                }

                jsonArray.Add(ConvertResponseToToken(response));
            }

            if (responses is IReadOnlyList<JsonRpcResponse> messagesList)
            {
                for (var i = 0; i < messagesList.Count; i++)
                {
                    ProcessMessage(messagesList[i], i);
                }
            }
            else
            {
                var i = 0;

                foreach (var response in responses)
                {
                    ProcessMessage(response, i++);
                }
            }

            try
            {
                using (var stringWriter = new StringWriter())
                {
                    using (var jsonWriter = new JsonTextWriter(stringWriter))
                    {
                        jsonWriter.ArrayPool = _jsonBufferPool;
                        jsonArray.WriteTo(jsonWriter);
                    }

                    return stringWriter.ToString();
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(Strings.GetString("core.serialize.json_issue"), e);
            }
        }

        private JsonRpcRequest ConvertTokenToRequest(JObject jsonObject)
        {
            if (!jsonObject.TryGetValue("jsonrpc", out var jsonTokenProtocol) || (jsonTokenProtocol.Type != JTokenType.String) || ((string)jsonTokenProtocol != "2.0"))
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.request.protocol.invalid_property"));
            }

            var requestId = default(JsonRpcId);

            if (jsonObject.TryGetValue("id", out var jsonValueId))
            {
                switch (jsonValueId.Type)
                {
                    case JTokenType.String:
                        {
                            requestId = (string)jsonValueId;
                        }
                        break;
                    case JTokenType.Integer:
                        {
                            requestId = (long)jsonValueId;
                        }
                        break;
                    case JTokenType.Null:
                        {
                        }
                        break;
                    default:
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.request.id.invalid_property"));
                        }
                }
            }

            if (!jsonObject.TryGetValue("method", out var jsonValueMethod) || (jsonValueMethod.Type != JTokenType.String))
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.request.method.invalid_property"), requestId);
            }

            var requestMethod = (string)jsonValueMethod;

            if (!_requestContracts.TryGetValue(requestMethod, out var contract))
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMethod, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.request.method.unsupported"), requestMethod), requestId);
            }
            if (contract == null)
            {
                throw new JsonRpcException(string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.request.method.contract.undefined"), requestMethod), requestId);
            }

            switch (contract.ParamsType)
            {
                case JsonRpcParamsType.ByPosition:
                    {
                        if (!jsonObject.TryGetValue("params", out var jsonTokenParams) || ((jsonTokenParams.Type != JTokenType.Array) && (jsonTokenParams.Type != JTokenType.Object)))
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.request.params.invalid_property"), requestId);
                        }
                        if (jsonTokenParams.Type != JTokenType.Array)
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.InvalidParams, Strings.GetString("core.deserialize.request.params.invalid_structure"), requestId);
                        }

                        var jsonArrayParams = (JArray)jsonTokenParams;

                        if (jsonArrayParams.Count < contract.ParamsByPosition.Count)
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.InvalidParams, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.request.params.invalid_count"), jsonArrayParams.Count), requestId);
                        }

                        var requestParams = new object[contract.ParamsByPosition.Count];

                        try
                        {
                            for (var i = 0; i < requestParams.Length; i++)
                            {
                                requestParams[i] = jsonArrayParams[i].ToObject(contract.ParamsByPosition[i], _jsonSerializer);
                            }
                        }
                        catch (JsonException e)
                        {
                            throw new JsonRpcException(Strings.GetString("core.deserialize.json_issue"), requestId, e);
                        }

                        return new JsonRpcRequest(requestMethod, requestId, requestParams);
                    }
                case JsonRpcParamsType.ByName:
                    {
                        if (!jsonObject.TryGetValue("params", out var jsonTokenParams) || ((jsonTokenParams.Type != JTokenType.Array) && (jsonTokenParams.Type != JTokenType.Object)))
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.request.params.invalid_property"), requestId);
                        }
                        if (jsonTokenParams.Type != JTokenType.Object)
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.InvalidParams, Strings.GetString("core.deserialize.request.params.invalid_structure"), requestId);
                        }

                        var jsonObjectParams = (JObject)jsonTokenParams;
                        var requestParams = new Dictionary<string, object>(contract.ParamsByName.Count, StringComparer.Ordinal);

                        try
                        {
                            foreach (var kvp in contract.ParamsByName)
                            {
                                if (!jsonObjectParams.TryGetValue(kvp.Key, StringComparison.Ordinal, out var jsonObjectParam))
                                {
                                    continue;
                                }

                                requestParams[kvp.Key] = jsonObjectParam.ToObject(kvp.Value, _jsonSerializer);
                            }
                        }
                        catch (JsonException e)
                        {
                            throw new JsonRpcException(Strings.GetString("core.deserialize.json_issue"), requestId, e);
                        }

                        return new JsonRpcRequest(requestMethod, requestId, requestParams);
                    }
                default:
                    {
                        return new JsonRpcRequest(requestMethod, requestId);
                    }
            }
        }

        private JObject ConvertRequestToToken(JsonRpcRequest request)
        {
            var jsonObject = new JObject
            {
                { "jsonrpc", "2.0" },
                { "method", request.Method }
            };

            switch (request.ParamsType)
            {
                case JsonRpcParamsType.ByPosition:
                    {
                        var jsonTokenParams = new JArray();

                        try
                        {
                            for (var i = 0; i < request.ParamsByPosition.Count; i++)
                            {
                                jsonTokenParams.Add(request.ParamsByPosition[i] != null ? JToken.FromObject(request.ParamsByPosition[i], _jsonSerializer) : JValue.CreateNull());
                            }
                        }
                        catch (JsonException e)
                        {
                            throw new JsonRpcException(Strings.GetString("core.serialize.json_issue"), request.Id, e);
                        }

                        jsonObject.Add("params", jsonTokenParams);
                    }
                    break;
                case JsonRpcParamsType.ByName:
                    {
                        var jsonTokenParams = new JObject();

                        try
                        {
                            foreach (var kvp in request.ParamsByName)
                            {
                                jsonTokenParams.Add(kvp.Key, kvp.Value != null ? JToken.FromObject(kvp.Value, _jsonSerializer) : JValue.CreateNull());
                            }
                        }
                        catch (JsonException e)
                        {
                            throw new JsonRpcException(Strings.GetString("core.serialize.json_issue"), request.Id, e);
                        }

                        jsonObject.Add("params", jsonTokenParams);
                    }
                    break;
            }

            switch (request.Id.Type)
            {
                case JsonRpcIdType.Number:
                    {
                        jsonObject.Add("id", new JValue((long)request.Id));
                    }
                    break;
                case JsonRpcIdType.String:
                    {
                        jsonObject.Add("id", new JValue((string)request.Id));
                    }
                    break;
            }

            return jsonObject;
        }

        private JsonRpcResponse ConvertTokenToResponse(JObject jsonObject)
        {
            if (!jsonObject.TryGetValue("jsonrpc", out var jsonTokenProtocol) || (jsonTokenProtocol.Type != JTokenType.String) || ((string)jsonTokenProtocol != "2.0"))
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.response.protocol.invalid_property"));
            }

            var responseId = default(JsonRpcId);

            if (jsonObject.TryGetValue("id", out var jsonValueId))
            {
                switch (jsonValueId.Type)
                {
                    case JTokenType.Integer:
                        {
                            responseId = (long)jsonValueId;
                        }
                        break;
                    case JTokenType.String:
                        {
                            responseId = (string)jsonValueId;
                        }
                        break;
                    case JTokenType.Null:
                        {
                        }
                        break;
                    default:
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.response.id.invalid_property"));
                        }
                }
            }

            var jsonTokenResult = jsonObject.GetValue("result");
            var jsonTokenError = jsonObject.GetValue("error");

            if (((jsonTokenResult == null) && (jsonTokenError == null)) || ((jsonTokenResult != null) && (jsonTokenError != null)))
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.response.invalid_properties"), responseId);
            }

            if (jsonTokenResult != null)
            {
                if (responseId == default)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.response.invalid_properties"), responseId);
                }

                var contract = GetResponseContract(responseId);
                var responseResult = default(object);

                if (contract.ResultType != null)
                {
                    try
                    {
                        responseResult = jsonTokenResult.ToObject(contract.ResultType, _jsonSerializer);
                    }
                    catch (JsonException e)
                    {
                        throw new JsonRpcException(Strings.GetString("core.deserialize.json_issue"), responseId, e);
                    }
                }

                return new JsonRpcResponse(responseResult, responseId);
            }
            else
            {
                if (jsonTokenError.Type != JTokenType.Object)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.response.error.invalid_type"), responseId);
                }

                var jsonObjectError = (JObject)jsonTokenError;

                if (!jsonObjectError.TryGetValue("code", out var jsonTokenErrorCode) || (jsonTokenErrorCode.Type != JTokenType.Integer))
                {
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.response.error.code.invalid_property"), responseId);
                }

                var responseErrorCode = (long)jsonTokenErrorCode;

                if (!jsonObjectError.TryGetValue("message", out var jsonTokenErrorMessage) || (jsonTokenErrorMessage.Type != JTokenType.String))
                {
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.response.error.message.invalid_property"), responseId);
                }

                var responseErrorMessage = (string)jsonTokenErrorMessage;
                var responseErrorData = default(object);

                if (jsonObjectError.TryGetValue("data", out var jsonTokenErrorData) && (jsonTokenErrorData.Type != JTokenType.Null))
                {
                    if (responseId.Type == default)
                    {
                        if (DefaultErrorDataType != null)
                        {
                            try
                            {
                                responseErrorData = jsonTokenErrorData.ToObject(DefaultErrorDataType, _jsonSerializer);
                            }
                            catch (JsonException e)
                            {
                                throw new JsonRpcException(Strings.GetString("core.deserialize.json_issue"), responseId, e);
                            }
                        }
                    }
                    else
                    {
                        var contract = GetResponseContract(responseId);

                        if (contract.ErrorDataType != null)
                        {
                            try
                            {
                                responseErrorData = jsonTokenErrorData.ToObject(contract.ErrorDataType, _jsonSerializer);
                            }
                            catch (JsonException e)
                            {
                                throw new JsonRpcException(Strings.GetString("core.deserialize.json_issue"), responseId, e);
                            }
                        }
                    }
                }

                var responseError = default(JsonRpcError);

                try
                {
                    responseError = new JsonRpcError(responseErrorCode, responseErrorMessage, responseErrorData);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, Strings.GetString("core.deserialize.response.error.code.invalid_range"), responseId, e);
                }

                return new JsonRpcResponse(responseError, responseId);
            }
        }

        private JsonRpcResponseContract GetResponseContract(JsonRpcId identifier)
        {
            if (!_dynamicResponseBindings.TryGetValue(identifier, out var contract))
            {
                if (_staticResponseBindings.TryGetValue(identifier, out var method) && (method != null))
                {
                    _responseContracts.TryGetValue(method, out contract);
                }
            }

            if (contract == null)
            {
                throw new JsonRpcException(Strings.GetString("core.deserialize.response.method.contract.undefined"), identifier);
            }

            return contract;
        }

        private JObject ConvertResponseToToken(JsonRpcResponse response)
        {
            var jsonObject = new JObject
            {
                { "jsonrpc", "2.0" }
            };

            if (response.Success)
            {
                var resultToken = default(JToken);

                try
                {
                    resultToken = JToken.FromObject(response.Result, _jsonSerializer);
                }
                catch (JsonException e)
                {
                    throw new JsonRpcException(Strings.GetString("core.serialize.json_issue"), response.Id, e);
                }

                jsonObject.Add("result", resultToken);
            }
            else
            {
                var errorToken = new JObject
                {
                    { "code", response.Error.Code },
                    { "message", response.Error.Message }
                };

                if (response.Error.Data != null)
                {
                    var responseErrorDataToken = default(JToken);

                    try
                    {
                        responseErrorDataToken = JToken.FromObject(response.Error.Data, _jsonSerializer);
                    }
                    catch (JsonException e)
                    {
                        throw new JsonRpcException(Strings.GetString("core.serialize.json_issue"), response.Id, e);
                    }

                    errorToken.Add("data", responseErrorDataToken);
                }

                jsonObject.Add("error", errorToken);
            }

            switch (response.Id.Type)
            {
                case JsonRpcIdType.None:
                    {
                        jsonObject.Add("id", JValue.CreateNull());
                    }
                    break;
                case JsonRpcIdType.Number:
                    {
                        jsonObject.Add("id", new JValue((long)response.Id));
                    }
                    break;
                case JsonRpcIdType.String:
                    {
                        jsonObject.Add("id", new JValue((string)response.Id));
                    }
                    break;
            }

            return jsonObject;
        }

        /// <summary>Clears static and dynamic response bindings.</summary>
        public void Dispose()
        {
            _dynamicResponseBindings.Clear();
            _staticResponseBindings.Clear();
        }

        /// <summary>Gets the container instance for request contracts.</summary>
        public IDictionary<string, JsonRpcRequestContract> RequestContracts
        {
            get => _requestContracts;
        }

        /// <summary>Gets the container instance for response contracts.</summary>
        public IDictionary<string, JsonRpcResponseContract> ResponseContracts
        {
            get => _responseContracts;
        }

        /// <summary>Gets the container instance for dynamic response bindings.</summary>
        public IDictionary<JsonRpcId, JsonRpcResponseContract> DynamicResponseBindings
        {
            get => _dynamicResponseBindings;
        }

        /// <summary>Gets the container instance for static response bindings.</summary>
        public IDictionary<JsonRpcId, string> StaticResponseBindings
        {
            get => _staticResponseBindings;
        }

        /// <summary>Gets or sets a type of default error data for deserializing a response.</summary>
        public Type DefaultErrorDataType
        {
            get;
            set;
        }
    }
}