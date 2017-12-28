using System.Collections.Generic;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcMethodSchemeTests
    {
        [Fact]
        public void ConstructorWhenParamsByPositionSchemeIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcMethodScheme(default(IReadOnlyList<Type>)));
        }

        [Fact]
        public void ConstructorWhenParamsByNameSchemeIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcMethodScheme(default(IReadOnlyDictionary<string, Type>)));
        }

        [Fact]
        public void ParamsTypeIsNone()
        {
            var scheme = new JsonRpcMethodScheme();

            Assert.Equal(JsonRpcParamsType.None, scheme.ParamsType);
        }

        [Fact]
        public void ParamsTypeIsByPosition()
        {
            var scheme = new JsonRpcMethodScheme(new[] { typeof(long) });

            Assert.Equal(JsonRpcParamsType.ByPosition, scheme.ParamsType);
        }

        [Fact]
        public void ParamsTypeIsByName()
        {
            var scheme = new JsonRpcMethodScheme(new Dictionary<string, Type> { ["p"] = typeof(long) });

            Assert.Equal(JsonRpcParamsType.ByName, scheme.ParamsType);
        }
    }
}