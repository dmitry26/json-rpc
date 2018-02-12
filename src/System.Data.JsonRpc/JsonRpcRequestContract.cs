using System.Collections.Generic;
using System.Data.JsonRpc.Resources;

namespace System.Data.JsonRpc
{
    /// <summary>Specifies a type contract for request deserialization.</summary>
    public sealed class JsonRpcRequestContract
    {
        /// <summary>Initializes a new instance of the <see cref="JsonRpcRequestContract" /> class.</summary>
        public JsonRpcRequestContract()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcRequestContract" /> class.</summary>
        /// <param name="parameters">The contract for parameters, provided by position.</param>
        /// <exception cref="ArgumentException"><paramref name="parameters" /> is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameters" /> is <see langword="null" />.</exception>
        public JsonRpcRequestContract(IReadOnlyList<Type> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            if (parameters.Count == 0)
            {
                throw new ArgumentException(Strings.GetString("request.params.invalid_count"), nameof(parameters));
            }

            ParametersByPosition = parameters;
            ParametersType = JsonRpcParametersType.ByPosition;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcRequestContract" /> class.</summary>
        /// <param name="parameters">The contract for parameters, provided by name.</param>
        /// <exception cref="ArgumentException"><paramref name="parameters" /> is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameters" /> is <see langword="null" />.</exception>
        public JsonRpcRequestContract(IReadOnlyDictionary<string, Type> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            if (parameters.Count == 0)
            {
                throw new ArgumentException(Strings.GetString("request.params.invalid_count"), nameof(parameters));
            }

            ParametersByName = parameters;
            ParametersType = JsonRpcParametersType.ByName;
        }

        /// <summary>Gets a contract for parameters, provided by position.</summary>
        public IReadOnlyList<Type> ParametersByPosition
        {
            get;
        }

        /// <summary>Gets a contract for parameters, provided by name.</summary>
        public IReadOnlyDictionary<string, Type> ParametersByName
        {
            get;
        }

        /// <summary>Gets parameters type.</summary>
        public JsonRpcParametersType ParametersType
        {
            get;
        }
    }
}