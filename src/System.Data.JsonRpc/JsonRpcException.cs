namespace System.Data.JsonRpc
{
    /// <summary>Represents errors that occur during RPC message processing.</summary>
    public sealed class JsonRpcException : Exception
    {
        internal JsonRpcException(string message, in JsonRpcId messageId)
            : base(message)
        {
            MessageId = messageId;
        }

        internal JsonRpcException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal JsonRpcException(string message, in JsonRpcId messageId, Exception innerException)
            : base(message, innerException)
        {
            MessageId = messageId;
        }

        internal JsonRpcException(JsonRpcExceptionType type, string message)
            : base(message)
        {
            Type = type;
        }

        internal JsonRpcException(JsonRpcExceptionType type, string message, in JsonRpcId messageId)
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

        internal JsonRpcException(JsonRpcExceptionType type, string message, in JsonRpcId messageId, Exception innerException)
            : base(message, innerException)
        {
            Type = type;
            MessageId = messageId;
        }

        /// <summary>Gets an identifier for the related message.</summary>
        public JsonRpcId MessageId
        {
            get;
        }

        /// <summary>Gets exception type.</summary>
        public JsonRpcExceptionType Type
        {
            get;
        }
    }
}