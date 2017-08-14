using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcErrorTests
    {
        [InlineData(-32769L, JsonRpcErrorType.CustomError)]
        [InlineData(-32768L, JsonRpcErrorType.SystemError)]
        [InlineData(-32767L, JsonRpcErrorType.SystemError)]
        [InlineData(-32700L, JsonRpcErrorType.ParseError)]
        [InlineData(-32603L, JsonRpcErrorType.InternalError)]
        [InlineData(-32602L, JsonRpcErrorType.InvalidParams)]
        [InlineData(-32601L, JsonRpcErrorType.InvalidMethod)]
        [InlineData(-32600L, JsonRpcErrorType.InvalidRequest)]
        [InlineData(-32100L, JsonRpcErrorType.SystemError)]
        [InlineData(-32099L, JsonRpcErrorType.ServerError)]
        [InlineData(-32098L, JsonRpcErrorType.ServerError)]
        [InlineData(-32001L, JsonRpcErrorType.ServerError)]
        [InlineData(-32000L, JsonRpcErrorType.ServerError)]
        [InlineData(-31999L, JsonRpcErrorType.CustomError)]
        [InlineData(-00001L, JsonRpcErrorType.CustomError)]
        [InlineData(+00000L, JsonRpcErrorType.CustomError)]
        [InlineData(+00001L, JsonRpcErrorType.CustomError)]
        [Theory]
        public void HasProperType(long code, JsonRpcErrorType type)
        {
            var jsonRpcError = new JsonRpcError(code, "test_message");

            Assert.Equal(type, jsonRpcError.Type);
        }

        [Fact]
        public void HasDataIsFalse()
        {
            var jsonRpcError = new JsonRpcError(100L, "test_message");

            Assert.False(jsonRpcError.HasData);
        }

        [Fact]
        public void HasDataIsTrue()
        {
            var jsonRpcError = new JsonRpcError(100L, "test_message", 200L);

            Assert.True(jsonRpcError.HasData);
        }

        [Fact]
        public void ConstructorWhenMessageIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcError(100L, default));
        }

        [Fact]
        public void ConstructorWhenMessageIsEmptyString()
        {
            new JsonRpcError(100L, string.Empty);
        }
    }
}