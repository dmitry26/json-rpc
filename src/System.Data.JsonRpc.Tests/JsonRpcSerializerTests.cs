using System.Data.JsonRpc.Tests.Resources;
using Newtonsoft.Json.Linq;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed partial class JsonRpcSerializerTests
    {
        [Fact]
        public void SerializeRequestWhenRequestIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeRequest(null));
        }

        [Fact]
        public void SerializeRequestsWhenCollectionIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeRequests(null));
        }

        [Fact]
        public void SerializeRequestsWhenCollectionIsEmptyCollection()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeRequests(new JsonRpcRequest[] { }));

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
        }

        [Fact]
        public void SerializeRequestsWhenCollectionContainsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeRequests(new JsonRpcRequest[] { null }));

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
        }

        [Fact]
        public void SerializeRequestsWhenCollectionContainsDuplicateIdAndIdIsNumber()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var jsonRpcMessages = new[]
            {
                new JsonRpcRequest("m_1", 1L),
                new JsonRpcRequest("m_2", 1L)
            };

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeRequests(jsonRpcMessages));

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
        }

        [Fact]
        public void SerializeRequestsWhenCollectionContainsDuplicateIdAndIdIsString()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var jsonRpcMessages = new[]
            {
                new JsonRpcRequest("m_1", "1"),
                new JsonRpcRequest("m_2", "1")
            };

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeRequests(jsonRpcMessages));

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
        }

        [Fact]
        public void SerializeRequestWithParameterEqualsNull()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_param_null.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("m", 1L, new object[] { null });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SerializeResponseWhenResponseIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeResponse(null));
        }

        [Fact]
        public void SerializeResponsesWhenCollectionIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeResponses(null));
        }

        [Fact]
        public void SerializeResponsesWhenCollectionIsEmptyCollection()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonResult = jsonRpcSerializer.SerializeResponses(new JsonRpcResponse[] { });

            Assert.Equal(string.Empty, jsonResult);
        }

        [Fact]
        public void SerializeResponsesWhenCollectionContainsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeResponses(new JsonRpcResponse[] { null }));

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
        }

        [Fact]
        public void SerializeResponsesWhenCollectionContainsDuplicateIdAndIdIsNumber()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var jsonRpcMessages = new[]
            {
                new JsonRpcResponse(new[] { 2L }, 1L),
                new JsonRpcResponse(new[] { 3L }, 1L)
            };

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeResponses(jsonRpcMessages));

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
        }

        [Fact]
        public void SerializeResponsesWhenCollectionContainsDuplicateIdAndIdIsString()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var jsonRpcMessages = new[]
            {
                new JsonRpcResponse(new[] { 2L }, "1"),
                new JsonRpcResponse(new[] { 3L }, "1")
            };

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeResponses(jsonRpcMessages));

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
        }

        [Fact]
        public void DeserializeRequestDataWhenJsonStringIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.DeserializeRequestData(null));
        }

        [Fact]
        public void DeserializeRequestDataWhenJsonStringIsEmptyString()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestData(string.Empty));

            Assert.Equal(JsonRpcExceptionType.Parsing, exception.Type);
        }

        [Fact]
        public void DeserializeRequestDataWhenJsonIsInvalid()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_json_invalid_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.Parsing, exception.Type);
        }

        [Fact]
        public void DeserializeRequestDataWhenJsonProtocolIsAbsent()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_protocol_undefined_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }

        [Fact]
        public void DeserializeResponseDataWhenJsonProtocolIsAbsent()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_protocol_undefined_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }

        [Fact]
        public void DeserializeRequestDataWhenJsonProtocolHasInvalidType()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_protocol_invalid_type_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }

        [Fact]
        public void DeserializeResponseDataWhenJsonProtocolHasInvalidType()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_protocol_invalid_type_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }

        [Fact]
        public void DeserializeRequestDataWhenJsonProtocolHasInvalidValue()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_protocol_invalid_value_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }

        [Fact]
        public void DeserializeResponseDataWhenJsonProtocolHasInvalidValue()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_protocol_invalid_value_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }

        [Fact]
        public void DeserializeRequestDataWhenParamsHasInvalidType()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_params_invalid_type.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.RequestContracts["m"] = new JsonRpcRequestContract(new[] { typeof(object) });

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }

        [Fact]
        public void DeserializeResponseDataWhenJsonStringIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.DeserializeResponseData(null));
        }

        [Fact]
        public void DeserializeResponseDataWhenJsonStringIsEmptyString()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(string.Empty);

            Assert.NotNull(jsonRpcData);
            Assert.True(jsonRpcData.IsEmpty);
        }

        [Fact]
        public void DeserializeResponseDataWhenJsonIsInvalid()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_json_invalid_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeResponseData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.Parsing, exception.Type);
        }

        [Fact]
        public void DeserializeResponseDataWhenMethodSchemeBindingIsDynamic()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_binding_dynamic.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.DynamicResponseBindings[1L] = new JsonRpcResponseContract(typeof(int));

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(1, jsonRpcMessage.Id);
            Assert.True(jsonRpcMessage.Success);
            Assert.Equal(42, jsonRpcMessage.Result);
        }

        [Fact]
        public void DeserializeResponseDataWhenResultIsNull()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_result_null.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.ResponseContracts["m"] = new JsonRpcResponseContract(typeof(string));
            jsonRpcSerializer.StaticResponseBindings[1L] = "m";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(1, jsonRpcMessage.Id);
            Assert.True(jsonRpcMessage.Success);
            Assert.Null(jsonRpcMessage.Result);
        }

        [Fact]
        public void DeserializeResponseDataWhenResultHasNoId()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_result_wo_id.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.ResponseContracts["m"] = new JsonRpcResponseContract(typeof(string));
            jsonRpcSerializer.StaticResponseBindings[1L] = "m";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }

        [Fact]
        public void DeserializeResponseDataWhenErrorCodeIsInvalid()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_error_code_invalid.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.ResponseContracts["m"] = new JsonRpcResponseContract(typeof(string));
            jsonRpcSerializer.StaticResponseBindings[1L] = "m";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }
    }
}