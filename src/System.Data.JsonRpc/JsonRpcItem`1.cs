using System.Diagnostics;

namespace System.Data.JsonRpc
{
    /// <summary>Represents information about an RPC message.</summary>
    /// <typeparam name="T">Type of the message.</typeparam>
    [DebuggerDisplay("Is Valid = {" + nameof(IsValid) + "}")]
    public struct JsonRpcItem<T>
        where T : JsonRpcMessage
    {
        private readonly JsonRpcException _exception;
        private readonly T _message;

        internal JsonRpcItem(T message)
        {
            _exception = null;
            _message = message;
        }

        internal JsonRpcItem(JsonRpcException exception)
        {
            _exception = exception;
            _message = null;
        }

        /// <summary>Gets an exception for the invalid item.</summary>
        /// <returns>An exception.</returns>
        /// <exception cref="InvalidOperationException">Converting was successful.</exception>
        public JsonRpcException GetException()
        {
            if (IsValid)
            {
                throw new InvalidOperationException("The item is a valid message");
            }

            return _exception;
        }

        /// <summary>Gets a message for the valid item.</summary>
        /// <returns>A message.</returns>
        /// <exception cref="InvalidOperationException">Converting was not successful.</exception>
        public T GetMessage()
        {
            if (!IsValid)
            {
                throw new InvalidOperationException("The item is an invalid message");
            }

            return _message;
        }

        /// <summary>Gets a value indicating whether the item represents valid.</summary>
        public bool IsValid => _message != null;
    }
}