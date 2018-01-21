namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC exception type.</summary>
    public enum JsonRpcExceptionType : long
    {
        /// <summary>A non-specific error.</summary>
        Undefined = 0L,

        /// <summary>The provided JSON is invalid.</summary>
        Parsing = -32700L,

        /// <summary>The specified method parameters are invalid.</summary>
        InvalidParams = -32602L,

        /// <summary>The specified method does not exist or is not available.</summary>
        InvalidMethod = -32601L,

        /// <summary>The provided message is not valid.</summary>
        InvalidMessage = -32600L
    }
}