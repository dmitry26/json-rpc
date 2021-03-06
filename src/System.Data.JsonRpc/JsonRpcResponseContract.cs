﻿namespace System.Data.JsonRpc
{
    /// <summary>Specifies a type contract for response deserialization.</summary>
    public sealed class JsonRpcResponseContract
    {
        private readonly Type _resultType;
        private readonly Type _errorDataType;

        /// <summary>Initializes a new instance of the <see cref="JsonRpcResponseContract" /> class.</summary>
        /// <param name="resultType">The type of method result.</param>
        /// <param name="errorDataType">The type of method error data.</param>
        public JsonRpcResponseContract(Type resultType = null, Type errorDataType = null)
        {
            _resultType = resultType;
            _errorDataType = errorDataType;
        }

        /// <summary>Gets a type of method result object.</summary>
        public Type ResultType
        {
            get => _resultType;
        }

        /// <summary>Gets a type of method error data object.</summary>
        public Type ErrorDataType
        {
            get => _errorDataType;
        }
    }
}