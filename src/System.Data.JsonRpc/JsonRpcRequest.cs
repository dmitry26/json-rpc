using System.Collections.Generic;
using System.Data.JsonRpc.Resources;
using System.Diagnostics;

namespace System.Data.JsonRpc
{
    /// <summary>Represents an RPC request message.</summary>
    [DebuggerDisplay("Method = {Method}, Id = {Id}, ParamsType = {ParamsType}")]
    public sealed class JsonRpcRequest : JsonRpcMessage
    {
        /// <summary>Initializes a new instance of the <see cref="JsonRpcRequest" /> class.</summary>
        /// <param name="method">The string containing the name of the method to be invoked.</param>
        /// <param name="id">The identifier established by the client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> is <see langword="null" />.</exception>
        public JsonRpcRequest(string method, in JsonRpcId id = default)
            : base(id)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            Method = method;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcRequest" /> class.</summary>
        /// <param name="method">The string containing the name of the method to be invoked.</param>
        /// <param name="id">The identifier established by the client.</param>
        /// <param name="params">The parameters to be used during the invocation of the method, provided by position.</param>
        /// <exception cref="ArgumentException"><paramref name="params" /> is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="params" /> is <see langword="null" />.</exception>
        public JsonRpcRequest(string method, in JsonRpcId id, IReadOnlyList<object> @params)
            : this(method, id)
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

        /// <summary>Initializes a new instance of the <see cref="JsonRpcRequest" /> class.</summary>
        /// <param name="method">The string containing the name of the method to be invoked.</param>
        /// <param name="params">The parameters to be used during the invocation of the method, provided by position.</param>
        /// <exception cref="ArgumentException"><paramref name="params" /> is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="params" /> is <see langword="null" />.</exception>
        public JsonRpcRequest(string method, IReadOnlyList<object> @params)
            : this(method, default, @params)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcRequest" /> class.</summary>
        /// <param name="method">The string containing the name of the method to be invoked.</param>
        /// <param name="id">The identifier established by the client.</param>
        /// <param name="params">The parameters to be used during the invocation of the method, provided by name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="params" /> is <see langword="null" />.</exception>
        public JsonRpcRequest(string method, in JsonRpcId id, IReadOnlyDictionary<string, object> @params)
            : this(method, id)
        {
            if (@params == null)
            {
                throw new ArgumentNullException(nameof(@params));
            }

            ParamsByName = @params;
            ParamsType = JsonRpcParamsType.ByName;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcRequest" /> class.</summary>
        /// <param name="method">The string containing the name of the method to be invoked.</param>
        /// <param name="params">The parameters to be used during the invocation of the method, provided by name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="params" /> is <see langword="null" />.</exception>
        public JsonRpcRequest(string method, IReadOnlyDictionary<string, object> @params)
            : this(method, default, @params)
        {
        }

        /// <summary>Checks whether the method is a system extension method.</summary>
        /// <param name="method">The method name.</param>
        /// <returns><see langword="true" /> if the specified method is a system extension method; otherwise, <see langword="false" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> is <see langword="null" />.</exception>
        public static bool IsSystemMethod(string method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            // Case is not defined explicitly by the specification, and thus is ignored in comparison

            return method.StartsWith("rpc.", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Gets a string containing the name of the method to be invoked.</summary>
        public string Method
        {
            get;
        }

        /// <summary>Gets parameters type.</summary>
        public JsonRpcParamsType ParamsType
        {
            get;
        }

        /// <summary>Gets parameters, provided by position.</summary>
        public IReadOnlyList<object> ParamsByPosition
        {
            get;
        }

        /// <summary>Gets parameters, provided by name.</summary>
        public IReadOnlyDictionary<string, object> ParamsByName
        {
            get;
        }

        /// <summary>Gets a value indicating whether the message is a notification.</summary>
        public bool IsNotification
        {
            get => Id.Type == JsonRpcIdType.None;
        }

        /// <summary>Gets a value indicating whether the message is a system extension.</summary>
        public bool IsSystem
        {
            get => IsSystemMethod(Method);
        }
    }
}