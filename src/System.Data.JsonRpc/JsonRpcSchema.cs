using System.Collections.Generic;

namespace System.Data.JsonRpc
{
    /// <summary>Specifies the types schema for deserializing JSON-RPC messages.</summary>
    public sealed class JsonRpcSchema
    {
        /// <summary>Initializes a new instance of the <see cref="JsonRpcSchema" /> class.</summary>
        public JsonRpcSchema()
        {
        }

        internal JsonRpcSchema Clone()
        {
            return new JsonRpcSchema
            {
                ErrorDataTypeGeneric = ErrorDataTypeGeneric,
                ErrorDataTypeBindings = new Dictionary<string, Type>(ErrorDataTypeBindings, StringComparer.Ordinal),
                ParameterTypeBindings = new Dictionary<string, Type>(ParameterTypeBindings, StringComparer.Ordinal),
                ResultTypeBindings = new Dictionary<string, Type>(ResultTypeBindings, StringComparer.Ordinal),
                SupportedMethods = new HashSet<string>(SupportedMethods, StringComparer.Ordinal)
            };
        }

        /// <summary>Gets or sets generic error data type binding for deserializing a response.</summary>
        public Type ErrorDataTypeGeneric { get; set; }

        /// <summary>Gets error data type bindings for deserializing a response (method name / error object type).</summary>
        public IDictionary<string, Type> ErrorDataTypeBindings { get; private set; } =
            new Dictionary<string, Type>(StringComparer.Ordinal);

        /// <summary>Gets parameter type bindings for deserializing a request (method name / parameter object type).</summary>
        public IDictionary<string, Type> ParameterTypeBindings { get; private set; } =
            new Dictionary<string, Type>(StringComparer.Ordinal);

        /// <summary>Gets result type bindings for deserializing a response (method name / result object type).</summary>
        public IDictionary<string, Type> ResultTypeBindings { get; private set; } =
            new Dictionary<string, Type>(StringComparer.Ordinal);

        /// <summary>Gets supported methods for deserializing a request.</summary>
        public ICollection<string> SupportedMethods { get; private set; } =
            new HashSet<string>(StringComparer.Ordinal);
    }
}