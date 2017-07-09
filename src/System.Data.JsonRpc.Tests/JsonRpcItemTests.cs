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

            jsonRpcScheme.Methods["test_method"] = new JsonRpcMethodScheme(true);

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrmi_01_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);
            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.IsValid);
            Assert.NotNull(jsonRpcMessageInfo.GetMessage());

            Assert.Throws<InvalidOperationException>(() =>
                jsonRpcMessageInfo.GetException());
        }

        [Fact]
        public void IsValidIsFalse()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["test_method"] = new JsonRpcMethodScheme(true);

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrmi_02_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);
            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.False(jsonRpcMessageInfo.IsValid);

            Assert.Throws<InvalidOperationException>(() =>
                jsonRpcMessageInfo.GetMessage());

            Assert.NotNull(jsonRpcMessageInfo.GetException());

            var jsonRpcMessageInfoException = jsonRpcMessageInfo.GetException();

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcMessageInfoException.Type);
        }
    }
}