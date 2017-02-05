using Newtonsoft.Json;

namespace System.Data.JsonRpc
{
    /// <summary>Specifies the settings for a <see cref="JsonRpcSerializer" /> object.</summary>
    public sealed class JsonRpcSettings
    {
        /// <summary>Initializes a new instance of the <see cref="JsonRpcSettings" /> class.</summary>
        public JsonRpcSettings()
        {
        }

        /// <summary>Gets or sets an optional <see cref="JsonSerializer" /> for serializing and deserializing JSON.</summary>
        public JsonSerializer JsonSerializer { get; set; }

        /// <summary>Gets or sets an optional character buffer pool for serializing and deserializing JSON.</summary>
        public IArrayPool<char> JsonSerializerArrayPool { get; set; }
    }
}