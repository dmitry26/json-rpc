using System.Data.JsonRpc.Resources;
using System.Diagnostics;

namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC error information.</summary>
    [DebuggerDisplay("Code = {Code}, Message = {Message}")]
    public sealed class JsonRpcError
    {
        /// <summary>Initializes a new instance of the <see cref="JsonRpcError" /> class.</summary>
        /// <param name="code">The number that indicates the error type that occurred.</param>
        /// <param name="message">The string providing a short description of the error. The message should be limited to a single concise sentence.</param>
        /// <param name="data">The primitive or structured value that contains additional information about the error.</param>
        /// <exception cref="ArgumentNullException"><paramref name="message" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="code" /> is outside the allowable range of the protocol error codes.</exception>
        public JsonRpcError(long code, string message, object data = null)
        {
            if ((code >= -32768L) && (code <= -32100L))
            {
                if ((code != -32700L) &&
                    (code != -32603L) &&
                    (code != -32602L) &&
                    (code != -32601L) &&
                    (code != -32600L))
                {
                    throw new ArgumentOutOfRangeException(nameof(code), code, Strings.GetString("error.code.invalid_range"));
                }
            }
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Code = code;
            Message = message;
            Data = data;
        }

        /// <summary>Gets a number that indicates the error type that occurred.</summary>
        public long Code
        {
            get;
        }

        /// <summary>Gets a string providing a short description of the error.</summary>
        public string Message
        {
            get;
        }

        /// <summary>Gets an optional primitive or structured value that contains additional information about the error.</summary>
        public object Data
        {
            get;
        }

        /// <summary>Gets RPC error type.</summary>
        public JsonRpcErrorType Type
        {
            get
            {
                switch (Code)
                {
                    case -32700L:
                        {
                            return JsonRpcErrorType.Parsing;
                        }
                    case -32603L:
                        {
                            return JsonRpcErrorType.Internal;
                        }
                    case -32602L:
                        {
                            return JsonRpcErrorType.InvalidParams;
                        }
                    case -32601L:
                        {
                            return JsonRpcErrorType.InvalidMethod;
                        }
                    case -32600L:
                        {
                            return JsonRpcErrorType.InvalidRequest;
                        }
                    default:
                        {
                            return (Code >= -32099L) && (Code <= -32000L) ? JsonRpcErrorType.Server : JsonRpcErrorType.Undefined;
                        }
                }
            }
        }
    }
}