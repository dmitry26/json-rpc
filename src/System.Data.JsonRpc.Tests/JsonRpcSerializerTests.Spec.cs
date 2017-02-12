using System.Data.JsonRpc.Tests.Support;
using Newtonsoft.Json;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    partial class JsonRpcSerializerTests
    {
        #region Example #01: rpc call with positional parameters

        [Fact]
        public void SpecExample010RequestDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.SupportedMethods.Add("subtract");
            jsonRpcSchema.ParameterTypeBindings["subtract"] = typeof(long[]);

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_01.0_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcRequest = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcIdType.Number, jsonRpcRequest.IdType);
            Assert.Equal(1L, jsonRpcRequest.GetIdAsNumber());
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
            var jsonSample = JsonTools.GetJsonSample("spec_01.0_req");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        [Fact]
        public void SpecExample010ResponseDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.ResultTypeBindings["subtract"] = typeof(long);

            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            jsonRpcBindingsProvider.SetBinding(1L, "subtract");

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_01.0_res");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindingsProvider);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcIdType.Number, jsonRpcResponse.IdType);
            Assert.Equal(1L, jsonRpcResponse.GetIdAsNumber());
            Assert.IsType<long>(jsonRpcResponse.Result);
            Assert.Equal(19L, jsonRpcResponse.Result);
        }

        [Fact]
        public void SpecExample010ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(1L, 19L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = JsonTools.GetJsonSample("spec_01.0_res");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        [Fact]
        public void SpecExample011RequestDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.SupportedMethods.Add("subtract");
            jsonRpcSchema.ParameterTypeBindings["subtract"] = typeof(long[]);

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_01.1_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcRequest = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcIdType.Number, jsonRpcRequest.IdType);
            Assert.Equal(2L, jsonRpcRequest.GetIdAsNumber());
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
            var jsonSample = JsonTools.GetJsonSample("spec_01.1_req");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        [Fact]
        public void SpecExample011ResponseDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.ResultTypeBindings["subtract"] = typeof(long);

            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            jsonRpcBindingsProvider.SetBinding(2L, "subtract");

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_01.1_res");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindingsProvider);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcIdType.Number, jsonRpcResponse.IdType);
            Assert.Equal(2L, jsonRpcResponse.GetIdAsNumber());
            Assert.IsType<long>(jsonRpcResponse.Result);
            Assert.Equal(-19L, jsonRpcResponse.Result);
        }

        [Fact]
        public void SpecExample011ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(2L, -19L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = JsonTools.GetJsonSample("spec_01.1_res");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        #endregion

        #region Example #02: rpc call with named parameters

        [Fact]
        public void SpecExample020RequestDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.SupportedMethods.Add("subtract");
            jsonRpcSchema.ParameterTypeBindings["subtract"] = typeof(SpecExample020Params);

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_02.0_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcRequest = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcIdType.Number, jsonRpcRequest.IdType);
            Assert.Equal(3L, jsonRpcRequest.GetIdAsNumber());
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
            var jsonSample = JsonTools.GetJsonSample("spec_02.0_req");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        [Fact]
        public void SpecExample020ResponseDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.ResultTypeBindings["subtract"] = typeof(long);

            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            jsonRpcBindingsProvider.SetBinding(3, "subtract");

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_02.0_res");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindingsProvider);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcIdType.Number, jsonRpcResponse.IdType);
            Assert.Equal(3L, jsonRpcResponse.GetIdAsNumber());
            Assert.IsType<long>(jsonRpcResponse.Result);
            Assert.Equal(19L, jsonRpcResponse.Result);
        }

        [Fact]
        public void SpecExample020ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(3L, 19L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = JsonTools.GetJsonSample("spec_02.0_res");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        [Fact]
        public void SpecExample021RequestDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.SupportedMethods.Add("subtract");
            jsonRpcSchema.ParameterTypeBindings["subtract"] = typeof(SpecExample020Params);

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_02.1_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcRequest = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcIdType.Number, jsonRpcRequest.IdType);
            Assert.Equal(4L, jsonRpcRequest.GetIdAsNumber());
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
            var jsonSample = JsonTools.GetJsonSample("spec_02.1_req");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        [Fact]
        public void SpecExample021ResponseDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.ResultTypeBindings["subtract"] = typeof(long);

            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            jsonRpcBindingsProvider.SetBinding(4, "subtract");

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_02.1_res");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindingsProvider);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcIdType.Number, jsonRpcResponse.IdType);
            Assert.Equal(4L, jsonRpcResponse.GetIdAsNumber());
            Assert.IsType<long>(jsonRpcResponse.Result);
            Assert.Equal(19L, jsonRpcResponse.Result);
        }

        [Fact]
        public void SpecExample021ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(4L, 19L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = JsonTools.GetJsonSample("spec_02.1_res");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        #endregion

        #region Example #03: a notification

        [Fact]
        public void SpecExample030RequestDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.SupportedMethods.Add("update");
            jsonRpcSchema.ParameterTypeBindings["update"] = typeof(long[]);

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_03.0_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcRequest = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcIdType.Null, jsonRpcRequest.IdType);
            Assert.Equal("update", jsonRpcRequest.Method);
            Assert.True(jsonRpcRequest.HasParams);
            Assert.IsType<long[]>(jsonRpcRequest.Params);
            Assert.Equal(new[] { 1L, 2L, 3L, 4L, 5L }, jsonRpcRequest.Params);
        }

        [Fact]
        public void SpecExample030RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcRequest = new JsonRpcRequest("update", new[] { 1L, 2L, 3L, 4L, 5L });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcRequest);
            var jsonSample = JsonTools.GetJsonSample("spec_03.0_req");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        [Fact]
        public void SpecExample031RequestDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.SupportedMethods.Add("foobar");

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_03.1_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcRequest = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcIdType.Null, jsonRpcRequest.IdType);
            Assert.Equal("foobar", jsonRpcRequest.Method);
            Assert.False(jsonRpcRequest.HasParams);
        }

        [Fact]
        public void SpecExample031RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcRequest = new JsonRpcRequest("foobar");
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcRequest);
            var jsonSample = JsonTools.GetJsonSample("spec_03.1_req");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        #endregion

        #region Example #04: rpc call of non-existent method

        [Fact]
        public void SpecExample040RequestDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.SupportedMethods.Add("foobar");

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_04.0_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcRequest = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcIdType.String, jsonRpcRequest.IdType);
            Assert.Equal("1", jsonRpcRequest.GetIdAsString());
            Assert.Equal("foobar", jsonRpcRequest.Method);
            Assert.False(jsonRpcRequest.HasParams);
        }

        [Fact]
        public void SpecExample040RequestSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcRequest = new JsonRpcRequest("foobar", "1");
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcRequest);
            var jsonSample = JsonTools.GetJsonSample("spec_04.0_req");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        [Fact]
        public void SpecExample040ResponseDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_04.0_res");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindingsProvider);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcIdType.String, jsonRpcResponse.IdType);
            Assert.Equal("1", jsonRpcResponse.GetIdAsString());
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
            var jsonRpcResponse = new JsonRpcResponse("1", new JsonRpcError(-32601L, "Method not found"));
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = JsonTools.GetJsonSample("spec_04.0_res");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        #endregion

        #region Example #05: rpc call with invalid JSON

        [Fact]
        public void SpecExample050RequestDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_05.0_req");

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
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_05.0_res");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindingsProvider);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcIdType.Null, jsonRpcResponse.IdType);
            Assert.False(jsonRpcResponse.Success);

            var jsonRpcResponseError = jsonRpcResponse.Error;

            Assert.Equal(JsonRpcErrorType.ParseError, jsonRpcResponseError.Type);
        }

        [Fact]
        public void SpecExample050ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(new JsonRpcError(-32700L, "Parse error"));
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = JsonTools.GetJsonSample("spec_05.0_res");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        #endregion

        #region Example #06: rpc call with invalid request object

        [Fact]
        public void SpecExample060RequestDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.SupportedMethods.Add("foobar");

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_06.0_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.False(jsonRpcMessageInfo.Success);

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
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_06.0_res");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindingsProvider);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcIdType.Null, jsonRpcResponse.IdType);
            Assert.False(jsonRpcResponse.Success);

            var jsonRpcResponseError = jsonRpcResponse.Error;

            Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcResponseError.Type);
        }

        [Fact]
        public void SpecExample060ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"));
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = JsonTools.GetJsonSample("spec_06.0_res");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        #endregion

        #region Example #07: rpc call batch, invalid JSON

        [Fact]
        public void SpecExample070RequestDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_07.0_req");

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
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_07.0_res");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindingsProvider);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcIdType.Null, jsonRpcResponse.IdType);
            Assert.False(jsonRpcResponse.Success);

            var jsonRpcResponseError = jsonRpcResponse.Error;

            Assert.Equal(JsonRpcErrorType.ParseError, jsonRpcResponseError.Type);
        }

        [Fact]
        public void SpecExample070ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(new JsonRpcError(-32700L, "Parse error"));
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = JsonTools.GetJsonSample("spec_07.0_res");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        #endregion

        #region Example #08: rpc call with an empty array

        [Fact]
        public void SpecExample080RequestDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_08.0_req");

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
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_08.0_res");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindingsProvider);

            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(JsonRpcIdType.Null, jsonRpcResponse.IdType);
            Assert.False(jsonRpcResponse.Success);

            var jsonRpcResponseError = jsonRpcResponse.Error;

            Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcResponseError.Type);
        }

        [Fact]
        public void SpecExample080ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"));
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = JsonTools.GetJsonSample("spec_08.0_res");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        #endregion

        #region Example #09: rpc call with an invalid batch (but not empty)

        [Fact]
        public void SpecExample090RequestDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_09.0_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.True(jsonRpcDataInfo.IsBatch);
            Assert.Equal(1, jsonRpcDataInfo.GetBatchItems().Count);

            var jsonRpcMessageInfo0 = jsonRpcDataInfo.GetBatchItems()[0];

            Assert.False(jsonRpcMessageInfo0.Success);

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
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_09.0_res");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindingsProvider);

            Assert.True(jsonRpcDataInfo.IsBatch);
            Assert.Equal(1, jsonRpcDataInfo.GetBatchItems().Count);

            var jsonRpcMessageInfo0 = jsonRpcDataInfo.GetBatchItems()[0];

            Assert.True(jsonRpcMessageInfo0.Success);

            var jsonRpcResponse0 = jsonRpcMessageInfo0.GetMessage();

            Assert.Equal(JsonRpcIdType.Null, jsonRpcResponse0.IdType);
            Assert.False(jsonRpcResponse0.Success);

            var jsonRpcResponse0Error = jsonRpcResponse0.Error;

            Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcResponse0Error.Type);
        }

        [Fact]
        public void SpecExample090ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponse = new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"));
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcResponse);
            var jsonSample = JsonTools.GetJsonSample("spec_08.0_res");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        #endregion

        #region Example #10: rpc call with invalid batch

        [Fact]
        public void SpecExample100RequestDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_10.0_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.True(jsonRpcDataInfo.IsBatch);
            Assert.Equal(3, jsonRpcDataInfo.GetBatchItems().Count);

            foreach (var jsonRpcMessageInfo in jsonRpcDataInfo.GetBatchItems())
                Assert.False(jsonRpcMessageInfo.Success);
        }

        [Fact]
        public void SpecExample100RequestSerialize()
        {
            // N/A
        }

        [Fact]
        public void SpecExample100ResponsDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_10.0_res");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindingsProvider);

            Assert.True(jsonRpcDataInfo.IsBatch);
            Assert.Equal(3, jsonRpcDataInfo.GetBatchItems().Count);

            foreach (var jsonRpcMessageInfo in jsonRpcDataInfo.GetBatchItems())
            {
                Assert.True(jsonRpcMessageInfo.Success);

                var jsonRpcResponse = jsonRpcMessageInfo.GetMessage();

                Assert.Equal(JsonRpcIdType.Null, jsonRpcResponse.IdType);
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
                jsonRpcResponseArray[i] = new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"));

            var jsonResult = jsonRpcSerializer.SerializeResponses(jsonRpcResponseArray);
            var jsonSample = JsonTools.GetJsonSample("spec_10.0_res");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        #endregion

        #region Example #11: rpc call batch

        [Fact]
        public void SpecExample110RequestDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.SupportedMethods.Add("sum");
            jsonRpcSchema.SupportedMethods.Add("notify_hello");
            jsonRpcSchema.SupportedMethods.Add("subtract");
            jsonRpcSchema.SupportedMethods.Add("get_data");
            jsonRpcSchema.ParameterTypeBindings["sum"] = typeof(long[]);
            jsonRpcSchema.ParameterTypeBindings["notify_hello"] = typeof(long[]);
            jsonRpcSchema.ParameterTypeBindings["subtract"] = typeof(long[]);

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_11.0_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.True(jsonRpcDataInfo.IsBatch);
            Assert.Equal(6, jsonRpcDataInfo.GetBatchItems().Count);

            var jsonRpcRequestInfo0 = jsonRpcDataInfo.GetBatchItems()[0];

            Assert.True(jsonRpcRequestInfo0.Success);

            var jsonRpcRequest0 = jsonRpcRequestInfo0.GetMessage();

            Assert.Equal(JsonRpcIdType.String, jsonRpcRequest0.IdType);
            Assert.Equal("1", jsonRpcRequest0.GetIdAsString());
            Assert.Equal("sum", jsonRpcRequest0.Method);
            Assert.True(jsonRpcRequest0.HasParams);
            Assert.IsType<long[]>(jsonRpcRequest0.Params);
            Assert.Equal(new[] { 1L, 2L, 4L }, jsonRpcRequest0.Params);

            var jsonRpcRequestInfo1 = jsonRpcDataInfo.GetBatchItems()[1];

            Assert.True(jsonRpcRequestInfo1.Success);

            var jsonRpcRequest1 = jsonRpcRequestInfo1.GetMessage();

            Assert.Equal(JsonRpcIdType.Null, jsonRpcRequest1.IdType);
            Assert.Equal("notify_hello", jsonRpcRequest1.Method);
            Assert.True(jsonRpcRequest1.HasParams);
            Assert.IsType<long[]>(jsonRpcRequest1.Params);
            Assert.Equal(new[] { 7L }, jsonRpcRequest1.Params);

            var jsonRpcRequestInfo2 = jsonRpcDataInfo.GetBatchItems()[2];

            Assert.True(jsonRpcRequestInfo2.Success);

            var jsonRpcRequest2 = jsonRpcRequestInfo2.GetMessage();

            Assert.Equal(JsonRpcIdType.String, jsonRpcRequest2.IdType);
            Assert.Equal("2", jsonRpcRequest2.GetIdAsString());
            Assert.Equal("subtract", jsonRpcRequest2.Method);
            Assert.True(jsonRpcRequest2.HasParams);
            Assert.IsType<long[]>(jsonRpcRequest2.Params);
            Assert.Equal(new[] { 42L, 23L }, jsonRpcRequest2.Params);

            var jsonRpcRequestInfo3 = jsonRpcDataInfo.GetBatchItems()[3];

            Assert.False(jsonRpcRequestInfo3.Success);

            var jsonRpcRequestInfo4 = jsonRpcDataInfo.GetBatchItems()[4];

            Assert.False(jsonRpcRequestInfo4.Success);

            var jsonRpcRequestInfo5 = jsonRpcDataInfo.GetBatchItems()[5];

            Assert.True(jsonRpcRequestInfo5.Success);

            var jsonRpcRequest5 = jsonRpcRequestInfo5.GetMessage();

            Assert.Equal(JsonRpcIdType.String, jsonRpcRequest5.IdType);
            Assert.Equal("9", jsonRpcRequest5.GetIdAsString());
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
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.ResultTypeBindings["sum"] = typeof(long);
            jsonRpcSchema.ResultTypeBindings["subtract"] = typeof(long);
            jsonRpcSchema.ResultTypeBindings["get_data"] = typeof(object[]);

            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();

            jsonRpcBindingsProvider.SetBinding("1", "sum");
            jsonRpcBindingsProvider.SetBinding("2", "subtract");
            jsonRpcBindingsProvider.SetBinding("5", "foo.get");
            jsonRpcBindingsProvider.SetBinding("9", "get_data");

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_11.0_res");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindingsProvider);

            Assert.True(jsonRpcDataInfo.IsBatch);
            Assert.Equal(5, jsonRpcDataInfo.GetBatchItems().Count);

            var jsonRpcMessageInfo0 = jsonRpcDataInfo.GetBatchItems()[0];

            Assert.True(jsonRpcMessageInfo0.Success);

            var jsonRpcResponse0 = jsonRpcMessageInfo0.GetMessage();

            Assert.Equal(JsonRpcIdType.String, jsonRpcResponse0.IdType);
            Assert.Equal("1", jsonRpcResponse0.GetIdAsString());
            Assert.IsType<long>(jsonRpcResponse0.Result);
            Assert.Equal(7L, jsonRpcResponse0.Result);

            var jsonRpcMessageInfo1 = jsonRpcDataInfo.GetBatchItems()[1];

            Assert.True(jsonRpcMessageInfo1.Success);

            var jsonRpcResponse1 = jsonRpcMessageInfo1.GetMessage();

            Assert.Equal(JsonRpcIdType.String, jsonRpcResponse1.IdType);
            Assert.Equal("2", jsonRpcResponse1.GetIdAsString());
            Assert.IsType<long>(jsonRpcResponse1.Result);
            Assert.Equal(19L, jsonRpcResponse1.Result);

            var jsonRpcMessageInfo2 = jsonRpcDataInfo.GetBatchItems()[2];

            Assert.True(jsonRpcMessageInfo2.Success);

            var jsonRpcResponse2 = jsonRpcMessageInfo2.GetMessage();

            Assert.Equal(JsonRpcIdType.Null, jsonRpcResponse2.IdType);
            Assert.False(jsonRpcResponse2.Success);

            var jsonRpcResponse2Error = jsonRpcResponse2.Error;

            Assert.Equal(JsonRpcErrorType.InvalidRequest, jsonRpcResponse2Error.Type);

            var jsonRpcMessageInfo3 = jsonRpcDataInfo.GetBatchItems()[3];

            Assert.True(jsonRpcMessageInfo3.Success);

            var jsonRpcResponse3 = jsonRpcMessageInfo3.GetMessage();

            Assert.Equal(JsonRpcIdType.String, jsonRpcResponse3.IdType);
            Assert.Equal("5", jsonRpcResponse3.GetIdAsString());
            Assert.False(jsonRpcResponse3.Success);

            var jsonRpcResponse3Error = jsonRpcResponse3.Error;

            Assert.Equal(JsonRpcErrorType.InvalidMethod, jsonRpcResponse3Error.Type);

            var jsonRpcMessageInfo4 = jsonRpcDataInfo.GetBatchItems()[4];

            Assert.True(jsonRpcMessageInfo4.Success);

            var jsonRpcResponse4 = jsonRpcMessageInfo4.GetMessage();

            Assert.Equal(JsonRpcIdType.String, jsonRpcResponse4.IdType);
            Assert.Equal("9", jsonRpcResponse4.GetIdAsString());
            Assert.IsType<object[]>(jsonRpcResponse4.Result);
            Assert.Equal(new object[] { "hello", 5L }, jsonRpcResponse4.Result);
        }

        [Fact]
        public void SpecExample110ResponseSerialize()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonSample = JsonTools.GetJsonSample("spec_11.0_res");
            var jsonRpcResponseArray = new JsonRpcResponse[5];

            jsonRpcResponseArray[0] = new JsonRpcResponse("1", 7L);
            jsonRpcResponseArray[1] = new JsonRpcResponse("2", 19L);
            jsonRpcResponseArray[2] = new JsonRpcResponse(new JsonRpcError(-32600L, "Invalid Request"));
            jsonRpcResponseArray[3] = new JsonRpcResponse("5", new JsonRpcError(-32601L, "Method not found"));
            jsonRpcResponseArray[4] = new JsonRpcResponse("9", new object[] { "hello", 5L });

            var jsonResult = jsonRpcSerializer.SerializeResponses(jsonRpcResponseArray);

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        #endregion

        #region Example #12: rpc call batch (all notifications)

        [Fact]
        public void SpecExample120RequestDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.SupportedMethods.Add("notify_sum");
            jsonRpcSchema.SupportedMethods.Add("notify_hello");
            jsonRpcSchema.ParameterTypeBindings["notify_sum"] = typeof(long[]);
            jsonRpcSchema.ParameterTypeBindings["notify_hello"] = typeof(long[]);

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_12.0_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.True(jsonRpcDataInfo.IsBatch);
            Assert.Equal(2, jsonRpcDataInfo.GetBatchItems().Count);

            var jsonRpcMessageInfo0 = jsonRpcDataInfo.GetBatchItems()[0];

            Assert.True(jsonRpcMessageInfo0.Success);

            var jsonRpcRequest0 = jsonRpcMessageInfo0.GetMessage();

            Assert.Equal(JsonRpcIdType.Null, jsonRpcRequest0.IdType);
            Assert.Equal("notify_sum", jsonRpcRequest0.Method);
            Assert.True(jsonRpcRequest0.HasParams);
            Assert.IsType<long[]>(jsonRpcRequest0.Params);
            Assert.Equal(new[] { 1L, 2L, 4L }, jsonRpcRequest0.Params);

            var jsonRpcMessageInfo1 = jsonRpcDataInfo.GetBatchItems()[1];

            Assert.True(jsonRpcMessageInfo1.Success);

            var jsonRpcRequest1 = jsonRpcMessageInfo1.GetMessage();

            Assert.Equal(JsonRpcIdType.Null, jsonRpcRequest1.IdType);
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

            jsonRpvRequestArray[0] = new JsonRpcRequest("notify_sum", new[] { 1L, 2L, 4L });
            jsonRpvRequestArray[1] = new JsonRpcRequest("notify_hello", new[] { 7L });

            var jsonResult = jsonRpcSerializer.SerializeRequests(jsonRpvRequestArray);
            var jsonSample = JsonTools.GetJsonSample("spec_12.0_req");

            Assert.True(JsonTools.CompareJsonStrings(jsonSample, jsonResult));
        }

        [Fact]
        public void SpecExample120ResponseDeserialize()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcBindingsProvider = new JsonRpcBindingsProvider();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("spec_12.0_res");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindingsProvider);

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