using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace System.Data.JsonRpc.Tests
{
    public sealed partial class JsonRpcSerializerTests
    {
        private readonly ITestOutputHelper _output;

        public JsonRpcSerializerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private void CompareJsonStrings(string expected, string actual)
        {
            var expectedToken = JToken.Parse(expected);
            var actualToken = JToken.Parse(actual);

            _output.WriteLine(actualToken.ToString(Formatting.Indented));

            Assert.True(JToken.DeepEquals(expectedToken, actualToken), "Actual JSON string differs from expected");
        }

        [Fact]
        public void CoreSerializeRequestWhenRequestIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeRequest(null));
        }

        [Fact]
        public void CoreSerializeRequestsWhenCollectionIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeRequests(null));
        }

        [Fact]
        public void CoreSerializeRequestsWhenCollectionIsEmpty()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeRequests(new JsonRpcRequest[] { }));

            Assert.Equal(JsonRpcErrorCode.InvalidMessage, exception.ErrorCode);
        }

        [Fact]
        public void CoreSerializeRequestsWhenCollectionContainsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeRequests(new JsonRpcRequest[] { null }));

            Assert.Equal(JsonRpcErrorCode.InvalidMessage, exception.ErrorCode);
        }

        [Fact]
        public void CoreSerializeResponseWhenResponseIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeResponse(null));
        }

        [Fact]
        public void CoreSerializeResponsesWhenCollectionIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeResponses(null));
        }

        [Fact]
        public void CoreSerializeResponsesWhenCollectionIsEmpty()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeResponses(new JsonRpcResponse[] { }));

            Assert.Equal(JsonRpcErrorCode.InvalidMessage, exception.ErrorCode);
        }

        [Fact]
        public void CoreSerializeResponsesWhenCollectionContainsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeResponses(new JsonRpcResponse[] { null }));

            Assert.Equal(JsonRpcErrorCode.InvalidMessage, exception.ErrorCode);
        }

        [Fact]
        public void CoreDeserializeRequestDataWhenJsonStringIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.DeserializeRequestData(null));
        }

        [Fact]
        public void CoreDeserializeRequestDataWhenJsonStringIsEmpty()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestData(string.Empty));

            Assert.Equal(JsonRpcErrorCode.InvalidJson, exception.ErrorCode);
        }

        [Fact]
        public void CoreDeserializeResponseDataWhenJsonStringIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.DeserializeResponseData(null));
        }

        [Fact]
        public void CoreDeserializeResponseDataWhenJsonStringIsEmpty()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeResponseData(string.Empty));

            Assert.Equal(JsonRpcErrorCode.InvalidJson, exception.ErrorCode);
        }
    }
}