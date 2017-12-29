using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcErrorTests
    {
        [Theory]
        [InlineData(-32769L, JsonRpcErrorType.Undefined)]
        [InlineData(-32768L, JsonRpcErrorType.System)]
        [InlineData(-32767L, JsonRpcErrorType.System)]
        [InlineData(-32700L, JsonRpcErrorType.Parsing)]
        [InlineData(-32603L, JsonRpcErrorType.Internal)]
        [InlineData(-32602L, JsonRpcErrorType.InvalidParams)]
        [InlineData(-32601L, JsonRpcErrorType.InvalidMethod)]
        [InlineData(-32600L, JsonRpcErrorType.InvalidRequest)]
        [InlineData(-32100L, JsonRpcErrorType.System)]
        [InlineData(-32099L, JsonRpcErrorType.Server)]
        [InlineData(-32098L, JsonRpcErrorType.Server)]
        [InlineData(-32001L, JsonRpcErrorType.Server)]
        [InlineData(-32000L, JsonRpcErrorType.Server)]
        [InlineData(-31999L, JsonRpcErrorType.Undefined)]
        [InlineData(-00001L, JsonRpcErrorType.Undefined)]
        [InlineData(+00000L, JsonRpcErrorType.Undefined)]
        [InlineData(+00001L, JsonRpcErrorType.Undefined)]
        public void HasProperType(long code, JsonRpcErrorType type)
        {
            var jsonRpcError = new JsonRpcError(code, "m");

            Assert.Equal(type, jsonRpcError.Type);
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
    }
}