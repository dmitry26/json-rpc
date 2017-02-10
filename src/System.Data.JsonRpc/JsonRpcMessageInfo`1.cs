using System.Diagnostics;

namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC message information.</summary>
    /// <typeparam name="T">Type of the message.</typeparam>
    [DebuggerDisplay("Success = {" + nameof(Success) + "}")]
    public struct JsonRpcMessageInfo<T>
        where T : JsonRpcMessage
    {
        private readonly JsonRpcException _exception;
        private readonly T _item;

        internal JsonRpcMessageInfo(T message)
        {
            _exception = null;
            _item = message;
        }

        internal JsonRpcMessageInfo(JsonRpcException exception)
        {
            _exception = exception;
            _item = null;
        }

        /// <summary>Gets an exception for invalid message information.</summary>
        /// <returns>An exception.</returns>
        /// <exception cref="JsonRpcException">Converting was successful.</exception>
        public JsonRpcException GetException()
        {
            if (Success)
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "Converting was successful");

            return _exception;
        }

        /// <summary>Gets a message for valid message information.</summary>
        /// <returns>A message.</returns>
        /// <exception cref="JsonRpcException">Converting was not successful.</exception>
        public T GetItem()
        {
            if (!Success)
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "Converting was not successful");

            return _item;
        }

        /// <summary>Gets a value indicating whether message converting was successful.</summary>
        public bool Success => _item != null;
    }
}