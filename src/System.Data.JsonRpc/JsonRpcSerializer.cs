using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace System.Data.JsonRpc
{
    /// <summary>Serializes and deserializes JSON-RPC 2.0 messages into and from the JSON format.</summary>
    public sealed class JsonRpcSerializer
    {
        private static readonly JsonSerializer _defaultJsonSerializer = JsonSerializer.CreateDefault();
        private static readonly JsonRpcDataInfo<JsonRpcResponse> _emptyResponseJsonRpcDataInfo = new JsonRpcDataInfo<JsonRpcResponse>();
        private static readonly JValue _protocolVersionToken = new JValue("2.0");
        private readonly JsonConverter[] _jsonConverters;
        private readonly JsonSerializer _jsonSerializer;
        private readonly IArrayPool<char> _jsonSerializerArrayPool;
        private readonly JsonRpcSchema _schema;

        /// <summary>Initializes a new instance of the <see cref="JsonRpcSerializer" /> class.</summary>
        public JsonRpcSerializer()
        {
            _jsonSerializer = _defaultJsonSerializer;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcSerializer" /> class.</summary>
        /// <param name="schema">The type schema for deserialization.</param>
        /// <exception cref="ArgumentNullException"><paramref name="schema" /> is <see langword="null" />.</exception>
        public JsonRpcSerializer(JsonRpcSchema schema)
        {
            if (schema == null)
                throw new ArgumentNullException(nameof(schema));

            _schema = schema.Clone();
            _jsonSerializer = _defaultJsonSerializer;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcSerializer" /> class.</summary>
        /// <param name="schema">The type schema for deserialization.</param>
        /// <param name="settings">The settings for serialization and deserialization.</param>
        /// <exception cref="ArgumentNullException"><paramref name="schema" /> or <paramref name="settings" /> is <see langword="null" />.</exception>
        public JsonRpcSerializer(JsonRpcSchema schema, JsonRpcSettings settings)
        {
            if (schema == null)
                throw new ArgumentNullException(nameof(schema));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _schema = schema.Clone();
            _jsonSerializer = settings.JsonSerializer;
            _jsonSerializerArrayPool = settings.JsonSerializerArrayPool;

            if (_jsonSerializer != null)
            {
                _jsonConverters = new JsonConverter[_jsonSerializer.Converters.Count];
                _jsonSerializer.Converters.CopyTo(_jsonConverters, 0);
            }
            else
                _jsonSerializer = _defaultJsonSerializer;
        }

        /// <summary>Deserializes the JSON string to the requests information.</summary>
        /// <param name="jsonString">The JSON string to deserialize.</param>
        /// <returns>RPC information about requests.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="jsonString" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during message processing.</exception>
        public JsonRpcDataInfo<JsonRpcRequest> DeserializeRequestsData(string jsonString)
        {
            if (jsonString == null)
                throw new ArgumentNullException(nameof(jsonString));

            var jsonToken = ConvertStringToToken(jsonString);

            switch (jsonToken.Type)
            {
                case JTokenType.Object:
                    {
                        var jsonObject = (JObject)jsonToken;
                        var item = default(JsonRpcMessageInfo<JsonRpcRequest>);

                        try
                        {
                            var request = ConvertTokenToRequest(jsonObject);

                            item = new JsonRpcMessageInfo<JsonRpcRequest>(request);
                        }
                        catch (JsonRpcException e)
                            when (e.Type != JsonRpcExceptionType.GenericError)
                        {
                            item = new JsonRpcMessageInfo<JsonRpcRequest>(e);
                        }

                        return new JsonRpcDataInfo<JsonRpcRequest>(item);
                    }
                case JTokenType.Array:
                    {
                        var jsonArray = (JArray)jsonToken;

                        if (jsonArray.Count == 0)
                            throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Batch is empty");

                        var items = new JsonRpcMessageInfo<JsonRpcRequest>[jsonArray.Count];
                        var idsNumber = default(HashSet<long>);
                        var idsString = default(HashSet<string>);

                        for (var i = 0; i < jsonArray.Count; i++)
                        {
                            var jsonObject = jsonArray[i];

                            if (jsonObject.Type != JTokenType.Object)
                            {
                                var exception = new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Item is not a message");

                                items[i] = new JsonRpcMessageInfo<JsonRpcRequest>(exception);

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
                                items[i] = new JsonRpcMessageInfo<JsonRpcRequest>(e);

                                continue;
                            }

                            switch (request.IdType)
                            {
                                case JsonRpcIdType.Number:
                                    {
                                        if ((jsonArray.Count - i > 1) && (idsNumber == null))
                                            idsNumber = new HashSet<long>();
                                        if ((idsNumber != null) && !idsNumber.Add(request.IdNumber))
                                        {
                                            throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, string.Format(CultureInfo.InvariantCulture,
                                                "Batch contains messages with the same identifier \"{0}\"", request.IdNumber));
                                        }
                                    }
                                    break;
                                case JsonRpcIdType.String:
                                    {
                                        if ((jsonArray.Count - i > 1) && (idsString == null))
                                            idsString = new HashSet<string>();
                                        if ((idsString != null) && !idsString.Add(request.IdString))
                                        {
                                            throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, string.Format(CultureInfo.InvariantCulture,
                                                "Batch contains messages with the same identifier \"{0}\"", request.IdString));
                                        }
                                    }
                                    break;
                            }

                            items[i] = new JsonRpcMessageInfo<JsonRpcRequest>(request);
                        }

                        return new JsonRpcDataInfo<JsonRpcRequest>(items);
                    }
                default:
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Invalid message");
            }
        }

        /// <summary>Deserializes the JSON string to the responses information.</summary>
        /// <param name="jsonString">The JSON string to deserialize.</param>
        /// <param name="bindingsProvider">Request identifier to method name bindings provider used to map response properties to the corresponding types.</param>
        /// <returns>RPC information about responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="jsonString" /> or <paramref name="bindingsProvider" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during message processing.</exception>
        public JsonRpcDataInfo<JsonRpcResponse> DeserializeResponsesData(string jsonString, IJsonRpcBindingsProvider bindingsProvider)
        {
            if (jsonString == null)
                throw new ArgumentNullException(nameof(jsonString));
            if (bindingsProvider == null)
                throw new ArgumentNullException(nameof(bindingsProvider));

            // Empty string is a valid case for an empty response.

            if (jsonString.Length == 0)
                return _emptyResponseJsonRpcDataInfo;

            var jsonToken = ConvertStringToToken(jsonString);

            switch (jsonToken.Type)
            {
                case JTokenType.Object:
                    {
                        var jsonObject = (JObject)jsonToken;
                        var item = default(JsonRpcMessageInfo<JsonRpcResponse>);

                        try
                        {
                            var response = ConvertTokenToResponse(jsonObject, bindingsProvider);

                            item = new JsonRpcMessageInfo<JsonRpcResponse>(response);
                        }
                        catch (JsonRpcException e)
                            when (e.Type != JsonRpcExceptionType.GenericError)
                        {
                            item = new JsonRpcMessageInfo<JsonRpcResponse>(e);
                        }

                        return new JsonRpcDataInfo<JsonRpcResponse>(item);
                    }
                case JTokenType.Array:
                    {
                        var jsonArray = (JArray)jsonToken;

                        if (jsonArray.Count == 0)
                            throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Batch is empty");

                        var items = new JsonRpcMessageInfo<JsonRpcResponse>[jsonArray.Count];
                        var idsNumber = default(HashSet<long>);
                        var idsString = default(HashSet<string>);

                        for (var i = 0; i < jsonArray.Count; i++)
                        {
                            var jsonObject = jsonArray[i];

                            if (jsonObject.Type != JTokenType.Object)
                            {
                                var exception = new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Item is not a message");

                                items[i] = new JsonRpcMessageInfo<JsonRpcResponse>(exception);

                                continue;
                            }

                            var response = default(JsonRpcResponse);

                            try
                            {
                                response = ConvertTokenToResponse((JObject)jsonObject, bindingsProvider);
                            }
                            catch (JsonRpcException e)
                                when (e.Type != JsonRpcExceptionType.GenericError)
                            {
                                items[i] = new JsonRpcMessageInfo<JsonRpcResponse>(e);

                                continue;
                            }

                            switch (response.IdType)
                            {
                                case JsonRpcIdType.Number:
                                    {
                                        if ((jsonArray.Count - i > 1) && (idsNumber == null))
                                            idsNumber = new HashSet<long>();
                                        if ((idsNumber != null) && !idsNumber.Add(response.IdNumber))
                                        {
                                            throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, string.Format(CultureInfo.InvariantCulture,
                                                "Batch contains messages with the same identifier \"{0}\"", response.IdNumber));
                                        }
                                    }
                                    break;
                                case JsonRpcIdType.String:
                                    {
                                        if ((jsonArray.Count - i > 1) && (idsString == null))
                                            idsString = new HashSet<string>();
                                        if ((idsString != null) && !idsString.Add(response.IdString))
                                        {
                                            throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, string.Format(CultureInfo.InvariantCulture,
                                                "Batch contains messages with the same identifier \"{0}\"", response.IdString));
                                        }
                                    }
                                    break;
                            }

                            items[i] = new JsonRpcMessageInfo<JsonRpcResponse>(response);
                        }

                        return new JsonRpcDataInfo<JsonRpcResponse>(items);
                    }
                default:
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Invalid message");
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
                throw new ArgumentNullException(nameof(request));

            var jsonObject = ConvertRequestToToken(request);
            var jsonString = ConvertTokenToString(jsonObject);

            return jsonString;
        }

        /// <summary>Serializes the specified collection of requests to a JSON string.</summary>
        /// <param name="requests">The specified collection of requests to serialize.</param>
        /// <returns>A JSON string representation of the specified collection of requests.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="requests" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during message processing.</exception>
        public string SerializeRequests(IReadOnlyCollection<JsonRpcRequest> requests)
        {
            if (requests == null)
                throw new ArgumentNullException(nameof(requests));

            // Empty collection is an invalid case.

            if (requests.Count == 0)
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "Batch is empty");

            var jsonArray = new JArray();
            var idsNumber = default(HashSet<long>);
            var idsString = default(HashSet<string>);

            void ProcessMessage(JsonRpcRequest request, int index)
            {
                if (request == null)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                        "Batch item at position {0} is not a message", index));
                }

                switch (request.IdType)
                {
                    case JsonRpcIdType.Number:
                        {
                            if ((requests.Count - index > 1) && (idsNumber == null))
                                idsNumber = new HashSet<long>();
                            if ((idsNumber != null) && !idsNumber.Add(request.IdNumber))
                            {
                                throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                                    "Batch contains messages with the same identifier \"{0}\"", request.IdNumber));
                            }
                        }
                        break;
                    case JsonRpcIdType.String:
                        {
                            if ((requests.Count - index > 1) && (idsString == null))
                                idsString = new HashSet<string>();
                            if ((idsString != null) && !idsString.Add(request.IdString))
                            {
                                throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                                    "Batch contains messages with the same identifier \"{0}\"", request.IdString));
                            }
                        }
                        break;
                }

                var jsonObject = ConvertRequestToToken(request);

                jsonArray.Add(jsonObject);
            }

            if (requests is IReadOnlyList<JsonRpcRequest> messagesList)
            {
                for (var i = 0; i < messagesList.Count; i++)
                    ProcessMessage(messagesList[i], i);
            }
            else
            {
                var i = 0;

                foreach (var request in requests)
                {
                    ProcessMessage(request, i);
                    i++;
                }
            }

            var jsonString = ConvertTokenToString(jsonArray);

            return jsonString;
        }

        /// <summary>Serializes the specified response to a JSON string.</summary>
        /// <param name="response">The specified response to serialize.</param>
        /// <returns>A JSON string representation of the specified response.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="response" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during message processing.</exception>
        public string SerializeResponse(JsonRpcResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            var jsonObject = ConvertResponseToToken(response);
            var jsonString = ConvertTokenToString(jsonObject);

            return jsonString;
        }

        /// <summary>Serializes the specified collection of responses to a JSON string.</summary>
        /// <param name="responses">The specified collection of responses to serialize.</param>
        /// <returns>A JSON string representation of the specified collection of responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="responses" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during message processing.</exception>
        public string SerializeResponses(IReadOnlyCollection<JsonRpcResponse> responses)
        {
            if (responses == null)
                throw new ArgumentNullException(nameof(responses));

            // Empty collection is a valid case for an empty response.

            if (responses.Count == 0)
                return string.Empty;

            var jsonArray = new JArray();
            var idsNumber = default(HashSet<long>);
            var idsString = default(HashSet<string>);

            void ProcessMessage(JsonRpcResponse response, int index)
            {
                if (response == null)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                        "Batch item at position {0} is not a message", index));
                }

                switch (response.IdType)
                {
                    case JsonRpcIdType.Number:
                        {
                            if ((responses.Count - index > 1) && (idsNumber == null))
                                idsNumber = new HashSet<long>();
                            if ((idsNumber != null) && !idsNumber.Add(response.IdNumber))
                            {
                                throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                                    "Batch contains messages with the same identifier \"{0}\"", response.IdNumber));
                            }
                        }
                        break;
                    case JsonRpcIdType.String:
                        {
                            if ((responses.Count - index > 1) && (idsString == null))
                                idsString = new HashSet<string>();
                            if ((idsString != null) && !idsString.Add(response.IdString))
                            {
                                throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                                    "Batch contains messages with the same identifier \"{0}\"", response.IdString));
                            }
                        }
                        break;
                }

                var jsonObject = ConvertResponseToToken(response);

                jsonArray.Add(jsonObject);
            }

            if (responses is IReadOnlyList<JsonRpcResponse> messagesList)
            {
                for (var i = 0; i < messagesList.Count; i++)
                    ProcessMessage(messagesList[i], i);
            }
            else
            {
                var i = 0;

                foreach (var response in responses)
                {
                    ProcessMessage(response, i);
                    i++;
                }
            }

            var jsonString = ConvertTokenToString(jsonArray);

            return jsonString;
        }

        private JsonRpcRequest ConvertTokenToRequest(JObject jsonObject)
        {
            if (_schema == null)
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "Type schema is not defined");

            if (!jsonObject.TryGetValue("jsonrpc", out var jsonTokenProtocol))
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Request does not have protocol property");
            if (jsonTokenProtocol.Type != JTokenType.String)
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Request has protocol property with invalid type");
            if (!object.Equals(jsonTokenProtocol, _protocolVersionToken))
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Request has invalid protocol version");

            var request = new JsonRpcRequest();

            if (!jsonObject.TryGetValue("method", out var jsonValueMethod))
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Request does not have method property");
            if (jsonValueMethod.Type != JTokenType.String)
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Request has method property with invalid type");

            request.Method = (string)jsonValueMethod;

            if (request.Method.Length == 0)
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Request has empty method name");
            if (!_schema.SupportedMethods.Contains(request.Method))
            {
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMethod, string.Format(CultureInfo.InvariantCulture,
                    "Request method \"{0}\" is not supported", request.Method));
            }

            if (jsonObject.TryGetValue("id", out var jsonValueId))
            {
                switch (jsonValueId.Type)
                {
                    case JTokenType.Integer:
                        {
                            request.IdNumber = (long)jsonValueId;
                            request.IdType = JsonRpcIdType.Number;
                        }
                        break;
                    case JTokenType.String:
                        {
                            request.IdString = (string)jsonValueId;
                            request.IdType = JsonRpcIdType.String;
                        }
                        break;
                    default:
                        throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Request has identifier property with invalid type");
                }
            }

            if (jsonObject.TryGetValue("params", out var jsonValueParams) && (jsonValueParams.Type != JTokenType.Null))
            {
                if (!_schema.ParameterTypeBindings.TryGetValue(request.Method, out var parametersType))
                {
                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                        "There is no type binding for parameters object of \"{0}\" method", request.Method));
                }
                if (parametersType == null)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                        "Invalid type binding for parameters object of \"{0}\" method", request.Method));
                }

                request.Params = ConvertTokenToObject(jsonValueParams, parametersType);
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
                var jsonTokenParams = ConvertObjectToToken(request.Params);

                // Type of "params" can be checked only after conversion of an object to JSON tokens due to possible custom converters.

                if (jsonTokenParams.Type != JTokenType.Object && jsonTokenParams.Type != JTokenType.Array)
                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, "Request has parameters property with invalid type");

                jsonObject.Add("params", jsonTokenParams);
            }

            switch (request.IdType)
            {
                case JsonRpcIdType.Number:
                    jsonObject.Add("id", new JValue(request.IdNumber));
                    break;
                case JsonRpcIdType.String:
                    jsonObject.Add("id", new JValue(request.IdString));
                    break;
            }

            return jsonObject;
        }

        private JsonRpcResponse ConvertTokenToResponse(JObject jsonObject, IJsonRpcBindingsProvider bindingsProvider)
        {
            if (_schema == null)
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "Type schema is not defined");

            if (!jsonObject.TryGetValue("jsonrpc", out var jsonTokenProtocol))
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Response does not have protocol property");
            if (jsonTokenProtocol.Type != JTokenType.String)
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Response has protocol property with invalid type");
            if (!object.Equals(jsonTokenProtocol, _protocolVersionToken))
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Response has invalid protocol version");

            var response = new JsonRpcResponse();

            if (jsonObject.TryGetValue("id", out var jsonValueId))
            {
                switch (jsonValueId.Type)
                {
                    case JTokenType.Null:
                        {
                            response.IdType = JsonRpcIdType.Null;
                        }
                        break;
                    case JTokenType.Integer:
                        {
                            response.IdNumber = (long)jsonValueId;
                            response.IdType = JsonRpcIdType.Number;
                        }
                        break;
                    case JTokenType.String:
                        {
                            response.IdString = (string)jsonValueId;
                            response.IdType = JsonRpcIdType.String;
                        }
                        break;
                }
            }

            var jsonTokenResult = jsonObject.GetValue("result");
            var jsonTokenError = jsonObject.GetValue("error");

            if ((jsonTokenResult == null) && (jsonTokenError == null))
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Response has neither result nor error properties");
            if ((jsonTokenResult != null) && (jsonTokenError != null))
                throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Response has result and error properties simultaneously");

            if (jsonTokenResult != null)
            {
                var messageMethod = default(string);

                switch (response.IdType)
                {
                    case JsonRpcIdType.Number:
                        {
                            if (!bindingsProvider.TryGetBinding(response.IdNumber, out messageMethod))
                            {
                                throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                                    "There is no method binding for response with identifier \"{0}\"", response.IdNumber));
                            }
                            if (messageMethod == null)
                            {
                                throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                                    "Invalid method binding for response with identifier \"{0}\"", response.IdNumber));
                            }
                        }
                        break;
                    case JsonRpcIdType.String:
                        {
                            if (!bindingsProvider.TryGetBinding(response.IdString, out messageMethod))
                            {
                                throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                                    "There is no method binding for response with identifier \"{0}\"", response.IdString));
                            }
                            if (messageMethod == null)
                            {
                                throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                                    "Invalid method binding for response with identifier \"{0}\"", response.IdString));
                            }
                        }
                        break;
                }

                if (!_schema.ResultTypeBindings.TryGetValue(messageMethod, out var resultType))
                {
                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                        "There is no type binding for result object of \"{0}\" method", messageMethod));
                }
                if (resultType == null)
                {
                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                        "Invalid type binding for result object of \"{0}\" method", messageMethod));
                }

                response.Result = ConvertTokenToObject(jsonTokenResult, resultType);
            }
            else
            {
                response.Error = new JsonRpcError();

                if (jsonTokenError.Type != JTokenType.Object)
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Response has error property with invalid type");

                var jsonObjectError = (JObject)jsonTokenError;

                if (!jsonObjectError.TryGetValue("code", out var jsonTokenErrorCode))
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Response does not have error code property");
                if (jsonTokenErrorCode.Type != JTokenType.Integer)
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Response has error code property with invalid type");

                response.Error.Code = (long)jsonTokenErrorCode;

                if (!jsonObjectError.TryGetValue("message", out var jsonTokenErrorMessage))
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Response does not have error message property");
                if (jsonTokenErrorMessage.Type != JTokenType.String)
                    throw new JsonRpcException(JsonRpcExceptionType.InvalidMessage, "Response has error message property with invalid type");

                response.Error.Message = (string)jsonTokenErrorMessage;

                if (jsonObjectError.TryGetValue("data", out var jsonTokenErrorData) && (jsonTokenErrorData.Type != JTokenType.Null))
                {
                    var messageMethod = default(string);

                    switch (response.IdType)
                    {
                        case JsonRpcIdType.Null:
                            {
                                if (_schema.ErrorDataTypeGeneric == null)
                                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, "There is no type binding for generic error data object");

                                response.Error.Data = ConvertTokenToObject(jsonTokenErrorData, _schema.ErrorDataTypeGeneric);
                            }
                            break;
                        case JsonRpcIdType.Number:
                            {
                                if (!bindingsProvider.TryGetBinding(response.IdNumber, out messageMethod))
                                {
                                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                                        "There is no method binding for response with identifier \"{0}\"", response.IdNumber));
                                }
                                if (messageMethod == null)
                                {
                                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                                        "Invalid method binding for response with identifier \"{0}\"", response.IdNumber));
                                }
                                if (!_schema.ErrorDataTypeBindings.TryGetValue(messageMethod, out var dataType))
                                {
                                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                                        "There is no type binding for error data object of \"{0}\" method", messageMethod));
                                }
                                if (dataType == null)
                                {
                                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                                        "Invalid type binding for error data object of \"{0}\" method", messageMethod));
                                }

                                response.Error.Data = ConvertTokenToObject(jsonTokenErrorData, dataType);
                            }
                            break;
                        case JsonRpcIdType.String:
                            {
                                if (!bindingsProvider.TryGetBinding(response.IdString, out messageMethod))
                                {
                                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                                        "There is no method binding for response with identifier \"{0}\"", response.IdString));
                                }
                                if (messageMethod == null)
                                {
                                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                                        "Invalid method binding for response with identifier \"{0}\"", response.IdString));
                                }
                                if (!_schema.ErrorDataTypeBindings.TryGetValue(messageMethod, out var dataType))
                                {
                                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                                        "There is no type binding for error data object of \"{0}\" method", messageMethod));
                                }
                                if (dataType == null)
                                {
                                    throw new JsonRpcException(JsonRpcExceptionType.GenericError, string.Format(CultureInfo.InvariantCulture,
                                        "Invalid type binding for error data object of \"{0}\" method", messageMethod));
                                }

                                response.Error.Data = ConvertTokenToObject(jsonTokenErrorData, dataType);
                            }
                            break;
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
                jsonObject.Add("result", ConvertObjectToToken(response.Result));
            }
            else
            {
                var errorToken = new JObject
                {
                    { "code", response.Error.Code },
                    { "message", response.Error.Message }
                };

                if (response.Error.Data != null)
                    errorToken.Add("data", ConvertObjectToToken(response.Error.Data));

                jsonObject.Add("error", errorToken);
            }

            // Property "id" should be included in an object for all possible values

            switch (response.IdType)
            {
                case JsonRpcIdType.Null:
                    jsonObject.Add("id", JValue.CreateNull());
                    break;
                case JsonRpcIdType.Number:
                    jsonObject.Add("id", new JValue(response.IdNumber));
                    break;
                case JsonRpcIdType.String:
                    jsonObject.Add("id", new JValue(response.IdString));
                    break;
            }

            return jsonObject;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JToken ConvertStringToToken(string jsonString)
        {
            try
            {
                using (var jsonTextReader = new JsonTextReader(new StringReader(jsonString)))
                {
                    if (_jsonSerializerArrayPool != null)
                        jsonTextReader.ArrayPool = _jsonSerializerArrayPool;

                    var jsonToken = JToken.ReadFrom(jsonTextReader);

                    return jsonToken;
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcExceptionType.ParseError, "JSON string parse error", e);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object ConvertTokenToObject(JToken jsonToken, Type objectType)
        {
            try
            {
                var messageObject = jsonToken.ToObject(objectType, _jsonSerializer);

                return messageObject;
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "CLR object construct error", e);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JToken ConvertObjectToToken(object messageObject)
        {
            try
            {
                var jsonToken = JToken.FromObject(messageObject, _jsonSerializer);

                return jsonToken;
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "CLR object deconstruct error", e);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ConvertTokenToString(JToken jsonToken)
        {
            try
            {
                var jsonString = jsonToken.ToString(_jsonSerializer.Formatting, _jsonConverters);

                return jsonString;
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "JSON string compose error", e);
            }
        }
    }
}