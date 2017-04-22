namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC message identifier type.</summary>
    public enum JsonRpcIdType
    {
        /// <summary>Identifier is not specified.</summary>
        None,

        /// <summary>Identifier is a number.</summary>
        Number,

        /// <summary>Identifier is a string.</summary>
        String
    }
}