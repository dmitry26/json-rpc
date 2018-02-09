using System.Data.JsonRpc.Tests.Resources;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcDataTests
    {
        [Fact]
        public void GetItemAndItemsWhenIsSingle()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.data_single.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.True(jsonRpcData.IsSingle);
            Assert.False(jsonRpcData.IsBatch);
            Assert.Null(jsonRpcData.BatchItems);
        }

        [Fact]
        public void GetItemAndItemsWhenIsBatch()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.data_batch.json");
            var jsonRpcSerializer = new JsonRpcSerializer();
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsSingle);
            Assert.True(jsonRpcData.IsBatch);
            Assert.NotNull(jsonRpcData.BatchItems);
        }
    }
}