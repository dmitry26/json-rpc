using System.Diagnostics;

namespace System.Data.JsonRpc
{
    /// <summary>Represents information about an RPC message.</summary>
    /// <typeparam name="T">Type of the message.</typeparam>
    [DebuggerDisplay("IsValid = {IsValid}")]
    public readonly struct JsonRpcItem<T>
        where T : JsonRpcMessage
    {
        internal JsonRpcItem(T message)
        {
            Message = message;
            Exception = null;
        }

        internal JsonRpcItem(JsonRpcException exception)
        {
            Message = null;
            Exception = exception;
        }

        /// <summary>Gets a message for the valid item.</summary>
        public T Message
        {
            get;
        }

        /// <summary>Gets an exception for the invalid item.</summary>
        public JsonRpcException Exception
        {
            get;
        }

        /// <summary>Gets a value indicating whether the item represents valid.</summary>
        public bool IsValid
        {
            get => Message != null;
        }
    }
}