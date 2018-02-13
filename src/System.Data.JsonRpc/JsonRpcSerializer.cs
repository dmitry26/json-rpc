using System.Collections.Generic;
using System.Data.JsonRpc.Internal;
using System.Data.JsonRpc.Resources;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace System.Data.JsonRpc
{
    /// <summary>Serializes and deserializes JSON-RPC messages into and from the JSON format.</summary>
    public sealed class JsonRpcSerializer : IDisposable
    {
        private readonly IArrayPool<char> _jsonBufferPool = new JsonBufferPool();
        private readonly IDictionary<string, JsonRpcRequestContract> _requestContracts;
        private readonly IDictionary<string, JsonRpcResponseContract> _responseContracts;
        private readonly IDictionary<JsonRpcId, string> _staticResponseBindings;
        private readonly IDictionary<JsonRpcId, JsonRpcResponseContract> _dynamicResponseBindings;

        /// <summary>Initializes a new instance of the <see cref="JsonRpcSerializer" /> class.</summary>
        /// <param name="requestContracts">The request contracts scheme instance.</param>
        /// <param name="responseContracts">The response contracts scheme instance.</param>
        /// <param name="staticResponseBindings">The static response bindings instance.</param>
        /// <param name="dynamicResponseBindings">The dynamic response bindings instance.</param>
        public JsonRpcSerializer(
            IDictionary<string, JsonRpcRequestContract> requestContracts = null,
            IDictionary<string, JsonRpcResponseContract> responseContracts = null,
            IDictionary<JsonRpcId, string> staticResponseBindings = null,
            IDictionary<JsonRpcId, JsonRpcResponseContract> dynamicResponseBindings = null)
        {
            _requestContracts = requestContracts ?? new Dictionary<string, JsonRpcRequestContract>(StringComparer.Ordinal);
            _responseContracts = responseContracts ?? new Dictionary<string, JsonRpcResponseContract>(StringComparer.Ordinal);
            _staticResponseBindings = staticResponseBindings ?? new Dictionary<JsonRpcId, string>();
            _dynamicResponseBindings = dynamicResponseBindings ?? new Dictionary<JsonRpcId, JsonRpcResponseContract>();

            CompatibilityLevel = JsonRpcCompatibilityLevel.Level2;
        }

        /// <summary>Deserializes the JSON string to the request data.</summary>
        /// <param name="jsonString">The JSON string to deserialize.</param>
        /// <returns>RPC information about requests.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="jsonString" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during request(s) deserialization.</exception>
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
                throw new JsonRpcException(JsonRpcErrorCode.InvalidJson, Strings.GetString("core.deserialize.json_issue"), default, e);
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
                            when (e.ErrorCode != JsonRpcErrorCode.InvalidOperation)
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
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.batch.empty"));
                        }

                        var items = new JsonRpcItem<JsonRpcRequest>[jsonArray.Count];

                        for (var i = 0; i < items.Length; i++)
                        {
                            var jsonObject = jsonArray[i];

                            if (jsonObject.Type != JTokenType.Object)
                            {
                                var exception = new JsonRpcException(JsonRpcErrorCode.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.batch.invalid_item"), i));

                                items[i] = new JsonRpcItem<JsonRpcRequest>(exception);

                                continue;
                            }

                            var request = default(JsonRpcRequest);

                            try
                            {
                                request = ConvertTokenToRequest((JObject)jsonObject);
                            }
                            catch (JsonRpcException e)
                                when (e.ErrorCode != JsonRpcErrorCode.InvalidOperation)
                            {
                                items[i] = new JsonRpcItem<JsonRpcRequest>(e);

                                continue;
                            }

                            items[i] = new JsonRpcItem<JsonRpcRequest>(request);
                        }

                        return new JsonRpcData<JsonRpcRequest>(items);
                    }
                default:
                    {
                        throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.input.invalid_structure"));
                    }
            }
        }

        /// <summary>Deserializes the JSON string to the response data.</summary>
        /// <param name="jsonString">The JSON string to deserialize.</param>
        /// <returns>RPC information about responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="jsonString" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during response(s) deserialization.</exception>
        public JsonRpcData<JsonRpcResponse> DeserializeResponseData(string jsonString)
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
                throw new JsonRpcException(JsonRpcErrorCode.InvalidJson, Strings.GetString("core.deserialize.json_issue"), default, e);
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
                            when (e.ErrorCode != JsonRpcErrorCode.InvalidOperation)
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
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.batch.empty"));
                        }

                        var items = new JsonRpcItem<JsonRpcResponse>[jsonArray.Count];

                        for (var i = 0; i < items.Length; i++)
                        {
                            var jsonObject = jsonArray[i];

                            if (jsonObject.Type != JTokenType.Object)
                            {
                                var exception = new JsonRpcException(JsonRpcErrorCode.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.batch.invalid_item"), i));

                                items[i] = new JsonRpcItem<JsonRpcResponse>(exception);

                                continue;
                            }

                            var response = default(JsonRpcResponse);

                            try
                            {
                                response = ConvertTokenToResponse((JObject)jsonObject);
                            }
                            catch (JsonRpcException e)
                                when (e.ErrorCode != JsonRpcErrorCode.InvalidOperation)
                            {
                                items[i] = new JsonRpcItem<JsonRpcResponse>(e);

                                continue;
                            }

                            items[i] = new JsonRpcItem<JsonRpcResponse>(response);
                        }

                        return new JsonRpcData<JsonRpcResponse>(items);
                    }
                default:
                    {
                        throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.input.invalid_structure"));
                    }
            }
        }

        /// <summary>Serializes the specified request to a JSON string.</summary>
        /// <param name="request">The specified request to serialize.</param>
        /// <returns>A JSON string representation of the specified request.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during request serialization.</exception>
        public string SerializeRequest(JsonRpcRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                var jsonToken = ConvertRequestToToken(request);

                using (var stringWriter = new StringWriter(new StringBuilder(32), CultureInfo.InvariantCulture))
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
                throw new JsonRpcException(JsonRpcErrorCode.InvalidOperation, Strings.GetString("core.serialize.json_issue"), request.Id, e);
            }
        }

        /// <summary>Serializes the specified collection of requests to a JSON string.</summary>
        /// <param name="requests">The specified collection of requests to serialize.</param>
        /// <returns>A JSON string representation of the specified collection of requests.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="requests" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during requests serialization.</exception>
        public string SerializeRequests(IReadOnlyList<JsonRpcRequest> requests)
        {
            if (requests == null)
            {
                throw new ArgumentNullException(nameof(requests));
            }
            if (requests.Count == 0)
            {
                throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.batch.empty"));
            }

            var jsonArray = new JArray();

            for (var i = 0; i < requests.Count; i++)
            {
                if (requests[i] == null)
                {
                    throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.batch.invalid_item"), i));
                }

                jsonArray.Add(ConvertRequestToToken(requests[i]));
            }

            try
            {
                using (var stringWriter = new StringWriter(new StringBuilder(32 * requests.Count), CultureInfo.InvariantCulture))
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
                throw new JsonRpcException(JsonRpcErrorCode.InvalidOperation, Strings.GetString("core.serialize.json_issue"), default, e);
            }
        }

        /// <summary>Serializes the specified response to a JSON string.</summary>
        /// <param name="response">The specified response to serialize.</param>
        /// <returns>A JSON string representation of the specified response.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="response" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during response serialization.</exception>
        public string SerializeResponse(JsonRpcResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            try
            {
                var jsonToken = ConvertResponseToToken(response);

                using (var stringWriter = new StringWriter(new StringBuilder(32), CultureInfo.InvariantCulture))
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
                throw new JsonRpcException(JsonRpcErrorCode.InvalidOperation, Strings.GetString("core.serialize.json_issue"), response.Id, e);
            }
        }

        /// <summary>Serializes the specified collection of responses to a JSON string.</summary>
        /// <param name="responses">The specified collection of responses to serialize.</param>
        /// <returns>A JSON string representation of the specified collection of responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="responses" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during responses serialization.</exception>
        public string SerializeResponses(IReadOnlyList<JsonRpcResponse> responses)
        {
            if (responses == null)
            {
                throw new ArgumentNullException(nameof(responses));
            }
            if (responses.Count == 0)
            {
                throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.batch.empty"));
            }

            var jsonArray = new JArray();

            for (var i = 0; i < responses.Count; i++)
            {
                if (responses[i] == null)
                {
                    throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.batch.invalid_item"), i));
                }

                jsonArray.Add(ConvertResponseToToken(responses[i]));
            }

            try
            {
                using (var stringWriter = new StringWriter(new StringBuilder(32 * responses.Count), CultureInfo.InvariantCulture))
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
                throw new JsonRpcException(JsonRpcErrorCode.InvalidOperation, Strings.GetString("core.serialize.json_issue"), default, e);
            }
        }

        private JsonRpcRequest ConvertTokenToRequest(JObject jsonObject)
        {
            if (CompatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                if (!jsonObject.TryGetValue("jsonrpc", out var jsonTokenProtocol) || (jsonTokenProtocol.Type != JTokenType.String) || ((string)jsonTokenProtocol != "2.0"))
                {
                    throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.request.protocol.invalid_property"));
                }
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
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.request.id.invalid_property"));
                        }
                }
            }

            if (!jsonObject.TryGetValue("method", out var jsonValueMethod) || (jsonValueMethod.Type != JTokenType.String))
            {
                throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.request.method.invalid_property"), requestId);
            }

            var requestMethod = (string)jsonValueMethod;

            if (!_requestContracts.TryGetValue(requestMethod, out var contract))
            {
                throw new JsonRpcException(JsonRpcErrorCode.InvalidMethod, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.request.method.unsupported"), requestMethod), requestId);
            }
            if (contract == null)
            {
                throw new JsonRpcException(JsonRpcErrorCode.InvalidOperation, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.request.method.contract.undefined"), requestMethod), requestId);
            }

            switch (contract.ParametersType)
            {
                case JsonRpcParametersType.ByPosition:
                    {
                        if (!jsonObject.TryGetValue("params", out var jsonTokenParameters) || ((jsonTokenParameters.Type != JTokenType.Array) && (jsonTokenParameters.Type != JTokenType.Object)))
                        {
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.request.params.invalid_property"), requestId);
                        }
                        if (jsonTokenParameters.Type != JTokenType.Array)
                        {
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidParameters, Strings.GetString("core.deserialize.request.params.invalid_structure"), requestId);
                        }

                        var jsonArrayParameters = (JArray)jsonTokenParameters;

                        if (jsonArrayParameters.Count < contract.ParametersByPosition.Count)
                        {
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidParameters, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.request.params.invalid_count"), jsonArrayParameters.Count), requestId);
                        }

                        var requestParameters = new object[contract.ParametersByPosition.Count];

                        try
                        {
                            for (var i = 0; i < requestParameters.Length; i++)
                            {
                                requestParameters[i] = jsonArrayParameters[i].ToObject(contract.ParametersByPosition[i]);
                            }
                        }
                        catch (JsonException e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidOperation, Strings.GetString("core.deserialize.json_issue"), requestId, e);
                        }

                        return new JsonRpcRequest(requestMethod, requestId, requestParameters);
                    }
                case JsonRpcParametersType.ByName:
                    {
                        if (!jsonObject.TryGetValue("params", out var jsonTokenParameters) || ((jsonTokenParameters.Type != JTokenType.Array) && (jsonTokenParameters.Type != JTokenType.Object)))
                        {
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.request.params.invalid_property"), requestId);
                        }
                        if (jsonTokenParameters.Type != JTokenType.Object)
                        {
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidParameters, Strings.GetString("core.deserialize.request.params.invalid_structure"), requestId);
                        }

                        var jsonObjectParameters = (JObject)jsonTokenParameters;
                        var requestParameters = new Dictionary<string, object>(contract.ParametersByName.Count, StringComparer.Ordinal);

                        try
                        {
                            foreach (var kvp in contract.ParametersByName)
                            {
                                if (!jsonObjectParameters.TryGetValue(kvp.Key, StringComparison.Ordinal, out var jsonObjectParam))
                                {
                                    continue;
                                }

                                requestParameters[kvp.Key] = jsonObjectParam.ToObject(kvp.Value);
                            }
                        }
                        catch (JsonException e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidOperation, Strings.GetString("core.deserialize.json_issue"), requestId, e);
                        }

                        return new JsonRpcRequest(requestMethod, requestId, requestParameters);
                    }
                default:
                    {
                        return new JsonRpcRequest(requestMethod, requestId);
                    }
            }
        }

        private JObject ConvertRequestToToken(JsonRpcRequest request)
        {
            var jsonObject = new JObject();

            if (CompatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                jsonObject["jsonrpc"] = "2.0";
            }

            jsonObject["method"] = request.Method;

            switch (request.ParametersType)
            {
                case JsonRpcParametersType.ByPosition:
                    {
                        var jsonTokenParameters = new JArray();

                        try
                        {
                            for (var i = 0; i < request.ParametersByPosition.Count; i++)
                            {
                                jsonTokenParameters.Add(request.ParametersByPosition[i] != null ? JToken.FromObject(request.ParametersByPosition[i]) : JValue.CreateNull());
                            }
                        }
                        catch (JsonException e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidOperation, Strings.GetString("core.serialize.json_issue"), request.Id, e);
                        }

                        jsonObject["params"] = jsonTokenParameters;
                    }
                    break;
                case JsonRpcParametersType.ByName:
                    {
                        if (CompatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                        {
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidOperation, Strings.GetString("core.serialize.request.params.unsupported_structure"), request.Id);
                        }

                        var jsonTokenParameters = new JObject();

                        try
                        {
                            foreach (var kvp in request.ParametersByName)
                            {
                                jsonTokenParameters.Add(kvp.Key, kvp.Value != null ? JToken.FromObject(kvp.Value) : JValue.CreateNull());
                            }
                        }
                        catch (JsonException e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidOperation, Strings.GetString("core.serialize.json_issue"), request.Id, e);
                        }

                        jsonObject["params"] = jsonTokenParameters;
                    }
                    break;
                case JsonRpcParametersType.None:
                    {
                        if (CompatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                        {
                            jsonObject["params"] = new JArray();
                        }
                    }
                    break;
            }

            switch (request.Id.Type)
            {
                case JsonRpcIdType.Number:
                    {
                        jsonObject["id"] = new JValue((long)request.Id);
                    }
                    break;
                case JsonRpcIdType.String:
                    {
                        jsonObject["id"] = new JValue((string)request.Id);
                    }
                    break;
                case JsonRpcIdType.None:
                    {
                        if (CompatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                        {
                            jsonObject["id"] = JValue.CreateNull();
                        }
                    }
                    break;
            }

            return jsonObject;
        }

        private JsonRpcResponse ConvertTokenToResponse(JObject jsonObject)
        {
            if (CompatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                if (!jsonObject.TryGetValue("jsonrpc", out var jsonTokenProtocol) || (jsonTokenProtocol.Type != JTokenType.String) || ((string)jsonTokenProtocol != "2.0"))
                {
                    throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.request.protocol.invalid_property"));
                }
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
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.response.id.invalid_property"));
                        }
                }
            }

            var jsonTokenResult = jsonObject.GetValue("result");
            var jsonTokenError = jsonObject.GetValue("error");

            var responseSuccess = false;

            if (CompatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                if (((jsonTokenResult == null) && (jsonTokenError == null)) || ((jsonTokenResult != null) && (jsonTokenError != null)))
                {
                    throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.response.invalid_properties"), responseId);
                }

                responseSuccess = jsonTokenError == null;
            }
            else
            {
                if ((jsonTokenResult == null) || (jsonTokenError == null))
                {
                    throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.response.invalid_properties"), responseId);
                }

                responseSuccess = jsonTokenError.Type == JTokenType.Null;
            }

            if (responseSuccess)
            {
                if (responseId.Type == JsonRpcIdType.None)
                {
                    throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.response.invalid_properties"), responseId);
                }

                var contract = GetResponseContract(responseId);
                var responseResult = default(object);

                if (contract.ResultType != null)
                {
                    try
                    {
                        responseResult = jsonTokenResult.ToObject(contract.ResultType);
                    }
                    catch (JsonException e)
                    {
                        throw new JsonRpcException(JsonRpcErrorCode.InvalidOperation, Strings.GetString("core.deserialize.json_issue"), responseId, e);
                    }
                }

                return new JsonRpcResponse(responseResult, responseId);
            }
            else
            {
                if (jsonTokenError.Type == JTokenType.Object)
                {
                    var jsonObjectError = (JObject)jsonTokenError;

                    var responseErrorCode = default(long);

                    if (jsonObjectError.TryGetValue("code", out var jsonTokenErrorCode) && (jsonTokenErrorCode.Type == JTokenType.Integer))
                    {
                        responseErrorCode = (long)jsonTokenErrorCode;
                    }
                    else
                    {
                        if (CompatibilityLevel == JsonRpcCompatibilityLevel.Level2)
                        {
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.response.error.code.invalid_property"), responseId);
                        }
                    }

                    var responseErrorMessage = default(string);

                    if (jsonObjectError.TryGetValue("message", out var jsonTokenErrorMessage) && (jsonTokenErrorMessage.Type == JTokenType.String))
                    {
                        responseErrorMessage = (string)jsonTokenErrorMessage;
                    }
                    else
                    {
                        if (CompatibilityLevel == JsonRpcCompatibilityLevel.Level2)
                        {
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.response.error.message.invalid_property"), responseId);
                        }
                        else
                        {
                            responseErrorMessage = string.Empty;
                        }
                    }

                    var responseError = default(JsonRpcError);

                    if (jsonObjectError.TryGetValue("data", out var jsonTokenErrorData))
                    {
                        var errorDataType = default(Type);

                        if (responseId.Type == JsonRpcIdType.None)
                        {
                            errorDataType = DefaultErrorDataType;
                        }
                        else
                        {
                            var contract = GetResponseContract(responseId);

                            errorDataType = contract.ErrorDataType;
                        }

                        var responseErrorData = default(object);

                        if (errorDataType != null)
                        {
                            try
                            {
                                responseErrorData = jsonTokenErrorData.ToObject(errorDataType);
                            }
                            catch (JsonException e)
                            {
                                throw new JsonRpcException(JsonRpcErrorCode.InvalidOperation, Strings.GetString("core.deserialize.json_issue"), responseId, e);
                            }
                        }

                        try
                        {
                            responseError = new JsonRpcError(responseErrorCode, responseErrorMessage, responseErrorData);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.response.error.code.invalid_range"), responseId, e);
                        }
                    }
                    else
                    {
                        try
                        {
                            responseError = new JsonRpcError(responseErrorCode, responseErrorMessage);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.response.error.code.invalid_range"), responseId, e);
                        }
                    }

                    return new JsonRpcResponse(responseError, responseId);
                }
                else
                {
                    if (CompatibilityLevel == JsonRpcCompatibilityLevel.Level2)
                    {
                        throw new JsonRpcException(JsonRpcErrorCode.InvalidMessage, Strings.GetString("core.deserialize.response.error.invalid_type"), responseId);
                    }
                    else
                    {
                        return new JsonRpcResponse(new JsonRpcError(default, string.Empty), responseId);
                    }
                }
            }
        }

        private JsonRpcResponseContract GetResponseContract(in JsonRpcId identifier)
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
                throw new JsonRpcException(JsonRpcErrorCode.InvalidOperation, Strings.GetString("core.deserialize.response.method.contract.undefined"), identifier);
            }

            return contract;
        }

        private JObject ConvertResponseToToken(JsonRpcResponse response)
        {
            var jsonObject = new JObject();

            if (CompatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                jsonObject["jsonrpc"] = "2.0";
            }

            if (response.Success)
            {
                var resultToken = default(JToken);

                try
                {
                    resultToken = JToken.FromObject(response.Result);
                }
                catch (JsonException e)
                {
                    throw new JsonRpcException(JsonRpcErrorCode.InvalidOperation, Strings.GetString("core.serialize.json_issue"), response.Id, e);
                }

                jsonObject["result"] = resultToken;

                if (CompatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                {
                    jsonObject["error"] = JValue.CreateNull();
                }
            }
            else
            {
                var errorToken = new JObject
                {
                    ["code"] = response.Error.Code,
                    ["message"] = response.Error.Message
                };

                if (response.Error.HasData)
                {
                    var responseErrorDataToken = default(JToken);

                    try
                    {
                        responseErrorDataToken = JToken.FromObject(response.Error.Data);
                    }
                    catch (JsonException e)
                    {
                        throw new JsonRpcException(JsonRpcErrorCode.InvalidOperation, Strings.GetString("core.serialize.json_issue"), response.Id, e);
                    }

                    errorToken["data"] = responseErrorDataToken;
                }

                if (CompatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                {
                    jsonObject["result"] = JValue.CreateNull();
                }

                jsonObject["error"] = errorToken;
            }

            switch (response.Id.Type)
            {
                case JsonRpcIdType.None:
                    {
                        jsonObject["id"] = JValue.CreateNull();
                    }
                    break;
                case JsonRpcIdType.Number:
                    {
                        jsonObject["id"] = new JValue((long)response.Id);
                    }
                    break;
                case JsonRpcIdType.String:
                    {
                        jsonObject["id"] = new JValue((string)response.Id);
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

        /// <summary>Gets the request contracts scheme.</summary>
        public IDictionary<string, JsonRpcRequestContract> RequestContracts
        {
            get => _requestContracts;
        }

        /// <summary>Gets the response contracts scheme.</summary>
        public IDictionary<string, JsonRpcResponseContract> ResponseContracts
        {
            get => _responseContracts;
        }

        /// <summary>Gets the dynamic response bindings.</summary>
        public IDictionary<JsonRpcId, JsonRpcResponseContract> DynamicResponseBindings
        {
            get => _dynamicResponseBindings;
        }

        /// <summary>Gets the static response bindings.</summary>
        public IDictionary<JsonRpcId, string> StaticResponseBindings
        {
            get => _staticResponseBindings;
        }

        /// <summary>Gets or sets a type of error data for deserializing an unsuccessful response with empty identifier.</summary>
        public Type DefaultErrorDataType
        {
            get;
            set;
        }

        /// <summary>Gets or sets the protocol compatibility level.</summary>
        public JsonRpcCompatibilityLevel CompatibilityLevel
        {
            get;
            set;
        }
    }
}