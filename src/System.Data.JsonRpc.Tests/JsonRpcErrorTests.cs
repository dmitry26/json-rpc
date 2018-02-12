using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcErrorTests
    {
        [Theory]
        [InlineData(JsonRpcErrorCode.StandardErrorsLowerBoundary - 1L)]
        [InlineData(JsonRpcErrorCode.InvalidJson)]
        [InlineData(JsonRpcErrorCode.InvalidOperation)]
        [InlineData(JsonRpcErrorCode.InvalidParameters)]
        [InlineData(JsonRpcErrorCode.InvalidMethod)]
        [InlineData(JsonRpcErrorCode.InvalidMessage)]
        [InlineData(JsonRpcErrorCode.ServerErrorsLowerBoundary)]
        [InlineData(JsonRpcErrorCode.ServerErrorsLowerBoundary + 1L)]
        [InlineData(JsonRpcErrorCode.ServerErrorsUpperBoundary - 1L)]
        [InlineData(JsonRpcErrorCode.ServerErrorsUpperBoundary)]
        [InlineData(JsonRpcErrorCode.StandardErrorsUpperBoundary + 1L)]
        [InlineData(default(long))]
        public void CodeIsValid(long code)
        {
            var jsonRpcError = new JsonRpcError(code, "m");

            Assert.Equal(code, jsonRpcError.Code);
        }

        [Theory]
        [InlineData(JsonRpcErrorCode.StandardErrorsLowerBoundary)]
        [InlineData(JsonRpcErrorCode.StandardErrorsLowerBoundary + 1L)]
        [InlineData(JsonRpcErrorCode.ServerErrorsLowerBoundary - 1L)]
        public void CodeIsInvalid(long code)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new JsonRpcError(code, "m"));
        }

        [Fact]
        public void MessageIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcError(1L, null));
        }

        [Fact]
        public void MessageIsEmptyString()
        {
            var jsonRpcError = new JsonRpcError(1L, string.Empty);

            Assert.Equal(string.Empty, jsonRpcError.Message);
        }

        [Fact]
        public void HasDataIsFalse()
        {
            var jsonRpcError = new JsonRpcError(1L, "m");

            Assert.False(jsonRpcError.HasData);
        }

        [Fact]
        public void HasDataIsTrue()
        {
            var jsonRpcError = new JsonRpcError(1L, "m", null);

            Assert.True(jsonRpcError.HasData);
        }
    }
}