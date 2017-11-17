using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcIdTests
    {
        [Fact]
        public void TypeIsNone()
        {
            var result = default(JsonRpcId);

            Assert.Equal(JsonRpcIdType.None, result.Type);
            Assert.Throws<InvalidOperationException>(() => (long)result);
            Assert.Throws<InvalidOperationException>(() => (string)result);
            Assert.True(result == default);
            Assert.False(result == new JsonRpcId(100L));
            Assert.False(result == new JsonRpcId("100"));
            Assert.False(result != default);
            Assert.True(result != new JsonRpcId(100L));
            Assert.True(result != new JsonRpcId("100"));
            Assert.False(result == 100L);
            Assert.False(result == "100");
            Assert.True(result != 100L);
            Assert.True(result != "100");
        }

        [Fact]
        public void TypeIsNumber()
        {
            var result = new JsonRpcId(100L);

            Assert.Equal(JsonRpcIdType.Number, result.Type);
            Assert.Equal(100L, (long)result);
            Assert.Throws<InvalidOperationException>(() => (string)result);
            Assert.False(result == default);
            Assert.True(result == new JsonRpcId(100L));
            Assert.False(result == new JsonRpcId("100"));
            Assert.True(result != default);
            Assert.False(result != new JsonRpcId(100L));
            Assert.True(result != new JsonRpcId("100"));
            Assert.True(result == 100L);
            Assert.False(result == "100");
            Assert.False(result != 100L);
            Assert.True(result != "100");
        }

        [Fact]
        public void TypeIsString()
        {
            var result = new JsonRpcId("100");

            Assert.Equal(JsonRpcIdType.String, result.Type);
            Assert.Throws<InvalidOperationException>(() => (long)result);
            Assert.Equal("100", (string)result);
            Assert.False(result == default);
            Assert.False(result == new JsonRpcId(100L));
            Assert.True(result == new JsonRpcId("100"));
            Assert.True(result != default);
            Assert.True(result != new JsonRpcId(100L));
            Assert.False(result != new JsonRpcId("100"));
            Assert.False(result == 100L);
            Assert.True(result == "100");
            Assert.True(result != 100L);
            Assert.False(result != "100");
        }
    }
}