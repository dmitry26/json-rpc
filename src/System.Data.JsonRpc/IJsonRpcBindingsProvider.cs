namespace System.Data.JsonRpc
{
    /// <summary>Request identifier to method name bindings provider used to map response properties to the corresponding types.</summary>
    public interface IJsonRpcBindingsProvider
    {
        /// <summary>Gets the method name associated with the specified request identifier.</summary>
        /// <param name="id">The request identifier whose method name to get.</param>
        /// <param name="method">When this method returns, the method name associated with the specified request identifier, if the request identifier is found; otherwise, <see langword="null" />. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true" /> if the object that implements <see cref="IJsonRpcBindingsProvider" /> contains a method name binding to the specified request identifier; otherwise, <see langword="false" />.</returns>
        bool TryGetBinding(string id, out string method);

        /// <summary>Gets the method name associated with the specified request identifier.</summary>
        /// <param name="id">The request identifier whose method name to get.</param>
        /// <param name="method">When this method returns, the method name associated with the specified request identifier, if the request identifier is found; otherwise, <see langword="null" />. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true" /> if the object that implements <see cref="IJsonRpcBindingsProvider" /> contains a method name binding to the specified request identifier; otherwise, <see langword="false" />.</returns>
        bool TryGetBinding(long id, out string method);
    }
}