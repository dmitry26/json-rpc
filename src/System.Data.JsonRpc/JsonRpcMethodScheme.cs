namespace System.Data.JsonRpc
{
    /// <summary>Specifies type scheme for an RPC method.</summary>
    public sealed class JsonRpcMethodScheme
    {
        /// <summary>Specifies method scheme without parameters for deserializing requests.</summary>
        public static readonly JsonRpcMethodScheme Empty = new JsonRpcMethodScheme();

        private JsonRpcMethodScheme()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcMethodScheme" /> class.</summary>
        /// <param name="parametersType">The parameters type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="parametersType" /> is <see langword="null" />.</exception>
        public JsonRpcMethodScheme(Type parametersType)
        {
            if (parametersType == null)
            {
                throw new ArgumentNullException(nameof(parametersType));
            }

            ParametersType = parametersType;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcMethodScheme" /> class.</summary>
        /// <param name="resultType">The result type.</param>
        /// <param name="errorDataType">The error data type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="resultType" /> or <paramref name="errorDataType" /> is <see langword="null" />.</exception>
        public JsonRpcMethodScheme(Type resultType, Type errorDataType)
        {
            if (resultType == null)
            {
                throw new ArgumentNullException(nameof(resultType));
            }
            if (errorDataType == null)
            {
                throw new ArgumentNullException(nameof(errorDataType));
            }

            ResultType = resultType;
            ErrorDataType = errorDataType;
        }

        /// <summary>Gets an error data type.</summary>
        public Type ErrorDataType { get; }

        /// <summary>Gets a parameters type.</summary>
        public Type ParametersType { get; }

        /// <summary>Gets a result type.</summary>
        public Type ResultType { get; }
    }
}