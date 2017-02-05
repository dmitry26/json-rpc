namespace System.Data.JsonRpc
{
    /// <summary>Represents JSON-RPC exception type.</summary>
    public enum JsonRpcExceptionType
    {
        /// <summary>An error occurred while parsing the JSON text.</summary>
        ParseError,

        /// <summary>Generic error.</summary>
        GenericError,

        /// <summary>The method does not exist / is not available.</summary>
        InvalidMethod,

        /// <summary>The JSON is not a valid message object.</summary>
        InvalidMessage
    }
}