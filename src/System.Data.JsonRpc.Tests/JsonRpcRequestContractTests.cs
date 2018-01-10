using System.Collections.Generic;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcRequestContractTests
    {
        [Fact]
        public void ConstructorWhenParamsByPositionSchemeIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcRequestContract(default(IReadOnlyList<Type>)));
        }

        [Fact]
        public void ConstructorWhenParamsByNameSchemeIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcRequestContract(default(IReadOnlyDictionary<string, Type>)));
        }

        [Fact]
        public void ParamsTypeIsNone()
        {
            var contract = JsonRpcRequestContract.Default;

            Assert.Equal(JsonRpcParamsType.None, contract.ParamsType);
        }

        [Fact]
        public void ParamsTypeIsByPosition()
        {
            var parameters = new[] { typeof(long) };
            var contract = new JsonRpcRequestContract(parameters);

            Assert.Equal(JsonRpcParamsType.ByPosition, contract.ParamsType);
        }

        [Fact]
        public void ParamsTypeIsByName()
        {
            var parameters = new Dictionary<string, Type> { ["p"] = typeof(long) };
            var contract = new JsonRpcRequestContract(parameters);

            Assert.Equal(JsonRpcParamsType.ByName, contract.ParamsType);
        }
    }
}