using System.Collections.Generic;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcRequestContractTests
    {
        [Fact]
        public void ParametersTypeIsByPositionWhenIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcRequestContract(default(IReadOnlyList<Type>)));
        }

        [Fact]
        public void ParametersTypeIsByNameWhenIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcRequestContract(default(IReadOnlyDictionary<string, Type>)));
        }

        [Fact]
        public void ParametersTypeIsNone()
        {
            var contract = new JsonRpcRequestContract();

            Assert.Equal(JsonRpcParametersType.None, contract.ParametersType);
        }

        [Fact]
        public void ParametersTypeIsByPosition()
        {
            var parameters = new[] { typeof(long) };
            var contract = new JsonRpcRequestContract(parameters);

            Assert.Equal(JsonRpcParametersType.ByPosition, contract.ParametersType);
        }

        [Fact]
        public void ParametersTypeIsByName()
        {
            var parameters = new Dictionary<string, Type> { ["p"] = typeof(long) };
            var contract = new JsonRpcRequestContract(parameters);

            Assert.Equal(JsonRpcParametersType.ByName, contract.ParametersType);
        }

        [Fact]
        public void ParametersTypeIsByPositionWhenCountIsZero()
        {
            var parameters = new Type[] { };

            Assert.Throws<ArgumentException>(() =>
                new JsonRpcRequestContract(parameters));
        }

        [Fact]
        public void ParametersTypeIsByNameWhenCountIsZero()
        {
            var parameters = new Dictionary<string, Type> { };

            Assert.Throws<ArgumentException>(() =>
                new JsonRpcRequestContract(parameters));
        }
    }
}