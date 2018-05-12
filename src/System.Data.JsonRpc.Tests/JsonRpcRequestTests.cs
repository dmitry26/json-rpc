using System.Collections.Generic;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcRequestTests
    {
        [Fact]
        public void IsNotificationIsTrueWhenIdIsNone()
        {
            var message = new JsonRpcRequest("m");

            Assert.Equal(default, message.Id);
            Assert.True(message.IsNotification);
        }

        [Fact]
        public void IsNotificationIsFalseWhenIdIsNumber()
        {
            var message = new JsonRpcRequest("m", 1L);

            Assert.Equal(1L, message.Id);
            Assert.False(message.IsNotification);
        }

        [Fact]
        public void IsNotificationIsFalseWhenIdIsString()
        {
            var message = new JsonRpcRequest("m", "1");

            Assert.Equal("1", message.Id);
            Assert.False(message.IsNotification);
        }

        [Fact]
        public void ParametersTypeIsNoneWhenIdIsNone()
        {
            var message = new JsonRpcRequest("m");

            Assert.Equal(JsonRpcParametersType.None, message.ParametersType);
        }

        [Fact]
        public void ParametersTypeIsByPositionWhenIdIsNone()
        {
            var parameters = new object[] { 1L };
            var message = new JsonRpcRequest("m", parameters);

            Assert.Equal(JsonRpcParametersType.ByPosition, message.ParametersType);
        }

        [Fact]
        public void ParametersTypeIsByNameWhenIdIsNone()
        {
            var parameters = new Dictionary<string, object> { ["p"] = 1L };
            var message = new JsonRpcRequest("m", parameters);

            Assert.Equal(JsonRpcParametersType.ByName, message.ParametersType);
        }

        [Fact]
        public void IsSystemIsFalse()
        {
            var message = new JsonRpcRequest("m");

            Assert.False(message.IsSystem);
        }

        [Fact]
        public void IsSystemIsTrue()
        {
            var message = new JsonRpcRequest("rpc.m");

            Assert.True(message.IsSystem);
        }

        [Fact]
        public void MethodIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcRequest((string)null));
        }

        [Fact]
        public void MethodIsEmptyString()
        {
            var message = new JsonRpcRequest("");

            Assert.Equal("", message.Method);
        }

        [Fact]
        public void IsSystemMethodWhenMethodIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                JsonRpcRequest.IsSystemMethod((string)null));
        }

        [Fact]
        public void IsSystemMethodIsFalse()
        {
            var result = JsonRpcRequest.IsSystemMethod("m");

            Assert.False(result);
        }

        [Fact]
        public void IsSystemMethodIsTrue()
        {
            var result = JsonRpcRequest.IsSystemMethod("rpc.m");

            Assert.True(result);
        }
    }
}