namespace System.Data.JsonRpc
{
    /// <summary>Represents errors that occur during RPC message processing.</summary>
    public class JsonRpcException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="JsonRpcException" /> class.</summary>
        /// <param name="message">The message that describes the error.</param>
        public JsonRpcException(string message)
            : base(message)
        {
            Type = JsonRpcExceptionType.GenericError;
        }

        internal JsonRpcException(JsonRpcExceptionType type, string message)
            : base(message)
        {
            Type = type;
        }

        internal JsonRpcException(JsonRpcExceptionType type, string message, JsonRpcId messageId)
            : base(message)
        {
            Type = type;
            MessageId = messageId;
        }

        internal JsonRpcException(JsonRpcExceptionType type, string message, Exception innerException)
            : base(message, innerException)
        {
            Type = type;
        }

        internal JsonRpcException(JsonRpcExceptionType type, string message, JsonRpcId messageId, Exception innerException)
            : base(message, innerException)
        {
            Type = type;
            MessageId = messageId;
        }

        /// <summary>Gets an identifier for the related message if any.</summary>
        public JsonRpcId MessageId { get; }

        /// <summary>Gets exception type.</summary>
        public JsonRpcExceptionType Type { get; }
    }
}