using System.Diagnostics;

namespace System.Data.JsonRpc
{
    /// <summary>Represents an RPC request message.</summary>
    [DebuggerDisplay("Id = {" + nameof(Id) + "}, Method = {" + nameof(Method) + "}, Has Params = {" + nameof(HasParams) + "}")]
    public sealed class JsonRpcRequest : JsonRpcMessage
    {
        internal JsonRpcRequest()
            : base()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcRequest" /> class.</summary>
        /// <param name="method">The string containing the name of the method to be invoked.</param>
        /// <param name="id">The identifier established by the client.</param>
        /// <param name="params">The structured value that holds the parameter values to be used during the invocation of the method.</param>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="method" /> is empty string.</exception>
        public JsonRpcRequest(string method, JsonRpcId id, object @params = null)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (method.Length == 0)
            {
                throw new ArgumentException("Value is an empty string", nameof(method));
            }

            Method = method;
            Id = id;
            Params = @params;
        }

        /// <summary>Gets a value indicating whether the response has parameters.</summary>
        public bool HasParams => Params != null;

        /// <summary>Gets a value indicating whether the message is a notification message.</summary>
        public bool IsNotification => Id.Type == JsonRpcIdType.None;

        /// <summary>Gets a value indicating whether the message is a system message.</summary>
        public bool IsSystem => Method.StartsWith("rpc.", StringComparison.Ordinal);

        /// <summary>Gets a string containing the name of the method to be invoked.</summary>
        public string Method { get; internal set; }

        /// <summary>Gets a structured value that holds the parameter values to be used during the invocation of the method.</summary>
        public object Params { get; internal set; }
    }
}