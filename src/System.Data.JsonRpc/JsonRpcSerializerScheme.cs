using System.Collections.Generic;

namespace System.Data.JsonRpc
{
    /// <summary>Specifies type scheme for deserializing RPC messages.</summary>
    public sealed class JsonRpcSerializerScheme
    {
        /// <summary>Initializes a new instance of the <see cref="JsonRpcSerializerScheme" /> class.</summary>
        public JsonRpcSerializerScheme()
        {
        }

        /// <summary>Gets or sets generic error data type binding for deserializing a response.</summary>
        public Type GenericErrorDataType { get; set; }

        /// <summary>
        /// Gets methods schemes.
        /// </summary>
        public IDictionary<string, JsonRpcMethodScheme> Methods { get; } = new Dictionary<string, JsonRpcMethodScheme>(StringComparer.Ordinal);
    }
}