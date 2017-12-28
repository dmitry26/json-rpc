namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC exception type.</summary>
    public enum JsonRpcExceptionType
    {
        /// <summary>Non-specific error.</summary>
        Undefined,

        /// <summary>An error occurred while parsing the JSON text.</summary>
        Parsing,

        /// <summary>Invalid method parameter(s).</summary>
        InvalidParams,

        /// <summary>The method does not exist / is not available.</summary>
        InvalidMethod,

        /// <summary>The JSON is not a valid message object.</summary>
        InvalidMessage
    }
}