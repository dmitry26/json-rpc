using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcRequestTests
    {
        [Fact]
        public void IdTypeIsNone()
        {
            var jsonRpcRequest = new JsonRpcRequest("test_method", JsonRpcId.None);

            Assert.Equal(JsonRpcId.None, jsonRpcRequest.Id);
            Assert.True(jsonRpcRequest.IsNotification);
        }

        [Fact]
        public void IdTypeIsNumber()
        {
            var jsonRpcRequest = new JsonRpcRequest("test_method", 100L);

            Assert.Equal(100L, jsonRpcRequest.Id);
            Assert.False(jsonRpcRequest.IsNotification);
        }

        [Fact]
        public void IdTypeIsString()
        {
            var jsonRpcRequest = new JsonRpcRequest("test_method", "100");

            Assert.Equal("100", jsonRpcRequest.Id);
            Assert.False(jsonRpcRequest.IsNotification);
        }

        [Fact]
        public void HasParamsIsFalse()
        {
            var jsonRpcRequest = new JsonRpcRequest("test_method", JsonRpcId.None);

            Assert.False(jsonRpcRequest.HasParams);
        }

        [Fact]
        public void HasParamsIsTrue()
        {
            var jsonRpcRequest = new JsonRpcRequest("test_method", JsonRpcId.None, new[] { 100L });

            Assert.True(jsonRpcRequest.HasParams);
        }

        [Fact]
        public void HasParamsIsFalseWhenIdIsNumber()
        {
            var jsonRpcRequest = new JsonRpcRequest("test_method", 100L);

            Assert.False(jsonRpcRequest.HasParams);
        }

        [Fact]
        public void HasParamsIsFalseWhenIdIsString()
        {
            var jsonRpcRequest = new JsonRpcRequest("test_method", "100");

            Assert.False(jsonRpcRequest.HasParams);
        }

        [Fact]
        public void HasParamsIsTrueWhenIdIsNumber()
        {
            var jsonRpcRequest = new JsonRpcRequest("test_method", 100L, new[] { 100L });

            Assert.True(jsonRpcRequest.HasParams);
        }

        [Fact]
        public void HasParamsIsTrueWhenIdIsString()
        {
            var jsonRpcRequest = new JsonRpcRequest("test_method", "100", new[] { 100L });

            Assert.True(jsonRpcRequest.HasParams);
        }

        [Fact]
        public void IsSystemIsFalse()
        {
            var jsonRpcRequest = new JsonRpcRequest("test_method", JsonRpcId.None);

            Assert.False(jsonRpcRequest.IsSystem);
        }

        [Fact]
        public void IsSystemIsTrue()
        {
            var jsonRpcRequest = new JsonRpcRequest("rpc.test_method", JsonRpcId.None);

            Assert.True(jsonRpcRequest.IsSystem);
        }

        [Fact]
        public void ConstructorWithMethodWhenMethodIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcRequest(default(string), JsonRpcId.None));
        }

        [Fact]
        public void ConstructorWithMethodWhenMethodIsEmptyString()
        {
            Assert.Throws<ArgumentException>(() =>
                new JsonRpcRequest(string.Empty, JsonRpcId.None));
        }

        [Fact]
        public void ConstructorWithMethodAndParamsWhenMethodIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcRequest(default(string), JsonRpcId.None, new[] { 100L }));
        }

        [Fact]
        public void ConstructorWithMethodAndParamsWhenMethodIsEmptyString()
        {
            Assert.Throws<ArgumentException>(() =>
                new JsonRpcRequest(string.Empty, JsonRpcId.None, new[] { 100L }));
        }

        [Fact]
        public void ConstructorWithMethodAndIdWhenMethodIsNullAndIdIsNumber()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcRequest(default(string), 100L));
        }

        [Fact]
        public void ConstructorWithMethodAndIdWhenMethodIsEmptyStringAndIdIsNumber()
        {
            Assert.Throws<ArgumentException>(() =>
                new JsonRpcRequest(string.Empty, 100L));
        }

        [Fact]
        public void ConstructorWithMethodAndIdAndParamsWhenMethodIsNullAndIdIsNumber()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcRequest(default(string), 100L, new[] { 100L }));
        }

        [Fact]
        public void ConstructorWithMethodAndIdAndParamsWhenMethodIsEmptyStringAndIdIsNumber()
        {
            Assert.Throws<ArgumentException>(() =>
                new JsonRpcRequest(string.Empty, 100L, new[] { 100L }));
        }

        [Fact]
        public void ConstructorWithMethodAndIdWhenMethodIsNullAndIdIsString()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcRequest(default(string), "100"));
        }

        [Fact]
        public void ConstructorWithMethodAndIdWhenMethodIsEmptyStringAndIdIsString()
        {
            Assert.Throws<ArgumentException>(() =>
                new JsonRpcRequest(string.Empty, "100"));
        }

        [Fact]
        public void ConstructorWithMethodAndIdWhenIdIsStringAndIdIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcRequest("test_method", default(string)));
        }

        [Fact]
        public void ConstructorWithMethodAndIdWhenIdIsStringAndIdIsEmptyString()
        {
            Assert.Throws<ArgumentException>(() =>
                new JsonRpcRequest("test_method", string.Empty));
        }

        [Fact]
        public void ConstructorWithMethodAndIdAndParamsWhenMethodIsNullAndIdIsString()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcRequest(default(string), "100", new[] { 100L }));
        }

        [Fact]
        public void ConstructorWithMethodAndIdAndParamsWhenMethodIsEmptyStringAndIdIsString()
        {
            Assert.Throws<ArgumentException>(() =>
                new JsonRpcRequest(string.Empty, "100", new[] { 100L }));
        }

        [Fact]
        public void ConstructorWithMethodAndIdAndParamsWhenIdIsStringAndIdIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcRequest("test_method", default(string), new[] { 100L }));
        }

        [Fact]
        public void ConstructorWithMethodAndIdAndParamsWhenIdIsStringAndIdIsEmptyString()
        {
            Assert.Throws<ArgumentException>(() =>
                new JsonRpcRequest("test_method", string.Empty, new[] { 100L }));
        }
    }
}