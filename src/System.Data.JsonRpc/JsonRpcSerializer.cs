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
        private const int _messageBufferSize = 32;

        private static readonly JValue _nullToken = JValue.CreateNull();

        private readonly IArrayPool<char> _jsonBufferPool = new JsonBufferPool();
        private readonly IDictionary<string, JsonRpcRequestContract> _requestContracts;
        private readonly IDictionary<string, JsonRpcResponseContract> _responseContracts;
        private readonly IDictionary<JsonRpcId, string> _staticResponseBindings;
        private readonly IDictionary<JsonRpcId, JsonRpcResponseContract> _dynamicResponseBindings;

        /// <summary>Initializes a new instance of the <see cref="JsonRpcSerializer" /> class.</summary>
        /// <param name="requestContracts">The request contracts.</param>
        /// <param name="responseContracts">The response contracts.</param>
        /// <param name="staticResponseBindings">The static response bindings.</param>
        /// <param name="dynamicResponseBindings">The dynamic response bindings.</param>
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
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>RPC information about requests.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="json" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during request(s) deserialization.</exception>
        public JsonRpcData<JsonRpcRequest> DeserializeRequestData(string json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var requestDataToken = default(JToken);

            try
            {
                using (var stringReader = new StringReader(json))
                {
                    using (var jsonReader = new JsonTextReader(stringReader))
                    {
                        jsonReader.ArrayPool = _jsonBufferPool;
                        requestDataToken = JToken.ReadFrom(jsonReader);
                    }
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidJson, Strings.GetString("core.deserialize.json_issue"), default, e);
            }

            switch (requestDataToken.Type)
            {
                case JTokenType.Object:
                    {
                        var requestToken = (JObject)requestDataToken;
                        var requestItem = default(JsonRpcItem<JsonRpcRequest>);

                        try
                        {
                            requestItem = new JsonRpcItem<JsonRpcRequest>(ConvertTokenToRequest(requestToken));
                        }
                        catch (JsonRpcException e)
                            when (e.ErrorCode != JsonRpcErrorCodes.InvalidOperation)
                        {
                            requestItem = new JsonRpcItem<JsonRpcRequest>(e);
                        }

                        return new JsonRpcData<JsonRpcRequest>(requestItem);
                    }
                case JTokenType.Array:
                    {
                        var requestArrayToken = (JArray)requestDataToken;

                        if (requestArrayToken.Count == 0)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.batch.empty"));
                        }

                        var requestItems = new JsonRpcItem<JsonRpcRequest>[requestArrayToken.Count];

                        for (var i = 0; i < requestItems.Length; i++)
                        {
                            var requestToken = requestArrayToken[i];

                            if (requestToken.Type != JTokenType.Object)
                            {
                                var exception = new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.batch.invalid_item"), i));

                                requestItems[i] = new JsonRpcItem<JsonRpcRequest>(exception);

                                continue;
                            }

                            var request = default(JsonRpcRequest);

                            try
                            {
                                request = ConvertTokenToRequest((JObject)requestToken);
                            }
                            catch (JsonRpcException e)
                                when (e.ErrorCode != JsonRpcErrorCodes.InvalidOperation)
                            {
                                requestItems[i] = new JsonRpcItem<JsonRpcRequest>(e);

                                continue;
                            }

                            requestItems[i] = new JsonRpcItem<JsonRpcRequest>(request);
                        }

                        return new JsonRpcData<JsonRpcRequest>(requestItems);
                    }
                default:
                    {
                        throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.input.invalid_structure"));
                    }
            }
        }

        /// <summary>Deserializes the JSON string to the response data.</summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>RPC information about responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="json" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during response(s) deserialization.</exception>
        public JsonRpcData<JsonRpcResponse> DeserializeResponseData(string json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var responseDataToken = default(JToken);

            try
            {
                using (var stringReader = new StringReader(json))
                {
                    using (var jsonReader = new JsonTextReader(stringReader))
                    {
                        jsonReader.ArrayPool = _jsonBufferPool;
                        responseDataToken = JToken.ReadFrom(jsonReader);
                    }
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidJson, Strings.GetString("core.deserialize.json_issue"), default, e);
            }

            switch (responseDataToken.Type)
            {
                case JTokenType.Object:
                    {
                        var responseToken = (JObject)responseDataToken;
                        var responseItem = default(JsonRpcItem<JsonRpcResponse>);

                        try
                        {
                            responseItem = new JsonRpcItem<JsonRpcResponse>(ConvertTokenToResponse(responseToken));
                        }
                        catch (JsonRpcException e)
                            when (e.ErrorCode != JsonRpcErrorCodes.InvalidOperation)
                        {
                            responseItem = new JsonRpcItem<JsonRpcResponse>(e);
                        }

                        return new JsonRpcData<JsonRpcResponse>(responseItem);
                    }
                case JTokenType.Array:
                    {
                        var responseArrayToken = (JArray)responseDataToken;

                        if (responseArrayToken.Count == 0)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.batch.empty"));
                        }

                        var responseItems = new JsonRpcItem<JsonRpcResponse>[responseArrayToken.Count];

                        for (var i = 0; i < responseItems.Length; i++)
                        {
                            var responseToken = responseArrayToken[i];

                            if (responseToken.Type != JTokenType.Object)
                            {
                                var exception = new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.batch.invalid_item"), i));

                                responseItems[i] = new JsonRpcItem<JsonRpcResponse>(exception);

                                continue;
                            }

                            var response = default(JsonRpcResponse);

                            try
                            {
                                response = ConvertTokenToResponse((JObject)responseToken);
                            }
                            catch (JsonRpcException e)
                                when (e.ErrorCode != JsonRpcErrorCodes.InvalidOperation)
                            {
                                responseItems[i] = new JsonRpcItem<JsonRpcResponse>(e);

                                continue;
                            }

                            responseItems[i] = new JsonRpcItem<JsonRpcResponse>(response);
                        }

                        return new JsonRpcData<JsonRpcResponse>(responseItems);
                    }
                default:
                    {
                        throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.input.invalid_structure"));
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

            var requestToken = ConvertRequestToToken(request);

            try
            {
                using (var stringWriter = new StringWriter(new StringBuilder(_messageBufferSize), CultureInfo.InvariantCulture))
                {
                    using (var jsonWriter = new JsonTextWriter(stringWriter))
                    {
                        jsonWriter.ArrayPool = _jsonBufferPool;
                        requestToken.WriteTo(jsonWriter);
                    }

                    return stringWriter.ToString();
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), request.Id, e);
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
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.batch.empty"));
            }

            var requestArrayToken = new JArray();

            for (var i = 0; i < requests.Count; i++)
            {
                if (requests[i] == null)
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.batch.invalid_item"), i));
                }

                requestArrayToken.Add(ConvertRequestToToken(requests[i]));
            }

            try
            {
                using (var stringWriter = new StringWriter(new StringBuilder(_messageBufferSize * requests.Count), CultureInfo.InvariantCulture))
                {
                    using (var jsonWriter = new JsonTextWriter(stringWriter))
                    {
                        jsonWriter.ArrayPool = _jsonBufferPool;
                        requestArrayToken.WriteTo(jsonWriter);
                    }

                    return stringWriter.ToString();
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), default, e);
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

            var responseToken = ConvertResponseToToken(response);

            try
            {
                using (var stringWriter = new StringWriter(new StringBuilder(_messageBufferSize), CultureInfo.InvariantCulture))
                {
                    using (var jsonWriter = new JsonTextWriter(stringWriter))
                    {
                        jsonWriter.ArrayPool = _jsonBufferPool;
                        responseToken.WriteTo(jsonWriter);
                    }

                    return stringWriter.ToString();
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), response.Id, e);
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
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.batch.empty"));
            }

            var responseArrayToken = new JArray();

            for (var i = 0; i < responses.Count; i++)
            {
                if (responses[i] == null)
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.batch.invalid_item"), i));
                }

                responseArrayToken.Add(ConvertResponseToToken(responses[i]));
            }

            try
            {
                using (var stringWriter = new StringWriter(new StringBuilder(_messageBufferSize * responses.Count), CultureInfo.InvariantCulture))
                {
                    using (var jsonWriter = new JsonTextWriter(stringWriter))
                    {
                        jsonWriter.ArrayPool = _jsonBufferPool;
                        responseArrayToken.WriteTo(jsonWriter);
                    }

                    return stringWriter.ToString();
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), default, e);
            }
        }

        private JsonRpcRequest ConvertTokenToRequest(JObject requestToken)
        {
            if (CompatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                if (!requestToken.TryGetValue("jsonrpc", out var protocolToken) || (protocolToken.Type != JTokenType.String) || ((string)protocolToken != "2.0"))
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.request.protocol.invalid_property"));
                }
            }

            var requestId = default(JsonRpcId);

            if (requestToken.TryGetValue("id", out var requestIdToken))
            {
                switch (requestIdToken.Type)
                {
                    case JTokenType.Null:
                        {
                        }
                        break;
                    case JTokenType.String:
                        {
                            requestId = (string)requestIdToken;
                        }
                        break;
                    case JTokenType.Integer:
                        {
                            try
                            {
                                requestId = (long)requestIdToken;
                            }
                            catch (OverflowException e)
                            {
                                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.message.id.large_number"), default, e);
                            }
                        }
                        break;
                    case JTokenType.Float:
                        {
                            try
                            {
                                requestId = (double)requestIdToken;
                            }
                            catch (OverflowException e)
                            {
                                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.message.id.large_number"), default, e);
                            }
                        }
                        break;
                    default:
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.request.id.invalid_property"));
                        }
                }
            }

            if (!requestToken.TryGetValue("method", out var requestMethodToken) || (requestMethodToken.Type != JTokenType.String))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.request.method.invalid_property"), requestId);
            }

            var requestMethod = (string)requestMethodToken;

            if (!_requestContracts.TryGetValue(requestMethod, out var contract))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMethod, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.request.method.unsupported"), requestMethod), requestId);
            }
            if (contract == null)
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.request.method.contract.undefined"), requestMethod), requestId);
            }

            switch (contract.ParametersType)
            {
                case JsonRpcParametersType.ByPosition:
                    {
                        if (!requestToken.TryGetValue("params", out var requestParametersToken) || ((requestParametersToken.Type != JTokenType.Array) && (requestParametersToken.Type != JTokenType.Object)))
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.request.params.invalid_property"), requestId);
                        }
                        if (requestParametersToken.Type != JTokenType.Array)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidParameters, Strings.GetString("core.deserialize.request.params.invalid_structure"), requestId);
                        }

                        var requestParametersArrayToken = (JArray)requestParametersToken;

                        if (requestParametersArrayToken.Count < contract.ParametersByPosition.Count)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidParameters, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.request.params.invalid_count"), requestParametersArrayToken.Count), requestId);
                        }

                        var requestParameters = new object[contract.ParametersByPosition.Count];

                        try
                        {
                            for (var i = 0; i < requestParameters.Length; i++)
                            {
                                requestParameters[i] = requestParametersArrayToken[i].ToObject(contract.ParametersByPosition[i]);
                            }
                        }
                        catch (Exception e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.deserialize.json_issue"), requestId, e);
                        }

                        return new JsonRpcRequest(requestMethod, requestId, requestParameters);
                    }
                case JsonRpcParametersType.ByName:
                    {
                        if (!requestToken.TryGetValue("params", out var requestParametersToken) || ((requestParametersToken.Type != JTokenType.Array) && (requestParametersToken.Type != JTokenType.Object)))
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.request.params.invalid_property"), requestId);
                        }
                        if (requestParametersToken.Type != JTokenType.Object)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidParameters, Strings.GetString("core.deserialize.request.params.invalid_structure"), requestId);
                        }

                        var requestParametersObjectToken = (JObject)requestParametersToken;
                        var requestParameters = new Dictionary<string, object>(contract.ParametersByName.Count, StringComparer.Ordinal);

                        try
                        {
                            foreach (var kvp in contract.ParametersByName)
                            {
                                if (!requestParametersObjectToken.TryGetValue(kvp.Key, StringComparison.Ordinal, out var requestParameterToken))
                                {
                                    continue;
                                }

                                requestParameters[kvp.Key] = requestParameterToken.ToObject(kvp.Value);
                            }
                        }
                        catch (Exception e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.deserialize.json_issue"), requestId, e);
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
            var requestToken = new JObject();

            if (CompatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                requestToken["jsonrpc"] = "2.0";
            }

            requestToken["method"] = request.Method;

            switch (request.ParametersType)
            {
                case JsonRpcParametersType.ByPosition:
                    {
                        var requestParametersArrayToken = new JArray();

                        try
                        {
                            for (var i = 0; i < request.ParametersByPosition.Count; i++)
                            {
                                requestParametersArrayToken.Add(request.ParametersByPosition[i] != null ? JToken.FromObject(request.ParametersByPosition[i]) : _nullToken);
                            }
                        }
                        catch (JsonException e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), request.Id, e);
                        }

                        requestToken["params"] = requestParametersArrayToken;
                    }
                    break;
                case JsonRpcParametersType.ByName:
                    {
                        if (CompatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.request.params.unsupported_structure"), request.Id);
                        }

                        var requestParametersObjectToken = new JObject();

                        try
                        {
                            foreach (var kvp in request.ParametersByName)
                            {
                                requestParametersObjectToken.Add(kvp.Key, kvp.Value != null ? JToken.FromObject(kvp.Value) : _nullToken);
                            }
                        }
                        catch (JsonException e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), request.Id, e);
                        }

                        requestToken["params"] = requestParametersObjectToken;
                    }
                    break;
                case JsonRpcParametersType.None:
                    {
                        if (CompatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                        {
                            requestToken["params"] = new JArray();
                        }
                    }
                    break;
            }

            switch (request.Id.Type)
            {
                case JsonRpcIdType.None:
                    {
                        if (CompatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                        {
                            requestToken["id"] = _nullToken;
                        }
                    }
                    break;
                case JsonRpcIdType.String:
                    {
                        requestToken["id"] = new JValue((string)request.Id);
                    }
                    break;
                case JsonRpcIdType.Integer:
                    {
                        requestToken["id"] = new JValue((long)request.Id);
                    }
                    break;
                case JsonRpcIdType.Float:
                    {
                        requestToken["id"] = new JValue((double)request.Id);
                    }
                    break;
            }

            return requestToken;
        }

        private JsonRpcResponse ConvertTokenToResponse(JObject responseObject)
        {
            if (CompatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                if (!responseObject.TryGetValue("jsonrpc", out var protocolToken) || (protocolToken.Type != JTokenType.String) || ((string)protocolToken != "2.0"))
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.request.protocol.invalid_property"));
                }
            }

            var responseId = default(JsonRpcId);

            if (responseObject.TryGetValue("id", out var responseIdToken))
            {
                switch (responseIdToken.Type)
                {
                    case JTokenType.Null:
                        {
                        }
                        break;
                    case JTokenType.String:
                        {
                            responseId = (string)responseIdToken;
                        }
                        break;
                    case JTokenType.Integer:
                        {
                            try
                            {
                                responseId = (long)responseIdToken;
                            }
                            catch (OverflowException e)
                            {
                                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.message.id.large_number"), default, e);
                            }
                        }
                        break;
                    case JTokenType.Float:
                        {
                            try
                            {
                                responseId = (double)responseIdToken;
                            }
                            catch (OverflowException e)
                            {
                                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.message.id.large_number"), default, e);
                            }
                        }
                        break;
                    default:
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.id.invalid_property"));
                        }
                }
            }

            var responseResultToken = responseObject.GetValue("result");
            var responseErrorToken = responseObject.GetValue("error");
            var responseSuccess = false;

            if (CompatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                if (((responseResultToken == null) && (responseErrorToken == null)) || ((responseResultToken != null) && (responseErrorToken != null)))
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.invalid_properties"), responseId);
                }

                responseSuccess = responseErrorToken == null;
            }
            else
            {
                if ((responseResultToken == null) || (responseErrorToken == null))
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.invalid_properties"), responseId);
                }

                responseSuccess = responseErrorToken.Type == JTokenType.Null;
            }

            if (responseSuccess)
            {
                if (responseId.Type == JsonRpcIdType.None)
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.invalid_properties"), responseId);
                }

                var contract = GetResponseContract(responseId);
                var responseResult = default(object);

                if (contract.ResultType != null)
                {
                    try
                    {
                        responseResult = responseResultToken.ToObject(contract.ResultType);
                    }
                    catch (Exception e)
                    {
                        throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.deserialize.json_issue"), responseId, e);
                    }
                }

                return new JsonRpcResponse(responseResult, responseId);
            }
            else
            {
                if (responseErrorToken.Type == JTokenType.Object)
                {
                    var responseErrorObjectToken = (JObject)responseErrorToken;
                    var responseErrorCode = default(long);

                    if (responseErrorObjectToken.TryGetValue("code", out var responseErrorCodeToken) && (responseErrorCodeToken.Type == JTokenType.Integer))
                    {
                        try
                        {
                            responseErrorCode = (long)responseErrorCodeToken;
                        }
                        catch (OverflowException e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.error.code.large_number"), responseId, e);
                        }
                    }
                    else
                    {
                        if (CompatibilityLevel == JsonRpcCompatibilityLevel.Level2)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.error.code.invalid_property"), responseId);
                        }
                    }

                    var responseErrorMessage = default(string);

                    if (responseErrorObjectToken.TryGetValue("message", out var responseErrorMessageToken) && (responseErrorMessageToken.Type == JTokenType.String))
                    {
                        responseErrorMessage = (string)responseErrorMessageToken;
                    }
                    else
                    {
                        if (CompatibilityLevel == JsonRpcCompatibilityLevel.Level2)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.error.message.invalid_property"), responseId);
                        }
                        else
                        {
                            responseErrorMessage = string.Empty;
                        }
                    }

                    var responseError = default(JsonRpcError);

                    if (responseErrorObjectToken.TryGetValue("data", out var responseErrorDataToken))
                    {
                        var responseErrorDataType = default(Type);

                        if (responseId.Type == JsonRpcIdType.None)
                        {
                            responseErrorDataType = DefaultErrorDataType;
                        }
                        else
                        {
                            var contract = GetResponseContract(responseId);

                            responseErrorDataType = contract.ErrorDataType;
                        }

                        var responseErrorData = default(object);

                        if (responseErrorDataType != null)
                        {
                            try
                            {
                                responseErrorData = responseErrorDataToken.ToObject(responseErrorDataType);
                            }
                            catch (Exception e)
                            {
                                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.deserialize.json_issue"), responseId, e);
                            }
                        }

                        try
                        {
                            responseError = new JsonRpcError(responseErrorCode, responseErrorMessage, responseErrorData);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.error.code.invalid_range"), responseId, e);
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
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.error.code.invalid_range"), responseId, e);
                        }
                    }

                    return new JsonRpcResponse(responseError, responseId);
                }
                else
                {
                    if (CompatibilityLevel == JsonRpcCompatibilityLevel.Level2)
                    {
                        throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.error.invalid_type"), responseId);
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
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.deserialize.response.method.contract.undefined"), identifier);
            }

            return contract;
        }

        private JObject ConvertResponseToToken(JsonRpcResponse response)
        {
            var responseToken = new JObject();

            if (CompatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                responseToken["jsonrpc"] = "2.0";
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
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), response.Id, e);
                }

                responseToken["result"] = resultToken;

                if (CompatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                {
                    responseToken["error"] = _nullToken;
                }
            }
            else
            {
                var responseErrorToken = new JObject
                {
                    ["code"] = response.Error.Code,
                    ["message"] = response.Error.Message
                };

                if (response.Error.HasData)
                {
                    var responseErrorDataToken = default(JToken);

                    try
                    {
                        responseErrorDataToken = response.Error.Data != null ? JToken.FromObject(response.Error.Data) : _nullToken;
                    }
                    catch (JsonException e)
                    {
                        throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), response.Id, e);
                    }

                    responseErrorToken["data"] = responseErrorDataToken;
                }

                if (CompatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                {
                    responseToken["result"] = _nullToken;
                }

                responseToken["error"] = responseErrorToken;
            }

            switch (response.Id.Type)
            {
                case JsonRpcIdType.None:
                    {
                        responseToken["id"] = _nullToken;
                    }
                    break;
                case JsonRpcIdType.String:
                    {
                        responseToken["id"] = new JValue((string)response.Id);
                    }
                    break;
                case JsonRpcIdType.Integer:
                    {
                        responseToken["id"] = new JValue((long)response.Id);
                    }
                    break;
                case JsonRpcIdType.Float:
                    {
                        responseToken["id"] = new JValue((double)response.Id);
                    }
                    break;
            }

            return responseToken;
        }

        /// <summary>Clears response bindings of the current instance.</summary>
        public void Dispose()
        {
            _dynamicResponseBindings.Clear();
            _staticResponseBindings.Clear();
        }

        /// <summary>Gets the request contracts.</summary>
        public IDictionary<string, JsonRpcRequestContract> RequestContracts
        {
            get => _requestContracts;
        }

        /// <summary>Gets the response contracts.</summary>
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