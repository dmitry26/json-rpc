using System.Data.JsonRpc.Tests.Resources;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcDataTests
    {
        [Fact]
        public void GetItemAndItemsWhenIsEmpty()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.data_empty.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.True(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsSingle);
            Assert.False(jsonRpcData.IsBatch);
            Assert.Null(jsonRpcData.SingleItem);
            Assert.Null(jsonRpcData.BatchItems);
        }

        [Fact]
        public void GetItemAndItemsWhenIsSingle()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.data_single.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.True(jsonRpcData.IsSingle);
            Assert.False(jsonRpcData.IsBatch);
            Assert.NotNull(jsonRpcData.SingleItem);
            Assert.Null(jsonRpcData.BatchItems);
        }

        [Fact]
        public void GetItemAndItemsWhenIsBatch()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.data_batch.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsSingle);
            Assert.True(jsonRpcData.IsBatch);
            Assert.Null(jsonRpcData.SingleItem);
            Assert.NotNull(jsonRpcData.BatchItems);
        }
    }
}