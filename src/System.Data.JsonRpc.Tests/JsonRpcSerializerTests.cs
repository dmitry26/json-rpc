using System.Buffers;
using System.Collections.Generic;
using System.Data.JsonRpc.Tests.Resources;
using FakeItEasy;
using Newtonsoft.Json;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed partial class JsonRpcSerializerTests
    {
        [Fact]
        public void ConstructorWithSchemeWhenSchemeIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcSerializer(default(JsonRpcSerializerScheme)));
        }

        [Fact]
        public void ConstructorWithSchemeAndSettingsWhenSchemeIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcSerializer(default(JsonRpcSerializerScheme), new JsonRpcSerializerSettings()));
        }

        [Fact]
        public void ConstructorWithSchemeAndSettingsWhenSettingsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcSerializer(new JsonRpcSerializerScheme(), default(JsonRpcSerializerSettings)));
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

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
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

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
        }

        [Fact]
        public void SerializeRequestsWhenCollectionContainsNull()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeRequests(new[] { default(JsonRpcRequest) }));

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
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

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
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

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
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

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
        }

        [Fact]
        public void SerializeResponsesWhenCollectionContainsDuplicateIdAndIdIsNumber()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponsesArray = new JsonRpcResponse[2];

            jsonRpcResponsesArray[0] = new JsonRpcResponse(new[] { 200L }, 100L);
            jsonRpcResponsesArray[1] = new JsonRpcResponse(new[] { 300L }, 100L);

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeResponses(jsonRpcResponsesArray));

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
        }

        [Fact]
        public void SerializeResponsesWhenCollectionContainsDuplicateIdAndIdIsString()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcResponsesArray = new JsonRpcResponse[2];

            jsonRpcResponsesArray[0] = new JsonRpcResponse(new[] { 200L }, "100");
            jsonRpcResponsesArray[1] = new JsonRpcResponse(new[] { 300L }, "100");

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.SerializeResponses(jsonRpcResponsesArray));

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, exception.Type);
        }

        [Fact]
        public void DeserializeRequestsDataWhenJsonStringIsNull()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.DeserializeRequestsData(default(string)));
        }

        [Fact]
        public void DeserializeRequestsDataWhenJsonStringIsEmptyString()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestsData(string.Empty));

            Assert.Equal(JsonRpcExceptionType.ParseError, exception.Type);
        }

        [Fact]
        public void DeserializeRequestsDataWhenSchemeIsNotDefined()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrs_01_req.txt");

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestsData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.GenericError, exception.Type);
        }

        [Fact]
        public void DeserializeRequestsDataWhenJsonIsInvalid()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrs_02_req.txt");

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestsData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.ParseError, exception.Type);
        }

        [Fact]
        public void DeserializeRequestsDataWhenJsonProtocolIsAbsent()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrs_03_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsEmpty);
            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.False(jsonRpcMessageInfo.IsValid);

            var jsonRpcException = jsonRpcMessageInfo.GetException();

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }

        [Fact]
        public void DeserializeRequestsDataWhenJsonProtocolHasInvalidType()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrs_04_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsEmpty);
            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.False(jsonRpcMessageInfo.IsValid);

            var jsonRpcException = jsonRpcMessageInfo.GetException();

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }

        [Fact]
        public void DeserializeRequestsDataWhenJsonProtocolHasInvalidValue()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrs_05_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsEmpty);
            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.False(jsonRpcMessageInfo.IsValid);

            var jsonRpcException = jsonRpcMessageInfo.GetException();

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }

        [Fact]
        public void DeserializeResponsesDataWhenJsonStringIsNull()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonRpcBindings = A.Fake<IReadOnlyDictionary<JsonRpcId, string>>(x => x.Strict());

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.DeserializeResponsesData(default(string), jsonRpcBindings));
        }

        [Fact]
        public void DeserializeResponsesDataWhenJsonStringIsEmptyString()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonRpcBindings = A.Fake<IReadOnlyDictionary<JsonRpcId, string>>(x => x.Strict());
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(string.Empty, jsonRpcBindings);

            Assert.NotNull(jsonRpcDataInfo);
            Assert.True(jsonRpcDataInfo.IsEmpty);
        }

        [Fact]
        public void DeserializeResponsesDataWhenSchemeIsNotDefined()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrs_01_res.txt");
            var jsonRpcBindings = A.Fake<IReadOnlyDictionary<JsonRpcId, string>>(x => x.Strict());

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindings));

            Assert.Equal(JsonRpcExceptionType.GenericError, exception.Type);
        }

        [Fact]
        public void DeserializeResponsesDataWhenJsonIsInvalid()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrs_02_res.txt");
            var jsonRpcBindings = A.Fake<IReadOnlyDictionary<JsonRpcId, string>>(x => x.Strict());

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindings));

            Assert.Equal(JsonRpcExceptionType.ParseError, exception.Type);
        }

        [Fact]
        public void DeserializeRequestDataWithArrayPool()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["test_method"] = new JsonRpcMethodScheme(true);

            var jsonRpcSettings = new JsonRpcSerializerSettings
            {
                JsonSerializerArrayPool = new TestJsonArrayPool()
            };

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme, jsonRpcSettings);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrap_01_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsEmpty);
            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);
        }

        [Fact]
        public void DeserializeRequestDataWithCustomJsonConverter()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["test_method"] = new JsonRpcMethodScheme(true, typeof(TestParams));

            var jsonRpcSettings = new JsonRpcSerializerSettings
            {
                JsonSerializer = JsonSerializer.CreateDefault()
            };

            jsonRpcSettings.JsonSerializer.Converters.Add(new TestJsonConverter());

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme, jsonRpcSettings);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrcc_01_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsEmpty);
            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcRequest = jsonRpcMessageInfo.GetMessage();

            Assert.IsType<TestParams>(jsonRpcRequest.Params);

            var jsonRpcRequestParams = (TestParams)jsonRpcRequest.Params;

            Assert.Equal("value", jsonRpcRequestParams.Value);
        }

        [Fact]
        public void DeserializeRequestDataWhenBindingIsNull()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["test_method"] = null;

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrib_01_req.txt");

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestsData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.GenericError, exception.Type);
        }

        [Fact]
        public void DeserializeRequestDataWhenNotificationIsFalseAndIdIsEmpty()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["test_method"] = new JsonRpcMethodScheme(false);

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrin_01_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsEmpty);
            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.False(jsonRpcMessageInfo.IsValid);

            var jsonRpcException = jsonRpcMessageInfo.GetException();

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }

        [Fact]
        public void DeserializeRequestDataWhenNotificationIsTrueAndIdIsNotEmpty()
        {
            var jsonRpcSerializerScheme = new JsonRpcSerializerScheme();

            jsonRpcSerializerScheme.Methods["test_method"] = new JsonRpcMethodScheme(true);

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSerializerScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrin_02_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsEmpty);
            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.False(jsonRpcMessageInfo.IsValid);

            var jsonRpcException = jsonRpcMessageInfo.GetException();

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }

        [Fact]
        public void DeserializeResponseDataWhenMethodSchemeBindingIsDynamic()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrds_01_res.txt");

            var bindings = new Dictionary<JsonRpcId, JsonRpcMethodScheme>
            {
                [1] = new JsonRpcMethodScheme(typeof(int), typeof(object))
            };

            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, bindings);

            Assert.False(jsonRpcDataInfo.IsEmpty);
            Assert.False(jsonRpcDataInfo.IsBatch);

            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);

            var jsonRpcMessage = jsonRpcMessageInfo.GetMessage();

            Assert.Equal(1, jsonRpcMessage.Id);
            Assert.True(jsonRpcMessage.Success);
            Assert.Equal(42, jsonRpcMessage.Result);
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