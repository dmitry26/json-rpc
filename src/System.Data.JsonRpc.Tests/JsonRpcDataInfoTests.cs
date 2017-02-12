using System.Data.JsonRpc.Tests.Support;
using Xunit;
using Fake = FakeItEasy.A;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcDataInfoTests
    {
        [Fact]
        public void GetItemAndItemsWhenIsEmpty()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("jrdi_01_res");
            var jsonRpcBindingsProvider = Fake.Fake<IJsonRpcBindingsProvider>();
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeResponsesData(jsonSample, jsonRpcBindingsProvider);

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
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("jrdi_02_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);

            Assert.False(jsonRpcDataInfo.IsEmpty);
            Assert.False(jsonRpcDataInfo.IsBatch);
            Assert.NotNull(jsonRpcDataInfo.GetSingleItem());

            var getItemsException = Assert.Throws<JsonRpcException>(() =>
                jsonRpcDataInfo.GetBatchItems());

            Assert.Equal(JsonRpcExceptionType.GenericError, getItemsException.Type);
        }

        [Fact]
        public void GetItemAndItemsWhenIsNotEmptyAndIsBatch()
        {
            var jsonRpcSchema = new JsonRpcSchema();
            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("jrdi_03_req");
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