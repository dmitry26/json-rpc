namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC error type.</summary>
    public enum JsonRpcErrorType : long
    {
        /// <summary>Non-specific error.</summary>
        Undefined = 0L,

        /// <summary>An error occurred on the server while parsing the JSON text.</summary>
        Parsing = -32700L,

        /// <summary>Internal JSON-RPC error.</summary>
        Internal = -32603L,

        /// <summary>Invalid method parameter(s).</summary>
        InvalidParams = -32602L,

        /// <summary>The method does not exist / is not available.</summary>
        InvalidMethod = -32601L,

        /// <summary>The JSON sent is not a valid request object.</summary>
        InvalidRequest = -32600L,

        /// <summary>Implementation-defined server error.</summary>
        Server = -32000L
    }
}