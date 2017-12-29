## System.Data.JsonRpc

Provides support for serialization and deserialization of [JSON-RPC 2.0](http://www.jsonrpc.org/specification) messages.

[![NuGet package](https://img.shields.io/nuget/v/System.Data.JsonRpc.svg?style=flat-square)](https://www.nuget.org/packages/System.Data.JsonRpc)

### Sample of communication with a server via JSON-RPC 2.0

Generating a random UUID using RANDOM.ORG service.

1. Define types for request parameters and result objects:

```cs
class UuidsRandom
{
    [JsonProperty("data")]
    public Guid[] Data { get; set; }
}
class UuidsResult
{
    [JsonProperty("random")]
    public UuidsRandom Random { get; set; }
}
```

2. Send a request to the server:

```cs
var jrParams = new Dictionary<string, object>
{
    ["apiKey"] = "00000000-0000-0000-0000-000000000000", ["n"] = 1L
};
var jrRequest = new JsonRpcRequest("generateUUIDs", 0L, jrParams);
var jrBindings = new Dictionary<JsonRpcId, JsonRpcMethodScheme>
{
    [0L] = new JsonRpcMethodScheme(typeof(UuidsResult))
};
var jrSerializer = new JsonRpcSerializer();
var httpRequestString = jrSerializer.SerializeRequest(jrRequest);
var httpRequestContent = new StringContent(httpRequestString, Encoding.UTF8, "application/json");
var httpRequestUri = new Uri("https://api.random.org/json-rpc/1/invoke");
var httpResponse = await new HttpClient().PostAsync(httpRequestUri, httpRequestContent);
var httpResponseString = await httpResponse.Content.ReadAsStringAsync();
var jrData = jrSerializer.DeserializeResponseData(httpResponseString, jrBindings);
var jrResult = (UuidsResult)jrData.SingleItem.Message.Result;

Console.Write("Random UUID: " + jrResult.Random.Data[0]);
```