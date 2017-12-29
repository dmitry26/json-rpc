using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcResponseTests
    {
        [Fact]
        public void SuccessIsFalse()
        {
            var message = new JsonRpcResponse(new JsonRpcError(2L, "m"), 1L);

            Assert.False(message.Success);
            Assert.NotNull(message.Error);
            Assert.Null(message.Result);
        }

        [Fact]
        public void SuccessIsTrue()
        {
            var message = new JsonRpcResponse(2L, 1L);

            Assert.True(message.Success);
            Assert.Null(message.Error);
            Assert.NotNull(message.Result);
        }

        [Fact]
        public void ConstructorWithResultWhenResultIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(default(object), default));
        }

        [Fact]
        public void ConstructorWithErrorWhenErrorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(default(JsonRpcError), default));
        }
    }
}