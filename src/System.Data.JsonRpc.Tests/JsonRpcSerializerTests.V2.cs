using System.Data.JsonRpc.Tests.Resources;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public partial class JsonRpcSerializerTests
    {
        // Core tests

        [Fact]
        public void V2CoreSerializeRequestWhenRequestIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeRequest(null));
        }

        [Fact]
        public void V2CoreSerializeRequestsWhenCollectionIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeRequests(null));
        }

        [Fact]
        public void V2CoreSerializeRequestsWhenCollectionIsEmptyCollection()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeRequests(new JsonRpcRequest[] { }));

            Assert.Equal(JsonRpcErrorCode.InvalidMessage, exception.ErrorCode);
        }

        [Fact]
        public void V2CoreSerializeRequestsWhenCollectionContainsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeRequests(new JsonRpcRequest[] { null }));

            Assert.Equal(JsonRpcErrorCode.InvalidMessage, exception.ErrorCode);
        }

        [Fact]
        public void V2CoreSerializeRequestWithParameterEqualsNull()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_param_null.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("m", 1L, new object[] { null });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V2CoreSerializeResponseWhenResponseIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeResponse(null));
        }

        [Fact]
        public void V2CoreSerializeResponsesWhenCollectionIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeResponses(null));
        }

        [Fact]
        public void V2CoreSerializeResponsesWhenCollectionContainsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeResponses(new JsonRpcResponse[] { null }));

            Assert.Equal(JsonRpcErrorCode.InvalidMessage, exception.ErrorCode);
        }

        [Fact]
        public void V2CoreDeserializeRequestDataWhenJsonStringIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.DeserializeRequestData(null));
        }

        [Fact]
        public void V2CoreDeserializeRequestDataWhenJsonStringIsEmptyString()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestData(string.Empty));

            Assert.Equal(JsonRpcErrorCode.InvalidJson, exception.ErrorCode);
        }

        [Fact]
        public void V2CoreDeserializeRequestDataWhenJsonIsInvalid()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_json_invalid_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestData(jsonSample));

            Assert.Equal(JsonRpcErrorCode.InvalidJson, exception.ErrorCode);
        }

        [Fact]
        public void V2CoreDeserializeRequestDataWhenProtocolIsAbsent()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_protocol_undefined_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcErrorCode.InvalidMessage, jsonRpcException.ErrorCode);
        }

        [Fact]
        public void V2CoreDeserializeResponseDataWhenProtocolIsAbsent()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_protocol_undefined_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcErrorCode.InvalidMessage, jsonRpcException.ErrorCode);
        }

        [Fact]
        public void V2CoreDeserializeRequestDataWhenProtocolHasInvalidType()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_protocol_invalid_type_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcErrorCode.InvalidMessage, jsonRpcException.ErrorCode);
        }

        [Fact]
        public void V2CoreDeserializeResponseDataWhenProtocolHasInvalidType()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_protocol_invalid_type_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcErrorCode.InvalidMessage, jsonRpcException.ErrorCode);
        }

        [Fact]
        public void V2CoreDeserializeRequestDataWhenProtocolHasInvalidValue()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_protocol_invalid_value_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcErrorCode.InvalidMessage, jsonRpcException.ErrorCode);
        }

        [Fact]
        public void V2CoreDeserializeResponseDataWhenProtocolHasInvalidValue()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_protocol_invalid_value_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcErrorCode.InvalidMessage, jsonRpcException.ErrorCode);
        }

        [Fact]
        public void V2CoreDeserializeRequestDataWhenParametersHasInvalidType()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_params_invalid_type.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.RequestContracts["m"] = new JsonRpcRequestContract(new[] { typeof(object) });

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcErrorCode.InvalidMessage, jsonRpcException.ErrorCode);
        }

        [Fact]
        public void V2CoreDeserializeResponseDataWhenJsonStringIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.DeserializeResponseData(null));
        }

        [Fact]
        public void V2CoreDeserializeResponseDataWhenJsonIsInvalid()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_json_invalid_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeResponseData(jsonSample));

            Assert.Equal(JsonRpcErrorCode.InvalidJson, exception.ErrorCode);
        }

        [Fact]
        public void V2CoreDeserializeResponseDataWhenMethodSchemeBindingIsDynamic()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_binding_dynamic.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.DynamicResponseBindings[1L] = new JsonRpcResponseContract(typeof(int));

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(1, jsonRpcMessage.Id);
            Assert.True(jsonRpcMessage.Success);
            Assert.Equal(42, jsonRpcMessage.Result);
        }

        [Fact]
        public void V2CoreDeserializeResponseDataWhenResultIsNull()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_result_null.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.ResponseContracts["m"] = new JsonRpcResponseContract(typeof(string));
            jsonRpcSerializer.StaticResponseBindings[1L] = "m";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(1, jsonRpcMessage.Id);
            Assert.True(jsonRpcMessage.Success);
            Assert.Null(jsonRpcMessage.Result);
        }

        [Fact]
        public void V2CoreDeserializeResponseDataWhenResultHasNoId()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_result_wo_id.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.ResponseContracts["m"] = new JsonRpcResponseContract(typeof(string));
            jsonRpcSerializer.StaticResponseBindings[1L] = "m";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcErrorCode.InvalidMessage, jsonRpcException.ErrorCode);
        }

        [Fact]
        public void V2CoreDeserializeResponseDataWhenErrorCodeIsInvalid()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_error_invalid_code.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.ResponseContracts["m"] = new JsonRpcResponseContract(typeof(string));
            jsonRpcSerializer.StaticResponseBindings[1L] = "m";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.False(jsonRpcItem.IsValid);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcErrorCode.InvalidMessage, jsonRpcException.ErrorCode);
        }
    }
}