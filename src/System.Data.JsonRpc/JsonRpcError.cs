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
        public JsonRpcError(long code, string message, object data = null)
        {
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

        /// <summary>Gets an error type.</summary>
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
                            if (Code <= -32000L)
                            {
                                if (Code >= -32099L)
                                {
                                    return JsonRpcErrorType.Server;
                                }
                                if (Code >= -32768L)
                                {
                                    return JsonRpcErrorType.System;
                                }
                            }

                            return default;
                        }
                }
            }
        }
    }
}