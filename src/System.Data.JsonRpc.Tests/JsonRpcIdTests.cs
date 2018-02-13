using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcIdTests
    {
        [Fact]
        public void IdTypeIsProper()
        {
            Assert.Equal(JsonRpcIdType.None, new JsonRpcId().Type);
            Assert.Equal(JsonRpcIdType.String, new JsonRpcId("1").Type);
            Assert.Equal(JsonRpcIdType.Integer, new JsonRpcId(1L).Type);
            Assert.Equal(JsonRpcIdType.Float, new JsonRpcId(1D).Type);
        }

        [Fact]
        public void OperatorEquality()
        {
            Assert.True(
                new JsonRpcId() == new JsonRpcId());
            Assert.False(
                new JsonRpcId() == new JsonRpcId("1"));
            Assert.False(
                new JsonRpcId() == new JsonRpcId(1L));
            Assert.False(
                new JsonRpcId() == new JsonRpcId(1D));
            Assert.False(
                new JsonRpcId() == "1");
            Assert.False(
                new JsonRpcId() == 1L);
            Assert.False(
                new JsonRpcId() == 1D);

            Assert.False(
                new JsonRpcId("1") == new JsonRpcId());
            Assert.True(
                new JsonRpcId("1") == new JsonRpcId("1"));
            Assert.False(
                new JsonRpcId("1") == new JsonRpcId("2"));
            Assert.False(
                new JsonRpcId("1") == new JsonRpcId(1L));
            Assert.False(
                new JsonRpcId("1") == new JsonRpcId(1D));
            Assert.True(
                new JsonRpcId("1") == "1");
            Assert.False(
                new JsonRpcId("1") == "2");
            Assert.False(
                new JsonRpcId("1") == 1L);
            Assert.False(
                new JsonRpcId("1") == 1D);

            Assert.False(
                new JsonRpcId(1L) == new JsonRpcId());
            Assert.False(
                new JsonRpcId(1L) == new JsonRpcId("1"));
            Assert.True(
                new JsonRpcId(1L) == new JsonRpcId(1L));
            Assert.False(
                new JsonRpcId(1L) == new JsonRpcId(2L));
            Assert.False(
                new JsonRpcId(1L) == new JsonRpcId(1D));
            Assert.False(
                new JsonRpcId(1L) == "1");
            Assert.True(
                new JsonRpcId(1L) == 1L);
            Assert.False(
                new JsonRpcId(1L) == 2L);
            Assert.False(
                new JsonRpcId(1L) == 1D);

            Assert.False(
                new JsonRpcId(1D) == new JsonRpcId());
            Assert.False(
                new JsonRpcId(1D) == new JsonRpcId("1"));
            Assert.False(
                new JsonRpcId(1D) == new JsonRpcId(1L));
            Assert.True(
                new JsonRpcId(1D) == new JsonRpcId(1D));
            Assert.False(
                new JsonRpcId(1D) == new JsonRpcId(2D));
            Assert.False(
                new JsonRpcId(1D) == "1");
            Assert.False(
                new JsonRpcId(1D) == 1L);
            Assert.True(
                new JsonRpcId(1D) == 1D);
            Assert.False(
                new JsonRpcId(1D) == 2D);
        }

        [Fact]
        public void OperatorInequality()
        {
            Assert.False(
                new JsonRpcId() != new JsonRpcId());
            Assert.True(
                new JsonRpcId() != new JsonRpcId("1"));
            Assert.True(
                new JsonRpcId() != new JsonRpcId(1L));
            Assert.True(
                new JsonRpcId() != new JsonRpcId(1D));
            Assert.True(
                new JsonRpcId() != "1");
            Assert.True(
                new JsonRpcId() != 1L);
            Assert.True(
                new JsonRpcId() != 1D);

            Assert.True(
                new JsonRpcId("1") != new JsonRpcId());
            Assert.False(
                new JsonRpcId("1") != new JsonRpcId("1"));
            Assert.True(
                new JsonRpcId("1") != new JsonRpcId("2"));
            Assert.True(
                new JsonRpcId("1") != new JsonRpcId(1L));
            Assert.True(
                new JsonRpcId("1") != new JsonRpcId(1D));
            Assert.False(
                new JsonRpcId("1") != "1");
            Assert.True(
                new JsonRpcId("1") != "2");
            Assert.True(
                new JsonRpcId("1") != 1L);
            Assert.True(
                new JsonRpcId("1") != 1D);

            Assert.True(
                new JsonRpcId(1L) != new JsonRpcId());
            Assert.True(
                new JsonRpcId(1L) != new JsonRpcId("1"));
            Assert.False(
                new JsonRpcId(1L) != new JsonRpcId(1L));
            Assert.True(
                new JsonRpcId(1L) != new JsonRpcId(2L));
            Assert.True(
                new JsonRpcId(1L) != new JsonRpcId(1D));
            Assert.True(
                new JsonRpcId(1L) != "1");
            Assert.False(
                new JsonRpcId(1L) != 1L);
            Assert.True(
                new JsonRpcId(1L) != 2L);
            Assert.True(
                new JsonRpcId(1L) != 1D);

            Assert.True(
                new JsonRpcId(1D) != new JsonRpcId());
            Assert.True(
                new JsonRpcId(1D) != new JsonRpcId("1"));
            Assert.True(
                new JsonRpcId(1D) != new JsonRpcId(1L));
            Assert.False(
                new JsonRpcId(1D) != new JsonRpcId(1D));
            Assert.True(
                new JsonRpcId(1D) != new JsonRpcId(2D));
            Assert.True(
                new JsonRpcId(1D) != "1");
            Assert.True(
                new JsonRpcId(1D) != 1L);
            Assert.False(
                new JsonRpcId(1D) != 1D);
            Assert.True(
                new JsonRpcId(1D) != 2D);
        }

        [Fact]
        public void ObjectCast()
        {
            Assert.Throws<InvalidCastException>(() => (string)new JsonRpcId());
            Assert.Equal("1", (string)new JsonRpcId("1"));
            Assert.Throws<InvalidCastException>(() => (string)new JsonRpcId(1L));
            Assert.Throws<InvalidCastException>(() => (string)new JsonRpcId(1D));

            Assert.Throws<InvalidCastException>(() => (long)new JsonRpcId());
            Assert.Throws<InvalidCastException>(() => (long)new JsonRpcId("1"));
            Assert.Equal(1L, (long)new JsonRpcId(1L));
            Assert.Throws<InvalidCastException>(() => (long)new JsonRpcId(1D));

            Assert.Throws<InvalidCastException>(() => (double)new JsonRpcId());
            Assert.Throws<InvalidCastException>(() => (double)new JsonRpcId("1"));
            Assert.Throws<InvalidCastException>(() => (double)new JsonRpcId(1L));
            Assert.Equal(1D, (double)new JsonRpcId(1D));
        }

        [Fact]
        public void ObjectEquals()
        {
            Assert.True(
                object.Equals(new JsonRpcId(), new JsonRpcId()));
            Assert.False(
                object.Equals(new JsonRpcId(), new JsonRpcId("1")));
            Assert.False(
                object.Equals(new JsonRpcId(), new JsonRpcId(1L)));
            Assert.False(
                object.Equals(new JsonRpcId(), new JsonRpcId(1D)));

            Assert.False(
                object.Equals(new JsonRpcId("1"), new JsonRpcId()));
            Assert.True(
                object.Equals(new JsonRpcId("1"), new JsonRpcId("1")));
            Assert.False(
                object.Equals(new JsonRpcId("1"), new JsonRpcId("2")));
            Assert.False(
                object.Equals(new JsonRpcId("1"), new JsonRpcId(1L)));
            Assert.False(
                object.Equals(new JsonRpcId("1"), new JsonRpcId(1D)));

            Assert.False(
                object.Equals(new JsonRpcId(1L), new JsonRpcId()));
            Assert.False(
                object.Equals(new JsonRpcId(1L), new JsonRpcId("1")));
            Assert.True(
                object.Equals(new JsonRpcId(1L), new JsonRpcId(1L)));
            Assert.False(
                object.Equals(new JsonRpcId(1L), new JsonRpcId(2L)));
            Assert.False(
                object.Equals(new JsonRpcId(1L), new JsonRpcId(1D)));

            Assert.False(
                object.Equals(new JsonRpcId(1D), new JsonRpcId()));
            Assert.False(
                object.Equals(new JsonRpcId(1D), new JsonRpcId("1")));
            Assert.False(
                object.Equals(new JsonRpcId(1D), new JsonRpcId(1L)));
            Assert.True(
                object.Equals(new JsonRpcId(1D), new JsonRpcId(1D)));
            Assert.False(
                object.Equals(new JsonRpcId(1D), new JsonRpcId(2D)));
        }

        [Fact]
        public void ObjectGetHashCode()
        {
            Assert.Equal(
                new JsonRpcId().GetHashCode(), new JsonRpcId().GetHashCode());
            Assert.NotEqual(
                new JsonRpcId().GetHashCode(), new JsonRpcId("1").GetHashCode());
            Assert.NotEqual(
                new JsonRpcId().GetHashCode(), new JsonRpcId(1L).GetHashCode());
            Assert.NotEqual(
                new JsonRpcId().GetHashCode(), new JsonRpcId(1D).GetHashCode());

            Assert.NotEqual(
                new JsonRpcId("1").GetHashCode(), new JsonRpcId().GetHashCode());
            Assert.Equal(
                new JsonRpcId("1").GetHashCode(), new JsonRpcId("1").GetHashCode());
            Assert.NotEqual(
                new JsonRpcId("1").GetHashCode(), new JsonRpcId("2").GetHashCode());
            Assert.NotEqual(
                new JsonRpcId("1").GetHashCode(), new JsonRpcId(1L).GetHashCode());
            Assert.NotEqual(
                new JsonRpcId("1").GetHashCode(), new JsonRpcId(1D).GetHashCode());

            Assert.NotEqual(
                new JsonRpcId(1L).GetHashCode(), new JsonRpcId().GetHashCode());
            Assert.NotEqual(
                new JsonRpcId(1L).GetHashCode(), new JsonRpcId("1").GetHashCode());
            Assert.Equal(
                new JsonRpcId(1L).GetHashCode(), new JsonRpcId(1L).GetHashCode());
            Assert.NotEqual(
                new JsonRpcId(1L).GetHashCode(), new JsonRpcId(2L).GetHashCode());
            Assert.NotEqual(
                new JsonRpcId(1L).GetHashCode(), new JsonRpcId(1D).GetHashCode());

            Assert.NotEqual(
                new JsonRpcId(1D).GetHashCode(), new JsonRpcId().GetHashCode());
            Assert.NotEqual(
                new JsonRpcId(1D).GetHashCode(), new JsonRpcId("1").GetHashCode());
            Assert.NotEqual(
                new JsonRpcId(1D).GetHashCode(), new JsonRpcId(1L).GetHashCode());
            Assert.Equal(
                new JsonRpcId(1D).GetHashCode(), new JsonRpcId(1D).GetHashCode());
            Assert.NotEqual(
                new JsonRpcId(1D).GetHashCode(), new JsonRpcId(2D).GetHashCode());
        }

        [Fact]
        public void ObjectToString()
        {
            Assert.Equal("", new JsonRpcId().ToString());
            Assert.Equal("1", new JsonRpcId("1").ToString());
            Assert.Equal("1", new JsonRpcId(1L).ToString());
            Assert.Equal("1.0", new JsonRpcId(1D).ToString());
        }

        [Fact]
        public void CompareTo()
        {
            Assert.Equal(+0, new JsonRpcId().CompareTo(new JsonRpcId()));
            Assert.Equal(-1, new JsonRpcId().CompareTo(new JsonRpcId("1")));
            Assert.Equal(-1, new JsonRpcId().CompareTo(new JsonRpcId(1L)));
            Assert.Equal(-1, new JsonRpcId().CompareTo(new JsonRpcId(1D)));

            Assert.Equal(+1, new JsonRpcId("1").CompareTo(new JsonRpcId()));
            Assert.Equal(+1, new JsonRpcId("1").CompareTo(new JsonRpcId("0")));
            Assert.Equal(+0, new JsonRpcId("1").CompareTo(new JsonRpcId("1")));
            Assert.Equal(-1, new JsonRpcId("1").CompareTo(new JsonRpcId("2")));
            Assert.Equal(-1, new JsonRpcId("1").CompareTo(new JsonRpcId(1L)));
            Assert.Equal(-1, new JsonRpcId("1").CompareTo(new JsonRpcId(1D)));

            Assert.Equal(+1, new JsonRpcId(1L).CompareTo(new JsonRpcId()));
            Assert.Equal(+1, new JsonRpcId(1L).CompareTo(new JsonRpcId("1")));
            Assert.Equal(+1, new JsonRpcId(1L).CompareTo(new JsonRpcId(0L)));
            Assert.Equal(+0, new JsonRpcId(1L).CompareTo(new JsonRpcId(1L)));
            Assert.Equal(-1, new JsonRpcId(1L).CompareTo(new JsonRpcId(2L)));
            Assert.Equal(-1, new JsonRpcId(1L).CompareTo(new JsonRpcId(1D)));

            Assert.Equal(+1, new JsonRpcId(1D).CompareTo(new JsonRpcId()));
            Assert.Equal(+1, new JsonRpcId(1D).CompareTo(new JsonRpcId("1")));
            Assert.Equal(+1, new JsonRpcId(1D).CompareTo(new JsonRpcId(0D)));
            Assert.Equal(+0, new JsonRpcId(1D).CompareTo(new JsonRpcId(1D)));
            Assert.Equal(-1, new JsonRpcId(1D).CompareTo(new JsonRpcId(2D)));
            Assert.Equal(+1, new JsonRpcId(1D).CompareTo(new JsonRpcId(1L)));
        }

        [Fact]
        public void TypeIsStringAndValueIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonRpcId((string)null));
        }

        [Fact]
        public void TypeIsStringAndValueIsEmpty()
        {
            Assert.Equal(JsonRpcIdType.String, new JsonRpcId("").Type);
        }
    }
}