namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC method parameters type.</summary>
    public enum JsonRpcParamsType
    {
        /// <summary>No parameters.</summary>
        None,

        /// <summary>Parameters are provided by position.</summary>
        ByPosition,

        /// <summary>Parameters are provided by name.</summary>
        ByName
    }
}