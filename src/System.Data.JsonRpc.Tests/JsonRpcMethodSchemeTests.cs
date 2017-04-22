using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcMethodSchemeTests
    {
        [Fact]
        public void ConstructorWhenParametersTypeIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcMethodScheme(default(Type)));
        }

        [Fact]
        public void ConstructorWhenResultTypeIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcMethodScheme(default(Type), typeof(object)));
        }

        [Fact]
        public void ConstructorWhenErrorDataTypeIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcMethodScheme(typeof(object), default(Type)));
        }
    }
}