namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC message.</summary>
    public abstract class JsonRpcMessage
    {
        internal JsonRpcMessage()
        {
        }

        internal JsonRpcMessage(long id)
        {
            IdNumber = id;
            IdType = JsonRpcIdType.Number;
        }

        internal JsonRpcMessage(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (id.Length == 0)
                throw new ArgumentException("Value is an empty string", nameof(id));

            IdString = id;
            IdType = JsonRpcIdType.String;
        }

        /// <summary>Gets RPC identifier as number if identifier is a number; throws an exception otherwise.</summary>
        /// <returns>An RPC identifier.</returns>
        /// <exception cref="JsonRpcException">RPC identifier is not a number.</exception>
        public long GetIdAsNumber() =>
            IdType == JsonRpcIdType.Number ? IdNumber : throw new JsonRpcException(JsonRpcExceptionType.GenericError, "Id is not a number");

        /// <summary>Gets RPC identifier as string if identifier is a string; throws an exception otherwise.</summary>
        /// <returns>An RPC identifier.</returns>
        /// <exception cref="JsonRpcException">RPC identifier is not a string.</exception>
        public string GetIdAsString() =>
            IdType == JsonRpcIdType.String ? IdString : throw new JsonRpcException(JsonRpcExceptionType.GenericError, "Id is not a string");

        internal long IdNumber { get; set; }

        internal string IdString { get; set; }

        /// <summary>Gets RPC identifier.</summary>
        public object Id
        {
            get
            {
                switch (IdType)
                {
                    case JsonRpcIdType.Number:
                        return IdNumber;
                    case JsonRpcIdType.String:
                        return IdString;
                    default:
                        return null;
                }
            }
        }

        /// <summary>Gets identifier type.</summary>
        public JsonRpcIdType IdType { get; internal set; }
    }
}