using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.JsonRpc
{
    /// <summary>Serializes and deserializes JSON-RPC messages into and from the JSON format.</summary>
    public interface IJsonRpcSerializer
    {
        /// <summary>Gets or sets the protocol compatibility level.</summary>        
        JsonRpcCompatibilityLevel CompatibilityLevel { get; set; }

        /// <summary>Gets or sets a type of error data for deserializing an unsuccessful response with empty identifier.</summary>
        Type DefaultErrorDataType { get; set; }

        /// <summary>Gets the dynamic response bindings.</summary>
        IDictionary<JsonRpcId,JsonRpcResponseContract> DynamicResponseBindings { get; }

        /// <summary>Gets the request contracts.</summary>
        IDictionary<string,JsonRpcRequestContract> RequestContracts { get; }

        /// <summary>Gets the response contracts.</summary>
        IDictionary<string,JsonRpcResponseContract> ResponseContracts { get; }

        /// <summary>Gets the static response bindings.</summary>
        IDictionary<JsonRpcId,string> StaticResponseBindings { get; }

        /// <summary>Deserializes the specified stream with a JSON string to request data.</summary>
        JsonRpcData<JsonRpcRequest> DeserializeRequestData(Stream stream);

        /// <summary>Deserializes the specified JSON string to request data.</summary>
        JsonRpcData<JsonRpcRequest> DeserializeRequestData(string json);

        /// <summary>Deserializes the specified stream with a JSON string to request data as an asynchronous operation.</summary>
        Task<JsonRpcData<JsonRpcRequest>> DeserializeRequestDataAsync(Stream stream,CancellationToken cancellationToken = default);

        /// <summary>Deserializes the specified stream with a JSON string to response data.</summary>
        JsonRpcData<JsonRpcResponse> DeserializeResponseData(Stream stream);

        /// <summary>Deserializes the specified JSON string to response data.</summary>
        JsonRpcData<JsonRpcResponse> DeserializeResponseData(string json);

        /// <summary>Deserializes the specified stream with a JSON string to response data as an asynchronous operation.</summary>
        Task<JsonRpcData<JsonRpcResponse>> DeserializeResponseDataAsync(Stream stream,CancellationToken cancellationToken = default);

        /// <summary>Serializes the specified request to a JSON string.</summary>
        string SerializeRequest(JsonRpcRequest request);

        /// <summary>Serializes the specified request to the specified stream.</summary>
        void SerializeRequest(JsonRpcRequest request,Stream stream);

        /// <summary>Serializes the specified request to the specified stream as an asynchronous operation.</summary>
        Task SerializeRequestAsync(JsonRpcRequest request,Stream stream,CancellationToken cancellationToken = default);

        /// <summary>Serializes the specified collection of requests to a JSON string.</summary>
        string SerializeRequests(IReadOnlyList<JsonRpcRequest> requests);

        /// <summary>Serializes the specified collection of requests to the specified stream.</summary>
        void SerializeRequests(IReadOnlyList<JsonRpcRequest> requests,Stream stream);

        /// <summary>Serializes the specified collection of requests to the specified stream as an asynchronous operation.</summary>
        Task SerializeRequestsAsync(IReadOnlyList<JsonRpcRequest> requests,Stream stream,CancellationToken cancellationToken = default);

        /// <summary>Serializes the specified response to a JSON string.</summary>
        string SerializeResponse(JsonRpcResponse response);

        /// <summary>Serializes the specified response to the specified stream.</summary>
        void SerializeResponse(JsonRpcResponse response,Stream stream);

        /// <summary>Serializes the specified response to the specified stream as an asynchronous operation.</summary>
        Task SerializeResponseAsync(JsonRpcResponse response,Stream stream,CancellationToken cancellationToken = default);

        /// <summary>Serializes the specified collection of responses to a JSON string.</summary>
        string SerializeResponses(IReadOnlyList<JsonRpcResponse> responses);

        /// <summary>Serializes the specified collection of responses to the specified stream.</summary>
        void SerializeResponses(IReadOnlyList<JsonRpcResponse> responses,Stream stream);

        /// <summary>Serializes the specified collection of responses to the specified stream as an asynchronous operation.</summary>
        Task SerializeResponsesAsync(IReadOnlyList<JsonRpcResponse> responses,Stream stream,CancellationToken cancellationToken = default);
    }
}