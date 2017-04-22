namespace System.Data.JsonRpc
{
    /// <summary>Represents an RPC message.</summary>
    public abstract class JsonRpcMessage
    {
        internal JsonRpcMessage()
        {
        }

        /// <summary>Gets an identifier.</summary>
        public JsonRpcId Id { get; internal set; }
    }
}