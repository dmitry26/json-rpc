using Newtonsoft.Json;

namespace System.Data.JsonRpc
{
    /// <summary>Specifies the settings for a <see cref="JsonRpcSerializer" /> object.</summary>
    public sealed class JsonRpcSerializerSettings
    {
        /// <summary>Initializes a new instance of the <see cref="JsonRpcSerializerSettings" /> class.</summary>
        public JsonRpcSerializerSettings()
        {
        }

        /// <summary>Gets or sets an optional <see cref="JsonSerializer" /> for serializing and deserializing JSON.</summary>
        public JsonSerializer JsonSerializer
        {
            get;
            set;
        }

        /// <summary>Gets or sets an optional character buffer pool for serializing and deserializing JSON.</summary>
        public IArrayPool<char> JsonSerializerBufferPool
        {
            get;
            set;
        }
    }
}