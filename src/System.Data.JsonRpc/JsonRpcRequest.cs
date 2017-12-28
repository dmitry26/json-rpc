﻿using System.Collections.Generic;
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
        public JsonRpcRequest(string method, in JsonRpcId id)
            : base(in id)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            Method = method;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcRequest" /> class.</summary>
        /// <param name="method">The string containing the name of the method to be invoked.</param>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> is <see langword="null" />.</exception>
        public JsonRpcRequest(string method)
            : this(method, in JsonRpcId.None)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcRequest" /> class.</summary>
        /// <param name="method">The string containing the name of the method to be invoked.</param>
        /// <param name="id">The identifier established by the client.</param>
        /// <param name="params">The parameters to be used during the invocation of the method, provided by position.</param>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="params" /> is <see langword="null" />.</exception>
        public JsonRpcRequest(string method, in JsonRpcId id, IReadOnlyList<object> @params)
            : this(method, in id)
        {
            if (@params == null)
            {
                throw new ArgumentNullException(nameof(@params));
            }

            ParamsByPosition = @params;
            ParamsType = JsonRpcParamsType.ByPosition;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcRequest" /> class.</summary>
        /// <param name="method">The string containing the name of the method to be invoked.</param>
        /// <param name="params">The parameters to be used during the invocation of the method, provided by position.</param>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="params" /> is <see langword="null" />.</exception>
        public JsonRpcRequest(string method, IReadOnlyList<object> @params)
            : this(method, in JsonRpcId.None, @params)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcRequest" /> class.</summary>
        /// <param name="method">The string containing the name of the method to be invoked.</param>
        /// <param name="id">The identifier established by the client.</param>
        /// <param name="params">The parameters to be used during the invocation of the method, provided by name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="params" /> is <see langword="null" />.</exception>
        public JsonRpcRequest(string method, in JsonRpcId id, IReadOnlyDictionary<string, object> @params)
            : this(method, in id)
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
            : this(method, in JsonRpcId.None, @params)
        {
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

        /// <summary>Gets a value indicating whether the message is a notification message.</summary>
        public bool IsNotification
        {
            get => Id.Type == JsonRpcIdType.None;
        }

        /// <summary>Gets a value indicating whether the message is a system message.</summary>
        public bool IsSystem
        {
            get => Method.StartsWith("rpc.", StringComparison.Ordinal);
        }
    }
}