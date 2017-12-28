namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC error type.</summary>
    public enum JsonRpcErrorType
    {
        /// <summary>Non-specific error.</summary>
        Undefined,

        /// <summary>An error occurred on the server while parsing the JSON text.</summary>
        Parsing,

        /// <summary>Internal JSON-RPC error.</summary>
        Internal,

        /// <summary>Invalid method parameter(s).</summary>
        InvalidParams,

        /// <summary>The method does not exist / is not available.</summary>
        InvalidMethod,

        /// <summary>The JSON sent is not a valid request object.</summary>
        InvalidRequest,

        /// <summary>Implementation-defined server error.</summary>
        Server,

        /// <summary>System error.</summary>
        System
    }
}