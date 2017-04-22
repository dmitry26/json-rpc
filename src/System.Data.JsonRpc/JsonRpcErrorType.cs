namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC error type.</summary>
    public enum JsonRpcErrorType
    {
        /// <summary>Invalid JSON was received by the server. An error occurred on the server while parsing the JSON text.</summary>
        ParseError,

        /// <summary>Internal JSON-RPC error.</summary>
        InternalError,

        /// <summary>Invalid method parameter(s).</summary>
        InvalidParams,

        /// <summary>The method does not exist / is not available.</summary>
        InvalidMethod,

        /// <summary>The JSON sent is not a valid request object.</summary>
        InvalidRequest,

        /// <summary>Implementation-defined server error.</summary>
        ServerError,

        /// <summary>System error.</summary>
        SystemError,

        /// <summary>Custom error.</summary>
        CustomError
    }
}