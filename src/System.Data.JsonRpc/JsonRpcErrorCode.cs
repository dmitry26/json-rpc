namespace System.Data.JsonRpc
{
    /// <summary>Defines standard RPC error codes and error codes ranges.</summary>
    public static class JsonRpcErrorCode
    {
        /// <summary>Gets the error code which specifies, that the provided JSON is invalid.</summary>
        public const long InvalidJson = -32700L;

        /// <summary>Gets the error code which specifies, that an error occurred during processing the message.</summary>
        public const long InvalidOperation = -32603L;

        /// <summary>Gets the error code which specifies, that the specified method parameters are invalid.</summary>
        public const long InvalidParameters = -32602L;

        /// <summary>Gets the error code which specifies, that the specified method does not exist or is not available.</summary>
        public const long InvalidMethod = -32601L;

        /// <summary>Gets the error code which specifies, that the provided message is not valid.</summary>
        public const long InvalidMessage = -32600L;

        /// <summary>Gets the lower boundary of the implementation-defined server error codes range.</summary>
        public const long ServerErrorsLowerBoundary = -32099L;

        /// <summary>Gets the upper boundary of the implementation-defined server error codes range.</summary>
        public const long ServerErrorsUpperBoundary = -32000L;

        /// <summary>Gets the lower boundary of the standard error codes range.</summary>
        public const long StandardErrorsLowerBoundary = -32768L;

        /// <summary>Gets the upper boundary of the standard error codes range.</summary>
        public const long StandardErrorsUpperBoundary = -32000L;
    }
}