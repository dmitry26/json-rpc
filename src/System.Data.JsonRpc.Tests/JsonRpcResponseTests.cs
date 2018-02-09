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
        public void SuccessIsTrueWhenResultIsNumber()
        {
            var message = new JsonRpcResponse(0L, 1L);

            Assert.True(message.Success);
            Assert.Null(message.Error);
        }

        [Fact]
        public void SuccessIsTrueWhenResultIsString()
        {
            var message = new JsonRpcResponse("0", 1L);

            Assert.True(message.Success);
            Assert.Null(message.Error);
        }

        [Fact]
        public void SuccessIsTrueWhenResultIsBoolean()
        {
            var message = new JsonRpcResponse(true, 1L);

            Assert.True(message.Success);
            Assert.Null(message.Error);
        }

        [Fact]
        public void SuccessIsTrueWhenResultIsObject()
        {
            var message = new JsonRpcResponse(new object(), 1L);

            Assert.True(message.Success);
            Assert.Null(message.Error);
        }

        [Fact]
        public void SuccessIsTrueWhenResultIsNull()
        {
            var message = new JsonRpcResponse(default(object), 1L);

            Assert.True(message.Success);
            Assert.Null(message.Error);
        }

        [Fact]
        public void ConstructorWithResultWhenIdIsNone()
        {
            Assert.Throws<ArgumentException>(() =>
                new JsonRpcResponse(1L, default(JsonRpcId)));
        }

        [Fact]
        public void ConstructorWithErrorWhenErrorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(default(JsonRpcError)));
        }
    }
}