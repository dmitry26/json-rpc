using System.Collections.Generic;
using System.Data.JsonRpc.Tests.Resources;
using Moq;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcDataTests
    {
        [Fact]
        public void GetItemAndItemsWhenIsEmpty()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.data_empty.txt");
            var jsonRpcBindingsMock = new Mock<IReadOnlyDictionary<JsonRpcId, string>>(MockBehavior.Strict);
            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample, jsonRpcBindingsMock.Object);

            Assert.True(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsSingle);
            Assert.False(jsonRpcData.IsBatch);
            Assert.Null(jsonRpcData.SingleItem);
            Assert.Null(jsonRpcData.BatchItems);
        }

        [Fact]
        public void GetItemAndItemsWhenIsSingle()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.data_single.txt");
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
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.data_batch.txt");
            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsEmpty);
            Assert.False(jsonRpcData.IsSingle);
            Assert.True(jsonRpcData.IsBatch);
            Assert.Null(jsonRpcData.SingleItem);
            Assert.NotNull(jsonRpcData.BatchItems);
        }
    }
}