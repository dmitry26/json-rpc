using System.Collections.Generic;
using System.Data.JsonRpc.Tests.Resources;
using FakeItEasy;
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
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrdi_01_res.txt");
            var jsonRpcBindings = A.Fake<IReadOnlyDictionary<JsonRpcId, string>>(x => x.Strict());
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindings);

            Assert.True(jsonRpcDataInfo.IsEmpty);
            Assert.False(jsonRpcDataInfo.IsBatch);

            var getItemException = Assert.Throws<JsonRpcException>(() =>
                jsonRpcDataInfo.GetSingleItem());

            Assert.Equal(JsonRpcExceptionType.GenericError, getItemException.Type);

            var getItemsException = Assert.Throws<JsonRpcException>(() =>
                jsonRpcDataInfo.GetBatchItems());

            Assert.Equal(JsonRpcExceptionType.GenericError, getItemsException.Type);
        }

        [Fact]
        public void GetItemAndItemsWhenIsNotEmptyAndIsNotBatch()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrdi_02_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsEmpty);
            Assert.False(jsonRpcDataInfo.IsBatch);

            var getItemsException = Assert.Throws<JsonRpcException>(() =>
                jsonRpcDataInfo.GetBatchItems());

            Assert.Equal(JsonRpcExceptionType.GenericError, getItemsException.Type);
        }

        [Fact]
        public void GetItemAndItemsWhenIsNotEmptyAndIsBatch()
        {
            var jsonRpcScheme = new JsonRpcSerializerScheme();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcScheme);
            var jsonSample = EmbeddedResourceManager.GetString("Assets.jrdi_03_req.txt");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsEmpty);
            Assert.True(jsonRpcDataInfo.IsBatch);

            var getItemException = Assert.Throws<JsonRpcException>(() =>
                jsonRpcDataInfo.GetSingleItem());

            Assert.Equal(JsonRpcExceptionType.GenericError, getItemException.Type);
            Assert.NotNull(jsonRpcDataInfo.GetBatchItems());
        }
    }
}