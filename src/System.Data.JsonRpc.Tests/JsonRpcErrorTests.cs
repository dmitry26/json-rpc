using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcErrorTests
    {
        [Theory]
        [InlineData(JsonRpcErrorCodes.StandardErrorsLowerBoundary - 1L)]
        [InlineData(JsonRpcErrorCodes.InvalidJson)]
        [InlineData(JsonRpcErrorCodes.InvalidOperation)]
        [InlineData(JsonRpcErrorCodes.InvalidParameters)]
        [InlineData(JsonRpcErrorCodes.InvalidMethod)]
        [InlineData(JsonRpcErrorCodes.InvalidMessage)]
        [InlineData(JsonRpcErrorCodes.ServerErrorsLowerBoundary)]
        [InlineData(JsonRpcErrorCodes.ServerErrorsLowerBoundary + 1L)]
        [InlineData(JsonRpcErrorCodes.ServerErrorsUpperBoundary - 1L)]
        [InlineData(JsonRpcErrorCodes.ServerErrorsUpperBoundary)]
        [InlineData(JsonRpcErrorCodes.StandardErrorsUpperBoundary + 1L)]
        [InlineData(default(long))]
        public void CodeIsValid(long code)
        {
            var jsonRpcError = new JsonRpcError(code, "m");

            Assert.Equal(code, jsonRpcError.Code);
        }

        [Theory]
        [InlineData(JsonRpcErrorCodes.StandardErrorsLowerBoundary)]
        [InlineData(JsonRpcErrorCodes.StandardErrorsLowerBoundary + 1L)]
        [InlineData(JsonRpcErrorCodes.ServerErrorsLowerBoundary - 1L)]
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