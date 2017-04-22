using System.Data.JsonRpc.Tests.Resources;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcItemTests
    {
        [Fact]
        public void IsValidIsTrue()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["test_method"] = JsonRpcMethodScheme.Empty;

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrmi_01_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);
            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);
            Assert.NotNull(jsonRpcMessageInfo.GetMessage());

            var getExceptionException = Assert.Throws<JsonRpcException>(() =>
                jsonRpcMessageInfo.GetException());

            Assert.Equal(JsonRpcExceptionType.GenericError, getExceptionException.Type);
        }

        [Fact]
        public void IsValidIsFalse()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["test_method"] = JsonRpcMethodScheme.Empty;

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrmi_02_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);
            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.False(jsonRpcMessageInfo.IsValid);

            var getItemException = Assert.Throws<JsonRpcException>(() =>
                jsonRpcMessageInfo.GetMessage());

            Assert.Equal(JsonRpcExceptionType.GenericError, getItemException.Type);
            Assert.NotNull(jsonRpcMessageInfo.GetException());

            var jsonRpcMessageInfoException = jsonRpcMessageInfo.GetException();

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcMessageInfoException.Type);
        }
    }
}