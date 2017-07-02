using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace System.Data.JsonRpc
{
    /// <summary>Serializes and deserializes JSON-RPC messages into and from the JSON format.</summary>
    public sealed class JsonRpcSerializer
    {
        private static readonly JsonSerializer _defaultJsonSerializer = JsonSerializer.CreateDefault();
        private static readonly JsonRpcData<JsonRpcResponse> _emptyResponseData = new JsonRpcData<JsonRpcResponse>();
        private static readonly JValue _protocolVersionToken = new JValue("2.0");
        private readonly JsonConverter[] _jsonConverters;
        private readonly JsonSerializer _jsonSerializer;
        private readonly IArrayPool<char> _jsonSerializerArrayPool;
        private readonly JsonRpcSerializerScheme _scheme;

        /// <summary>Initializes a new instance of the <see cref="JsonRpcSerializer" /> class.</summary>
        public JsonRpcSerializer()
        {
            _jsonSerializer = _defaultJsonSerializer;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcSerializer" /> class.</summary>
        /// <param name="scheme">The type scheme for deserialization.</param>
        /// <exception cref="ArgumentNullException"><paramref name="scheme" /> is <see langword="null" />.</exception>
        public JsonRpcSerializer(JsonRpcSerializerScheme scheme)
        {
            if (scheme == null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }

            _scheme = scheme;
            _jsonSerializer = _defaultJsonSerializer;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcSerializer" /> class.</summary>
        /// <param name="scheme">The type scheme for deserialization.</param>
        /// <param name="settings">The settings for serialization and deserialization.</param>
        /// <exception cref="ArgumentNullException"><paramref name="scheme" /> or <paramref name="settings" /> is <see langword="null" />.</exception>
        public JsonRpcSerializer(JsonRpcSerializerScheme scheme, JsonRpcSerializerSettings settings)
        {
            if (scheme == null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _scheme = scheme;
            _jsonSerializer = settings.JsonSerializer;
            _jsonSerializerArrayPool = settings.JsonSerializerArrayPool;

            if (_jsonSerializer != null)
            {
                _jsonConverters = new JsonConverter[_jsonSerializer.Converters.Count];
                _jsonSerializer.Converters.CopyTo(_jsonConverters, 0);
            }
            else
            {
                _jsonSerializer = _defaultJsonSerializer;
            }
        }

        /// <summary>Deserializes the JSON string to the request data.</summary>
        /// <param name="jsonString">The JSON string to deserialize.</param>
        /// <returns>RPC information about requests.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="jsonString" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during message processing.</exception>
        public JsonRpcData<JsonRpcRequest> DeserializeRequestsData(string jsonString)
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
                    if (_jsonSerializerArrayPool != null)
                    {
                        jsonTextReader.ArrayPool = _jsonSerializerArrayPool;
                    }

                    jsonToken = JToken.ReadFrom(jsonTextReader);
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcExceptionType.ParseError, "JSON deserialization error", e);
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
                            when (e.Type != JsonRpcExceptionType.GenericError)
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
                            throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The batch is empty");
                        }

                        var items = new JsonRpcItem<JsonRpcRequest>[jsonArray.Count];
                        var identifiers = default(HashSet<JsonRpcId>);

                        for (var i = 0; i < jsonArray.Count; i++)
                        {
                            var jsonObject = jsonArray[i];

                            if (jsonObject.Type != JTokenType.Object)
                            {
                                var exception = new JsonRpcException(JsonRpcExceptionType.InvalidMessage, $"The batch item at position {i} is not a message");

                                items[i] = new JsonRpcItem<JsonRpcRequest>(exception);

                                continue;
                            }

                            var request = default(JsonRpcRequest);

                            try
                            {
                                request = ConvertTokenToRequest((JObject)jsonObject);
                            }
                            catch (JsonRpcException e)
                                when (e.Type != JsonRpcExceptionType.GenericError)
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
                                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, $"The batch contains messages with the same identifier: \"{request.Id}\"");
                                }
                            }

                            items[i] = new JsonRpcItem<JsonRpcRequest>(request);
                        }

                        return new JsonRpcData<JsonRpcRequest>(items);
                    }
                default:
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Invalid data structure");
            }
        }

        /// <summary>Deserializes the JSON string to the response data.</summary>
        /// <param name="jsonString">The JSON string to deserialize.</param>
        /// <param name="bindings">Request identifier to method name bindings used to map response properties to the corresponding types.</param>
        /// <returns>RPC information about responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="jsonString" /> or <paramref name="bindings" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during message processing.</exception>
        public JsonRpcData<JsonRpcResponse> DeserializeResponsesData(string jsonString, IReadOnlyDictionary<JsonRpcId, string> bindings)
        {
            if (jsonString == null)
            {
                throw new ArgumentNullException(nameof(jsonString));
            }
            if (bindings == null)
            {
                throw new ArgumentNullException(nameof(bindings));
            }

            if (jsonString.Length == 0)
            {
                return _emptyResponseData;
            }

            var jsonToken = default(JToken);

            try
            {
                using (var jsonTextReader = new JsonTextReader(new StringReader(jsonString)))
                {
                    if (_jsonSerializerArrayPool != null)
                    {
                        jsonTextReader.ArrayPool = _jsonSerializerArrayPool;
                    }

                    jsonToken = JToken.ReadFrom(jsonTextReader);
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcExceptionType.ParseError, "JSON deserialization error", e);
            }

            switch (jsonToken.Type)
            {
                case JTokenType.Object:
                    {
                        var jsonObject = (JObject)jsonToken;
                        var item = default(JsonRpcItem<JsonRpcResponse>);

                        try
                        {
                            item = new JsonRpcItem<JsonRpcResponse>(ConvertTokenToResponse(jsonObject, bindings));
                        }
                        catch (JsonRpcException e)
                            when (e.Type != JsonRpcExceptionType.GenericError)
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
                            throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The batch is empty");
                        }

                        var items = new JsonRpcItem<JsonRpcResponse>[jsonArray.Count];
                        var identifiers = default(HashSet<JsonRpcId>);

                        for (var i = 0; i < jsonArray.Count; i++)
                        {
                            var jsonObject = jsonArray[i];

                            if (jsonObject.Type != JTokenType.Object)
                            {
                                var exception = new JsonRpcException(JsonRpcExceptionType.InvalidMessage, $"The batch item at position {i} is not a message");

                                items[i] = new JsonRpcItem<JsonRpcResponse>(exception);

                                continue;
                            }

                            var response = default(JsonRpcResponse);

                            try
                            {
                                response = ConvertTokenToResponse((JObject)jsonObject, bindings);
                            }
                            catch (JsonRpcException e)
                                when (e.Type != JsonRpcExceptionType.GenericError)
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
                                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, $"The batch contains messages with the same identifier: \"{response.Id}\"");
                                }
                            }

                            items[i] = new JsonRpcItem<JsonRpcResponse>(response);
                        }

                        return new JsonRpcData<JsonRpcResponse>(items);
                    }
                default:
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Invalid data structure");
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
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "JSON serialization error", request.Id, e);
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
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The batch is empty");
            }

            var jsonArray = new JArray();
            var identifiers = default(HashSet<JsonRpcId>);

            void ProcessMessage(JsonRpcRequest request, int index)
            {
                if (request == null)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, $"The batch item at position {index} is not a message");
                }

                if (request.Id.Type != JsonRpcIdType.None)
                {
                    if ((requests.Count - index > 1) && (identifiers == null))
                    {
                        identifiers = new HashSet<JsonRpcId>();
                    }
                    if ((identifiers != null) && !identifiers.Add(request.Id))
                    {
                        throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, $"The batch contains messages with the same identifier: \"{request.Id}\"");
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
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "JSON serialization error", e);
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
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "JSON serialization error", response.Id, e);
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
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, $"The batch item at position {index} is not a message");
                }

                if (response.Id.Type != JsonRpcIdType.None)
                {
                    if ((responses.Count - index > 1) && (identifiers == null))
                    {
                        identifiers = new HashSet<JsonRpcId>();
                    }
                    if ((identifiers != null) && !identifiers.Add(response.Id))
                    {
                        throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, $"The batch contains messages with the same identifier: \"{response.Id}\"");
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
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "JSON serialization error", e);
            }
        }

        private JsonRpcRequest ConvertTokenToRequest(JObject jsonObject)
        {
            if (_scheme == null)
            {
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "The type scheme is not defined");
            }
            if (!jsonObject.TryGetValue("jsonrpc", out var jsonTokenProtocol))
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The request does not have the protocol property");
            }
            if (jsonTokenProtocol.Type != JTokenType.String)
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The request has the protocol property with invalid type");
            }
            if (!object.Equals(jsonTokenProtocol, _protocolVersionToken))
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The request has an invalid protocol version");
            }

            var request = new JsonRpcRequest();

            if (jsonObject.TryGetValue("id", out var jsonValueId))
            {
                switch (jsonValueId.Type)
                {
                    case JTokenType.Integer:
                        {
                            request.Id = (long)jsonValueId;
                        }
                        break;
                    case JTokenType.String:
                        {
                            request.Id = (string)jsonValueId;
                        }
                        break;
                    default:
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The request has the identifier property with invalid type");
                        }
                }
            }
            
            if (!jsonObject.TryGetValue("method", out var jsonValueMethod))
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The request does not have the method property", request.Id);
            }
            if (jsonValueMethod.Type != JTokenType.String)
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The request has the method property with invalid type", request.Id);
            }

            request.Method = (string)jsonValueMethod;

            if (request.Method.Length == 0)
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The request has an empty method name", request.Id);
            }
            if (!_scheme.Methods.TryGetValue(request.Method, out var methodScheme))
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMethod, $"The request method \"{request.Method}\" is not supported", request.Id);
            }
            if (methodScheme == null)
            {
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, $"Invalid type binding for parameters' object of the \"{request.Method}\" method", request.Id);
            }
            if (methodScheme.IsNotification && !request.IsNotification)
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, $"The request is not a notification", request.Id);
            }
            if (!methodScheme.IsNotification && request.IsNotification)
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, $"The request is a notification", request.Id);
            }
            if ((methodScheme.ParametersType != null) && jsonObject.TryGetValue("params", out var jsonValueParams) && (jsonValueParams.Type != JTokenType.Null))
            {
                try
                {
                    request.Params = jsonValueParams.ToObject(methodScheme.ParametersType, _jsonSerializer);
                }
                catch (JsonException e)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, "JSON deserialization error", request.Id, e);
                }
            }

            return request;
        }

        private JObject ConvertRequestToToken(JsonRpcRequest request)
        {
            var jsonObject = new JObject
            {
                { "jsonrpc", _protocolVersionToken },
                { "method", request.Method }
            };

            if (request.Params != null)
            {
                var jsonTokenParams = default(JToken);

                try
                {
                    jsonTokenParams = JToken.FromObject(request.Params, _jsonSerializer);
                }
                catch (JsonException e)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, "JSON serialization error", request.Id, e);
                }

                if (jsonTokenParams.Type != JTokenType.Object && jsonTokenParams.Type != JTokenType.Array)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The request has the parameters' property with invalid type", request.Id);
                }

                jsonObject.Add("params", jsonTokenParams);
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

        private JsonRpcResponse ConvertTokenToResponse(JObject jsonObject, IReadOnlyDictionary<JsonRpcId, string> bindings)
        {
            if (_scheme == null)
            {
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "The type scheme is not defined");
            }
            if (!jsonObject.TryGetValue("jsonrpc", out var jsonTokenProtocol))
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The response does not have the protocol property");
            }
            if (jsonTokenProtocol.Type != JTokenType.String)
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The response has the protocol property with invalid type");
            }
            if (!object.Equals(jsonTokenProtocol, _protocolVersionToken))
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The response has an invalid protocol version");
            }

            var response = new JsonRpcResponse();

            if (jsonObject.TryGetValue("id", out var jsonValueId))
            {
                switch (jsonValueId.Type)
                {
                    case JTokenType.Integer:
                        {
                            response.Id = (long)jsonValueId;
                        }
                        break;
                    case JTokenType.String:
                        {
                            response.Id = (string)jsonValueId;
                        }
                        break;
                }
            }

            var jsonTokenResult = jsonObject.GetValue("result");
            var jsonTokenError = jsonObject.GetValue("error");

            if ((jsonTokenResult == null) && (jsonTokenError == null))
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The response has neither result nor error properties", response.Id);
            }
            if ((jsonTokenResult != null) && (jsonTokenError != null))
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The response has the result and error properties simultaneously", response.Id);
            }

            if (jsonTokenResult != null)
            {
                if (!bindings.TryGetValue(response.Id, out var messageMethod))
                {
                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, $"There is no method binding for the response with the \"{response.Id}\" identifier", response.Id);
                }
                if (messageMethod == null)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, $"Invalid method binding for the response with the \"{response.Id}\" identifier", response.Id);
                }
                if (!_scheme.Methods.TryGetValue(messageMethod, out var methodScheme))
                {
                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, $"There is no type binding for the result's object of the \"{messageMethod}\" method", response.Id);
                }
                if (methodScheme == null)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, $"Invalid type binding for result's object of the \"{messageMethod}\" method", response.Id);
                }

                try
                {
                    response.Result = jsonTokenResult.ToObject(methodScheme.ResultType, _jsonSerializer);
                }
                catch (JsonException e)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, "JSON deserialization error", response.Id, e);
                }
            }
            else
            {
                response.Error = new JsonRpcError();

                if (jsonTokenError.Type != JTokenType.Object)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The response has the error property with invalid type", response.Id);
                }

                var jsonObjectError = (JObject)jsonTokenError;

                if (!jsonObjectError.TryGetValue("code", out var jsonTokenErrorCode))
                {
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The response does not have the error code property", response.Id);
                }
                if (jsonTokenErrorCode.Type != JTokenType.Integer)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The response has the error code property with invalid type", response.Id);
                }

                response.Error.Code = (long)jsonTokenErrorCode;

                if (!jsonObjectError.TryGetValue("message", out var jsonTokenErrorMessage))
                {
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The response does not have the error message property", response.Id);
                }
                if (jsonTokenErrorMessage.Type != JTokenType.String)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "The response has the error message property with invalid type", response.Id);
                }

                response.Error.Message = (string)jsonTokenErrorMessage;

                if (jsonObjectError.TryGetValue("data", out var jsonTokenErrorData) && (jsonTokenErrorData.Type != JTokenType.Null))
                {
                    if (response.Id.Type == JsonRpcIdType.None)
                    {
                        if (_scheme.GenericErrorDataType == null)
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.GenericError, "There is no type binding for the generic error data object", response.Id);
                        }

                        try
                        {
                            response.Error.Data = jsonTokenErrorData.ToObject(_scheme.GenericErrorDataType, _jsonSerializer);
                        }
                        catch (JsonException e)
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.GenericError, "JSON deserialization error", response.Id, e);
                        }
                    }
                    else
                    {
                        if (!bindings.TryGetValue(response.Id, out var messageMethod))
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.GenericError, $"There is no method binding for the response with the \"{response.Id}\" identifier", response.Id);
                        }
                        if (messageMethod == null)
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.GenericError, $"Invalid method binding for the response with the \"{response.Id}\" identifier", response.Id);
                        }
                        if (!_scheme.Methods.TryGetValue(messageMethod, out var methodScheme))
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.GenericError, $"There is no type binding for the error data object of the \"{messageMethod}\" method", response.Id);
                        }
                        if (methodScheme == null)
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.GenericError, $"Invalid type binding for the error data object of the \"{messageMethod}\" method", response.Id);
                        }

                        try
                        {
                            response.Error.Data = jsonTokenErrorData.ToObject(methodScheme.ErrorDataType, _jsonSerializer);
                        }
                        catch (JsonException e)
                        {
                            throw new JsonRpcException(JsonRpcExceptionType.GenericError, "JSON deserialization error", response.Id, e);
                        }
                    }
                }
            }

            return response;
        }

        private JObject ConvertResponseToToken(JsonRpcResponse response)
        {
            var jsonObject = new JObject
            {
                { "jsonrpc", _protocolVersionToken }
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
                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, "JSON serialization error", response.Id, e);
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
                        throw new JsonRpcException(JsonRpcExceptionType.GenericError, "JSON serialization error", response.Id, e);
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