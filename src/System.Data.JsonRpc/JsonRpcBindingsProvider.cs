using System.Collections.Generic;
using System.Diagnostics;

namespace System.Data.JsonRpc
{
    /// <summary>Request identifier to method name bindings provider used to map response properties to the corresponding types.</summary>
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public sealed class JsonRpcBindingsProvider : IJsonRpcBindingsProvider
    {
        private readonly IDictionary<string, string> _bindingsString = new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly IDictionary<long, string> _bindingsNumber = new Dictionary<long, string>();

        /// <summary>Initializes a new instance of the <see cref="JsonRpcBindingsProvider" /> class.</summary>
        public JsonRpcBindingsProvider()
        {
        }

        /// <summary>Removes all items from the <see cref="IJsonRpcBindingsProvider" />.</summary>
        public void ClearBindings()
        {
            _bindingsString.Clear();
            _bindingsNumber.Clear();
        }

        /// <summary>Removes the binding with the specified request identifier from the <see cref="IJsonRpcBindingsProvider" />.</summary>
        /// <param name="id">The request identifier of the binding to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="id" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="id" /> is empty string.</exception>
        public void RemoveBinding(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (id.Length == 0)
                throw new ArgumentException("Value is empty string", nameof(id));

            _bindingsString.Remove(id);
        }

        /// <summary>Removes the binding with the specified request identifier from the <see cref="IJsonRpcBindingsProvider" />.</summary>
        /// <param name="id">The request identifier of the binding to remove.</param>
        public void RemoveBinding(long id)
        {
            _bindingsNumber.Remove(id);
        }

        /// <summary>Adds an binding with the provided request identifier and method name to the <see cref="IJsonRpcBindingsProvider" />.</summary>
        /// <param name="id">The object to use as the request identifier of the binding to add.</param>
        /// <param name="method">The object to use as the method name of the binding to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="id" /> or <paramref name="method" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="id" /> or <paramref name="method" /> is empty string.</exception>
        public void SetBinding(string id, string method)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (id.Length == 0)
                throw new ArgumentException("Value is empty string", nameof(id));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (method.Length == 0)
                throw new ArgumentException("Value is empty string", nameof(method));

            _bindingsString[id] = method;
        }

        /// <summary>Adds an binding with the provided request identifier and method name to the <see cref="IJsonRpcBindingsProvider" />.</summary>
        /// <param name="id">The object to use as the request identifier of the binding to add.</param>
        /// <param name="method">The object to use as the method name of the binding to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="method" /> is empty string.</exception>
        public void SetBinding(long id, string method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (method.Length == 0)
                throw new ArgumentException("Value is empty string", nameof(method));

            _bindingsNumber[id] = method;
        }

        /// <summary>Gets the method name associated with the specified request identifier.</summary>
        /// <param name="id">The request identifier whose binding to get.</param>
        /// <param name="method">When this method returns, the method name associated with the specified request identifier, if the request identifier is found; otherwise, <see langword="null" />. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true" /> if the object that implements <see cref="IJsonRpcBindingsProvider" /> contains a method name binding to the specified request identifier; otherwise, <see langword="false" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="id" /> is empty string.</exception>
        public bool TryGetBinding(string id, out string method)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (id.Length == 0)
                throw new ArgumentException("Value is empty string", nameof(id));

            return _bindingsString.TryGetValue(id, out method);
        }

        /// <summary>Gets the method name associated with the specified request identifier.</summary>
        /// <param name="id">The request identifier whose binding to get.</param>
        /// <param name="method">When this method returns, the method name associated with the specified request identifier, if the request identifier is found; otherwise, <see langword="null" />. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true" /> if the object that implements <see cref="IJsonRpcBindingsProvider" /> contains a method name binding to the specified request identifier; otherwise, <see langword="false" />.</returns>
        public bool TryGetBinding(long id, out string method)
        {
            return _bindingsNumber.TryGetValue(id, out method);
        }

        /// <summary>Gets the number of bindings contained in the <see cref="IJsonRpcBindingsProvider" />.</summary>
        public int Count => _bindingsNumber.Count + _bindingsString.Count;
    }
}