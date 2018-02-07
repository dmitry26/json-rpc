using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcIdTests
    {
        [Fact]
        public void IdTypeIsProper()
        {
            Assert.Equal(JsonRpcIdType.None, new JsonRpcId().Type);
            Assert.Equal(JsonRpcIdType.Number, new JsonRpcId(1L).Type);
            Assert.Equal(JsonRpcIdType.String, new JsonRpcId("1").Type);
        }

        [Fact]
        public void OperatorEquality()
        {
            Assert.True(new JsonRpcId() == default);
            Assert.False(new JsonRpcId() == new JsonRpcId(1L));
            Assert.False(new JsonRpcId() == new JsonRpcId("1"));
            Assert.False(new JsonRpcId() == 1L);
            Assert.False(new JsonRpcId() == "1");
            Assert.False(new JsonRpcId() == 2L);
            Assert.False(new JsonRpcId() == "2");
            Assert.False(new JsonRpcId(1L) == default);
            Assert.True(new JsonRpcId(1L) == new JsonRpcId(1L));
            Assert.False(new JsonRpcId(1L) == new JsonRpcId("1"));
            Assert.True(new JsonRpcId(1L) == 1L);
            Assert.False(new JsonRpcId(1L) == "1");
            Assert.False(new JsonRpcId(1L) == 2L);
            Assert.False(new JsonRpcId(1L) == "2");
            Assert.False(new JsonRpcId("1") == default);
            Assert.False(new JsonRpcId("1") == new JsonRpcId(1L));
            Assert.True(new JsonRpcId("1") == new JsonRpcId("1"));
            Assert.False(new JsonRpcId("1") == 1L);
            Assert.True(new JsonRpcId("1") == "1");
            Assert.False(new JsonRpcId("1") == 2L);
            Assert.False(new JsonRpcId("1") == "2");
        }

        [Fact]
        public void OperatorInequality()
        {
            Assert.False(new JsonRpcId() != default);
            Assert.True(new JsonRpcId() != new JsonRpcId(1L));
            Assert.True(new JsonRpcId() != new JsonRpcId("1"));
            Assert.True(new JsonRpcId() != 1L);
            Assert.True(new JsonRpcId() != "1");
            Assert.True(new JsonRpcId(1L) != default);
            Assert.False(new JsonRpcId(1L) != new JsonRpcId(1L));
            Assert.True(new JsonRpcId(1L) != new JsonRpcId("1"));
            Assert.False(new JsonRpcId(1L) != 1L);
            Assert.True(new JsonRpcId(1L) != "1");
            Assert.True(new JsonRpcId("1") != default);
            Assert.True(new JsonRpcId("1") != new JsonRpcId(1L));
            Assert.False(new JsonRpcId("1") != new JsonRpcId("1"));
            Assert.True(new JsonRpcId("1") != 1L);
            Assert.False(new JsonRpcId("1") != "1");
        }

        [Fact]
        public void ObjectCast()
        {
            Assert.Throws<InvalidCastException>(() => (long)new JsonRpcId());
            Assert.Throws<InvalidCastException>(() => (string)new JsonRpcId());
            Assert.Equal(1L, (long)new JsonRpcId(1L));
            Assert.Throws<InvalidCastException>(() => (string)new JsonRpcId(1L));
            Assert.Throws<InvalidCastException>(() => (long)new JsonRpcId("1"));
            Assert.Equal("1", (string)new JsonRpcId("1"));
        }

        [Fact]
        public void ObjectEquals()
        {
            Assert.True(object.Equals(new JsonRpcId(), new JsonRpcId()));
            Assert.True(object.Equals(new JsonRpcId(1L), new JsonRpcId(1L)));
            Assert.True(object.Equals(new JsonRpcId("1"), new JsonRpcId("1")));
            Assert.False(object.Equals(new JsonRpcId(), new JsonRpcId(1L)));
            Assert.False(object.Equals(new JsonRpcId(), new JsonRpcId("1")));
            Assert.False(object.Equals(new JsonRpcId(1L), new JsonRpcId(2L)));
            Assert.False(object.Equals(new JsonRpcId("1"), new JsonRpcId("2")));
            Assert.False(object.Equals(new JsonRpcId(1L), new JsonRpcId("1")));
        }

        [Fact]
        public void ObjectGetHashCode()
        {
            Assert.Equal(new JsonRpcId().GetHashCode(), new JsonRpcId().GetHashCode());
            Assert.Equal(new JsonRpcId(1L).GetHashCode(), new JsonRpcId(1L).GetHashCode());
            Assert.Equal(new JsonRpcId("1").GetHashCode(), new JsonRpcId("1").GetHashCode());
            Assert.NotEqual(new JsonRpcId().GetHashCode(), new JsonRpcId(1L).GetHashCode());
            Assert.NotEqual(new JsonRpcId().GetHashCode(), new JsonRpcId("1").GetHashCode());
            Assert.NotEqual(new JsonRpcId(1L).GetHashCode(), new JsonRpcId(2L).GetHashCode());
            Assert.NotEqual(new JsonRpcId("1").GetHashCode(), new JsonRpcId("2").GetHashCode());
            Assert.NotEqual(new JsonRpcId(1L).GetHashCode(), new JsonRpcId("1").GetHashCode());
        }

        [Fact]
        public void ObjectToString()
        {
            Assert.Equal("", new JsonRpcId().ToString());
            Assert.Equal("1", new JsonRpcId(1L).ToString());
            Assert.Equal("1", new JsonRpcId("1").ToString());
        }

        [Fact]
        public void ConstructorWhenTypeIsStringAndEqualsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonRpcId(null));
        }

        [Fact]
        public void ConstructorWhenTypeIsStringAndEqualsEmptyString()
        {
            Assert.Equal(JsonRpcIdType.String, new JsonRpcId("").Type);
        }
    }
}