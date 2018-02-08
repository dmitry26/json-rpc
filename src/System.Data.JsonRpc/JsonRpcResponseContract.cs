namespace System.Data.JsonRpc
{
    /// <summary>Specifies a type contract for response deserialization.</summary>
    public sealed class JsonRpcResponseContract
    {
        /// <summary>Initializes a new instance of the <see cref="JsonRpcResponseContract" /> class.</summary>
        /// <param name="resultType">The type of method result.</param>
        /// <param name="errorDataType">The type of method error data.</param>
        public JsonRpcResponseContract(Type resultType = null, Type errorDataType = null)
        {
            ResultType = resultType;
            ErrorDataType = errorDataType;
        }

        /// <summary>Gets a type of method result object.</summary>
        public Type ResultType
        {
            get;
        }

        /// <summary>Gets a type of method error data object.</summary>
        public Type ErrorDataType
        {
            get;
        }
    }
}