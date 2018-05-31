using System.Diagnostics;

namespace System.Data.JsonRpc
{
    /// <summary>Represents information about an RPC message.</summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    [DebuggerDisplay("IsValid = {IsValid}")]
    public readonly struct JsonRpcItem<T>
        where T : JsonRpcMessage
    {
        private readonly T _message;
        private readonly JsonRpcException _exception;

        internal JsonRpcItem(T message)
        {
            _message = message;
            _exception = null;
        }

        internal JsonRpcItem(JsonRpcException exception)
        {
            _message = null;
            _exception = exception;
        }

        /// <summary>Gets a message for the valid item.</summary>
        public T Message
        {
            get => _message;
        }

        /// <summary>Gets an exception for the invalid item.</summary>
        public JsonRpcException Exception
        {
            get => _exception;
        }

        /// <summary>Gets a value indicating whether the item represents a valid message.</summary>
        public bool IsValid
        {
            get => _message != null;
        }
    }
}