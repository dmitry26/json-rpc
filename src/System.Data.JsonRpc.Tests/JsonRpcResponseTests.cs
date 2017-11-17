using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcResponseTests
    {
        [Fact]
        public void IdTypeIsNone()
        {
            var jsonRpcResponse = new JsonRpcResponse(new[] { 100L }, JsonRpcId.None);

            Assert.Equal(JsonRpcId.None, jsonRpcResponse.Id);
            Assert.False(jsonRpcResponse.HasId);
        }

        [Fact]
        public void IdTypeIsNumber()
        {
            var jsonRpcResponse = new JsonRpcResponse(new[] { 100L }, 100L);

            Assert.Equal(100L, jsonRpcResponse.Id);
            Assert.True(jsonRpcResponse.HasId);
        }

        [Fact]
        public void IdTypeIsString()
        {
            var jsonRpcResponse = new JsonRpcResponse(new[] { 100L }, "100");

            Assert.Equal("100", jsonRpcResponse.Id);
            Assert.True(jsonRpcResponse.HasId);
        }

        [Fact]
        public void SuccessIsFalse()
        {
            var jsonRpcResponse = new JsonRpcResponse(new JsonRpcError(200L, "test_message"), 100L);

            Assert.False(jsonRpcResponse.Success);
            Assert.NotNull(jsonRpcResponse.Error);
            Assert.Null(jsonRpcResponse.Result);
        }

        [Fact]
        public void SuccessIsTrue()
        {
            var jsonRpcResponse = new JsonRpcResponse(new[] { 100L }, 100L);

            Assert.True(jsonRpcResponse.Success);
            Assert.Null(jsonRpcResponse.Error);
            Assert.NotNull(jsonRpcResponse.Result);
        }

        [Fact]
        public void ConstructorWithResultWhenResultIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(default(object), JsonRpcId.None));
        }

        [Fact]
        public void ConstructorWithErrorWhenErrorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(default(JsonRpcError), JsonRpcId.None));
        }

        [Fact]
        public void ConstructorWithIdAndResultWhenIdIsNumberAndResultIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(default(object), 100L));
        }

        [Fact]
        public void ConstructorWithIdAndErrorWhenIdIsNumberAndErrorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(default(JsonRpcError), 100L));
        }

        [Fact]
        public void ConstructorWithIdAndResultWhenIdIsStringAndIdIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(new[] { 100L }, default(string)));
        }

        [Fact]
        public void ConstructorWithIdAndResultWhenIdIsStringAndResultIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(default(object), "100"));
        }

        [Fact]
        public void ConstructorWithIdAndErrorWhenIdIsStringAndIdIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(new JsonRpcError(100L, "test_message"), default(string)));
        }

        [Fact]
        public void ConstructorWithIdAndErrorWhenIdIsStringAndErrorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(default(JsonRpcError), "100"));
        }
    }
}