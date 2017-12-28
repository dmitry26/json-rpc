using System.Collections.Generic;

namespace System.Data.JsonRpc
{
    /// <summary>Specifies type scheme for deserializing RPC messages.</summary>
    public sealed class JsonRpcSerializerScheme
    {
        /// <summary>Initializes a new instance of the <see cref="JsonRpcSerializerScheme" /> class.</summary>
        public JsonRpcSerializerScheme()
        {
            Methods = new Dictionary<string, JsonRpcMethodScheme>(StringComparer.Ordinal);
        }

        /// <summary>Gets schemes for deserializing methods.</summary>
        public IDictionary<string, JsonRpcMethodScheme> Methods
        {
            get;
        }

        /// <summary>Gets or sets a type of default error data for deserializing a response.</summary>
        public Type DefaultErrorDataType
        {
            get;
            set;
        }
    }
}