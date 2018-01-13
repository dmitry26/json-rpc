namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC exception type.</summary>
    public enum JsonRpcExceptionType : long
    {
        /// <summary>Non-specific error.</summary>
        Undefined = 0L,

        /// <summary>An error occurred while parsing the JSON text.</summary>
        Parsing = -32700L,

        /// <summary>Invalid method parameter(s).</summary>
        InvalidParams = -32602L,

        /// <summary>The method does not exist / is not available.</summary>
        InvalidMethod = -32601L,

        /// <summary>The JSON is not a valid message object.</summary>
        InvalidMessage = -32600L
    }
}