using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcErrorTests
    {
        [Theory]
        [InlineData(-32769L, JsonRpcErrorType.Undefined)]
        [InlineData(-32700L, JsonRpcErrorType.Parsing)]
        [InlineData(-32603L, JsonRpcErrorType.Internal)]
        [InlineData(-32602L, JsonRpcErrorType.InvalidParams)]
        [InlineData(-32601L, JsonRpcErrorType.InvalidMethod)]
        [InlineData(-32600L, JsonRpcErrorType.InvalidRequest)]
        [InlineData(-32099L, JsonRpcErrorType.Server)]
        [InlineData(-32098L, JsonRpcErrorType.Server)]
        [InlineData(-32001L, JsonRpcErrorType.Server)]
        [InlineData(-32000L, JsonRpcErrorType.Server)]
        [InlineData(-31999L, JsonRpcErrorType.Undefined)]
        [InlineData(-00001L, JsonRpcErrorType.Undefined)]
        [InlineData(+00000L, JsonRpcErrorType.Undefined)]
        [InlineData(+00001L, JsonRpcErrorType.Undefined)]
        public void ConstructorWhenCodeIsValid(long code, JsonRpcErrorType type)
        {
            var jsonRpcError = new JsonRpcError(code, "m");

            Assert.Equal(type, jsonRpcError.Type);
            Assert.False(jsonRpcError.HasData);
        }

        [Theory]
        [InlineData(-32768L)]
        [InlineData(-32101L)]
        public void ConstructorWhenCodeIsInvalid(long code)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new JsonRpcError(code, "m"));
        }

        [Fact]
        public void ConstructorWhenMessageIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcError(1L, null));
        }

        [Fact]
        public void ConstructorWhenMessageIsEmptyString()
        {
            new JsonRpcError(1L, string.Empty);
        }

        [Fact]
        public void HasDataIsTrue()
        {
            var jsonRpcError = new JsonRpcError(1L, "m", null);

            Assert.True(jsonRpcError.HasData);
        }
    }
}