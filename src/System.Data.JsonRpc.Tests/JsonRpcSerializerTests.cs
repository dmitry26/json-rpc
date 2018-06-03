using System.Data.JsonRpc.Tests.Resources;
using System.IO;
using System.Text;
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
        public void CoreSerializeRequestWhenParametersAreByPositionAndIsEmptyCollection()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("m", 1L, new object[] { });

            var exception = Assert.Throws<JsonRpcException>(() =>
                 jsonRpcSerializer.SerializeRequest(jsonRpcMessage));

            Assert.Equal(JsonRpcErrorCodes.InvalidOperation, exception.ErrorCode);
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

            Assert.Equal(JsonRpcErrorCodes.InvalidMessage, exception.ErrorCode);
        }

        [Fact]
        public void CoreSerializeRequestsWhenCollectionContainsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeRequests(new JsonRpcRequest[] { null }));

            Assert.Equal(JsonRpcErrorCodes.InvalidMessage, exception.ErrorCode);
        }

        [Fact]
        public void CoreSerializeRequestToStreamWhenRequestIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("m", 0L);

            using (var jsonStream = new MemoryStream())
            {
                Assert.Throws<ArgumentNullException>(() =>
                    jsonRpcSerializer.SerializeRequest(null, jsonStream));
            }
        }

        [Fact]
        public void CoreSerializeRequestToStreamWhenStreamIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("m", 0L);

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeRequest(jsonRpcMessage, null));
        }

        [Fact]
        public void CoreSerializeRequestToStream()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("m", 0L);

            using (var jsonStream = new MemoryStream())
            {
                jsonRpcSerializer.SerializeRequest(jsonRpcMessage, jsonStream);

                var jsonResult = Encoding.UTF8.GetString(jsonStream.ToArray());

                CompareJsonStrings(jsonSample, jsonResult);
            }
        }

        [Fact]
        public void CoreSerializeRequestsToStreamWhenRequestsIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("m", 0L);

            using (var jsonStream = new MemoryStream())
            {
                Assert.Throws<ArgumentNullException>(() =>
                    jsonRpcSerializer.SerializeRequests(null, jsonStream));
            }
        }

        [Fact]
        public void CoreSerializeRequestsToStreamWhenStreamIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage1 = new JsonRpcRequest("m", 0L);
            var jsonRpcMessage2 = new JsonRpcRequest("m", 1L);
            var jsonRpcMessages = new[] { jsonRpcMessage1, jsonRpcMessage2 };

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeRequests(jsonRpcMessages, null));
        }

        [Fact]
        public void CoreSerializeRequestsToStream()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_batch_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage1 = new JsonRpcRequest("m", 0L);
            var jsonRpcMessage2 = new JsonRpcRequest("m", 1L);
            var jsonRpcMessages = new[] { jsonRpcMessage1, jsonRpcMessage2 };

            using (var jsonStream = new MemoryStream())
            {
                jsonRpcSerializer.SerializeRequests(jsonRpcMessages, jsonStream);

                var jsonResult = Encoding.UTF8.GetString(jsonStream.ToArray());

                CompareJsonStrings(jsonSample, jsonResult);
            }
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

            Assert.Equal(JsonRpcErrorCodes.InvalidMessage, exception.ErrorCode);
        }

        [Fact]
        public void CoreSerializeResponsesWhenCollectionContainsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeResponses(new JsonRpcResponse[] { null }));

            Assert.Equal(JsonRpcErrorCodes.InvalidMessage, exception.ErrorCode);
        }

        [Fact]
        public void CoreSerializeResponseToStreamWhenResponseIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("m", 0L);

            using (var jsonStream = new MemoryStream())
            {
                Assert.Throws<ArgumentNullException>(() =>
                    jsonRpcSerializer.SerializeResponse(null, jsonStream));
            }
        }

        [Fact]
        public void CoreSerializeResponseToStreamWhenStreamIsNull()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(0L, 0L);

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeResponse(jsonRpcMessage, null));
        }

        [Fact]
        public void CoreSerializeResponseToStream()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(0L, 0L);

            using (var jsonStream = new MemoryStream())
            {
                jsonRpcSerializer.SerializeResponse(jsonRpcMessage, jsonStream);

                var jsonResult = Encoding.UTF8.GetString(jsonStream.ToArray());

                CompareJsonStrings(jsonSample, jsonResult);
            }
        }

        [Fact]
        public void CoreSerializeResponsesToStreamWhenResponsesIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("m", 0L);

            using (var jsonStream = new MemoryStream())
            {
                Assert.Throws<ArgumentNullException>(() =>
                    jsonRpcSerializer.SerializeResponses(null, jsonStream));
            }
        }

        [Fact]
        public void CoreSerializeResponsesToStreamWhenStreamIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage1 = new JsonRpcResponse(0L, 0L);
            var jsonRpcMessage2 = new JsonRpcResponse(0L, 1L);
            var jsonRpcMessages = new[] { jsonRpcMessage1, jsonRpcMessage2 };

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeResponses(jsonRpcMessages, null));
        }

        [Fact]
        public void CoreSerializeResponsesToStream()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_batch_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage1 = new JsonRpcResponse(0L, 0L);
            var jsonRpcMessage2 = new JsonRpcResponse(0L, 1L);
            var jsonRpcMessages = new[] { jsonRpcMessage1, jsonRpcMessage2 };

            using (var jsonStream = new MemoryStream())
            {
                jsonRpcSerializer.SerializeResponses(jsonRpcMessages, jsonStream);

                var jsonResult = Encoding.UTF8.GetString(jsonStream.ToArray());

                CompareJsonStrings(jsonSample, jsonResult);
            }
        }

        [Fact]
        public void CoreDeserializeRequestDataWhenJsonStringIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.DeserializeRequestData((string)null));
        }

        [Fact]
        public void CoreDeserializeRequestDataWhenJsonStringIsEmpty()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestData(string.Empty));

            Assert.Equal(JsonRpcErrorCodes.InvalidJson, exception.ErrorCode);
        }

        [Fact]
        public void CoreDeserializeRequestDataFromStreamWhenJsonStreamIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.DeserializeRequestData((Stream)null));
        }

        [Fact]
        public void CoreDeserializeRequestDataFromStreamWhenJsonStreamIsEmpty()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestData(Stream.Null));

            Assert.Equal(JsonRpcErrorCodes.InvalidJson, exception.ErrorCode);
        }

        [Fact]
        public void CoreDeserializeRequestDatatFromStream()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.RequestContracts["m"] = new JsonRpcRequestContract();

            using (var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonSample)))
            {
                var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

                Assert.False(jsonRpcData.IsBatch);

                var jsonRpcItem = jsonRpcData.Item;

                Assert.True(jsonRpcItem.IsValid);

                var jsonRpcMessage = jsonRpcItem.Message;

                Assert.Equal(0L, jsonRpcMessage.Id);
                Assert.Equal("m", jsonRpcMessage.Method);
                Assert.Equal(JsonRpcParametersType.None, jsonRpcMessage.ParametersType);
            }
        }

        [Fact]
        public void CoreDeserializeResponseDataWhenJsonStringIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.DeserializeResponseData((string)null));
        }

        [Fact]
        public void CoreDeserializeResponseDataWhenJsonStringIsEmpty()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeResponseData(string.Empty));

            Assert.Equal(JsonRpcErrorCodes.InvalidJson, exception.ErrorCode);
        }

        [Fact]
        public void CoreDeserializeResponseDataFromStreamWhenJsonStreamIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.DeserializeResponseData((Stream)null));
        }

        [Fact]
        public void CoreDeserializeResponseDataFromStreamWhenJsonStreamIsEmpty()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeResponseData(Stream.Null));

            Assert.Equal(JsonRpcErrorCodes.InvalidJson, exception.ErrorCode);
        }

        [Fact]
        public void CoreDeserializeResponseDatatFromStream()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.DynamicResponseBindings[0L] = new JsonRpcResponseContract(typeof(long));

            using (var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonSample)))
            {
                var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

                Assert.False(jsonRpcData.IsBatch);

                var jsonRpcItem = jsonRpcData.Item;

                Assert.True(jsonRpcItem.IsValid);

                var jsonRpcMessage = jsonRpcItem.Message;

                Assert.Equal(0L, jsonRpcMessage.Id);
                Assert.True(jsonRpcMessage.Success);
                Assert.Equal(0L, jsonRpcMessage.Result);
            }
        }
    }
}