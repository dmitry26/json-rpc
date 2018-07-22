// Copyright (c) DMO Consulting LLC. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.JsonRpc.Resources;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utf8Json;
using Utf8Json.Resolvers;

namespace System.Data.JsonRpc
{
    /// <summary>Serializes and deserializes JSON-RPC messages into and from the JSON format.</summary>
    public class Utf8JsonRpcSerializer : IJsonRpcSerializer, IDisposable 
    {
        /// <summary>Initializes a new instance of the <see cref="JsonRpcSerializer" /> class.</summary>
        /// <param name="requestContracts">The request contracts.</param>
        /// <param name="responseContracts">The response contracts.</param>
        /// <param name="staticResponseBindings">The static response bindings.</param>
        /// <param name="dynamicResponseBindings">The dynamic response bindings.</param>
        public Utf8JsonRpcSerializer(
            IDictionary<string,JsonRpcRequestContract> requestContracts = null,
            IDictionary<string,JsonRpcResponseContract> responseContracts = null,
            IDictionary<JsonRpcId,string> staticResponseBindings = null,
            IDictionary<JsonRpcId,JsonRpcResponseContract> dynamicResponseBindings = null)
        {
            RequestContracts = requestContracts ?? new Dictionary<string,JsonRpcRequestContract>(StringComparer.Ordinal);
            ResponseContracts = responseContracts ?? new Dictionary<string,JsonRpcResponseContract>(StringComparer.Ordinal);
            StaticResponseBindings = staticResponseBindings ?? new Dictionary<JsonRpcId,string>();
            DynamicResponseBindings = dynamicResponseBindings ?? new Dictionary<JsonRpcId,JsonRpcResponseContract>();
            _compatibilityLevel = JsonRpcCompatibilityLevel.Level2;
        }

        /// <summary>Deserializes the specified JSON string to request data.</summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>RPC information about requests.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="json" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during request(s) deserialization.</exception>
        public JsonRpcData<JsonRpcRequest> DeserializeRequestData(string json)
        {
            _ = json ?? throw new ArgumentNullException(nameof(json));

            if (json == string.Empty)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidJson,Strings.GetString("core.deserialize.json_issue"),default,
                    new ArgumentException("'json' is empty"));

