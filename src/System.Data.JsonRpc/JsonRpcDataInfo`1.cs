using System.Collections.Generic;
using System.Diagnostics;

namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC data information.</summary>
    /// <typeparam name="T">Type of the message.</typeparam>
    [DebuggerDisplay("Is Empty = {" + nameof(IsEmpty) + "}, Is Batch = {" + nameof(IsBatch) + "}")]
    public sealed class JsonRpcDataInfo<T>
        where T : JsonRpcMessage
    {
        private readonly JsonRpcMessageInfo<T> _item;
        private readonly IReadOnlyList<JsonRpcMessageInfo<T>> _items;

        internal JsonRpcDataInfo() => IsEmpty = true;
        internal JsonRpcDataInfo(JsonRpcMessageInfo<T> item) => _item = item;
        internal JsonRpcDataInfo(IReadOnlyList<JsonRpcMessageInfo<T>> items) => _items = items;

        /// <summary>Gets an <see cref="JsonRpcMessageInfo{T}"/> object for non-batch data.</summary>
        /// <returns>An <see cref="JsonRpcMessageInfo{T}"/> object.</returns>
        /// <exception cref="JsonRpcException">Data is empty or is a batch.</exception>
        public JsonRpcMessageInfo<T> GetItem()
        {
            if (IsEmpty)
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "Data is empty");
            if (IsBatch)
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "Data is a batch");

            return _item;
        }

        /// <summary>Gets a collection of <see cref="JsonRpcMessageInfo{T}"/> objects for batch data.</summary>
        /// <returns>A collection of <see cref="JsonRpcMessageInfo{T}"/> objects.</returns>
        /// <exception cref="JsonRpcException">Data is empty or is not a batch.</exception>
        public IReadOnlyList<JsonRpcMessageInfo<T>> GetItems()
        {
            if (IsEmpty)
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "Data is empty");
            if (!IsBatch)
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "Data is not a batch");

            return _items;
        }

        /// <summary>Gets a value indicating whether data is a batch.</summary>
        public bool IsBatch => _items != null;

        /// <summary>Gets a value indicating whether data is empty.</summary>
        public bool IsEmpty { get; }
    }
}