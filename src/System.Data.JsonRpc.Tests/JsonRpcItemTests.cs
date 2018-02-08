using System.Data.JsonRpc.Tests.Resources;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcItemTests
    {
        [Fact]
        public void IsValidIsTrue()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.item_valid_true.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.RequestContracts["m"] = new JsonRpcRequestContract();

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);
            var jsonRpcItem = jsonRpcData.SingleItem;

            Assert.True(jsonRpcItem.IsValid);
            Assert.NotNull(jsonRpcItem.Message);
            Assert.Null(jsonRpcItem.Exception);
        }

        [Fact]
        public void IsValidIsFalse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.item_valid_false.json");
            var jsonRpcSerializer = new JsonRpcSerializer();

            jsonRpcSerializer.RequestContracts["m"] = new JsonRpcRequestContract();

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