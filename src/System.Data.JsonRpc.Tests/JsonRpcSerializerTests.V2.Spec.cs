using System.Collections.Generic;
using System.Data.JsonRpc.Tests.Resources;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public partial class JsonRpcSerializerTests
    {
        // Tests based on the JSON-RPC 2.0 specification (http://www.jsonrpc.org/specification)

        #region Example V2 T01: RPC call with positional parameters

        [Fact]
        public void V2SpecT010DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_01.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.RequestContracts["subtract"] = new JsonRpcRequestContract(new[] { typeof(long), typeof(long) });

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(1L, jsonRpcMessage.Id);
            Assert.Equal("subtract", jsonRpcMessage.Method);
            Assert.Equal(JsonRpcParamsType.ByPosition, jsonRpcMessage.ParamsType);
            Assert.Equal(new object[] { 42L, 23L }, jsonRpcMessage.ParamsByPosition);
        }

        [Fact]
        public void V2SpecT010SerializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_01.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("subtract", 1L, new object[] { 42L, 23L });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V2SpecT010DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_01.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.ResponseContracts["subtract"] = new JsonRpcResponseContract(typeof(long));
            jsonRpcSerializer.StaticResponseBindings[1L] = "subtract";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(1L, jsonRpcMessage.Id);
            Assert.IsType<long>(jsonRpcMessage.Result);
            Assert.Equal(19L, jsonRpcMessage.Result);
        }

        [Fact]
        public void V2SpecT010SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_01.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(19L, 1L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V2SpecT011DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_01.1_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.RequestContracts["subtract"] = new JsonRpcRequestContract(new[] { typeof(long), typeof(long) });

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(2L, jsonRpcMessage.Id);
            Assert.Equal("subtract", jsonRpcMessage.Method);
            Assert.Equal(JsonRpcParamsType.ByPosition, jsonRpcMessage.ParamsType);
            Assert.Equal(new object[] { 23L, 42L }, jsonRpcMessage.ParamsByPosition);
        }

        [Fact]
        public void V2SpecT011SerializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_01.1_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("subtract", 2L, new object[] { 23L, 42L });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V2SpecT011DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_01.1_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.ResponseContracts["subtract"] = new JsonRpcResponseContract(typeof(long));
            jsonRpcSerializer.StaticResponseBindings[2L] = "subtract";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(2L, jsonRpcMessage.Id);
            Assert.IsType<long>(jsonRpcMessage.Result);
            Assert.Equal(-19L, jsonRpcMessage.Result);
        }

        [Fact]
        public void V2SpecT011SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_01.1_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(-19L, 2L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        #endregion

        #region Example V2 T02: RPC call with named parameters

        [Fact]
        public void V2SpecT020DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_02.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            var jsonRpcSubtractParamsScheme = new Dictionary<string, Type>
            {
                ["subtrahend"] = typeof(long),
                ["minuend"] = typeof(long)
            };

            jsonRpcSerializer.RequestContracts["subtract"] = new JsonRpcRequestContract(jsonRpcSubtractParamsScheme);

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(3L, jsonRpcMessage.Id);
            Assert.Equal("subtract", jsonRpcMessage.Method);
            Assert.Equal(JsonRpcParamsType.ByName, jsonRpcMessage.ParamsType);
            Assert.Equal(23L, jsonRpcMessage.ParamsByName["subtrahend"]);
            Assert.Equal(42L, jsonRpcMessage.ParamsByName["minuend"]);
        }

        [Fact]
        public void V2SpecT020SerializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_02.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            var jsonRpcSubtractParams = new Dictionary<string, object>
            {
                ["subtrahend"] = 23L,
                ["minuend"] = 42L
            };

            var jsonRpcMessage = new JsonRpcRequest("subtract", 3L, jsonRpcSubtractParams);
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V2SpecT020DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_02.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.ResponseContracts["subtract"] = new JsonRpcResponseContract(typeof(long));
            jsonRpcSerializer.StaticResponseBindings[3] = "subtract";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(3L, jsonRpcMessage.Id);
            Assert.IsType<long>(jsonRpcMessage.Result);
            Assert.Equal(19L, jsonRpcMessage.Result);
        }

        [Fact]
        public void V2SpecT020SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_02.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(19L, 3L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V2SpecT021DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_02.1_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            var jsonRpcSubtractParamsScheme = new Dictionary<string, Type>
            {
                ["subtrahend"] = typeof(long),
                ["minuend"] = typeof(long)
            };

            jsonRpcSerializer.RequestContracts["subtract"] = new JsonRpcRequestContract(jsonRpcSubtractParamsScheme);

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(4L, jsonRpcMessage.Id);
            Assert.Equal("subtract", jsonRpcMessage.Method);
            Assert.Equal(JsonRpcParamsType.ByName, jsonRpcMessage.ParamsType);
            Assert.Equal(23L, jsonRpcMessage.ParamsByName["subtrahend"]);
            Assert.Equal(42L, jsonRpcMessage.ParamsByName["minuend"]);
        }

        [Fact]
        public void V2SpecT021SerializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_02.1_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            var jsonRpcSubtractParams = new Dictionary<string, object>
            {
                ["subtrahend"] = 23L,
                ["minuend"] = 42L
            };

            var jsonRpcMessage = new JsonRpcRequest("subtract", 4L, jsonRpcSubtractParams);
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V2SpecT021DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_02.1_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.ResponseContracts["subtract"] = new JsonRpcResponseContract(typeof(long));
            jsonRpcSerializer.StaticResponseBindings[4] = "subtract";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(4L, jsonRpcMessage.Id);
            Assert.IsType<long>(jsonRpcMessage.Result);
            Assert.Equal(19L, jsonRpcMessage.Result);
        }

        [Fact]
        public void V2SpecT021SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_02.1_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(19L, 4L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        #endregion

        #region Example V2 T03: a notification

        [Fact]
        public void V2SpecT030DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_03.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcSubtractParamsScheme = new[] { typeof(long), typeof(long), typeof(long), typeof(long), typeof(long) };

            jsonRpcSerializer.RequestContracts["update"] = new JsonRpcRequestContract(jsonRpcSubtractParamsScheme);

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(default, jsonRpcMessage.Id);
            Assert.Equal("update", jsonRpcMessage.Method);
            Assert.Equal(JsonRpcParamsType.ByPosition, jsonRpcMessage.ParamsType);
            Assert.Equal(new object[] { 1L, 2L, 3L, 4L, 5L }, jsonRpcMessage.ParamsByPosition);
        }

        [Fact]
        public void V2SpecT030SerializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_03.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("update", new object[] { 1L, 2L, 3L, 4L, 5L });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V2SpecT031DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_03.1_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.RequestContracts["foobar"] = new JsonRpcRequestContract();

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(default, jsonRpcMessage.Id);
            Assert.Equal("foobar", jsonRpcMessage.Method);
            Assert.Equal(JsonRpcParamsType.None, jsonRpcMessage.ParamsType);
        }

        [Fact]
        public void V2SpecT031SerializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_03.1_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("foobar");
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        #endregion

        #region Example V2 T04: RPC call of non-existent method

        [Fact]
        public void V2SpecT040DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_04.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.RequestContracts["foobar"] = new JsonRpcRequestContract();

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal("1", jsonRpcMessage.Id);
            Assert.Equal("foobar", jsonRpcMessage.Method);
            Assert.Equal(JsonRpcParamsType.None, jsonRpcMessage.ParamsType);
        }

        [Fact]
        public void V2SpecT040SerializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_04.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("foobar", "1");
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V2SpecT040DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_04.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal("1", jsonRpcMessage.Id);
            Assert.False(jsonRpcMessage.Success);

            var jsonRpcError = jsonRpcMessage.Error;

            Assert.Equal(JsonRpcErrorType.InvalidMethod, jsonRpcError.Type);
            Assert.NotNull(jsonRpcError.Message);
            Assert.Equal("Method not found", jsonRpcError.Message);
            Assert.False(jsonRpcError.HasData);
        }

        [Fact]
        public void V2SpecT040SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_04.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(new JsonRpcError(-32601L, "Method not found"), "1");
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        #endregion

        #region Example V2 T05: RPC call with invalid JSON

        [Fact]
        public void V2SpecT050DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_05.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.Parsing, exception.Type);
        }

        [Fact]
        public void V2SpecT050SerializeRequest()
        {
            // N/A
        }

        [Fact]
        public void V2SpecT050DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_05.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(default, jsonRpcMessage.Id);
            Assert.False(jsonRpcMessage.Success);

            var jsonRpcError = jsonRpcMessage.Error;

            Assert.Equal(JsonRpcErrorType.Parsing, jsonRpcError.Type);
            Assert.False(jsonRpcError.HasData);
        }

        [Fact]
        public void V2SpecT050SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_05.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(new JsonRpcError(-32700L, "Parse error"));
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        #endregion

        #region Example V2 T06: RPC call with invalid request object

        [Fact]
        public void V2SpecT060DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_06.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.RequestContracts["subtract"] = new JsonRpcRequestContract();

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.False(jsonRpcItem.IsValid);
            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcItem.Exception.Type);
        }

        [Fact]
        public void V2SpecT060SerializeRequest()
        {
            // N/A
        }

        [Fact]
        public void V2SpecT060DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_06.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(default, jsonRpcMessage.Id);
            Assert.False(jsonRpcMessage.Success);

            var jsonRpcError = jsonRpcMessage.Error;

            Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcError.Type);
            Assert.False(jsonRpcError.HasData);
        }

        [Fact]
        public void V2SpecT060SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_06.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"));
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        #endregion

        #region Example V2 T07: RPC call batch, invalid JSON

        [Fact]
        public void V2SpecT070DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_07.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.Parsing, exception.Type);
        }

        [Fact]
        public void V2SpecT070SerializeRequest()
        {
            // N/A
        }

        [Fact]
        public void V2SpecT070DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_07.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(default, jsonRpcMessage.Id);
            Assert.False(jsonRpcMessage.Success);

            var jsonRpcError = jsonRpcMessage.Error;

            Assert.Equal(JsonRpcErrorType.Parsing, jsonRpcError.Type);
            Assert.False(jsonRpcError.HasData);
        }

        [Fact]
        public void V2SpecT070SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_07.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(new JsonRpcError(-32700L, "Parse error"));
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        #endregion

        #region Example V2 T08: RPC call with an empty array

        [Fact]
        public void V2SpecT080DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_08.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
        }

        [Fact]
        public void V2SpecT080SerializeRequest()
        {
            // N/A
        }

        [Fact]
        public void V2SpecT080DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_08.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(default, jsonRpcMessage.Id);
            Assert.False(jsonRpcMessage.Success);

            var jsonRpcError = jsonRpcMessage.Error;

            Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcError.Type);
            Assert.False(jsonRpcError.HasData);
        }

        [Fact]
        public void V2SpecT080SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_08.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"));
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        #endregion

        #region Example V2 T09: RPC call with an invalid batch (but not empty)

        [Fact]
        public void V2SpecT090DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_09.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.True(jsonRpcData.IsBatch);
            Assert.Equal(1, jsonRpcData.BatchItems.Count);

            var jsonRpcItem0 = jsonRpcData.BatchItems[0];

            Assert.False(jsonRpcItem0.IsValid);
            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcItem0.Exception.Type);
        }

        [Fact]
        public void V2SpecT090SerializeRequest()
        {
            // N/A
        }

        [Fact]
        public void V2SpecT090DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_09.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.True(jsonRpcData.IsBatch);
            Assert.Equal(1, jsonRpcData.BatchItems.Count);

            var jsonRpcItem0 = jsonRpcData.BatchItems[0];

            Assert.True(jsonRpcItem0.IsValid);

            var jsonRpcMessage0 = jsonRpcItem0.Message;

            Assert.Equal(default, jsonRpcMessage0.Id);
            Assert.False(jsonRpcMessage0.Success);

            var jsonRpcError0 = jsonRpcMessage0.Error;

            Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcError0.Type);
            Assert.False(jsonRpcError0.HasData);
        }

        [Fact]
        public void V2SpecT090SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_09.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"));
            var jsonResult = jsonRpcSerializer.SerializeResponses(new[] { jsonRpcMessage });

            CompareJsonStrings(jsonSample, jsonResult);
        }

        #endregion

        #region Example V2 T10: RPC call with invalid batch

        [Fact]
        public void V2SpecT100DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_10.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.True(jsonRpcData.IsBatch);
            Assert.Equal(3, jsonRpcData.BatchItems.Count);

            foreach (var jsonRpcItem in jsonRpcData.BatchItems)
            {
                Assert.False(jsonRpcItem.IsValid);
            }
        }

        [Fact]
        public void V2SpecT100SerializeRequest()
        {
            // N/A
        }

        [Fact]
        public void V2SpecT100DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_10.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.True(jsonRpcData.IsBatch);
            Assert.Equal(3, jsonRpcData.BatchItems.Count);

            foreach (var jsonRpcItem in jsonRpcData.BatchItems)
            {
                Assert.True(jsonRpcItem.IsValid);

                var jsonRpcMessage = jsonRpcItem.Message;

                Assert.Equal(default, jsonRpcMessage.Id);
                Assert.False(jsonRpcMessage.Success);

                var jsonRpcError = jsonRpcMessage.Error;

                Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcError.Type);
                Assert.False(jsonRpcError.HasData);
            }
        }

        [Fact]
        public void V2SpecT100SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_10.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            var jsonRpcMessages = new[]
            {
                new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request")),
                new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request")),
                new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"))
            };

            var jsonResult = jsonRpcSerializer.SerializeResponses(jsonRpcMessages);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        #endregion

        #region Example V2 T11: RPC call batch

        [Fact]
        public void V2SpecT110DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_11.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.RequestContracts["sum"] = new JsonRpcRequestContract(new[] { typeof(long), typeof(long), typeof(long) });
            jsonRpcSerializer.RequestContracts["notify_hello"] = new JsonRpcRequestContract(new[] { typeof(long) });
            jsonRpcSerializer.RequestContracts["subtract"] = new JsonRpcRequestContract(new[] { typeof(long), typeof(long) });
            jsonRpcSerializer.RequestContracts["get_data"] = new JsonRpcRequestContract();

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.True(jsonRpcData.IsBatch);
            Assert.Equal(6, jsonRpcData.BatchItems.Count);

            var jsonRpcItem0 = jsonRpcData.BatchItems[0];

            Assert.True(jsonRpcItem0.IsValid);

            var jsonRpcMessage0 = jsonRpcItem0.Message;

            Assert.Equal("1", jsonRpcMessage0.Id);
            Assert.Equal("sum", jsonRpcMessage0.Method);
            Assert.Equal(JsonRpcParamsType.ByPosition, jsonRpcMessage0.ParamsType);
            Assert.Equal(new object[] { 1L, 2L, 4L }, jsonRpcMessage0.ParamsByPosition);

            var jsonRpcItem1 = jsonRpcData.BatchItems[1];

            Assert.True(jsonRpcItem1.IsValid);

            var jsonRpcMessage1 = jsonRpcItem1.Message;

            Assert.Equal(default, jsonRpcMessage1.Id);
            Assert.Equal("notify_hello", jsonRpcMessage1.Method);
            Assert.Equal(JsonRpcParamsType.ByPosition, jsonRpcMessage1.ParamsType);
            Assert.Equal(new object[] { 7L }, jsonRpcMessage1.ParamsByPosition);

            var jsonRpcItem2 = jsonRpcData.BatchItems[2];

            Assert.True(jsonRpcItem2.IsValid);

            var jsonRpcMessage2 = jsonRpcItem2.Message;

            Assert.Equal("2", jsonRpcMessage2.Id);
            Assert.Equal("subtract", jsonRpcMessage2.Method);
            Assert.Equal(JsonRpcParamsType.ByPosition, jsonRpcMessage2.ParamsType);
            Assert.Equal(new object[] { 42L, 23L }, jsonRpcMessage2.ParamsByPosition);

            var jsonRpcItem3 = jsonRpcData.BatchItems[3];

            Assert.False(jsonRpcItem3.IsValid);

            var jsonRpcItem4 = jsonRpcData.BatchItems[4];

            Assert.False(jsonRpcItem4.IsValid);

            var jsonRpcItem5 = jsonRpcData.BatchItems[5];

            Assert.True(jsonRpcItem5.IsValid);

            var jsonRpcMessage5 = jsonRpcItem5.Message;

            Assert.Equal("9", jsonRpcMessage5.Id);
            Assert.Equal("get_data", jsonRpcMessage5.Method);
            Assert.Equal(JsonRpcParamsType.None, jsonRpcMessage5.ParamsType);
        }

        [Fact]
        public void V2SpecT110SerializeRequest()
        {
            // N/A
        }

        [Fact]
        public void V2SpecT110DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_11.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.ResponseContracts["sum"] = new JsonRpcResponseContract(typeof(long));
            jsonRpcSerializer.ResponseContracts["subtract"] = new JsonRpcResponseContract(typeof(long));
            jsonRpcSerializer.ResponseContracts["get_data"] = new JsonRpcResponseContract(typeof(object[]));
            jsonRpcSerializer.StaticResponseBindings["1"] = "sum";
            jsonRpcSerializer.StaticResponseBindings["2"] = "subtract";
            jsonRpcSerializer.StaticResponseBindings["5"] = "foo.get";
            jsonRpcSerializer.StaticResponseBindings["9"] = "get_data";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.True(jsonRpcData.IsBatch);
            Assert.Equal(5, jsonRpcData.BatchItems.Count);

            var jsonRpcItem0 = jsonRpcData.BatchItems[0];

            Assert.True(jsonRpcItem0.IsValid);

            var jsonRpcMessage0 = jsonRpcItem0.Message;

            Assert.Equal("1", jsonRpcMessage0.Id);
            Assert.IsType<long>(jsonRpcMessage0.Result);
            Assert.Equal(7L, jsonRpcMessage0.Result);

            var jsonRpcItem1 = jsonRpcData.BatchItems[1];

            Assert.True(jsonRpcItem1.IsValid);

            var jsonRpcMessage1 = jsonRpcItem1.Message;

            Assert.Equal("2", jsonRpcMessage1.Id);
            Assert.IsType<long>(jsonRpcMessage1.Result);
            Assert.Equal(19L, jsonRpcMessage1.Result);

            var jsonRpcItem2 = jsonRpcData.BatchItems[2];

            Assert.True(jsonRpcItem2.IsValid);

            var jsonRpcMessage2 = jsonRpcItem2.Message;

            Assert.Equal(default, jsonRpcMessage2.Id);
            Assert.False(jsonRpcMessage2.Success);

            var jsonRpcError2 = jsonRpcMessage2.Error;

            Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcError2.Type);
            Assert.False(jsonRpcError2.HasData);

            var jsonRpcItem3 = jsonRpcData.BatchItems[3];

            Assert.True(jsonRpcItem3.IsValid);

            var jsonRpcMessage3 = jsonRpcItem3.Message;

            Assert.Equal("5", jsonRpcMessage3.Id);
            Assert.False(jsonRpcMessage3.Success);

            var jsonRpcError3 = jsonRpcMessage3.Error;

            Assert.Equal(JsonRpcErrorType.InvalidMethod, jsonRpcError3.Type);
            Assert.False(jsonRpcError3.HasData);

            var jsonRpcItem4 = jsonRpcData.BatchItems[4];

            Assert.True(jsonRpcItem4.IsValid);

            var jsonRpcMessage4 = jsonRpcItem4.Message;

            Assert.Equal("9", jsonRpcMessage4.Id);
            Assert.IsType<object[]>(jsonRpcMessage4.Result);
            Assert.Equal(new object[] { "hello", 5L }, jsonRpcMessage4.Result);
        }

        [Fact]
        public void V2SpecT110SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_11.0_res.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            var jsonRpcMessages = new[]
            {
                new JsonRpcResponse(7L, "1"),
                new JsonRpcResponse(19L, "2"),
                new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request")),
                new JsonRpcResponse(new JsonRpcError(-32601L, "Method not found"), "5"),
                new JsonRpcResponse(new object[] { "hello", 5L }, "9")
            };

            var jsonResult = jsonRpcSerializer.SerializeResponses(jsonRpcMessages);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        #endregion

        #region Example V2 T12: RPC call batch (all notifications)

        [Fact]
        public void V2SpecT120DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_12.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.RequestContracts["notify_sum"] = new JsonRpcRequestContract(new[] { typeof(long), typeof(long), typeof(long) });
            jsonRpcSerializer.RequestContracts["notify_hello"] = new JsonRpcRequestContract(new[] { typeof(long) });

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.True(jsonRpcData.IsBatch);
            Assert.Equal(2, jsonRpcData.BatchItems.Count);

            var jsonRpcItem0 = jsonRpcData.BatchItems[0];

            Assert.True(jsonRpcItem0.IsValid);

            var jsonRpcMessage0 = jsonRpcItem0.Message;

            Assert.Equal(default, jsonRpcMessage0.Id);
            Assert.Equal("notify_sum", jsonRpcMessage0.Method);
            Assert.Equal(JsonRpcParamsType.ByPosition, jsonRpcMessage0.ParamsType);
            Assert.Equal(new object[] { 1L, 2L, 4L }, jsonRpcMessage0.ParamsByPosition);

            var jsonRpcItem1 = jsonRpcData.BatchItems[1];

            Assert.True(jsonRpcItem1.IsValid);

            var jsonRpcMessage1 = jsonRpcItem1.Message;

            Assert.Equal(default, jsonRpcMessage1.Id);
            Assert.Equal("notify_hello", jsonRpcMessage1.Method);
            Assert.Equal(JsonRpcParamsType.ByPosition, jsonRpcMessage1.ParamsType);
            Assert.Equal(new object[] { 7L }, jsonRpcMessage1.ParamsByPosition);
        }

        [Fact]
        public void V2SpecT120SerializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v2_spec_12.0_req.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            var jsonRpcMessages = new[]
            {
                new JsonRpcRequest("notify_sum", new object[] { 1L, 2L, 4L }),
                new JsonRpcRequest("notify_hello", new object[] { 7L })
            };

            var jsonResult = jsonRpcSerializer.SerializeRequests(jsonRpcMessages);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V2SpecT120DeserializeResponse()
        {
            // N/A
        }

        [Fact]
        public void V2SpecT120SerializeResponse()
        {
            // N/A
        }

        #endregion
    }
}