namespace System.Data.JsonRpc
{
    /// <summary>Represents errors that occur during RPC message processing.</summary>
    public sealed class JsonRpcException : Exception
    {
        internal JsonRpcException(long errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        internal JsonRpcException(long errorCode, string message, in JsonRpcId messageId)
            : base(message)
        {
            ErrorCode = errorCode;
            MessageId = messageId;
        }

        internal JsonRpcException(long errorCode, string message, in JsonRpcId messageId, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            MessageId = messageId;
        }

        /// <summary>Gets an identifier for the related message.</summary>
        public JsonRpcId MessageId
        {
            get;
        }

        /// <summary>Gets the corresponding error code.</summary>
        public long ErrorCode
        {
            get;
        }
    }
}