            try
            {
                var items = JsonSerializer.Deserialize<IReadOnlyList<JsonRpcItem<JsonRpcRequest>>>(json,JsonRpcRequestResolver);

                if (items == null || items.Count == 0)
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.batch.empty"));

                return ValidateRequestItems(items);
            }            
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidJson,Strings.GetString("core.deserialize.json_issue"),default,x);
            }
        }

        /// <summary>Deserializes the specified stream with a JSON string to request data.</summary>
        /// <param name="stream">The stream with a JSON string to deserialize.</param>
        /// <returns>RPC information about requests.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during request(s) deserialization.</exception>
        public JsonRpcData<JsonRpcRequest> DeserializeRequestData(Stream stream)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            if (stream == Stream.Null)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidJson,Strings.GetString("core.deserialize.json_issue"),default,
                    new InvalidOperationException("stream is Stream.Null"));

            try
            {
                var items = JsonSerializer.Deserialize<IReadOnlyList<JsonRpcItem<JsonRpcRequest>>>(stream,JsonRpcRequestResolver);

                if (items.Count == 0)
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.batch.empty"));

                return ValidateRequestItems(items);
            }           
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidJson,Strings.GetString("core.deserialize.json_issue"),default,x);
            }
        }

        /// <summary>Deserializes the specified stream with a JSON string to request data as an asynchronous operation.</summary>
        /// <param name="stream">The stream with a JSON string to deserialize.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is RPC information about requests.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during request(s) deserialization.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<JsonRpcData<JsonRpcRequest>> DeserializeRequestDataAsync(Stream stream,CancellationToken cancellationToken = default)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            if (stream == Stream.Null)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidJson,Strings.GetString("core.deserialize.json_issue"),default,
                    new InvalidOperationException("stream is Stream.Null"));

            try
            {
                var items = await JsonSerializer.DeserializeAsync<IReadOnlyList<JsonRpcItem<JsonRpcRequest>>>(stream,JsonRpcRequestResolver,cancellationToken);

                if (items.Count == 0)
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.batch.empty"));

                return ValidateRequestItems(items);
            }
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidJson,Strings.GetString("core.deserialize.json_issue"),default,x);
            }
        }

        private JsonRpcData<JsonRpcRequest> ValidateRequestItems(IReadOnlyList<JsonRpcItem<JsonRpcRequest>> items)
        {
            if (items is JsonRpcItem<JsonRpcRequest>[] && items.Count == 1)
            {
                ref readonly var item = ref ((JsonRpcItem<JsonRpcRequest>[])items)[0];
                return new JsonRpcData<JsonRpcRequest>(in item);                
            }
            
            return new JsonRpcData<JsonRpcRequest>(items);
        }

        /// <summary>Deserializes the specified JSON string to response data.</summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>RPC information about responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="json" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during response(s) deserialization.</exception>
        public JsonRpcData<JsonRpcResponse> DeserializeResponseData(string json)
        {
            _ = json ?? throw new ArgumentNullException(nameof(json));

            if (json == string.Empty)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidJson,Strings.GetString("core.deserialize.json_issue"),default,
                    new ArgumentException("'json' is empty"));

            try
            {
                var items = JsonSerializer.Deserialize<IReadOnlyList<JsonRpcItem<JsonRpcResponse>>>(json,JsonRpcResponseResolver);

                if (items.Count == 0)
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.batch.empty"));

                return ValidateResponseItems(items);
            }           
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidJson,Strings.GetString("core.deserialize.json_issue"),default,x);
            }
        }

        /// <summary>Deserializes the specified stream with a JSON string to response data.</summary>
        /// <param name="stream">The stream with a JSON string to deserialize.</param>
        /// <returns>RPC information about responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during response(s) deserialization.</exception>
        public JsonRpcData<JsonRpcResponse> DeserializeResponseData(Stream stream)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            if (stream == Stream.Null)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidJson,Strings.GetString("core.deserialize.json_issue"),default,
                    new InvalidOperationException("stream is Stream.Null"));

            try
            {
                var items = JsonSerializer.Deserialize<IReadOnlyList<JsonRpcItem<JsonRpcResponse>>>(stream,JsonRpcResponseResolver);

                if (items.Count == 0)
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.batch.empty"));

                return ValidateResponseItems(items);
            }            
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidJson,Strings.GetString("core.deserialize.json_issue"),default,x);
            }
        }

        /// <summary>Deserializes the specified stream with a JSON string to response data as an asynchronous operation.</summary>
        /// <param name="stream">The stream with a JSON string to deserialize.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is RPC information about responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during response(s) deserialization.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<JsonRpcData<JsonRpcResponse>> DeserializeResponseDataAsync(Stream stream,CancellationToken cancellationToken = default)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            if (stream == Stream.Null)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidJson,Strings.GetString("core.deserialize.json_issue"),default,
                    new InvalidOperationException("stream is Stream.Null"));

            try
            {
                var items = await JsonSerializer.DeserializeAsync<IReadOnlyList<JsonRpcItem<JsonRpcResponse>>>(stream,JsonRpcResponseResolver,cancellationToken);

                if (items.Count == 0)
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.batch.empty"));

                return ValidateResponseItems(items);
            }            
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidJson,Strings.GetString("core.deserialize.json_issue"),default,x);
            }
        }

        private JsonRpcData<JsonRpcResponse> ValidateResponseItems(IReadOnlyList<JsonRpcItem<JsonRpcResponse>> items)
        {
            if (items is JsonRpcItem<JsonRpcResponse>[] && items.Count == 1)
            {
                ref readonly var item = ref ((JsonRpcItem<JsonRpcResponse>[])items)[0];
                return new JsonRpcData<JsonRpcResponse>(item);
            }

            return new JsonRpcData<JsonRpcResponse>(items);
        }

        private static Lazy<IJsonFormatterResolver> _jsonRpcRequestResolverV1 =
            new Lazy<IJsonFormatterResolver>(() => CompositeResolver.Create(new IJsonFormatter[] {
                new Utf8JsonRpcMsgListFormatter<JsonRpcRequest>(),
                new Utf8JsonRpcRequestFormatterV1()
            },new[] { StandardResolver.ExcludeNullCamelCase }));

        private static Lazy<IJsonFormatterResolver> _jsonRpcRequestResolverV2 =
            new Lazy<IJsonFormatterResolver>(() => CompositeResolver.Create(new IJsonFormatter[] {
                new Utf8JsonRpcMsgListFormatter<JsonRpcRequest>(),
                new Utf8JsonRpcRequestFormatterV2()
            },new[] { StandardResolver.ExcludeNullCamelCase }));       

        private IJsonFormatterResolver JsonRpcRequestResolver
        {
            get => _compatibilityLevel == JsonRpcCompatibilityLevel.Level2
               ? new JsonRpcRequestFormatterResolver(_jsonRpcRequestResolverV2.Value,RequestContracts)
               : new JsonRpcRequestFormatterResolver(_jsonRpcRequestResolverV1.Value,RequestContracts);
        }

        /// <summary>Serializes the specified request to a JSON string.</summary>
        /// <param name="request">The request to serialize.</param>
        /// <returns>A JSON string representation of the specified request.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during request serialization.</exception>
        public string SerializeRequest(JsonRpcRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            try
            {
                return JsonSerializer.ToJsonString(request,JsonRpcRequestResolver);
            }            
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.deserialize.json_issue"),request.Id,x);
            }
        }

        /// <summary>Serializes the specified request to the specified stream.</summary>
        /// <param name="request">The request to serialize.</param>
        /// <param name="stream">The stream for a JSON string.</param>
        /// <exception cref="ArgumentNullException"><paramref name="request" /> or <paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during request serialization.</exception>
        public void SerializeRequest(JsonRpcRequest request,Stream stream)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            try
            {
                JsonSerializer.Serialize(stream,request,JsonRpcRequestResolver);
            }
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.deserialize.json_issue"),request.Id,x);
            }
        }

        /// <summary>Serializes the specified request to the specified stream as an asynchronous operation.</summary>
        /// <param name="request">The request to serialize.</param>
        /// <param name="stream">The stream for a JSON string.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a JSON string representation of the specified request.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request" /> or <paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during request serialization.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task SerializeRequestAsync(JsonRpcRequest request,Stream stream,CancellationToken cancellationToken = default)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            try
            {
                return Utf8Json.JsonSerializer.SerializeAsync(stream,request,JsonRpcRequestResolver,cancellationToken);
            }
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.deserialize.json_issue"),request.Id,x);
            }
        }

        /// <summary>Serializes the specified collection of requests to a JSON string.</summary>
        /// <param name="requests">The collection of requests to serialize.</param>
        /// <returns>A JSON string representation of the specified collection of requests.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="requests" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during requests serialization.</exception>
        public string SerializeRequests(IReadOnlyList<JsonRpcRequest> requests)
        {
            if ((requests?.Count ?? throw new ArgumentNullException(nameof(requests))) == 0)            
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.batch.empty"));            

            try
            {
                return JsonSerializer.ToJsonString(requests,JsonRpcRequestResolver);
            }
            catch (ArgumentNullException x)
            {
                var item = requests.Select((r,i) =>
                   r ?? throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,string.Format(CultureInfo.InvariantCulture,Strings.GetString("core.batch.invalid_item"),i)))
                .Any(r => r == null);

                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.serialize.json_issue"),default,x);
            }
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.serialize.json_issue"),default,x);
            }
        }

        /// <summary>Serializes the specified collection of requests to the specified stream.</summary>
        /// <param name="requests">The collection of requests to serialize.</param>
        /// <param name="stream">The stream for a JSON string.</param>
        /// <exception cref="ArgumentNullException"><paramref name="requests" /> or <paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during requests serialization.</exception>
        public void SerializeRequests(IReadOnlyList<JsonRpcRequest> requests,Stream stream)
        {
            _ = requests ?? throw new ArgumentNullException(nameof(requests));
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            try
            {
                JsonSerializer.Serialize(stream,requests,JsonRpcRequestResolver);
            }
            catch (ArgumentNullException x)
            {
                var item = requests.Select((r,i) =>
                   r ?? throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,string.Format(CultureInfo.InvariantCulture,Strings.GetString("core.batch.invalid_item"),i)))
                .Any(r => r == null);

                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.serialize.json_issue"),default,x);
            }
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.serialize.json_issue"),default,x);
            }
        }

        /// <summary>Serializes the specified collection of requests to the specified stream as an asynchronous operation.</summary>
        /// <param name="requests">The collection of requests to serialize.</param>
        /// <param name="stream">The stream for a JSON string.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a JSON string representation of the specified collection of requests.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="requests" /> or <paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during requests serialization.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task SerializeRequestsAsync(IReadOnlyList<JsonRpcRequest> requests,Stream stream,CancellationToken cancellationToken = default)
        {
            _ = requests ?? throw new ArgumentNullException(nameof(requests));
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            try
            {
                return JsonSerializer.SerializeAsync(stream,requests,JsonRpcRequestResolver,cancellationToken);
            }
            catch (ArgumentNullException x)
            {
                var item = requests.Select((r,i) =>
                   r ?? throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,string.Format(CultureInfo.InvariantCulture,Strings.GetString("core.batch.invalid_item"),i)))
                .Any(r => r == null);

                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.serialize.json_issue"),default,x);
            }
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.serialize.json_issue"),default,x);
            }
        }

        private static Lazy<IJsonFormatterResolver> _jsonRpcResponseResolverV1 =
            new Lazy<IJsonFormatterResolver>(() => CompositeResolver.Create(new IJsonFormatter[] {
                new Utf8JsonRpcMsgListFormatter<JsonRpcResponse>(),
                new Utf8JsonRpcResponseFormatterV1()
            },new[] { StandardResolver.ExcludeNullCamelCase }));

        private static Lazy<IJsonFormatterResolver> _jsonRpcResponseResolverV2 =
            new Lazy<IJsonFormatterResolver>(() => CompositeResolver.Create(new IJsonFormatter[] {
                new Utf8JsonRpcMsgListFormatter<JsonRpcResponse>(),
                new Utf8JsonRpcResponseFormatterV2()
            },new[] { StandardResolver.ExcludeNullCamelCase }));

        private IJsonFormatterResolver JsonRpcResponseResolver
        {
            get => _compatibilityLevel == JsonRpcCompatibilityLevel.Level2
               ? new JsonRpcResponseFormatterResolver(_jsonRpcResponseResolverV2.Value,
                   ResponseContracts,StaticResponseBindings,DynamicResponseBindings,_defaultErrorDataType)
               : new JsonRpcResponseFormatterResolver(_jsonRpcResponseResolverV1.Value,
                   ResponseContracts,StaticResponseBindings,DynamicResponseBindings,_defaultErrorDataType);
        }

        /// <summary>Serializes the specified response to a JSON string.</summary>
        /// <param name="response">The response to serialize.</param>
        /// <returns>A JSON string representation of the specified response.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="response" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during response serialization.</exception>
        public string SerializeResponse(JsonRpcResponse response)
        {
            _ = response ?? throw new ArgumentNullException(nameof(response));

            try
            {
                return JsonSerializer.ToJsonString(response,JsonRpcResponseResolver);
            }
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.serialize.json_issue"),response.Id,x);
            }
        }

        /// <summary>Serializes the specified response to the specified stream.</summary>
        /// <param name="response">The response to serialize.</param>
        /// <param name="stream">The stream for a JSON string.</param>
        /// <exception cref="ArgumentNullException"><paramref name="response" /> or <paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during response serialization.</exception>
        public void SerializeResponse(JsonRpcResponse response,Stream stream)
        {
            _ = response ?? throw new ArgumentNullException(nameof(response));
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            try
            {
                JsonSerializer.Serialize(stream,response,JsonRpcResponseResolver);
            }
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.serialize.json_issue"),response.Id,x);
            }
        }

        /// <summary>Serializes the specified response to the specified stream as an asynchronous operation.</summary>
        /// <param name="response">The response to serialize.</param>
        /// <param name="stream">The stream for a JSON string.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a JSON string representation of the specified response.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="response" /> or <paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during response serialization.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task SerializeResponseAsync(JsonRpcResponse response,Stream stream,CancellationToken cancellationToken = default)
        {
            _ = response ?? throw new ArgumentNullException(nameof(response));
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            try
            {
                return JsonSerializer.SerializeAsync(stream,response,JsonRpcResponseResolver,cancellationToken);
            }
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.serialize.json_issue"),response.Id,x);
            }
        }

        /// <summary>Serializes the specified collection of responses to a JSON string.</summary>
        /// <param name="responses">The collection of responses to serialize.</param>
        /// <returns>A JSON string representation of the specified collection of responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="responses" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during responses serialization.</exception>
        public string SerializeResponses(IReadOnlyList<JsonRpcResponse> responses)
        {
            if ((responses?.Count ?? throw new ArgumentNullException(nameof(responses))) == 0)
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.batch.empty"));
            }

            try
            {
                return JsonSerializer.ToJsonString(responses,JsonRpcResponseResolver);
            }
            catch (ArgumentNullException x)
            {
                var item = responses.Select((r,i) =>
                   r ??  throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,string.Format(CultureInfo.InvariantCulture,Strings.GetString("core.batch.invalid_item"),i)))
                .Any(r => r == null);

                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.serialize.json_issue"),default,x);
            }
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.serialize.json_issue"),default,x);
            }
        }

        /// <summary>Serializes the specified collection of responses to the specified stream.</summary>
        /// <param name="responses">The collection of responses to serialize.</param>
        /// <param name="stream">The stream for a JSON string.</param>
        /// <exception cref="ArgumentNullException"><paramref name="responses" /> or <paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during responses serialization.</exception>
        public void SerializeResponses(IReadOnlyList<JsonRpcResponse> responses,Stream stream)
        {
            _ = responses ?? throw new ArgumentNullException(nameof(responses));
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            try
            {
                JsonSerializer.Serialize(stream,responses,JsonRpcResponseResolver);
            }
            catch (ArgumentNullException x)
            {
                var item = responses.Select((r,i) =>
                   r ??  throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,string.Format(CultureInfo.InvariantCulture,Strings.GetString("core.batch.invalid_item"),i)))
                .Any(r => r == null);

                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.serialize.json_issue"),default,x);
            }
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.serialize.json_issue"),default,x);
            }
        }

        /// <summary>Serializes the specified collection of responses to the specified stream as an asynchronous operation.</summary>
        /// <param name="responses">The collection of responses to serialize.</param>
        /// <param name="stream">The stream for a JSON string.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a JSON string representation of the specified collection of responses.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="responses" /> or <paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcException">An error occurred during responses serialization.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task SerializeResponsesAsync(IReadOnlyList<JsonRpcResponse> responses,Stream stream,CancellationToken cancellationToken = default)
        {
            _ = responses ?? throw new ArgumentNullException(nameof(responses));
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            try
            {
                return JsonSerializer.SerializeAsync(stream,responses,JsonRpcResponseResolver,cancellationToken);
            }
            catch (ArgumentNullException x)
            {
                var item = responses.Select((r,i) =>
                   r ??  throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,string.Format(CultureInfo.InvariantCulture,Strings.GetString("core.batch.invalid_item"),i)))
                .Any(r => r == null);

                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.serialize.json_issue"),default,x);
            }
            catch (Exception x) when (!(x is JsonRpcException))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.serialize.json_issue"),default,x);
            }
        }

        private JsonRpcResponseContract GetResponseContract(in JsonRpcId identifier)
        {
            if (!DynamicResponseBindings.TryGetValue(identifier,out var contract))
            {
                if (StaticResponseBindings.TryGetValue(identifier,out var method) && (method != null))                
                    ResponseContracts.TryGetValue(method,out contract);                
            }

            return contract;
        }

        /// <summary>Clears response bindings of the current instance.</summary>
        public void Dispose()
        {
            DynamicResponseBindings.Clear();
            StaticResponseBindings.Clear();
        }

		/// <summary>Gets the request contracts.</summary>
		public IDictionary<string,JsonRpcRequestContract> RequestContracts { get; }

		/// <summary>Gets the response contracts.</summary>
		public IDictionary<string,JsonRpcResponseContract> ResponseContracts { get; }

		/// <summary>Gets the dynamic response bindings.</summary>
		public IDictionary<JsonRpcId,JsonRpcResponseContract> DynamicResponseBindings { get; }

		/// <summary>Gets the static response bindings.</summary>
		public IDictionary<JsonRpcId,string> StaticResponseBindings { get; }

		private Type _defaultErrorDataType;

        /// <summary>Gets or sets a type of error data for deserializing an unsuccessful response with empty identifier.</summary>
        public Type DefaultErrorDataType
        {
            get => _defaultErrorDataType;
            set => _defaultErrorDataType = value;
        }

        private JsonRpcCompatibilityLevel _compatibilityLevel;

        /// <summary>Gets or sets the protocol compatibility level.</summary>
        public JsonRpcCompatibilityLevel CompatibilityLevel
        {
            get => _compatibilityLevel;
            set => _compatibilityLevel = value;
        }
    }    
}
