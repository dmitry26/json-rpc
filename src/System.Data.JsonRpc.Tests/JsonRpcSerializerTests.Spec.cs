using System.Collections.Generic;
using System.Data.JsonRpc.Tests.Resources;
using Newtonsoft.Json.Linq;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public partial class JsonRpcSerializerTests
    {
        #region Example #01: RPC call with positional parameters

        [Fact]
        public void SpecExample010RequestDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["subtract"] = new JsonRpcMethodScheme(new[] { typeof(long), typeof(long) });

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_01.0_req.txt");
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
        public void SpecExample010RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("subtract", 1L, new object[] { 42L, 23L });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_01.0_req.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample010ResponseDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["subtract"] = new JsonRpcMethodScheme(typeof(long));

            var jsonRpcBindings = new Dictionary<JsonRpcId, string>
            {
                [1L] = "subtract"
            };

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_01.0_res.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(1L, jsonRpcMessage.Id);
            Assert.IsType<long>(jsonRpcMessage.Result);
            Assert.Equal(19L, jsonRpcMessage.Result);
        }

        [Fact]
        public void SpecExample010ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(19L, 1L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_01.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample011RequestDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["subtract"] = new JsonRpcMethodScheme(new[] { typeof(long), typeof(long) });

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_01.1_req.txt");
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
        public void SpecExample011RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("subtract", 2L, new object[] { 23L, 42L });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_01.1_req.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample011ResponseDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["subtract"] = new JsonRpcMethodScheme(typeof(long));

            var jsonRpcBindings = new Dictionary<JsonRpcId, string>
            {
                [2L] = "subtract"
            };

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_01.1_res.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(2L, jsonRpcMessage.Id);
            Assert.IsType<long>(jsonRpcMessage.Result);
            Assert.Equal(-19L, jsonRpcMessage.Result);
        }

        [Fact]
        public void SpecExample011ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(-19L, 2L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_01.1_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #02: RPC call with named parameters

        [Fact]
        public void SpecExample020RequestDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            var jsonRpcSubtractParamsScheme = new Dictionary<string, Type>
            {
                ["subtrahend"] = typeof(long),
                ["minuend"] = typeof(long)
            };

            jsonRpcScheme.Methods["subtract"] = new JsonRpcMethodScheme(jsonRpcSubtractParamsScheme);

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_02.0_req.txt");
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
        public void SpecExample020RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var jsonRpcSubtractParams = new Dictionary<string, object>
            {
                ["subtrahend"] = 23L,
                ["minuend"] = 42L
            };

            var jsonRpcMessage = new JsonRpcRequest("subtract", 3L, jsonRpcSubtractParams);
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_02.0_req.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample020ResponseDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["subtract"] = new JsonRpcMethodScheme(typeof(long));

            var jsonRpcBindings = new Dictionary<JsonRpcId, string>
            {
                [3] = "subtract"
            };

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_02.0_res.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(3L, jsonRpcMessage.Id);
            Assert.IsType<long>(jsonRpcMessage.Result);
            Assert.Equal(19L, jsonRpcMessage.Result);
        }

        [Fact]
        public void SpecExample020ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(19L, 3L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_02.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample021RequestDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            var jsonRpcSubtractParamsScheme = new Dictionary<string, Type>
            {
                ["subtrahend"] = typeof(long),
                ["minuend"] = typeof(long)
            };

            jsonRpcScheme.Methods["subtract"] = new JsonRpcMethodScheme(jsonRpcSubtractParamsScheme);

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_02.1_req.txt");
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
        public void SpecExample021RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var jsonRpcSubtractParams = new Dictionary<string, object>
            {
                ["subtrahend"] = 23L,
                ["minuend"] = 42L
            };

            var jsonRpcMessage = new JsonRpcRequest("subtract", 4L, jsonRpcSubtractParams);
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_02.1_req.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample021ResponseDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["subtract"] = new JsonRpcMethodScheme(typeof(long));

            var jsonRpcBindings = new Dictionary<JsonRpcId, string>
            {
                [4] = "subtract"
            };

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_02.1_res.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(4L, jsonRpcMessage.Id);
            Assert.IsType<long>(jsonRpcMessage.Result);
            Assert.Equal(19L, jsonRpcMessage.Result);
        }

        [Fact]
        public void SpecExample021ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(19L, 4L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_02.1_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #03: a notification

        [Fact]
        public void SpecExample030RequestDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSubtractParamsScheme = new[] { typeof(long), typeof(long), typeof(long), typeof(long), typeof(long) };

            jsonRpcScheme.Methods["update"] = new JsonRpcMethodScheme(jsonRpcSubtractParamsScheme);

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_03.0_req.txt");
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
        public void SpecExample030RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("update", new object[] { 1L, 2L, 3L, 4L, 5L });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_03.0_req.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample031RequestDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["foobar"] = new JsonRpcMethodScheme();

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_03.1_req.txt");
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
        public void SpecExample031RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("foobar");
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_03.1_req.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #04: RPC call of non-existent method

        [Fact]
        public void SpecExample040RequestDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["foobar"] = new JsonRpcMethodScheme();

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_04.0_req.txt");
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
        public void SpecExample040RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcRequest("foobar", "1");
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_04.0_req.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample040ResponseDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcBindings = new Dictionary<JsonRpcId, string>();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_04.0_res.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal("1", jsonRpcMessage.Id);
            Assert.False(jsonRpcMessage.Success);

            var jsonRpcError = jsonRpcMessage.Error;

            Assert.Equal(JsonRpcErrorType.InvalidMethod, jsonRpcError.Type);
            Assert.NotNull(jsonRpcError.Message);
            Assert.False(jsonRpcError.Message.Length == 0);
        }

        [Fact]
        public void SpecExample040ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(new JsonRpcError(-32601L, "Method not found"), "1");
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_04.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #05: RPC call with invalid JSON

        [Fact]
        public void SpecExample050RequestDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_05.0_req.txt");

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.Parsing, exception.Type);
        }

        [Fact]
        public void SpecExample050RequestSerialize()
        {
            // N/A
        }

        [Fact]
        public void SpecExample050ResponseDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcBindings = new Dictionary<JsonRpcId, string>();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_05.0_res.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(default, jsonRpcMessage.Id);
            Assert.False(jsonRpcMessage.Success);

            var jsonRpcError = jsonRpcMessage.Error;

            Assert.Equal(JsonRpcErrorType.Parsing, jsonRpcError.Type);
        }

        [Fact]
        public void SpecExample050ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(new JsonRpcError(-32700L, "Parse error"), default);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_05.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #06: RPC call with invalid request object

        [Fact]
        public void SpecExample060RequestDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["subtract"] = new JsonRpcMethodScheme();

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_06.0_req.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.False(jsonRpcItem.IsValid);
            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcItem.Exception.Type);
        }

        [Fact]
        public void SpecExample060RequestSerialize()
        {
            // N/A
        }

        [Fact]
        public void SpecExample060ResponseDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcBindings = new Dictionary<JsonRpcId, string>();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_06.0_res.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(default, jsonRpcMessage.Id);
            Assert.False(jsonRpcMessage.Success);

            var jsonRpcError = jsonRpcMessage.Error;

            Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcError.Type);
        }

        [Fact]
        public void SpecExample060ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"), default);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_06.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #07: RPC call batch, invalid JSON

        [Fact]
        public void SpecExample070RequestDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_07.0_req.txt");

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.Parsing, exception.Type);
        }

        [Fact]
        public void SpecExample070RequestSerialize()
        {
            // N/A
        }

        [Fact]
        public void SpecExample070ResponseDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcBindings = new Dictionary<JsonRpcId, string>();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_07.0_res.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(default, jsonRpcMessage.Id);
            Assert.False(jsonRpcMessage.Success);

            var jsonRpcError = jsonRpcMessage.Error;

            Assert.Equal(JsonRpcErrorType.Parsing, jsonRpcError.Type);
        }

        [Fact]
        public void SpecExample070ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(new JsonRpcError(-32700L, "Parse error"), default);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_07.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #08: RPC call with an empty array

        [Fact]
        public void SpecExample080RequestDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_08.0_req.txt");

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
        }

        [Fact]
        public void SpecExample080RequestSerialize()
        {
            // N/A
        }

        [Fact]
        public void SpecExample080ResponseDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcBindings = new Dictionary<JsonRpcId, string>();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_08.0_res.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(default, jsonRpcMessage.Id);
            Assert.False(jsonRpcMessage.Success);

            var jsonRpcError = jsonRpcMessage.Error;

            Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcError.Type);
        }

        [Fact]
        public void SpecExample080ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"), default);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_08.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #09: RPC call with an invalid batch (but not empty)

        [Fact]
        public void SpecExample090RequestDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_09.0_req.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.True(jsonRpcData.IsBatch);
            Assert.Equal(1, jsonRpcData.BatchItems.Count);

            var jsonRpcItem0 = jsonRpcData.BatchItems[0];

            Assert.False(jsonRpcItem0.IsValid);
            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcItem0.Exception.Type);
        }

        [Fact]
        public void SpecExample090RequestSerialize()
        {
            // N/A
        }

        [Fact]
        public void SpecExample090ResponseDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcBindings = new Dictionary<JsonRpcId, string>();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_09.0_res.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, jsonRpcBindings);

            Assert.True(jsonRpcData.IsBatch);
            Assert.Equal(1, jsonRpcData.BatchItems.Count);

            var jsonRpcItem0 = jsonRpcData.BatchItems[0];

            Assert.True(jsonRpcItem0.IsValid);

            var jsonRpcMessage0 = jsonRpcItem0.Message;

            Assert.Equal(default, jsonRpcMessage0.Id);
            Assert.False(jsonRpcMessage0.Success);

            var jsonRpcMessage0Error = jsonRpcMessage0.Error;

            Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcMessage0Error.Type);
        }

        [Fact]
        public void SpecExample090ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcMessage = new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"), default);
            var jsonResult = jsonRpcSerializer.SerializeResponses(new[] { jsonRpcMessage });
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_09.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #10: RPC call with invalid batch

        [Fact]
        public void SpecExample100RequestDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_10.0_req.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.True(jsonRpcData.IsBatch);
            Assert.Equal(3, jsonRpcData.BatchItems.Count);

            foreach (var jsonRpcItem in jsonRpcData.BatchItems)
            {
                Assert.False(jsonRpcItem.IsValid);
            }
        }

        [Fact]
        public void SpecExample100RequestSerialize()
        {
            // N/A
        }

        [Fact]
        public void SpecExample100ResponsDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcBindings = new Dictionary<JsonRpcId, string>();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_10.0_res.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, jsonRpcBindings);

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
            }
        }

        [Fact]
        public void SpecExample100ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var jsonRpcMessages = new[]
            {
                new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"), default),
                new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"), default),
                new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"), default)
            };

            var jsonResult = jsonRpcSerializer.SerializeResponses(jsonRpcMessages);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_10.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #11: RPC call batch

        [Fact]
        public void SpecExample110RequestDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["sum"] = new JsonRpcMethodScheme(new[] { typeof(long), typeof(long), typeof(long) });
            jsonRpcScheme.Methods["notify_hello"] = new JsonRpcMethodScheme(new[] { typeof(long) });
            jsonRpcScheme.Methods["subtract"] = new JsonRpcMethodScheme(new[] { typeof(long), typeof(long) });
            jsonRpcScheme.Methods["get_data"] = new JsonRpcMethodScheme();

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_11.0_req.txt");
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
        public void SpecExample110RequestSerialize()
        {
            // N/A
        }

        [Fact]
        public void SpecExample110ResponseDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["sum"] = new JsonRpcMethodScheme(typeof(long));
            jsonRpcScheme.Methods["subtract"] = new JsonRpcMethodScheme(typeof(long));
            jsonRpcScheme.Methods["get_data"] = new JsonRpcMethodScheme(typeof(object[]));

            var jsonRpcBindings = new Dictionary<JsonRpcId, string>
            {
                ["1"] = "sum",
                ["2"] = "subtract",
                ["5"] = "foo.get",
                ["9"] = "get_data"
            };

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_11.0_res.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, jsonRpcBindings);

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

            var jsonRpcItem3 = jsonRpcData.BatchItems[3];

            Assert.True(jsonRpcItem3.IsValid);

            var jsonRpcMessage3 = jsonRpcItem3.Message;

            Assert.Equal("5", jsonRpcMessage3.Id);
            Assert.False(jsonRpcMessage3.Success);

            var jsonRpcError3 = jsonRpcMessage3.Error;

            Assert.Equal(JsonRpcErrorType.InvalidMethod, jsonRpcError3.Type);

            var jsonRpcItem4 = jsonRpcData.BatchItems[4];

            Assert.True(jsonRpcItem4.IsValid);

            var jsonRpcMessage4 = jsonRpcItem4.Message;

            Assert.Equal("9", jsonRpcMessage4.Id);
            Assert.IsType<object[]>(jsonRpcMessage4.Result);
            Assert.Equal(new object[] { "hello", 5L }, jsonRpcMessage4.Result);
        }

        [Fact]
        public void SpecExample110ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_11.0_res.txt");

            var jsonRpcMessages = new[]
            {
                new JsonRpcResponse(7L, "1"),
                new JsonRpcResponse(19L, "2"),
                new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"), default),
                new JsonRpcResponse(new JsonRpcError(-32601L, "Method not found"), "5"),
                new JsonRpcResponse(new object[] { "hello", 5L }, "9")
            };

            var jsonResult = jsonRpcSerializer.SerializeResponses(jsonRpcMessages);

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #12: RPC call batch (all notifications)

        [Fact]
        public void SpecExample120RequestDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["notify_sum"] = new JsonRpcMethodScheme(new[] { typeof(long), typeof(long), typeof(long) });
            jsonRpcScheme.Methods["notify_hello"] = new JsonRpcMethodScheme(new[] { typeof(long) });

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_12.0_req.txt");
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
        public void SpecExample120RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var jsonRpcMessages = new[]
            {
                new JsonRpcRequest("notify_sum", new object[] { 1L, 2L, 4L }),
                new JsonRpcRequest("notify_hello", new object[] { 7L })
            };

            var jsonResult = jsonRpcSerializer.SerializeRequests(jsonRpcMessages);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_12.0_req.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample120ResponseDeserialize()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcBindings = new Dictionary<JsonRpcId, string>();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.spec_12.0_res.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, jsonRpcBindings);

            Assert.True(jsonRpcData.IsEmpty);
        }

        [Fact]
        public void SpecExample120ResponseSerialize()
        {
            // N/A
        }

        #endregion
    }
}