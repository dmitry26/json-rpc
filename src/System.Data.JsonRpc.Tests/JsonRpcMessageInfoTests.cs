using System.Data.JsonRpc.Tests.Support;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public sealed class JsonRpcMessageInfoTests
    {
        [Fact]
        public void SuccessIsTrue()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.SupportedMethods.Add("test_method");

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("jrmi_01_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);
            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.True(jsonRpcMessageInfo.Success);
            Assert.NotNull(jsonRpcMessageInfo.GetMessage());

            var getExceptionException = Assert.Throws<JsonRpcException>(() =>
                jsonRpcMessageInfo.GetException());

            Assert.Equal(JsonRpcExceptionType.GenericError, getExceptionException.Type);
        }

        [Fact]
        public void SuccessIsFalse()
        {
            var jsonRpcSchema = new JsonRpcSchema();

            jsonRpcSchema.SupportedMethods.Add("test_method");

            var jsonRpcSerializer = new JsonRpcSerializer(jsonRpcSchema);
            var jsonSample = JsonTools.GetJsonSample("jrmi_02_req");
            var jsonRpcDataInfo = jsonRpcSerializer.DeserializeRequestsData(jsonSample);
            var jsonRpcMessageInfo = jsonRpcDataInfo.GetSingleItem();

            Assert.False(jsonRpcMessageInfo.Success);

            var getItemException = Assert.Throws<JsonRpcException>(() =>
                jsonRpcMessageInfo.GetMessage());

            Assert.Equal(JsonRpcExceptionType.GenericError, getItemException.Type);
            Assert.NotNull(jsonRpcMessageInfo.GetException());

            var jsonRpcMessageInfoException = jsonRpcMessageInfo.GetException();

            Assert.Equal(JsonRpcExceptionType.InvalidMessage, jsonRpcMessageInfoException.Type);
        }
    }
}