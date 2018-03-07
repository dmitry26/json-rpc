using System.Collections.Generic;

namespace System.Data.JsonRpc
{
    /// <summary>Represents deserialized RPC data.</summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    public sealed class JsonRpcData<T>
        where T : JsonRpcMessage
    {
        private readonly JsonRpcItem<T> _item;
        private readonly IReadOnlyList<JsonRpcItem<T>> _items;

        internal JsonRpcData(in JsonRpcItem<T> item)
        {
            _item = item;
        }

        internal JsonRpcData(IReadOnlyList<JsonRpcItem<T>> items)
        {
            _items = items;
        }

        /// <summary>Gets a value indicating whether the data is a batch.</summary>
        public bool IsBatch
        {
            get => Items != null;
        }

        /// <summary>Gets an item for non-batch data.</summary>
        public ref readonly JsonRpcItem<T> Item
        {
            get => ref _item;
        }

        /// <summary>Gets a collection of items for batch data.</summary>
        public IReadOnlyList<JsonRpcItem<T>> Items
        {
            get => _items;
        }
    }
}