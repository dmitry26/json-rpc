namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC error type.</summary>
    public enum JsonRpcErrorType : long
    {
        /// <summary>A non-specific error.</summary>
        Undefined = 0L,

        /// <summary>The provided JSON is invalid.</summary>
        Parsing = -32700L,

        /// <summary>An error occurred during processing the request.</summary>
        Internal = -32603L,

        /// <summary>The specified method parameters are invalid.</summary>
        InvalidParams = -32602L,

        /// <summary>The specified method does not exist or is not available.</summary>
        InvalidMethod = -32601L,

        /// <summary>The provided message is not a valid request.</summary>
        InvalidRequest = -32600L,

        /// <summary>An implementation-defined server error.</summary>
        Server = -32000L
    }
}