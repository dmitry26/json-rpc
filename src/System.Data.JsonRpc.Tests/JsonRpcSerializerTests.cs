using System.Buffers;
using System.Collections.Generic;
using System.Data.JsonRpc.Tests.Support;
using Newtonsoft.Json;
using Xunit;
using Fake = FakeItEasy.A;

namespace System.Data.JsonRpc.Tests
{
    public sealed partial class JsonRpcSerializerTests
    {
        [Fact]
        public void ConstructorWithSchemaWhenSchemaIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcSerializer(default(JsonRpcSchema)));
        }

        [Fact]
        public void ConstructorWithSchemaAndSettingsWhenSchemaIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcSerializer(default(JsonRpcSchema), new JsonRpcSettings()));
        }

        [Fact]
        public void ConstructorWithSchemaAndSettingsWhenSettingsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcSerializer(new JsonRpcSchema(), default(JsonRpcSettings)));
        }

        [Fact]
        public void SerializeRequestWhenRequestIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeRequest(default(JsonRpcRequest)));
        }

        [Fact]
        public void SerializeRequestWhenParamsIsInvalid()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcRequest = new JsonRpcRequest("test_method", 100L, 200L);

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeRequest(jsonRpcRequest));

            Assert.Equal(JsonRpcExceptionType.GenericError, exception.Type);
        }

        [Fact]
        public void SerializeRequestsWhenCollectionIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeRequests(default(IReadOnlyCollection<JsonRpcRequest>)));
        }

        [Fact]
        public void SerializeRequestsWhenCollectionIsEmptyCollection()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeRequests(new JsonRpcRequest[] { }));

            Assert.Equal(JsonRpcExceptionType.GenericError, exception.Type);
        }

        [Fact]
        public void SerializeRequestsWhenCollectionContainsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeRequests(new[] { default(JsonRpcRequest) }));

            Assert.Equal(JsonRpcExceptionType.GenericError, exception.Type);
        }

        [Fact]
        public void SerializeRequestsWhenCollectionContainsDuplicateIdAndIdIsNumber()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcRequestArray = new JsonRpcRequest[2];

            jsonRpcRequestArray[0] = new JsonRpcRequest("test_method_1", 100L);
            jsonRpcRequestArray[1] = new JsonRpcRequest("test_method_2", 100L);

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeRequests(jsonRpcRequestArray));

            Assert.Equal(JsonRpcExceptionType.GenericError, exception.Type);
        }

        [Fact]
        public void SerializeRequestsWhenCollectionContainsDuplicateIdAndIdIsString()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcRequestArray = new JsonRpcRequest[2];

            jsonRpcRequestArray[0] = new JsonRpcRequest("test_method_1", "100");
            jsonRpcRequestArray[1] = new JsonRpcRequest("test_method_2", "100");

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeRequests(jsonRpcRequestArray));

            Assert.Equal(JsonRpcExceptionType.GenericError, exception.Type);
        }

        [Fact]
        public void SerializeResponseWhenResponseIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeResponse(default(JsonRpcResponse)));
        }

        [Fact]
        public void SerializeResponsesWhenCollectionIsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.SerializeResponses(default(IReadOnlyCollection<JsonRpcResponse>)));
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
                jsonRpcSerializer.SerializeResponses(new[] { default(JsonRpcResponse) }));

            Assert.Equal(JsonRpcExceptionType.GenericError, exception.Type);
        }

        [Fact]
        public void SerializeResponsesWhenCollectionContainsDuplicateIdAndIdIsNumber()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponsesArray = new JsonRpcResponse[2];

            jsonRpcResponsesArray[0] = new JsonRpcResponse(100L, new[] { 200L });
            jsonRpcResponsesArray[1] = new JsonRpcResponse(100L, new[] { 300L });

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeResponses(jsonRpcResponsesArray));

            Assert.Equal(JsonRpcExceptionType.GenericError, exception.Type);
        }

        [Fact]
        public void SerializeResponsesWhenCollectionContainsDuplicateIdAndIdIsString()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponsesArray = new JsonRpcResponse[2];

            jsonRpcResponsesArray[0] = new JsonRpcResponse("100", new[] { 200L });
            jsonRpcResponsesArray[1] = new JsonRpcResponse("100", new[] { 300L });

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeResponses(jsonRpcResponsesArray));

            Assert.Equal(JsonRpcExceptionType.GenericError, exception.Type);
        }

        [Fact]
        public void DeserializeRequestsDataWhenJsonStringIsNull()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.DeserializeRequestsData(default(string)));
        }

        [Fact]
        public void DeserializeRequestsDataWhenJsonStringIsEmptyString()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestsData(string.Empty));

            Assert.Equal(JsonRpcExceptionType.ParseError, exception.Type);
        }

        [Fact]
        public void DeserializeRequestsDataWhenSchemaIsNotDefined()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonSample = JsonTools.GetJsonSample("jrs_01_req");

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestsData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.GenericError, exception.Type);
        }

        [Fact]
        public void DeserializeRequestsDataWhenJsonIsInvalid()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("jrs_02_req");

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestsData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.ParseError, exception.Type);
        }

        [Fact]
        public void DeserializeRequestsDataWhenJsonProtocolIsAbsent()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("jrs_03_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsEmpty);
            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetItem();

            Assert.False(jsonRpcMessageInfo.Success);

            var jsonRpcException = jsonRpcMessageInfo.GetException();

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }

        [Fact]
        public void DeserializeRequestsDataWhenJsonProtocolHasInvalidType()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("jrs_04_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsEmpty);
            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetItem();

            Assert.False(jsonRpcMessageInfo.Success);

            var jsonRpcException = jsonRpcMessageInfo.GetException();

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }

        [Fact]
        public void DeserializeRequestsDataWhenJsonProtocolHasInvalidValue()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("jrs_05_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsEmpty);
            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetItem();

            Assert.False(jsonRpcMessageInfo.Success);

            var jsonRpcException = jsonRpcMessageInfo.GetException();

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }

        [Fact]
        public void DeserializeResponsesDataWhenJsonStringIsNull()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonRpcBindingsProvider = Fake.Fake<IJsonRpcBindingsProvider>();

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.DeserializeResponsesData(default(string), jsonRpcBindingsProvider));
        }

        [Fact]
        public void DeserializeResponsesDataWhenJsonStringIsEmptyString()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonRpcBindingsProvider = Fake.Fake<IJsonRpcBindingsProvider>();
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(string.Empty, jsonRpcBindingsProvider);

            Assert.NotNull(jsonRpcDataInfo);
            Assert.True(jsonRpcDataInfo.IsEmpty);
        }

        [Fact]
        public void DeserializeResponsesDataWhenSchemaIsNotDefined()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonSample = JsonTools.GetJsonSample("jrs_01_res");
            var jsonRpcBindingsProvider = Fake.Fake<IJsonRpcBindingsProvider>();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindingsProvider));

            Assert.Equal(JsonRpcExceptionType.GenericError, exception.Type);
        }

        [Fact]
        public void DeserializeResponsesDataWhenJsonIsInvalid()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("jrs_02_res");
            var jsonRpcBindingsProvider = Fake.Fake<IJsonRpcBindingsProvider>();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindingsProvider));

            Assert.Equal(JsonRpcExceptionType.ParseError, exception.Type);
        }

        [Fact]
        public void DeserializeRequestDataWithArrayPool()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.SupportedMethods.Add("test_method");

            var jsonRpcSettings = new JsonRpcSettings();

            jsonRpcSettings.JsonSerializerArrayPool = new TestJsonArrayPool();

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema, jsonRpcSettings);
            var jsonSample = JsonTools.GetJsonSample("jrap_01_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsEmpty);
            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetItem();

            Assert.NotNull(jsonRpcMessageInfo);
            Assert.True(jsonRpcMessageInfo.Success);
        }

        [Fact]
        public void DeserializeRequestDataWithCustomJsonConverter()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.SupportedMethods.Add("test_method");
            jsonRpcSchema.ParameterTypeBindings["test_method"] = typeof(TestParams);

            var jsonSerializer = JsonSerializer.CreateDefault();

            jsonSerializer.Converters.Add(new TestJsonConverter());

            var jsonRpcSettings = new JsonRpcSettings();

            jsonRpcSettings.JsonSerializer = jsonSerializer;

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema, jsonRpcSettings);
            var jsonSample = JsonTools.GetJsonSample("jrcc_01_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsEmpty);
            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetItem();

            Assert.NotNull(jsonRpcMessageInfo);
            Assert.True(jsonRpcMessageInfo.Success);

            var jsonRpcRequest = jsonRpcMessageInfo.GetItem();

            Assert.IsType<TestParams>(jsonRpcRequest.Params);

            var jsonRpcRequestParams = (TestParams)jsonRpcRequest.Params;

            Assert.Equal("value", jsonRpcRequestParams.Value);
        }

        #region Test Types

        private sealed class TestJsonArrayPool : IArrayPool<char>
        {
            public char[] Rent(int minimumLength) =>
                ArrayPool<char>.Shared.Rent(minimumLength);

            public void Return(char[] array) =>
                ArrayPool<char>.Shared.Return(array);
        }

        [JsonObject(MemberSerialization.OptIn)]
        private sealed class TestParams
        {
            [JsonProperty("value")]
            public string Value { get; set; }
        }

        private sealed class TestJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) =>
                objectType == typeof(string);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
                (string)reader.Value == "default_value" ? "value" : reader.Value;

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
                throw new NotImplementedException();
        }

        #endregion
    }
}