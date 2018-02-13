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
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="code" /> is outside the allowable range.</exception>
        public JsonRpcError(long code, string message, object data)
            : this(code, message)
        {
            Data = data;
            HasData = true;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcError" /> class.</summary>
        /// <param name="code">The number that indicates the error type that occurred.</param>
        /// <param name="message">The string providing a short description of the error. The message should be limited to a single concise sentence.</param>
        /// <exception cref="ArgumentNullException"><paramref name="message" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="code" /> is outside the allowable range.</exception>
        public JsonRpcError(long code, string message)
        {
            if ((code >= JsonRpcErrorCode.StandardErrorsLowerBoundary) && (code <= JsonRpcErrorCode.StandardErrorsUpperBoundary))
            {
                if ((code < JsonRpcErrorCode.ServerErrorsLowerBoundary) || (code > JsonRpcErrorCode.ServerErrorsUpperBoundary))
                {
                    if ((code != JsonRpcErrorCode.InvalidJson) &&
                        (code != JsonRpcErrorCode.InvalidOperation) &&
                        (code != JsonRpcErrorCode.InvalidParameters) &&
                        (code != JsonRpcErrorCode.InvalidMethod) &&
                        (code != JsonRpcErrorCode.InvalidMessage))
                    {
                        throw new ArgumentOutOfRangeException(nameof(code), code, Strings.GetString("error.code.invalid_range"));
                    }
                }
            }
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Code = code;
            Message = message;
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

        /// <summary>Gets an optional value that contains additional information about the error.</summary>
        public object Data
        {
            get;
        }

        /// <summary>Gets a value indicating whether the message has additional information specified.</summary>
        public bool HasData
        {
            get;
        }
    }
}