using System.Data.JsonRpc.Tests.Resources;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

namespace System.Data.JsonRpc.Tests.Support
{
    /// <summary>Support tools for JSON.</summary>
    internal static class JsonTools
    {
        /// <summary>Compares the values of two JSON strings.</summary>
        /// <param name="jsonString1">The first JSON string to compare.</param>
        /// <param name="jsonString2">The second JSON string to compare.</param>
        /// <returns><see langword="true" /> if the tokens are equal; otherwise <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareJsonStrings(string jsonString1, string jsonString2) =>
            JToken.DeepEquals(JToken.Parse(jsonString1), JToken.Parse(jsonString2));

        /// <summary>Returns the content of the specified JSON-RPC sample.</summary>
        /// <param name="name">The name of the JSON-RPC sample to retrieve.</param>
        /// <returns>The content of the JSON-RPC sample.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetJsonSample(string name) =>
            EmbeddedResourceManager.GetString(FormattableString.Invariant($"Assets.{name}.txt"));
    }
}