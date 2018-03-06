namespace System.Data.JsonRpc
{
    /// <summary>Represents an RPC message.</summary>
    public abstract class JsonRpcMessage
    {
        private readonly JsonRpcId _id;

        internal JsonRpcMessage(in JsonRpcId id)
        {
            _id = id;
        }

        /// <summary>Gets an identifier.</summary>
        public ref readonly JsonRpcId Id
        {
            get => ref _id;
        }
    }
}