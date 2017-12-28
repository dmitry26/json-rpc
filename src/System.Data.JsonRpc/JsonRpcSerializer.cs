using System.Collections.Generic;
using System.Data.JsonRpc.Resources;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace System.Data.JsonRpc
{
    /// <summary>Serializes and deserializes JSON-RPC messages into and from the JSON format.</summary>
    public sealed class JsonRpcSerializer
    {
        private static readonly JValue _jsonTokenProtocol = new JValue("2.0");

        private readonly JsonConverter[] _jsonConverters;
        private readonly JsonSerializer _jsonSerializer;
        private readonly IArrayPool<char> _jsonSerializerBufferPool;
        private readonly JsonRpcSerializerScheme _scheme;

        /// <summary>Initializes a new instance of the <see cref="JsonRpcSerializer" /> class.</summary>
        /// <param name="scheme">The type scheme for deserialization.</param>
        /// <param name="settings">The settings for serialization and deserialization.</param>
        public JsonRpcSerializer(JsonRpcSerializerScheme scheme = null, JsonRpcSerializerSettings settings = null)
        {
            _scheme = scheme ?? new JsonRpcSerializerScheme();
            _jsonSerializer = settings?.JsonSerializer;
            _jsonSerializerBufferPool = settings?.JsonSerializerBufferPool;

            if (_jsonSerializer != null)
            {
                _jsonConverters = new JsonConverter[_jsonSerializer.Converters.Count];
                _jsonSerializer.Converters.CopyTo(_jsonConverters, 0);
            }
            else
            {
                _jsonSerializer = JsonSerializer.CreateDefault();
            }
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
                using (var jsonTextReader = new JsonTextReader(new StringReader(jsonString)))
                {
                    if (_jsonSerializerBufferPool != null)
                    {
                        jsonTextReader.ArrayPool = _jsonSerializerBufferPool;
                    }

                    jsonToken = JToken.ReadFrom(jsonTextReader);
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
                            when (e.Type != JsonRpcExceptionType.Undefined)
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
                                when (e.Type != JsonRpcExceptionType.Undefined)
                            {
                                items[i] = new JsonRpcItem<JsonRpcRequest>(e);

                                continue;
                            }

                            if (request.Id.Type != JsonRpcIdType.None)
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
        /// <param name="bindings">Request identifier to method name bindings used to map response properties to the corresponding types.</param>
        /// <returns>RPC information about responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="jsonString" /> or <paramref name="bindings" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during message processing.</exception>
        public JsonRpcData<JsonRpcResponse> DeserializeResponseData(string jsonString, IReadOnlyDictionary<JsonRpcId, string> bindings)
        {
            if (jsonString == null)
            {
                throw new ArgumentNullException(nameof(jsonString));
            }
            if (bindings == null)
            {
                throw new ArgumentNullException(nameof(bindings));
            }

            return DeserializeResponseData(jsonString, bindings, null);
        }

        /// <summary>Deserializes the JSON string to the response data.</summary>
        /// <param name="jsonString">The JSON string to deserialize.</param>
        /// <param name="bindings">Request identifier to method scheme bindings used to map response properties to the corresponding types.</param>
        /// <returns>RPC information about responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="jsonString" /> or <paramref name="bindings" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during message processing.</exception>
        public JsonRpcData<JsonRpcResponse> DeserializeResponseData(string jsonString, IReadOnlyDictionary<JsonRpcId, JsonRpcMethodScheme> bindings)
        {
            if (jsonString == null)
            {
                throw new ArgumentNullException(nameof(jsonString));
            }
            if (bindings == null)
            {
                throw new ArgumentNullException(nameof(bindings));
            }

            return DeserializeResponseData(jsonString, null, bindings);
        }

        private JsonRpcData<JsonRpcResponse> DeserializeResponseData(string jsonString, IReadOnlyDictionary<JsonRpcId, string> methodNameBindings, IReadOnlyDictionary<JsonRpcId, JsonRpcMethodScheme> methodSchemeBindings)
        {
            if (jsonString.Length == 0)
            {
                return new JsonRpcData<JsonRpcResponse>();
            }

            var jsonToken = default(JToken);

            try
            {
                using (var jsonTextReader = new JsonTextReader(new StringReader(jsonString)))
                {
                    if (_jsonSerializerBufferPool != null)
                    {
                        jsonTextReader.ArrayPool = _jsonSerializerBufferPool;
                    }

                    jsonToken = JToken.ReadFrom(jsonTextReader);
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
                            item = new JsonRpcItem<JsonRpcResponse>(ConvertTokenToResponse(jsonObject, methodNameBindings, methodSchemeBindings));
                        }
                        catch (JsonRpcException e)
                            when (e.Type != JsonRpcExceptionType.Undefined)
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
                                response = ConvertTokenToResponse((JObject)jsonObject, methodNameBindings, methodSchemeBindings);
                            }
                            catch (JsonRpcException e)
                                when (e.Type != JsonRpcExceptionType.Undefined)
                            {
                                items[i] = new JsonRpcItem<JsonRpcResponse>(e);

                                continue;
                            }

                            if (response.Id.Type != JsonRpcIdType.None)
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
                return ConvertRequestToToken(request).ToString(_jsonSerializer.Formatting, _jsonConverters);
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.serialize.json_issue"), request.Id, e);
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

                if (request.Id.Type != JsonRpcIdType.None)
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
                return jsonArray.ToString(_jsonSerializer.Formatting, _jsonConverters);
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.serialize.json_issue"), e);
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
                return ConvertResponseToToken(response).ToString(_jsonSerializer.Formatting, _jsonConverters);
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.serialize.json_issue"), response.Id, e);
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

                if (response.Id.Type != JsonRpcIdType.None)
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
                return jsonArray.ToString(_jsonSerializer.Formatting, _jsonConverters);
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.serialize.json_issue"), e);
            }
        }

        private JsonRpcRequest ConvertTokenToRequest(JObject jsonObject)
        {
            if (_scheme == null)
            {
                throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.deserialize.scheme.undefined"));
            }
            if (!jsonObject.TryGetValue("jsonrpc", out var jsonTokenProtocol) || (jsonTokenProtocol.Type != JTokenType.String) || !object.Equals(jsonTokenProtocol, _jsonTokenProtocol))
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

            if (!_scheme.Methods.TryGetValue(requestMethod, out var methodScheme))
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMethod, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.request.method.unsupported"), requestMethod), requestId);
            }
            if (methodScheme == null)
            {
                throw new JsonRpcException(JsonRpcExceptionType.Undefined, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.request.method.scheme.undefined"), requestMethod), requestId);
            }

            switch (methodScheme.ParamsType)
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

                        if (jsonArrayParams.Count < methodScheme.ParamsByPositionScheme.Count)
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.InvalidParams, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.request.params.invalid_count"), jsonArrayParams.Count), requestId);
                        }

                        var requestParams = new object[jsonArrayParams.Count];

                        for (var i = 0; i < jsonArrayParams.Count; i++)
                        {
                            try
                            {
                                requestParams[i] = jsonArrayParams[i].ToObject(methodScheme.ParamsByPositionScheme[i], _jsonSerializer);
                            }
                            catch (JsonException e)
                            {
                                throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.deserialize.json_issue"), requestId, e);
                            }
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
                        var requestParams = new Dictionary<string, object>(StringComparer.Ordinal);

                        foreach (var kvp in methodScheme.ParamsByNameScheme)
                        {
                            if (!jsonObjectParams.TryGetValue(kvp.Key, out var jsonObjectParam))
                            {
                                continue;
                            }

                            try
                            {
                                requestParams[kvp.Key] = jsonObjectParam.ToObject(kvp.Value, _jsonSerializer);
                            }
                            catch (JsonException e)
                            {
                                throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.deserialize.json_issue"), requestId, e);
                            }
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
                { "jsonrpc", _jsonTokenProtocol },
                { "method", request.Method }
            };

            switch (request.ParamsType)
            {
                case JsonRpcParamsType.ByPosition:
                    {
                        var jsonTokenParams = new JArray();

                        for (var i = 0; i < request.ParamsByPosition.Count; i++)
                        {
                            try
                            {
                                jsonTokenParams.Add(request.ParamsByPosition[i] != null ? JToken.FromObject(request.ParamsByPosition[i], _jsonSerializer) : JValue.CreateNull());
                            }
                            catch (JsonException e)
                            {
                                throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.serialize.json_issue"), request.Id, e);
                            }
                        }

                        jsonObject.Add("params", jsonTokenParams);
                    }
                    break;
                case JsonRpcParamsType.ByName:
                    {
                        var jsonTokenParams = new JObject();

                        foreach (var kvp in request.ParamsByName)
                        {
                            try
                            {
                                jsonTokenParams.Add(kvp.Key, kvp.Value != null ? JToken.FromObject(kvp.Value, _jsonSerializer) : JValue.CreateNull());
                            }
                            catch (JsonException e)
                            {
                                throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.serialize.json_issue"), request.Id, e);
                            }
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

        private JsonRpcResponse ConvertTokenToResponse(JObject jsonObject, IReadOnlyDictionary<JsonRpcId, string> methodNameBindings, IReadOnlyDictionary<JsonRpcId, JsonRpcMethodScheme> methodSchemeBindings)
        {
            if (!jsonObject.TryGetValue("jsonrpc", out var jsonTokenProtocol) || (jsonTokenProtocol.Type != JTokenType.String) || !object.Equals(jsonTokenProtocol, _jsonTokenProtocol))
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
                var methodScheme = GetMethodScheme(responseId, methodNameBindings, methodSchemeBindings);
                var responseResult = default(object);

                if (methodScheme.ResultType != null)
                {
                    try
                    {
                        responseResult = jsonTokenResult.ToObject(methodScheme.ResultType, _jsonSerializer);
                    }
                    catch (JsonException e)
                    {
                        throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.deserialize.json_issue"), responseId, e);
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
                    if (responseId.Type == JsonRpcIdType.None)
                    {
                        if (_scheme == null)
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.deserialize.scheme.undefined"));
                        }

                        if (_scheme.DefaultErrorDataType != null)
                        {
                            try
                            {
                                responseErrorData = jsonTokenErrorData.ToObject(_scheme.DefaultErrorDataType, _jsonSerializer);
                            }
                            catch (JsonException e)
                            {
                                throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.deserialize.json_issue"), responseId, e);
                            }
                        }
                    }
                    else
                    {
                        var methodScheme = GetMethodScheme(responseId, methodNameBindings, methodSchemeBindings);

                        if (methodScheme.ErrorDataType != null)
                        {
                            try
                            {
                                responseErrorData = jsonTokenErrorData.ToObject(methodScheme.ErrorDataType, _jsonSerializer);
                            }
                            catch (JsonException e)
                            {
                                throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.deserialize.json_issue"), responseId, e);
                            }
                        }
                    }
                }

                var responseError = new JsonRpcError(responseErrorCode, responseErrorMessage, responseErrorData);

                return new JsonRpcResponse(responseError, responseId);
            }
        }

        private JsonRpcMethodScheme GetMethodScheme(JsonRpcId identifier, IReadOnlyDictionary<JsonRpcId, string> methodNameBindings, IReadOnlyDictionary<JsonRpcId, JsonRpcMethodScheme> methodSchemeBindings)
        {
            var methodScheme = default(JsonRpcMethodScheme);

            if (methodSchemeBindings != null)
            {
                methodSchemeBindings.TryGetValue(identifier, out methodScheme);
            }
            else
            {
                if (_scheme == null)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.deserialize.scheme.undefined"));
                }
                if (methodNameBindings.TryGetValue(identifier, out var messageMethod) && (messageMethod != null))
                {
                    _scheme.Methods.TryGetValue(messageMethod, out methodScheme);
                }
            }

            if (methodScheme == null)
            {
                throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.deserialize.response.method.scheme.undefined"), identifier);
            }

            return methodScheme;
        }

        private JObject ConvertResponseToToken(JsonRpcResponse response)
        {
            var jsonObject = new JObject
            {
                { "jsonrpc", _jsonTokenProtocol }
            };

            if (response.Result != null)
            {
                var resultToken = default(JToken);

                try
                {
                    resultToken = JToken.FromObject(response.Result, _jsonSerializer);
                }
                catch (JsonException e)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.serialize.json_issue"), response.Id, e);
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
                        throw new JsonRpcException(JsonRpcExceptionType.Undefined, Strings.GetString("core.serialize.json_issue"), response.Id, e);
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
    }
}