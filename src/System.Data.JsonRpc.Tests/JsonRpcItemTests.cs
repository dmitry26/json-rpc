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

            jsonRpcScheme.Methods["m"] = new JsonRpcMethodScheme();

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.item_valid_true.json");
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);
            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);
            Assert.NotNull(jsonRpcItem.Message);
            Assert.Null(jsonRpcItem.Exception);
        }

        [Fact]
        public void IsValidIsFalse()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();

            jsonRpcScheme.Methods["m"] = new JsonRpcMethodScheme();

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.item_valid_false.json");
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);
            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.False(jsonRpcItem.IsValid);
            Assert.Null(jsonRpcItem.Message);
            Assert.NotNull(jsonRpcItem.Exception);

            var jsonRpcException = jsonRpcItem.Exception;

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcException.Type);
        }
    }
}