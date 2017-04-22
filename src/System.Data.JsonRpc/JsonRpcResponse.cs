using System.Diagnostics;

namespace System.Data.JsonRpc
{
    /// <summary>Represents an RPC response message.</summary>
    [DebuggerDisplay("Id = {" + nameof(Id) + "}, Success = {" + nameof(Success) + "}")]
    public sealed class JsonRpcResponse : JsonRpcMessage
    {
        internal JsonRpcResponse()
            : base()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcResponse" /> class.</summary>
        /// <param name="result">The value, which is determined by the method invoked on the server.</param>
        /// <param name="id">The identifier, which must be the same as the value in the request object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="result" /> is <see langword="null" />.</exception>
        public JsonRpcResponse(object result, JsonRpcId id)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            Result = result;
            Id = id;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcResponse" /> class.</summary>
        /// <param name="error">The <see cref="JsonRpcError" /> object with information.</param>
        /// <param name="id">The identifier, which must be the same as the value in the request object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="error" /> is <see langword="null" />.</exception>
        public JsonRpcResponse(JsonRpcError error, JsonRpcId id)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            Error = error;
            Id = id;
        }

        /// <summary>Gets an object, which represents error information. This member is required on error.</summary>
        public JsonRpcError Error { get; internal set; }

        /// <summary>Gets a value indicating whether the response has identifier.</summary>
        public bool HasId => Id.Type != JsonRpcIdType.None;

        /// <summary>Gets a value, which is determined by the method invoked on the server. This member is required on success.</summary>
        public object Result { get; internal set; }

        /// <summary>Gets a value indicating whether the response was successful.</summary>
        public bool Success => Result != null;
    }
}