using System.Diagnostics;

namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC response message.</summary>
    [DebuggerDisplay("Id = {" + nameof(Id) + "}, Success = {" + nameof(Success) + "}")]
    public sealed class JsonRpcResponse : JsonRpcMessage
    {
        internal JsonRpcResponse()
            : base()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcResponse" /> class.</summary>
        /// <param name="result">The value, which is determined by the method invoked on the server.</param>
        /// <exception cref="ArgumentNullException"><paramref name="result" /> is <see langword="null" />.</exception>
        public JsonRpcResponse(object result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            Result = result;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcResponse" /> class.</summary>
        /// <param name="error">The <see cref="JsonRpcError"/> object with information.</param>
        /// <exception cref="ArgumentNullException"><paramref name="error" /> is <see langword="null" />.</exception>
        public JsonRpcResponse(JsonRpcError error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            Error = error;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcResponse" /> class.</summary>
        /// <param name="id">The identifier, which must be the same as the value in the request object.</param>
        /// <param name="result">The value, which is determined by the method invoked on the server.</param>
        /// <exception cref="ArgumentNullException"><paramref name="result" /> is <see langword="null" />.</exception>
        public JsonRpcResponse(long id, object result)
            : base(id)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            Result = result;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcResponse" /> class.</summary>
        /// <param name="id">The identifier, which must be the same as the value in the request object.</param>
        /// <param name="error">The <see cref="JsonRpcError"/> object with information.</param>
        /// <exception cref="ArgumentNullException"><paramref name="error" /> is <see langword="null" />.</exception>
        public JsonRpcResponse(long id, JsonRpcError error)
            : base(id)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            Error = error;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcResponse" /> class.</summary>
        /// <param name="id">The identifier, which must be the same as the value in the request object.</param>
        /// <param name="result">The value, which is determined by the method invoked on the server.</param>
        /// <exception cref="ArgumentNullException"><paramref name="id" /> or <paramref name="result" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="id" /> is empty string.</exception>
        public JsonRpcResponse(string id, object result)
            : base(id)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            Result = result;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcResponse" /> class.</summary>
        /// <param name="id">The identifier, which must be the same as the value in the request object.</param>
        /// <param name="error">The <see cref="JsonRpcError"/> object with information.</param>
        /// <exception cref="ArgumentNullException"><paramref name="id" /> or <paramref name="error" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="id" /> is empty string.</exception>
        public JsonRpcResponse(string id, JsonRpcError error)
            : base(id)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            Error = error;
        }

        /// <summary>Gets an object, which represents error information. This member is required on error.</summary>
        public JsonRpcError Error { get; internal set; }

        /// <summary>Gets a value indicating whether the response has identifier.</summary>
        public bool HasId => IdType != JsonRpcIdType.Null;

        /// <summary>Gets a value, which is determined by the method invoked on the server. This member is required on success.</summary>
        public object Result { get; internal set; }

        /// <summary>Gets a value indicating whether the response was successful.</summary>
        public bool Success => Result != null;
    }
}