using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcIdTests
    {
        [Fact]
        public void TypeIsNone()
        {
            var result = new JsonRpcId();

            Assert.Equal(JsonRpcIdType.None, result.Type);
            Assert.Throws<InvalidCastException>(() => (long)result);
            Assert.Throws<InvalidCastException>(() => (string)result);
            Assert.True(result == default);
            Assert.False(result == new JsonRpcId(1L));
            Assert.False(result == new JsonRpcId("1"));
            Assert.False(result != default);
            Assert.True(result != new JsonRpcId(1L));
            Assert.True(result != new JsonRpcId("1"));
            Assert.False(result == 1L);
            Assert.False(result == "1");
            Assert.True(result != 1L);
            Assert.True(result != "1");
            Assert.False(result == 2L);
            Assert.False(result == "2");
        }

        [Fact]
        public void TypeIsNumber()
        {
            var result = new JsonRpcId(1L);

            Assert.Equal(JsonRpcIdType.Number, result.Type);
            Assert.Equal(1L, (long)result);
            Assert.Throws<InvalidCastException>(() => (string)result);
            Assert.False(result == default);
            Assert.True(result == new JsonRpcId(1L));
            Assert.False(result == new JsonRpcId("1"));
            Assert.True(result != default);
            Assert.False(result != new JsonRpcId(1L));
            Assert.True(result != new JsonRpcId("1"));
            Assert.True(result == 1L);
            Assert.False(result == "1");
            Assert.False(result != 1L);
            Assert.True(result != "1");
            Assert.False(result == 2L);
            Assert.False(result == "2");
        }

        [Fact]
        public void TypeIsString()
        {
            var result = new JsonRpcId("1");

            Assert.Equal(JsonRpcIdType.String, result.Type);
            Assert.Throws<InvalidCastException>(() => (long)result);
            Assert.Equal("1", (string)result);
            Assert.False(result == default);
            Assert.False(result == new JsonRpcId(1L));
            Assert.True(result == new JsonRpcId("1"));
            Assert.True(result != default);
            Assert.True(result != new JsonRpcId(1L));
            Assert.False(result != new JsonRpcId("1"));
            Assert.False(result == 1L);
            Assert.True(result == "1");
            Assert.True(result != 1L);
            Assert.False(result != "1");
            Assert.False(result == 2L);
            Assert.False(result == "2");
        }

        [Fact]
        public void ConstructorWhenTypeIsStringAndEqualsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcId(null));
        }

        [Fact]
        public void ConstructorWhenTypeIsStringAndEqualsEmptyString()
        {
            var result = new JsonRpcId("");

            Assert.Equal(JsonRpcIdType.String, result.Type);
        }
    }
}