// Copyright (c) DMO Consulting LLC. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using Utf8Json;

namespace System.Data.JsonRpc
{
    internal class JsonRpcResponseFormatterResolver : IJsonFormatterResolver
    {
        public JsonRpcResponseFormatterResolver(
            IJsonFormatterResolver resolver,
            IDictionary<string,JsonRpcResponseContract> responseContracts = null,
            IDictionary<JsonRpcId,string> staticResponseBindings = null,
            IDictionary<JsonRpcId,JsonRpcResponseContract> dynamicResponseBindings = null,
            Type defaultErrorDataType = null)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _responseContracts = responseContracts ?? new Dictionary<string,JsonRpcResponseContract>(StringComparer.Ordinal);
            _staticResponseBindings = staticResponseBindings ?? new Dictionary<JsonRpcId,string>();
            _dynamicResponseBindings = dynamicResponseBindings ?? new Dictionary<JsonRpcId,JsonRpcResponseContract>();
            DefaultErrorDataType = defaultErrorDataType;
        }

        private readonly IJsonFormatterResolver _resolver;
        private readonly IDictionary<string,JsonRpcResponseContract> _responseContracts;
        private readonly IDictionary<JsonRpcId,string> _staticResponseBindings;
        private readonly IDictionary<JsonRpcId,JsonRpcResponseContract> _dynamicResponseBindings;

        public Type DefaultErrorDataType { get; }

        public IJsonFormatter<T> GetFormatter<T>() => _resolver.GetFormatter<T>();

        public JsonRpcResponseContract GetContract(in JsonRpcId identifier)
        {
            if (!_dynamicResponseBindings.TryGetValue(identifier,out var contract))
            {
                if (_staticResponseBindings.TryGetValue(identifier,out var method) && (method != null))
                {
                    _responseContracts.TryGetValue(method,out contract);
                }
            }

            return contract;
        }
    }

    internal class JsonRpcRequestFormatterResolver : IJsonFormatterResolver
    {
        public JsonRpcRequestFormatterResolver(
            IJsonFormatterResolver resolver,
            IDictionary<string,JsonRpcRequestContract> requestContracts = null)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _requestContracts = requestContracts ?? new Dictionary<string,JsonRpcRequestContract>(StringComparer.Ordinal);
        }

        private readonly IJsonFormatterResolver _resolver;
        private readonly IDictionary<string,JsonRpcRequestContract> _requestContracts;

        public IJsonFormatter<T> GetFormatter<T>() => _resolver.GetFormatter<T>();

        public bool TryGetContract(string method,out JsonRpcRequestContract contract) =>
            _requestContracts.TryGetValue(method,out contract);
    }
}
