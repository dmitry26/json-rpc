using System.Collections.Generic;
using System.Data.JsonRpc.Resources;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace System.Data.JsonRpc
{
    /// <summary>Serializes and deserializes JSON-RPC messages into and from the JSON format.</summary>
    public sealed partial class JsonRpcSerializer : IDisposable
    {
        // The minimum length of JSON string with a JSON-RPC 2.0 message is about 32 bytes, this is used during deserialization
        // to get the more proper initial buffer size.

        private const int _minimumMessageSize = 32;

        // The stream reading and writing buffer size is the same as in the BCL.

        private const int _streamBufferSize = 1024;

        private static readonly Encoding _streamEncoding = new UTF8Encoding(false);

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

        /// <summary>Deserializes the specified JSON string to request data.</summary>
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

            using (var stringReader = new StringReader(json))
            {
                return DeserializeRequestData(stringReader);
            }
        }

        /// <summary>Deserializes the specified stream with a JSON string to request data.</summary>
        /// <param name="stream">The stream with a JSON string to deserialize.</param>
        /// <returns>RPC information about requests.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during request(s) deserialization.</exception>
        public JsonRpcData<JsonRpcRequest> DeserializeRequestData(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var streamReader = new StreamReader(stream, _streamEncoding, false, _streamBufferSize, true))
            {
                return DeserializeRequestData(streamReader);
            }
        }

        private JsonRpcData<JsonRpcRequest> DeserializeRequestData(TextReader textReader)
        {
            var requestDataToken = default(JToken);

            try
            {
                using (var jsonReader = new JsonTextReader(textReader))
                {
                    jsonReader.ArrayPool = _jsonBufferPool;
                    requestDataToken = JToken.ReadFrom(jsonReader);
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

        /// <summary>Deserializes the specified JSON string to response data.</summary>
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

            using (var stringReader = new StringReader(json))
            {
                return DeserializeResponseData(stringReader);
            }
        }

        /// <summary>Deserializes the specified stream with a JSON string to response data.</summary>
        /// <param name="stream">The stream with a JSON string to deserialize.</param>
        /// <returns>RPC information about responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during response(s) deserialization.</exception>
        public JsonRpcData<JsonRpcResponse> DeserializeResponseData(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var streamReader = new StreamReader(stream, _streamEncoding, false, _streamBufferSize, true))
            {
                return DeserializeResponseData(streamReader);
            }
        }

        private JsonRpcData<JsonRpcResponse> DeserializeResponseData(TextReader textReader)
        {
            var responseDataToken = default(JToken);

            try
            {
                using (var jsonReader = new JsonTextReader(textReader))
                {
                    jsonReader.ArrayPool = _jsonBufferPool;
                    responseDataToken = JToken.ReadFrom(jsonReader);
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
        /// <param name="request">The request to serialize.</param>
        /// <returns>A JSON string representation of the specified request.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during request serialization.</exception>
        public string SerializeRequest(JsonRpcRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using (var stringWriter = new StringWriter(new StringBuilder(_minimumMessageSize), CultureInfo.InvariantCulture))
            {
                SerializeRequest(request, stringWriter);

                return stringWriter.ToString();
            }
        }

        /// <summary>Serializes the specified request to the specified stream.</summary>
        /// <param name="request">The request to serialize.</param>
        /// <param name="stream">The stream for a JSON string.</param>
        /// <returns>A JSON string representation of the specified request.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request" /> or <paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during request serialization.</exception>
        public void SerializeRequest(JsonRpcRequest request, Stream stream)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var streamWriter = new StreamWriter(stream, _streamEncoding, _streamBufferSize, true))
            {
                SerializeRequest(request, streamWriter);
            }
        }

        private void SerializeRequest(JsonRpcRequest request, TextWriter textWriter)
        {
            var requestToken = ConvertRequestToToken(request);

            try
            {
                using (var jsonWriter = new JsonTextWriter(textWriter))
                {
                    jsonWriter.ArrayPool = _jsonBufferPool;
                    requestToken.WriteTo(jsonWriter);
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), request.Id, e);
            }
        }

        /// <summary>Serializes the specified collection of requests to a JSON string.</summary>
        /// <param name="requests">The collection of requests to serialize.</param>
        /// <returns>A JSON string representation of the specified collection of requests.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="requests" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during requests serialization.</exception>
        public string SerializeRequests(IReadOnlyList<JsonRpcRequest> requests)
        {
            if (requests == null)
            {
                throw new ArgumentNullException(nameof(requests));
            }

            using (var stringWriter = new StringWriter(new StringBuilder(_minimumMessageSize * requests.Count), CultureInfo.InvariantCulture))
            {
                SerializeRequests(requests, stringWriter);

                return stringWriter.ToString();
            }
        }

        /// <summary>Serializes the specified collection of requests to the specified stream.</summary>
        /// <param name="requests">The collection of requests to serialize.</param>
        /// <param name="stream">The stream for a JSON string.</param>
        /// <returns>A JSON string representation of the specified collection of requests.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="requests" /> or <paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during requests serialization.</exception>
        public void SerializeRequests(IReadOnlyList<JsonRpcRequest> requests, Stream stream)
        {
            if (requests == null)
            {
                throw new ArgumentNullException(nameof(requests));
            }
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var streamWriter = new StreamWriter(stream, _streamEncoding, _streamBufferSize, true))
            {
                SerializeRequests(requests, streamWriter);
            }
        }

        private void SerializeRequests(IReadOnlyList<JsonRpcRequest> requests, TextWriter textWriter)
        {
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
                using (var jsonWriter = new JsonTextWriter(textWriter))
                {
                    jsonWriter.ArrayPool = _jsonBufferPool;
                    requestArrayToken.WriteTo(jsonWriter);
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), default, e);
            }
        }

        /// <summary>Serializes the specified response to a JSON string.</summary>
        /// <param name="response">The response to serialize.</param>
        /// <returns>A JSON string representation of the specified response.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="response" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during response serialization.</exception>
        public string SerializeResponse(JsonRpcResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            using (var stringWriter = new StringWriter(new StringBuilder(_minimumMessageSize), CultureInfo.InvariantCulture))
            {
                SerializeResponse(response, stringWriter);

                return stringWriter.ToString();
            }
        }

        /// <summary>Serializes the specified response to the specified stream.</summary>
        /// <param name="response">The response to serialize.</param>
        /// <param name="stream">The stream for a JSON string.</param>
        /// <returns>A JSON string representation of the specified response.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="response" /> or <paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during response serialization.</exception>
        public void SerializeResponse(JsonRpcResponse response, Stream stream)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var streamWriter = new StreamWriter(stream, _streamEncoding, _streamBufferSize, true))
            {
                SerializeResponse(response, streamWriter);
            }
        }

        private void SerializeResponse(JsonRpcResponse response, TextWriter textWriter)
        {
            var responseToken = ConvertResponseToToken(response);

            try
            {
                using (var jsonWriter = new JsonTextWriter(textWriter))
                {
                    jsonWriter.ArrayPool = _jsonBufferPool;
                    responseToken.WriteTo(jsonWriter);
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), response.Id, e);
            }
        }

        /// <summary>Serializes the specified collection of responses to a JSON string.</summary>
        /// <param name="responses">The collection of responses to serialize.</param>
        /// <returns>A JSON string representation of the specified collection of responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="responses" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during responses serialization.</exception>
        public string SerializeResponses(IReadOnlyList<JsonRpcResponse> responses)
        {
            if (responses == null)
            {
                throw new ArgumentNullException(nameof(responses));
            }

            using (var stringWriter = new StringWriter(new StringBuilder(_minimumMessageSize * responses.Count), CultureInfo.InvariantCulture))
            {
                SerializeResponses(responses, stringWriter);

                return stringWriter.ToString();
            }
        }

        /// <summary>Serializes the specified collection of responses to the specified stream.</summary>
        /// <param name="responses">The collection of responses to serialize.</param>
        /// <param name="stream">The stream for a JSON string.</param>
        /// <returns>A JSON string representation of the specified collection of responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="responses" /> or <paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during responses serialization.</exception>
        public void SerializeResponses(IReadOnlyList<JsonRpcResponse> responses, Stream stream)
        {
            if (responses == null)
            {
                throw new ArgumentNullException(nameof(responses));
            }
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var streamWriter = new StreamWriter(stream, _streamEncoding, _streamBufferSize, true))
            {
                SerializeResponses(responses, streamWriter);
            }
        }

        private void SerializeResponses(IReadOnlyList<JsonRpcResponse> responses, TextWriter textWriter)
        {
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
                using (var jsonWriter = new JsonTextWriter(textWriter))
                {
                    jsonWriter.ArrayPool = _jsonBufferPool;
                    responseArrayToken.WriteTo(jsonWriter);
                }
            }
            catch (JsonException e)
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), default, e);
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