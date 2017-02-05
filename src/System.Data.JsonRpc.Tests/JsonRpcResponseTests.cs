using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcResponseTests
    {
        [Fact]
        public void IdTypeIsNull()
        {
            var jsonRpcResponse = new JsonRpcResponse(new[] { 100L });

            Assert.Equal(JsonRpcIdType.Null, jsonRpcResponse.IdType);
            Assert.False(jsonRpcResponse.HasId);
        }

        [Fact]
        public void IdTypeIsNumber()
        {
            var jsonRpcResponse = new JsonRpcResponse(100L, new[] { 100L });

            Assert.Equal(JsonRpcIdType.Number, jsonRpcResponse.IdType);
            Assert.True(jsonRpcResponse.HasId);
        }

        [Fact]
        public void IdTypeIsString()
        {
            var jsonRpcResponse = new JsonRpcResponse("100", new[] { 100L });

            Assert.Equal(JsonRpcIdType.String, jsonRpcResponse.IdType);
            Assert.True(jsonRpcResponse.HasId);
        }

        [Fact]
        public void SuccessIsFalse()
        {
            var jsonRpcResponse = new JsonRpcResponse(100L, new JsonRpcError(200L, "test_message"));

            Assert.False(jsonRpcResponse.Success);
            Assert.NotNull(jsonRpcResponse.Error);
            Assert.Null(jsonRpcResponse.Result);
        }

        [Fact]
        public void SuccessIsTrue()
        {
            var jsonRpcResponse = new JsonRpcResponse(100L, new[] { 100L });

            Assert.True(jsonRpcResponse.Success);
            Assert.Null(jsonRpcResponse.Error);
            Assert.NotNull(jsonRpcResponse.Result);
        }

        [Fact]
        public void ConstructorWithResultWhenResultIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(default(object)));
        }

        [Fact]
        public void ConstructorWithErrorWhenErrorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(default(JsonRpcError)));
        }

        [Fact]
        public void ConstructorWithIdAndResultWhenIdIsNumberAndResultIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(100L, default(object)));
        }

        [Fact]
        public void ConstructorWithIdAndErrorWhenIdIsNumberAndErrorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(100L, default(JsonRpcError)));
        }

        [Fact]
        public void ConstructorWithIdAndResultWhenIdIsStringAndIdIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(default(string), new[] { 100L }));
        }

        [Fact]
        public void ConstructorWithIdAndResultWhenIdIsStringAndIdIsEmptyString()
        {
            Assert.Throws<ArgumentException>(() =>
                new JsonRpcResponse(string.Empty, new[] { 100L }));
        }

        [Fact]
        public void ConstructorWithIdAndResultWhenIdIsStringAndResultIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse("100", default(object)));
        }

        [Fact]
        public void ConstructorWithIdAndErrorWhenIdIsStringAndIdIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse(default(string), new JsonRpcError(100L, "test_message")));
        }

        [Fact]
        public void ConstructorWithIdAndErrorWhenIdIsStringAndIdIsEmptyString()
        {
            Assert.Throws<ArgumentException>(() =>
                new JsonRpcResponse(string.Empty, new JsonRpcError(100L, "test_message")));
        }

        [Fact]
        public void ConstructorWithIdAndErrorWhenIdIsStringAndErrorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcResponse("100", default(JsonRpcError)));
        }
    }
}