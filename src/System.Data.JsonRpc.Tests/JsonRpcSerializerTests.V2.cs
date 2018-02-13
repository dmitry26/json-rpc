using System.Data.JsonRpc.Tests.Resources;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public partial class JsonRpcSerializerTests
    {
        // 2.0 Core tests

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
        public void V2CoreDeserializeResponseDataWhenJsonIsInvalid()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_core_json_invalid_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeResponseData(jsonSample));

            Assert.Equal(JsonRpcErrorCode.InvalidJson, exception.ErrorCode);
        }

        [Fact]
        public void V2CoreDeserializeResponseDataWhenBindingsAreDynamic()
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

        [Fact]
        public void V2CoreSerializeRequestWithFloatId()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_id_float_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("m", 1D);
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V2CoreDeserializeRequestDataWithFloatId()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_id_float_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.RequestContracts["m"] = new JsonRpcRequestContract();

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;
            var jsonRpcId = jsonRpcMessage.Id;

            Assert.Equal(JsonRpcIdType.Float, jsonRpcId.Type);
            Assert.Equal(1D, (double)jsonRpcId);
        }

        [Fact]
        public void V2CoreSerializeResponseWithFloatId()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_id_float_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(string.Empty, 1D);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V2CoreDeserializeResponseDataWithFloatId()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_id_float_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.ResponseContracts["m"] = new JsonRpcResponseContract(typeof(string));
            jsonRpcSerializer.StaticResponseBindings[1D] = "m";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;
            var jsonRpcId = jsonRpcMessage.Id;

            Assert.Equal(JsonRpcIdType.Float, jsonRpcId.Type);
            Assert.Equal(1D, (double)jsonRpcId);
        }
    }
}