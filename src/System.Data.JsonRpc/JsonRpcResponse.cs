using System.Data.JsonRpc.Resources;
using System.Diagnostics;

namespace System.Data.JsonRpc
{
    /// <summary>Represents an RPC response message.</summary>
    [DebuggerDisplay("Success = {Success}, Id = {Id}")]
    public sealed class JsonRpcResponse : JsonRpcMessage
    {
        /// <summary>Initializes a new instance of the <see cref="JsonRpcResponse" /> class.</summary>
        /// <param name="result">The value, which is determined by the method invoked on the server.</param>
        /// <param name="id">The identifier, which must be the same as the value in the request object.</param>
        /// <exception cref="ArgumentException"><paramref name="id" /> has empty value.</exception>
        public JsonRpcResponse(object result, in JsonRpcId id)
            : base(id)
        {
            if (id.Type == JsonRpcIdType.None)
            {
                throw new ArgumentException(Strings.GetString("response.empty_id"), nameof(id));
            }

            Result = result;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcResponse" /> class.</summary>
        /// <param name="error">The <see cref="JsonRpcError" /> object with information.</param>
        /// <param name="id">The identifier, which must be the same as the value in the request object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="error" /> is <see langword="null" />.</exception>
        public JsonRpcResponse(JsonRpcError error, in JsonRpcId id = default)
            : base(id)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            Error = error;
        }

        /// <summary>Gets a value, which is determined by the method invoked.</summary>
        public object Result
        {
            get;
        }

        /// <summary>Gets an object, which represents error information.</summary>
        public JsonRpcError Error
        {
            get;
        }

        /// <summary>Gets a value indicating whether the request was successful.</summary>
        public bool Success
        {
            get => Error == null;
        }
    }
}