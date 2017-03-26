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

        internal JsonRpcDataInfo()
        {
            IsEmpty = true;
            IsBatch = false;
        }
        internal JsonRpcDataInfo(JsonRpcMessageInfo<T> item)
        {
            _item = item;
            IsBatch = false;
        }
        internal JsonRpcDataInfo(IReadOnlyList<JsonRpcMessageInfo<T>> items)
        {
            _items = items;
            IsBatch = true;
        }

        /// <summary>Gets an <see cref="JsonRpcMessageInfo{T}" /> object for non-batch data.</summary>
        /// <returns>An <see cref="JsonRpcMessageInfo{T}" /> object.</returns>
        /// <exception cref="JsonRpcException">The data is empty or is a batch.</exception>
        public JsonRpcMessageInfo<T> GetSingleItem()
        {
            if (IsEmpty)
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "The data is empty");
            if (IsBatch)
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "The data is a batch");

            return _item;
        }

        /// <summary>Gets a collection of <see cref="JsonRpcMessageInfo{T}" /> objects for batch data.</summary>
        /// <returns>A collection of <see cref="JsonRpcMessageInfo{T}" /> objects.</returns>
        /// <exception cref="JsonRpcException">The data is empty or is not a batch.</exception>
        public IReadOnlyList<JsonRpcMessageInfo<T>> GetBatchItems()
        {
            if (IsEmpty)
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "The data is empty");
            if (!IsBatch)
                throw new JsonRpcException(JsonRpcExceptionType.GenericError, "The data is not a batch");

            return _items;
        }

        /// <summary>Gets a value indicating whether the data is a batch.</summary>
        public bool IsBatch { get; }

        /// <summary>Gets a value indicating whether the data is empty.</summary>
        public bool IsEmpty { get; }
    }
}