using System.Diagnostics;

namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC error information.</summary>
    [DebuggerDisplay("Code = {" + nameof(Code) + "}, Message = {" + nameof(Message) + "}, Has Data = {" + nameof(HasData) + "}")]
    public sealed class JsonRpcError
    {
        internal JsonRpcError()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcError" /> class.</summary>
        /// <param name="code">The number that indicates the error type that occurred.</param>
        /// <param name="message">The string providing a short description of the error. The message should be limited to a single concise sentence.</param>
        /// <exception cref="ArgumentNullException"><paramref name="message" /> is <see langword="null" />.</exception>
        public JsonRpcError(long code, string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            Code = code;
            Message = message;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcError" /> class.</summary>
        /// <param name="code">The number that indicates the error type that occurred.</param>
        /// <param name="message">The string providing a short description of the error. The message should be limited to a single concise sentence.</param>
        /// <param name="data">The primitive or structured value that contains additional information about the error.</param>
        /// <exception cref="ArgumentNullException"><paramref name="message" /> or <paramref name="data" /> is <see langword="null" />.</exception>
        public JsonRpcError(long code, string message, object data)
            : this(code, message)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Data = data;
        }

        /// <summary>Gets a number that indicates the error type that occurred.</summary>
        public long Code { get; internal set; }

        /// <summary>Gets an optional primitive or structured value that contains additional information about the error.</summary>
        public object Data { get; internal set; }

        /// <summary>Gets a value indicating whether the response has additional information about the error.</summary>
        public bool HasData => Data != null;

        /// <summary>Gets a string providing a short description of the error.</summary>
        public string Message { get; internal set; }

        /// <summary>Gets an error type.</summary>
        public JsonRpcErrorType Type
        {
            get
            {
                switch (Code)
                {
                    case -32700L:
                        return JsonRpcErrorType.ParseError;
                    case -32603L:
                        return JsonRpcErrorType.InternalError;
                    case -32602L:
                        return JsonRpcErrorType.InvalidParams;
                    case -32601L:
                        return JsonRpcErrorType.InvalidMethod;
                    case -32600L:
                        return JsonRpcErrorType.InvalidRequest;
                    default:
                        {
                            if (Code <= -32000L)
                            {
                                if (Code >= -32099L)
                                    return JsonRpcErrorType.ServerError;
                                if (Code >= -32768L)
                                    return JsonRpcErrorType.SystemError;
                            }

                            return JsonRpcErrorType.CustomError;
                        }
                }
            }
        }
    }
}