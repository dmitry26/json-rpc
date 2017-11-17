using System.Collections.Generic;
using System.Diagnostics;

namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC data.</summary>
    /// <typeparam name="T">Type of the message.</typeparam>
    [DebuggerDisplay("Is Empty = {" + nameof(IsEmpty) + "}, Is Batch = {" + nameof(IsBatch) + "}")]
    public sealed class JsonRpcData<T>
        where T : JsonRpcMessage
    {
        private readonly JsonRpcItem<T> _item;
        private readonly IReadOnlyList<JsonRpcItem<T>> _items;

        internal JsonRpcData()
        {
        }

        internal JsonRpcData(JsonRpcItem<T> item)
        {
            _item = item;
        }

        internal JsonRpcData(IReadOnlyList<JsonRpcItem<T>> items)
        {
            _items = items;
        }

        /// <summary>Gets an item for non-batch data.</summary>
        /// <returns>An <see cref="JsonRpcItem{T}" /> object.</returns>
        /// <exception cref="InvalidOperationException">The data is empty or is a batch.</exception>
        public JsonRpcItem<T> GetSingleItem()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("The data is empty");
            }
            if (IsBatch)
            {
                throw new InvalidOperationException("The data is a batch");
            }

            return _item;
        }

        /// <summary>Gets a collection of items for batch data.</summary>
        /// <returns>A collection of <see cref="JsonRpcItem{T}" /> objects.</returns>
        /// <exception cref="InvalidOperationException">The data is empty or is not a batch.</exception>
        public IReadOnlyList<JsonRpcItem<T>> GetBatchItems()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("The data is empty");
            }
            if (!IsBatch)
            {
                throw new InvalidOperationException("The data is not a batch");
            }

            return _items;
        }

        /// <summary>Gets a value indicating whether the data is a batch.</summary>
        public bool IsBatch => _items != null;

        /// <summary>Gets a value indicating whether the data is empty.</summary>
        public bool IsEmpty => object.Equals(_item, default(JsonRpcItem<T>)) && (_items == null);
    }
}