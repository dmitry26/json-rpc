using System.Collections.Generic;
using System.Data.JsonRpc.Tests.Resources;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public partial class JsonRpcSerializerTests
    {
        // Tests based on the Bitcoin protocol specification (https://bitcoin.org/en/developer-reference#remote-procedure-calls-rpcs)

        #region Example V1 T01: Bitcoin method "getblockhash" with successful result

        [Fact]
        public void V1BitcoinT01DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_btc_01_req.json");

            var jsonRpcSerializer = new JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            jsonRpcSerializer.RequestContracts["getblockhash"] = new JsonRpcRequestContract(new[] { typeof(long) });

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal("foo", jsonRpcMessage.Id);
            Assert.Equal("getblockhash", jsonRpcMessage.Method);
            Assert.Equal(JsonRpcParametersType.ByPosition, jsonRpcMessage.ParametersType);
            Assert.Equal(new object[] { 0L }, jsonRpcMessage.ParametersByPosition);
        }

        [Fact]
        public void V1BitcoinT01SerializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_btc_01_req.json");

            var jsonRpcSerializer = new JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            var jsonRpcMessage = new JsonRpcRequest("getblockhash", "foo", new object[] { 0L });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V1BitcoinT01DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_btc_01_res.json");

            var jsonRpcSerializer = new JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            jsonRpcSerializer.ResponseContracts["getblockhash"] = new JsonRpcResponseContract(typeof(string));
            jsonRpcSerializer.StaticResponseBindings["foo"] = "getblockhash";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal("foo", jsonRpcMessage.Id);
            Assert.IsType<string>(jsonRpcMessage.Result);
            Assert.Equal("000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f", jsonRpcMessage.Result);
        }

        [Fact]
        public void V1BitcoinT01SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_btc_01_res.json");

            var jsonRpcSerializer = new JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            var jsonRpcMessage = new JsonRpcResponse("000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f", "foo");
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        #endregion

        #region Example V1 T02: Bitcoin method "getblockhash" with error result

        [Fact]
        public void V1BitcoinT02DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_btc_02_req.json");

            var jsonRpcSerializer = new JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            jsonRpcSerializer.RequestContracts["getblockhash"] = new JsonRpcRequestContract(new[] { typeof(long) });

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal("foo", jsonRpcMessage.Id);
            Assert.Equal("getblockhash", jsonRpcMessage.Method);
            Assert.Equal(JsonRpcParametersType.ByPosition, jsonRpcMessage.ParametersType);
            Assert.Equal(new object[] { -1L }, jsonRpcMessage.ParametersByPosition);
        }

        [Fact]
        public void V1BitcoinT02SerializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_btc_02_req.json");

            var jsonRpcSerializer = new JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            var jsonRpcMessage = new JsonRpcRequest("getblockhash", "foo", new object[] { -1L });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V1BitcoinT02DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_btc_02_res.json");

            var jsonRpcSerializer = new JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            jsonRpcSerializer.ResponseContracts["getblockhash"] = new JsonRpcResponseContract(typeof(string));
            jsonRpcSerializer.StaticResponseBindings["foo"] = "getblockhash";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal("foo", jsonRpcMessage.Id);
            Assert.False(jsonRpcMessage.Success);

            var jsonRpcError = jsonRpcMessage.Error;

            Assert.Equal(-8L, jsonRpcError.Code);
            Assert.Equal("Block height out of range", jsonRpcError.Message);
        }

        [Fact]
        public void V1BitcoinT02SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_btc_02_res.json");

            var jsonRpcSerializer = new JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            var jsonRpcMessage = new JsonRpcResponse(new JsonRpcError(-8L, "Block height out of range"), "foo");
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        #endregion

        // 1.0 Core tests

        [Fact]
        public void V1CoreSerializeRequestWhenParametersAreByName()
        {
            var jsonRpcSerializer = new JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            var parameters = new Dictionary<string, object>
            {
                ["p"] = 1L
            };

            var message = new JsonRpcRequest("m", parameters);

            Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeRequest(message));
        }

        [Fact]
        public void V1CoreDeserializeResponseWhenErrorTypeIsInvali()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_core_error_type_invalid.json");

            var jsonRpcSerializer = new JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            jsonRpcSerializer.ResponseContracts["m"] = new JsonRpcResponseContract(typeof(string));
            jsonRpcSerializer.StaticResponseBindings[1L] = "m";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.False(jsonRpcMessage.Success);
            Assert.Equal(1L, jsonRpcMessage.Id);
            Assert.Null(jsonRpcMessage.Result);
            Assert.NotNull(jsonRpcMessage.Error);

            var jsonRpcError = jsonRpcMessage.Error;

            Assert.Equal(default, jsonRpcError.Code);
            Assert.Equal(string.Empty, jsonRpcError.Message);
            Assert.False(jsonRpcError.HasData);
        }

        [Fact]
        public void V1CoreDeserializeResponseWhenStructureIsInvalidAndSuccessIsFalse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_core_struct_invalid_success_false.json");

            var jsonRpcSerializer = new JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

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
        public void V1CoreDeserializeResponseWhenStructureIsInvalidAndSuccessIsTrue()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_core_struct_invalid_success_true.json");

            var jsonRpcSerializer = new JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

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