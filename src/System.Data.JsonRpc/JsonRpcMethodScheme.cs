using System.Collections.Generic;

namespace System.Data.JsonRpc
{
    /// <summary>Specifies type scheme for an RPC method.</summary>
    public sealed class JsonRpcMethodScheme
    {
        /// <summary>Initializes a new instance of the <see cref="JsonRpcMethodScheme" /> class.</summary>
        public JsonRpcMethodScheme()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcMethodScheme" /> class.</summary>
        /// <param name="paramsScheme">The scheme for parameters, provided by position.</param>
        /// <exception cref="ArgumentNullException"><paramref name="paramsScheme" /> is <see langword="null" />.</exception>
        public JsonRpcMethodScheme(IReadOnlyList<Type> paramsScheme)
        {
            if (paramsScheme == null)
            {
                throw new ArgumentNullException(nameof(paramsScheme));
            }

            ParamsByPositionScheme = paramsScheme;
            ParamsType = JsonRpcParamsType.ByPosition;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcMethodScheme" /> class.</summary>
        /// <param name="paramsScheme">The scheme for parameters, provided by name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="paramsScheme" /> is <see langword="null" />.</exception>
        public JsonRpcMethodScheme(IReadOnlyDictionary<string, Type> paramsScheme)
        {
            if (paramsScheme == null)
            {
                throw new ArgumentNullException(nameof(paramsScheme));
            }

            ParamsByNameScheme = paramsScheme;
            ParamsType = JsonRpcParamsType.ByName;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcMethodScheme" /> class.</summary>
        /// <param name="resultType">The type of method result.</param>
        /// <param name="errorDataType">The type of method error data.</param>
        public JsonRpcMethodScheme(Type resultType, Type errorDataType = null)
        {
            ResultType = resultType;
            ErrorDataType = errorDataType;
        }

        /// <summary>Gets a scheme for parameters, provided by position, for deserializing a request.</summary>
        public IReadOnlyList<Type> ParamsByPositionScheme
        {
            get;
        }

        /// <summary>Gets a scheme for parameters, provided by name, for deserializing a request.</summary>
        public IReadOnlyDictionary<string, Type> ParamsByNameScheme
        {
            get;
        }

        /// <summary>Gets parameters type for deserializing a request.</summary>
        public JsonRpcParamsType ParamsType
        {
            get;
        }

        /// <summary>Gets a type of method result for deserializing a response.</summary>
        public Type ResultType
        {
            get;
        }

        /// <summary>Gets a type of method error data for deserializing a response.</summary>
        public Type ErrorDataType
        {
            get;
        }
    }
}