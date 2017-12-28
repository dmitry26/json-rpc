namespace System.Data.JsonRpc
{
    /// <summary>Represents an RPC message.</summary>
    public abstract class JsonRpcMessage
    {
        internal JsonRpcMessage(in JsonRpcId id)
        {
            Id = id;
        }

        /// <summary>Gets an identifier.</summary>
        public JsonRpcId Id
        {
            get;
        }
    }
}