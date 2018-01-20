using System.Collections.Generic;
using System.Diagnostics;

namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC data.</summary>
    /// <typeparam name="T">Type of the message.</typeparam>
    [DebuggerDisplay("IsEmpty = {IsEmpty}, IsBatch = {IsBatch}")]
    public sealed class JsonRpcData<T>
        where T : JsonRpcMessage
    {
        internal JsonRpcData()
        {
        }

        internal JsonRpcData(in JsonRpcItem<T> singleItem)
        {
            SingleItem = singleItem;
        }

        internal JsonRpcData(IReadOnlyList<JsonRpcItem<T>> batchItems)
        {
            BatchItems = batchItems;
        }

        /// <summary>Gets a value indicating whether the data is a batch.</summary>
        public bool IsBatch
        {
            get => BatchItems != null;
        }

        /// <summary>Gets a value indicating whether the data is empty.</summary>
        public bool IsEmpty
        {
            get => !IsSingle && !IsBatch;
        }

        /// <summary>Gets a value indicating whether the data is a single item.</summary>
        public bool IsSingle
        {
            get => (SingleItem.Message != null) || (SingleItem.Exception != null);
        }

        /// <summary>Gets an item for non-batch data.</summary>
        public JsonRpcItem<T> SingleItem
        {
            get;
        }

        /// <summary>Gets a collection of items for batch data.</summary>
        public IReadOnlyList<JsonRpcItem<T>> BatchItems
        {
            get;
        }
    }
}