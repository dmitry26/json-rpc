using System.Data.JsonRpc.Tests.Resources;
using Xunit;

namespace System.Data.JsonRpc.Tests
{
    public partial class Utf8JsonRpcSerializerTests
    {
        // Tests based on the JSON-RPC 1.0 specification (http://www.jsonrpc.org/specification_v1)

        #region Example V2 T01: Echo service

        [Fact]
        public void V1SpecT010DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_01.0_req.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            jsonRpcSerializer.RequestContracts["echo"] = new JsonRpcRequestContract(new[] { typeof(string) });

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(1L, jsonRpcMessage.Id);
            Assert.Equal("echo", jsonRpcMessage.Method);
            Assert.Equal(JsonRpcParametersType.ByPosition, jsonRpcMessage.ParametersType);
            Assert.Equal(new object[] { "Hello JSON-RPC" }, jsonRpcMessage.ParametersByPosition);
        }

        [Fact]
        public void V1SpecT010SerializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_01.0_req.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            var jsonRpcMessage = new JsonRpcRequest("echo", 1L, new object[] { "Hello JSON-RPC" });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V1SpecT010DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_01.0_res.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            jsonRpcSerializer.ResponseContracts["echo"] = new JsonRpcResponseContract(typeof(string));
            jsonRpcSerializer.StaticResponseBindings[1L] = "echo";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(1L, jsonRpcMessage.Id);
            Assert.IsType<string>(jsonRpcMessage.Result);
            Assert.Equal("Hello JSON-RPC", jsonRpcMessage.Result);
        }

        [Fact]
        public void V1SpecT010SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_01.0_res.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            var jsonRpcMessage = new JsonRpcResponse("Hello JSON-RPC", 1L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        #endregion

        #region Example V2 T02: Chat application

        [Fact]
        public void V1SpecT020DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_02.0_req.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            jsonRpcSerializer.RequestContracts["postMessage"] = new JsonRpcRequestContract(new[] { typeof(string) });

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(99L, jsonRpcMessage.Id);
            Assert.Equal("postMessage", jsonRpcMessage.Method);
            Assert.Equal(JsonRpcParametersType.ByPosition, jsonRpcMessage.ParametersType);
            Assert.Equal(new object[] { "Hello all!" }, jsonRpcMessage.ParametersByPosition);
        }

        [Fact]
        public void V1SpecT020SerializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_02.0_req.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            var jsonRpcMessage = new JsonRpcRequest("postMessage", 99L, new object[] { "Hello all!" });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V1SpecT020DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_02.0_res.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            jsonRpcSerializer.ResponseContracts["echo"] = new JsonRpcResponseContract(typeof(long));
            jsonRpcSerializer.StaticResponseBindings[99L] = "echo";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(99L, jsonRpcMessage.Id);
            Assert.IsType<long>(jsonRpcMessage.Result);
            Assert.Equal(1L, jsonRpcMessage.Result);
        }

        [Fact]
        public void V1SpecT020SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_02.0_res.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            var jsonRpcMessage = new JsonRpcResponse(1L, 99L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V1SpecT021DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_02.1_req.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            jsonRpcSerializer.RequestContracts["handleMessage"] = new JsonRpcRequestContract(new[] { typeof(string), typeof(string) });

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(default, jsonRpcMessage.Id);
            Assert.Equal("handleMessage", jsonRpcMessage.Method);
            Assert.Equal(JsonRpcParametersType.ByPosition, jsonRpcMessage.ParametersType);
            Assert.Equal(new object[] { "user1", "we were just talking" }, jsonRpcMessage.ParametersByPosition);
        }

        [Fact]
        public void V1SpecT021SerializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_02.1_req.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            var jsonRpcMessage = new JsonRpcRequest("handleMessage", new object[] { "user1", "we were just talking" });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V1SpecT021DeserializeResponse()
        {
            // N/A
        }

        [Fact]
        public void V1SpecT021SerializeResponse()
        {
            // N/A
        }

        [Fact]
        public void V1SpecT022DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_02.2_req.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            jsonRpcSerializer.RequestContracts["handleMessage"] = new JsonRpcRequestContract(new[] { typeof(string), typeof(string) });

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(default, jsonRpcMessage.Id);
            Assert.Equal("handleMessage", jsonRpcMessage.Method);
            Assert.Equal(JsonRpcParametersType.ByPosition, jsonRpcMessage.ParametersType);
            Assert.Equal(new object[] { "user3", "sorry, gotta go now, ttyl" }, jsonRpcMessage.ParametersByPosition);
        }

        [Fact]
        public void V1SpecT022SerializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_02.2_req.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            var jsonRpcMessage = new JsonRpcRequest("handleMessage", new object[] { "user3", "sorry, gotta go now, ttyl" });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V1SpecT022DeserializeResponse()
        {
            // N/A
        }

        [Fact]
        public void V1SpecT022SerializeResponse()
        {
            // N/A
        }

        [Fact]
        public void V1SpecT023DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_02.3_req.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            jsonRpcSerializer.RequestContracts["postMessage"] = new JsonRpcRequestContract(new[] { typeof(string) });

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(101L, jsonRpcMessage.Id);
            Assert.Equal("postMessage", jsonRpcMessage.Method);
            Assert.Equal(JsonRpcParametersType.ByPosition, jsonRpcMessage.ParametersType);
            Assert.Equal(new object[] { "I have a question:" }, jsonRpcMessage.ParametersByPosition);
        }

        [Fact]
        public void V1SpecT023SerializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_02.3_req.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            var jsonRpcMessage = new JsonRpcRequest("postMessage", 101L, new object[] { "I have a question:" });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V1SpecT023DeserializeResponse()
        {
            // N/A
        }

        [Fact]
        public void V1SpecT023SerializeResponse()
        {
            // N/A
        }

        [Fact]
        public void V1SpecT024DeserializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_02.4_req.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            jsonRpcSerializer.RequestContracts["userLeft"] = new JsonRpcRequestContract(new[] { typeof(string) });

            var jsonRpcData = jsonRpcSerializer.DeserializeRequestData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(default, jsonRpcMessage.Id);
            Assert.Equal("userLeft", jsonRpcMessage.Method);
            Assert.Equal(JsonRpcParametersType.ByPosition, jsonRpcMessage.ParametersType);
            Assert.Equal(new object[] { "user3" }, jsonRpcMessage.ParametersByPosition);
        }

        [Fact]
        public void V1SpecT024SerializeRequest()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_02.4_req.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            var jsonRpcMessage = new JsonRpcRequest("userLeft", new object[] { "user3" });
            var jsonResult = jsonRpcSerializer.SerializeRequest(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        [Fact]
        public void V1SpecT024DeserializeResponse()
        {
            // N/A
        }

        [Fact]
        public void V1SpecT024SerializeResponse()
        {
            // N/A
        }

        [Fact]
        public void V1SpecT025DeserializeRequest()
        {
            // N/A
        }

        [Fact]
        public void V1SpecT025SerializeRequest()
        {
            // N/A
        }

        [Fact]
        public void V1SpecT025DeserializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_02.5_res.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            jsonRpcSerializer.ResponseContracts["postMessage"] = new JsonRpcResponseContract(typeof(long));
            jsonRpcSerializer.StaticResponseBindings[101L] = "postMessage";

            var jsonRpcData = jsonRpcSerializer.DeserializeResponseData(jsonSample);

            Assert.False(jsonRpcData.IsBatch);

            var jsonRpcItem = jsonRpcData.Item;

            Assert.True(jsonRpcItem.IsValid);

            var jsonRpcMessage = jsonRpcItem.Message;

            Assert.Equal(101L, jsonRpcMessage.Id);
            Assert.IsType<long>(jsonRpcMessage.Result);
            Assert.Equal(1L, jsonRpcMessage.Result);
        }

        [Fact]
        public void V1SpecT025SerializeResponse()
        {
            var jsonSample = EmbeddedResourceManager.GetString("Assets.v1_spec_02.5_res.json");

            var jsonRpcSerializer = new Utf8JsonRpcSerializer
            {
                CompatibilityLevel = JsonRpcCompatibilityLevel.Level1
            };

            var jsonRpcMessage = new JsonRpcResponse(1L, 101L);
            var jsonResult = jsonRpcSerializer.SerializeResponse(jsonRpcMessage);

            CompareJsonStrings(jsonSample, jsonResult);
        }

        #endregion
    }
}