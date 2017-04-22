using System.Collections.Generic;
using System.Data.JsonRpc.Tests.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    partial class JsonRpcSerializerTests
    {
        #region Example #01: RPC call with positional parameters

        [Fact]
        public void SpecExample010RequestDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["subtract"] = new JsonRpcMethodScheme(typeof(long[]));

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_01.0_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcRequest = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(1L, jsonRpcRequest.Id);
            Assert.Equal("subtract", jsonRpcRequest.Method);
            Assert.True(jsonRpcRequest.HasParams);
            Assert.IsType<long[]>(jsonRpcRequest.Params);
            Assert.Equal(new[] { 42L, 23L }, jsonRpcRequest.Params);
        }

        [Fact]
        public void SpecExample010RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcRequest = new JsonRpcRequest("subtract", 1L, new[] { 42L, 23L });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcRequest);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_01.0_req.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample010ResponseDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["subtract"] = new JsonRpcMethodScheme(typeof(long), typeof(object));

            var jsonRpcBindings = new Dictionary<JsonRpcId, string>
            {
                [1L] = "subtract"
            };

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_01.0_res.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(1L, jsonRpcResponse.Id);
            Assert.IsType<long>(jsonRpcResponse.Result);
            Assert.Equal(19L, jsonRpcResponse.Result);
        }

        [Fact]
        public void SpecExample010ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(19L, 1L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_01.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample011RequestDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["subtract"] = new JsonRpcMethodScheme(typeof(long[]));

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_01.1_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcRequest = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(2L, jsonRpcRequest.Id);
            Assert.Equal("subtract", jsonRpcRequest.Method);
            Assert.True(jsonRpcRequest.HasParams);
            Assert.IsType<long[]>(jsonRpcRequest.Params);
            Assert.Equal(new[] { 23L, 42L }, jsonRpcRequest.Params);
        }

        [Fact]
        public void SpecExample011RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcRequest = new JsonRpcRequest("subtract", 2L, new[] { 23L, 42L });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcRequest);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_01.1_req.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample011ResponseDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["subtract"] = new JsonRpcMethodScheme(typeof(long), typeof(object));

            var jsonRpcBindings = new Dictionary<JsonRpcId, string>
            {
                [2L] = "subtract"
            };

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_01.1_res.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(2L, jsonRpcResponse.Id);
            Assert.IsType<long>(jsonRpcResponse.Result);
            Assert.Equal(-19L, jsonRpcResponse.Result);
        }

        [Fact]
        public void SpecExample011ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(-19L, 2L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_01.1_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #02: RPC call with named parameters

        [Fact]
        public void SpecExample020RequestDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["subtract"] = new JsonRpcMethodScheme(typeof(SpecExample020Params));

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_02.0_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcRequest = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(3L, jsonRpcRequest.Id);
            Assert.Equal("subtract", jsonRpcRequest.Method);
            Assert.True(jsonRpcRequest.HasParams);
            Assert.IsType<SpecExample020Params>(jsonRpcRequest.Params);

            var @params = (SpecExample020Params)jsonRpcRequest.Params;

            Assert.Equal(23L, @params.Subtrahend);
            Assert.Equal(42L, @params.Minuend);
        }

        [Fact]
        public void SpecExample020RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcRequest = new JsonRpcRequest("subtract", 3L, new SpecExample020Params { Subtrahend = 23L, Minuend = 42L });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcRequest);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_02.0_req.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample020ResponseDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["subtract"] = new JsonRpcMethodScheme(typeof(long), typeof(object));

            var jsonRpcBindings = new Dictionary<JsonRpcId, string>
            {
                [3] = "subtract"
            };

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_02.0_res.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(3L, jsonRpcResponse.Id);
            Assert.IsType<long>(jsonRpcResponse.Result);
            Assert.Equal(19L, jsonRpcResponse.Result);
        }

        [Fact]
        public void SpecExample020ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(19L, 3L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_02.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample021RequestDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["subtract"] = new JsonRpcMethodScheme(typeof(SpecExample020Params));

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_02.1_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcRequest = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(4L, jsonRpcRequest.Id);
            Assert.Equal("subtract", jsonRpcRequest.Method);
            Assert.True(jsonRpcRequest.HasParams);
            Assert.IsType<SpecExample020Params>(jsonRpcRequest.Params);

            var @params = (SpecExample020Params)jsonRpcRequest.Params;

            Assert.Equal(23L, @params.Subtrahend);
            Assert.Equal(42L, @params.Minuend);
        }

        [Fact]
        public void SpecExample021RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcRequest = new JsonRpcRequest("subtract", 4L, new SpecExample020Params { Subtrahend = 23L, Minuend = 42L });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcRequest);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_02.1_req.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample021ResponseDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["subtract"] = new JsonRpcMethodScheme(typeof(long), typeof(object));

            var jsonRpcBindings = new Dictionary<JsonRpcId, string>
            {
                [4] = "subtract"
            };

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_02.1_res.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(4L, jsonRpcResponse.Id);
            Assert.IsType<long>(jsonRpcResponse.Result);
            Assert.Equal(19L, jsonRpcResponse.Result);
        }

        [Fact]
        public void SpecExample021ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(19L, 4L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_02.1_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #03: a notification

        [Fact]
        public void SpecExample030RequestDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["update"] = new JsonRpcMethodScheme(typeof(long[]));

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_03.0_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcRequest = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcId.None, jsonRpcRequest.Id);
            Assert.Equal("update", jsonRpcRequest.Method);
            Assert.True(jsonRpcRequest.HasParams);
            Assert.IsType<long[]>(jsonRpcRequest.Params);
            Assert.Equal(new[] { 1L, 2L, 3L, 4L, 5L }, jsonRpcRequest.Params);
        }

        [Fact]
        public void SpecExample030RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcRequest = new JsonRpcRequest("update", JsonRpcId.None, new[] { 1L, 2L, 3L, 4L, 5L });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcRequest);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_03.0_req.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample031RequestDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["foobar"] = JsonRpcMethodScheme.Empty;

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_03.1_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcRequest = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcId.None, jsonRpcRequest.Id);
            Assert.Equal("foobar", jsonRpcRequest.Method);
            Assert.False(jsonRpcRequest.HasParams);
        }

        [Fact]
        public void SpecExample031RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcRequest = new JsonRpcRequest("foobar", JsonRpcId.None);
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcRequest);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_03.1_req.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #04: RPC call of non-existent method

        [Fact]
        public void SpecExample040RequestDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["foobar"] = JsonRpcMethodScheme.Empty;

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_04.0_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcRequest = jsonRpcMessageInfo.GetMessage();

            Assert.Equal("1", jsonRpcRequest.Id);
            Assert.Equal("foobar", jsonRpcRequest.Method);
            Assert.False(jsonRpcRequest.HasParams);
        }

        [Fact]
        public void SpecExample040RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcRequest = new JsonRpcRequest("foobar", "1");
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcRequest);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_04.0_req.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample040ResponseDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcBindings = new Dictionary<JsonRpcId, string>();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_04.0_res.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal("1", jsonRpcResponse.Id);
            Assert.False(jsonRpcResponse.Success);

            var jsonRpcResponseError = jsonRpcResponse.Error;

            Assert.Equal(JsonRpcErrorType.InvalidMethod, jsonRpcResponseError.Type);
            Assert.NotNull(jsonRpcResponseError.Message);
            Assert.False(jsonRpcResponseError.Message.Length == 0);
        }

        [Fact]
        public void SpecExample040ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(new JsonRpcError(-32601L, "Method not found"), "1");
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_04.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #05: RPC call with invalid JSON

        [Fact]
        public void SpecExample050RequestDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_05.0_req.txt");

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestsData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.ParseError, exception.Type);
        }

        [Fact]
        public void SpecExample050RequestSerialize()
        {
            // N/A
        }

        [Fact]
        public void SpecExample050ResponseDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcBindings = new Dictionary<JsonRpcId, string>();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_05.0_res.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcId.None, jsonRpcResponse.Id);
            Assert.False(jsonRpcResponse.Success);

            var jsonRpcResponseError = jsonRpcResponse.Error;

            Assert.Equal(JsonRpcErrorType.ParseError, jsonRpcResponseError.Type);
        }

        [Fact]
        public void SpecExample050ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(new JsonRpcError(-32700L, "Parse error"), JsonRpcId.None);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_05.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #06: RPC call with invalid request object

        [Fact]
        public void SpecExample060RequestDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["subtract"] = JsonRpcMethodScheme.Empty;

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_06.0_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.False(jsonRpcMessageInfo.IsValid);

            var jsonRpcMessageInfoException = jsonRpcMessageInfo.GetException();

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcMessageInfoException.Type);
        }

        [Fact]
        public void SpecExample060RequestSerialize()
        {
            // N/A
        }

        [Fact]
        public void SpecExample060ResponseDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcBindings = new Dictionary<JsonRpcId, string>();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_06.0_res.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcId.None, jsonRpcResponse.Id);
            Assert.False(jsonRpcResponse.Success);

            var jsonRpcResponseError = jsonRpcResponse.Error;

            Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcResponseError.Type);
        }

        [Fact]
        public void SpecExample060ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"), JsonRpcId.None);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_06.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #07: RPC call batch, invalid JSON

        [Fact]
        public void SpecExample070RequestDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_07.0_req.txt");

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestsData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.ParseError, exception.Type);
        }

        [Fact]
        public void SpecExample070RequestSerialize()
        {
            // N/A
        }

        [Fact]
        public void SpecExample070ResponseDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcBindings = new Dictionary<JsonRpcId, string>();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_07.0_res.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcId.None, jsonRpcResponse.Id);
            Assert.False(jsonRpcResponse.Success);

            var jsonRpcResponseError = jsonRpcResponse.Error;

            Assert.Equal(JsonRpcErrorType.ParseError, jsonRpcResponseError.Type);
        }

        [Fact]
        public void SpecExample070ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(new JsonRpcError(-32700L, "Parse error"), JsonRpcId.None);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_07.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #08: RPC call with an empty array

        [Fact]
        public void SpecExample080RequestDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_08.0_req.txt");

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestsData(jsonSample));

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
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcBindings = new Dictionary<JsonRpcId, string>();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_08.0_res.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindings);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcId.None, jsonRpcResponse.Id);
            Assert.False(jsonRpcResponse.Success);

            var jsonRpcResponseError = jsonRpcResponse.Error;

            Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcResponseError.Type);
        }

        [Fact]
        public void SpecExample080ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"), JsonRpcId.None);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_08.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #09: RPC call with an invalid batch (but not empty)

        [Fact]
        public void SpecExample090RequestDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_09.0_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.True(jsonRpcDataInfo.IsBatch);
            Assert.Equal(1, jsonRpcDataInfo.GetBatchItems().Count);

            var jsonRpcMessageInfo0 = jsonRpcDataInfo.GetBatchItems()[0];

            Assert.False(jsonRpcMessageInfo0.IsValid);

            var jsonRpcMessageInfo0Exception = jsonRpcMessageInfo0.GetException();

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcMessageInfo0Exception.Type);
        }

        [Fact]
        public void SpecExample090RequestSerialize()
        {
            // N/A
        }

        [Fact]
        public void SpecExample090ResponseDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcBindings = new Dictionary<JsonRpcId, string>();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_09.0_res.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindings);

            Assert.True(jsonRpcDataInfo.IsBatch);
            Assert.Equal(1, jsonRpcDataInfo.GetBatchItems().Count);

            var jsonRpcMessageInfo0 = jsonRpcDataInfo.GetBatchItems()[0];

            Assert.True(jsonRpcMessageInfo0.IsValid);

            var jsonRpcResponse0 = jsonRpcMessageInfo0.GetMessage();

            Assert.Equal(JsonRpcId.None, jsonRpcResponse0.Id);
            Assert.False(jsonRpcResponse0.Success);

            var jsonRpcResponse0Error = jsonRpcResponse0.Error;

            Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcResponse0Error.Type);
        }

        [Fact]
        public void SpecExample090ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"), JsonRpcId.None);
            var jsonResult = jsonRpcSerializer.SerializeResponses(new[] { jsonRpcResponse });
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_09.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #10: RPC call with invalid batch

        [Fact]
        public void SpecExample100RequestDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_10.0_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.True(jsonRpcDataInfo.IsBatch);
            Assert.Equal(3, jsonRpcDataInfo.GetBatchItems().Count);

            foreach (var jsonRpcMessageInfo in jsonRpcDataInfo.GetBatchItems())
                Assert.False(jsonRpcMessageInfo.IsValid);
        }

        [Fact]
        public void SpecExample100RequestSerialize()
        {
            // N/A
        }

        [Fact]
        public void SpecExample100ResponsDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcBindings = new Dictionary<JsonRpcId, string>();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_10.0_res.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindings);

            Assert.True(jsonRpcDataInfo.IsBatch);
            Assert.Equal(3, jsonRpcDataInfo.GetBatchItems().Count);

            foreach (var jsonRpcMessageInfo in jsonRpcDataInfo.GetBatchItems())
            {
                Assert.True(jsonRpcMessageInfo.IsValid);

                var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

                Assert.Equal(JsonRpcId.None, jsonRpcResponse.Id);
                Assert.False(jsonRpcResponse.Success);

                var jsonRpcResponseError = jsonRpcResponse.Error;

                Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcResponseError.Type);
            }
        }

        [Fact]
        public void SpecExample100ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponseArray = new JsonRpcResponse[3];

            for (var i = 0; i < jsonRpcResponseArray.Length; i++)
                jsonRpcResponseArray[i] = new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"), JsonRpcId.None);

            var jsonResult = jsonRpcSerializer.SerializeResponses(jsonRpcResponseArray);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_10.0_res.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #11: RPC call batch

        [Fact]
        public void SpecExample110RequestDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["sum"] = new JsonRpcMethodScheme(typeof(long[]));
            jsonRpcSerializerScheme.Methods["notify_hello"] = new JsonRpcMethodScheme(typeof(long[]));
            jsonRpcSerializerScheme.Methods["subtract"] = new JsonRpcMethodScheme(typeof(long[]));
            jsonRpcSerializerScheme.Methods["get_data"] = JsonRpcMethodScheme.Empty;

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_11.0_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.True(jsonRpcDataInfo.IsBatch);
            Assert.Equal(6, jsonRpcDataInfo.GetBatchItems().Count);

            var jsonRpcRequestInfo0 = jsonRpcDataInfo.GetBatchItems()[0];

            Assert.True(jsonRpcRequestInfo0.IsValid);

            var jsonRpcRequest0 = jsonRpcRequestInfo0.GetMessage();

            Assert.Equal("1", jsonRpcRequest0.Id);
            Assert.Equal("sum", jsonRpcRequest0.Method);
            Assert.True(jsonRpcRequest0.HasParams);
            Assert.IsType<long[]>(jsonRpcRequest0.Params);
            Assert.Equal(new[] { 1L, 2L, 4L }, jsonRpcRequest0.Params);

            var jsonRpcRequestInfo1 = jsonRpcDataInfo.GetBatchItems()[1];

            Assert.True(jsonRpcRequestInfo1.IsValid);

            var jsonRpcRequest1 = jsonRpcRequestInfo1.GetMessage();

            Assert.Equal(JsonRpcId.None, jsonRpcRequest1.Id);
            Assert.Equal("notify_hello", jsonRpcRequest1.Method);
            Assert.True(jsonRpcRequest1.HasParams);
            Assert.IsType<long[]>(jsonRpcRequest1.Params);
            Assert.Equal(new[] { 7L }, jsonRpcRequest1.Params);

            var jsonRpcRequestInfo2 = jsonRpcDataInfo.GetBatchItems()[2];

            Assert.True(jsonRpcRequestInfo2.IsValid);

            var jsonRpcRequest2 = jsonRpcRequestInfo2.GetMessage();

            Assert.Equal("2", jsonRpcRequest2.Id);
            Assert.Equal("subtract", jsonRpcRequest2.Method);
            Assert.True(jsonRpcRequest2.HasParams);
            Assert.IsType<long[]>(jsonRpcRequest2.Params);
            Assert.Equal(new[] { 42L, 23L }, jsonRpcRequest2.Params);

            var jsonRpcRequestInfo3 = jsonRpcDataInfo.GetBatchItems()[3];

            Assert.False(jsonRpcRequestInfo3.IsValid);

            var jsonRpcRequestInfo4 = jsonRpcDataInfo.GetBatchItems()[4];

            Assert.False(jsonRpcRequestInfo4.IsValid);

            var jsonRpcRequestInfo5 = jsonRpcDataInfo.GetBatchItems()[5];

            Assert.True(jsonRpcRequestInfo5.IsValid);

            var jsonRpcRequest5 = jsonRpcRequestInfo5.GetMessage();

            Assert.Equal("9", jsonRpcRequest5.Id);
            Assert.Equal("get_data", jsonRpcRequest5.Method);
            Assert.False(jsonRpcRequest5.HasParams);
        }

        [Fact]
        public void SpecExample110RequestSerialize()
        {
            // N/A
        }

        [Fact]
        public void SpecExample110ResponseDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["sum"] = new JsonRpcMethodScheme(typeof(long), typeof(object));
            jsonRpcSerializerScheme.Methods["subtract"] = new JsonRpcMethodScheme(typeof(long), typeof(object));
            jsonRpcSerializerScheme.Methods["get_data"] = new JsonRpcMethodScheme(typeof(object[]), typeof(object));

            var jsonRpcBindings = new Dictionary<JsonRpcId, string>
            {
                ["1"] = "sum",
                ["2"] = "subtract",
                ["5"] = "foo.get",
                ["9"] = "get_data"
            };

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_11.0_res.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindings);

            Assert.True(jsonRpcDataInfo.IsBatch);
            Assert.Equal(5, jsonRpcDataInfo.GetBatchItems().Count);

            var jsonRpcMessageInfo0 = jsonRpcDataInfo.GetBatchItems()[0];

            Assert.True(jsonRpcMessageInfo0.IsValid);

            var jsonRpcResponse0 = jsonRpcMessageInfo0.GetMessage();

            Assert.Equal("1", jsonRpcResponse0.Id);
            Assert.IsType<long>(jsonRpcResponse0.Result);
            Assert.Equal(7L, jsonRpcResponse0.Result);

            var jsonRpcMessageInfo1 = jsonRpcDataInfo.GetBatchItems()[1];

            Assert.True(jsonRpcMessageInfo1.IsValid);

            var jsonRpcResponse1 = jsonRpcMessageInfo1.GetMessage();

            Assert.Equal("2", jsonRpcResponse1.Id);
            Assert.IsType<long>(jsonRpcResponse1.Result);
            Assert.Equal(19L, jsonRpcResponse1.Result);

            var jsonRpcMessageInfo2 = jsonRpcDataInfo.GetBatchItems()[2];

            Assert.True(jsonRpcMessageInfo2.IsValid);

            var jsonRpcResponse2 = jsonRpcMessageInfo2.GetMessage();

            Assert.Equal(JsonRpcId.None, jsonRpcResponse2.Id);
            Assert.False(jsonRpcResponse2.Success);

            var jsonRpcResponse2Error = jsonRpcResponse2.Error;

            Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcResponse2Error.Type);

            var jsonRpcMessageInfo3 = jsonRpcDataInfo.GetBatchItems()[3];

            Assert.True(jsonRpcMessageInfo3.IsValid);

            var jsonRpcResponse3 = jsonRpcMessageInfo3.GetMessage();

            Assert.Equal("5", jsonRpcResponse3.Id);
            Assert.False(jsonRpcResponse3.Success);

            var jsonRpcResponse3Error = jsonRpcResponse3.Error;

            Assert.Equal(JsonRpcErrorType.InvalidMethod, jsonRpcResponse3Error.Type);

            var jsonRpcMessageInfo4 = jsonRpcDataInfo.GetBatchItems()[4];

            Assert.True(jsonRpcMessageInfo4.IsValid);

            var jsonRpcResponse4 = jsonRpcMessageInfo4.GetMessage();

            Assert.Equal("9", jsonRpcResponse4.Id);
            Assert.IsType<object[]>(jsonRpcResponse4.Result);
            Assert.Equal(new object[] { "hello", 5L }, jsonRpcResponse4.Result);
        }

        [Fact]
        public void SpecExample110ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_11.0_res.txt");
            var jsonRpcResponseArray = new JsonRpcResponse[5];

            jsonRpcResponseArray[0] = new JsonRpcResponse(7L, "1");
            jsonRpcResponseArray[1] = new JsonRpcResponse(19L, "2");
            jsonRpcResponseArray[2] = new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"), JsonRpcId.None);
            jsonRpcResponseArray[3] = new JsonRpcResponse(new JsonRpcError(-32601L, "Method not found"), "5");
            jsonRpcResponseArray[4] = new JsonRpcResponse(new object[] { "hello", 5L }, "9");

            var jsonResult = jsonRpcSerializer.SerializeResponses(jsonRpcResponseArray);

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        #endregion

        #region Example #12: RPC call batch (all notifications)

        [Fact]
        public void SpecExample120RequestDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["notify_sum"] = new JsonRpcMethodScheme(typeof(long[]));
            jsonRpcSerializerScheme.Methods["notify_hello"] = new JsonRpcMethodScheme(typeof(long[]));

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_12.0_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.True(jsonRpcDataInfo.IsBatch);
            Assert.Equal(2, jsonRpcDataInfo.GetBatchItems().Count);

            var jsonRpcMessageInfo0 = jsonRpcDataInfo.GetBatchItems()[0];

            Assert.True(jsonRpcMessageInfo0.IsValid);

            var jsonRpcRequest0 = jsonRpcMessageInfo0.GetMessage();

            Assert.Equal(JsonRpcId.None, jsonRpcRequest0.Id);
            Assert.Equal("notify_sum", jsonRpcRequest0.Method);
            Assert.True(jsonRpcRequest0.HasParams);
            Assert.IsType<long[]>(jsonRpcRequest0.Params);
            Assert.Equal(new[] { 1L, 2L, 4L }, jsonRpcRequest0.Params);

            var jsonRpcMessageInfo1 = jsonRpcDataInfo.GetBatchItems()[1];

            Assert.True(jsonRpcMessageInfo1.IsValid);

            var jsonRpcRequest1 = jsonRpcMessageInfo1.GetMessage();

            Assert.Equal(JsonRpcId.None, jsonRpcRequest1.Id);
            Assert.Equal("notify_hello", jsonRpcRequest1.Method);
            Assert.True(jsonRpcRequest1.HasParams);
            Assert.IsType<long[]>(jsonRpcRequest1.Params);
            Assert.Equal(new[] { 7L }, jsonRpcRequest1.Params);
        }

        [Fact]
        public void SpecExample120RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpvRequestArray = new JsonRpcRequest[2];

            jsonRpvRequestArray[0] = new JsonRpcRequest("notify_sum", JsonRpcId.None, new[] { 1L, 2L, 4L });
            jsonRpvRequestArray[1] = new JsonRpcRequest("notify_hello", JsonRpcId.None, new[] { 7L });

            var jsonResult = jsonRpcSerializer.SerializeRequests(jsonRpvRequestArray);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_12.0_req.txt");

            Assert.True(JToken.DeepEquals(JToken.Parse(jsonSample), JToken.Parse(jsonResult)));
        }

        [Fact]
        public void SpecExample120ResponseDeserialize()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcBindings = new Dictionary<JsonRpcId, string>();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString($"Assets.spec_12.0_res.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindings);

            Assert.True(jsonRpcDataInfo.IsEmpty);
        }

        [Fact]
        public void SpecExample120ResponseSerialize()
        {
            // N/A
        }

        #endregion

        #region Test Types

        [JsonObject(MemberSerialization.OptIn)]
        private sealed class SpecExample020Params
        {
            [JsonProperty("minuend")]
            public long Minuend { get; set; }

            [JsonProperty("subtrahend")]
            public long Subtrahend { get; set; }
        }

        #endregion
    }
}