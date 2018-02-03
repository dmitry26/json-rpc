using System.Collections.Generic;
using System.Data.JsonRpc.Resources;

namespace System.Data.JsonRpc
{
    /// <summary>Specifies a type contract for request deserialization.</summary>
    public sealed class JsonRpcRequestContract
    {
        private static readonly JsonRpcRequestContract _empty = new JsonRpcRequestContract();

        /// <summary>Initializes a new instance of the <see cref="JsonRpcRequestContract" /> class.</summary>
        public JsonRpcRequestContract()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcRequestContract" /> class.</summary>
        /// <param name="params">The contract for parameters, provided by position.</param>
        /// <exception cref="ArgumentException"><paramref name="params" /> is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="params" /> is <see langword="null" />.</exception>
        public JsonRpcRequestContract(IReadOnlyList<Type> @params)
        {
            if (@params == null)
            {
                throw new ArgumentNullException(nameof(@params));
            }
            if (@params.Count == 0)
            {
                throw new ArgumentException(Strings.GetString("request.params.invalid_count"), nameof(@params));
            }

            ParamsByPosition = @params;
            ParamsType = JsonRpcParamsType.ByPosition;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcRequestContract" /> class.</summary>
        /// <param name="params">The contract for parameters, provided by name.</param>
        /// <exception cref="ArgumentException"><paramref name="params" /> is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="params" /> is <see langword="null" />.</exception>
        public JsonRpcRequestContract(IReadOnlyDictionary<string, Type> @params)
        {
            if (@params == null)
            {
                throw new ArgumentNullException(nameof(@params));
            }
            if (@params.Count == 0)
            {
                throw new ArgumentException(Strings.GetString("request.params.invalid_count"), nameof(@params));
            }

            ParamsByName = @params;
            ParamsType = JsonRpcParamsType.ByName;
        }

        /// <summary>Gets a default request contract.</summary>
        public static JsonRpcRequestContract Default
        {
            get => _empty;
        }

        /// <summary>Gets a contract for parameters, provided by position.</summary>
        public IReadOnlyList<Type> ParamsByPosition
        {
            get;
        }

        /// <summary>Gets a contract for parameters, provided by name.</summary>
        public IReadOnlyDictionary<string, Type> ParamsByName
        {
            get;
        }

        /// <summary>Gets parameters type.</summary>
        public JsonRpcParamsType ParamsType
        {
            get;
        }
    }
}