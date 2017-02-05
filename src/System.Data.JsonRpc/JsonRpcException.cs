namespace System.Data.JsonRpc
{
    /// <summary>Represents errors that occur during RPC message processing.</summary>
    public sealed class JsonRpcException : Exception
    {
        internal JsonRpcException(JsonRpcExceptionType type, string message)
            : base(message) => Type = type;

        internal JsonRpcException(JsonRpcExceptionType type, string message, Exception innerException)
            : base(message, innerException) => Type = type;

        /// <summary>Gets exception type.</summary>
        public JsonRpcExceptionType Type { get; }
    }
}