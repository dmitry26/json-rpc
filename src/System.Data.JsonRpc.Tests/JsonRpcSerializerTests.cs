using System.Buffers;
using System.Collections.Generic;
using System.Data.JsonRpc.Tests.Resources;
using Moq;
using Newtonsoft.Json;
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
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_param_null.json");
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
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.DeserializeRequestData(null));
        }

        [Fact]
        public void DeserializeRequestDataWhenJsonStringIsEmptyString()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestData(string.Empty));

            Assert.Equal(JsonRpcExceptionType.Parsing, exception.Type);
        }

        [Fact]
        public void DeserializeRequestDataWhenJsonIsInvalid()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_json_invalid_req.json");

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.Parsing, exception.Type);
        }

        [Fact]
        public void DeserializeRequestDataWhenJsonProtocolIsAbsent()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_protocol_undefined_req.json");
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
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_protocol_undefined_res.json");
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, new Dictionary<JsonRpcId, string>());

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
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_protocol_invalid_type_req.json");
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
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_protocol_invalid_type_res.json");
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, new Dictionary<JsonRpcId, string>());

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
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_protocol_invalid_value_req.json");
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
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_protocol_invalid_value_res.json");
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, new Dictionary<JsonRpcId, string>());

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
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["m"] = new JsonRpcMethodScheme(new[] { typeof(object) });

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_params_invalid_type.json");
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
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonRpcBindingsMock = new Mock<IReadOnlyDictionary<JsonRpcId, string>>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() =>
                jsonRpcSerializer.DeserializeResponseData(null, jsonRpcBindingsMock.Object));
        }

        [Fact]
        public void DeserializeResponseDataWhenJsonStringIsEmptyString()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonRpcBindingsMock = new Mock<IReadOnlyDictionary<JsonRpcId, string>>(MockBehavior.Strict);
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(string.Empty, jsonRpcBindingsMock.Object);

            Assert.NotNull(jsonRpcData);
            Assert.True(jsonRpcData.IsEmpty);
        }

        [Fact]
        public void DeserializeResponseDataWhenJsonIsInvalid()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_json_invalid_res.json");
            var jsonRpcBindingsMock = new Mock<IReadOnlyDictionary<JsonRpcId, string>>(MockBehavior.Strict);

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeResponseData(jsonSample, jsonRpcBindingsMock.Object));

            Assert.Equal(JsonRpcExceptionType.Parsing, exception.Type);
        }

        [Fact]
        public void DeserializeRequestDataWithBufferPool()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["m"] = new JsonRpcMethodScheme();

            var jsonRpcSettings = new JsonRpcSerializerSettings
            {
                JsonSerializerBufferPool = new TestJsonBufferPool()
            };

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme, jsonRpcSettings);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_buffer_pool.json");
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);
        }

        [Fact]
        public void DeserializeRequestDataWithCustomJsonConverter()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            var jsonRpcMethodParamsScheme = new Dictionary<string, Type>
            {
                ["p"] = typeof(string)
            };

            jsonRpcScheme.Methods["m"] = new JsonRpcMethodScheme(jsonRpcMethodParamsScheme);

            var jsonRpcSettings = new JsonRpcSerializerSettings
            {
                JsonSerializer = JsonSerializer.CreateDefault()
            };

            jsonRpcSettings.JsonSerializer.Converters.Add(new TestJsonConverter());

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme, jsonRpcSettings);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_json_converter.json");
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(JsonRpcParamsType.ByName, jsonRpcMessage.ParamsType);
            Assert.Equal("v2", jsonRpcMessage.ParamsByName["p"]);
        }

        [Fact]
        public void DeserializeRequestDataWhenBindingIsNull()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["m"] = null;

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_binding_invalid.json");

            var exception = Assert.Throws<JsonRpcException>(() =>
                jsonRpcSerializer.DeserializeRequestData(jsonSample));

            Assert.Equal(JsonRpcExceptionType.Undefined, exception.Type);
        }

        [Fact]
        public void DeserializeResponseDataWhenMethodSchemeBindingIsDynamic()
        {
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonSample = EmbeddedResourceManager.GetString("Assets.core_binding_dynamic.json");

            var bindings = new Dictionary<JsonRpcId, JsonRpcMethodScheme>
            {
                [1L] = new JsonRpcMethodScheme(typeof(int), typeof(object))
            };

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, bindings);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(1, jsonRpcMessage.Id);
            Assert.True(jsonRpcMessage.Success);
            Assert.Equal(42, jsonRpcMessage.Result);
        }

        #region Test Types

        private sealed class TestJsonBufferPool : IArrayPool<char>
        {
            public char[] Rent(int minimumLength)
            {
                return ArrayPool<char>.Shared.Rent(minimumLength);
            }

            public void Return(char[] array)
            {
                ArrayPool<char>.Shared.Return(array);
            }
        }

        private sealed class TestJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return (string)reader.Value == "v1" ? "v2" : reader.Value;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotSupportedException();
            }
        }

        #endregion
    }
}