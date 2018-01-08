## System.Data.JsonRpc

Provides support for serialization and deserialization of [JSON-RPC 2.0](http://www.jsonrpc.org/specification) messages.

[![NuGet package](https://img.shields.io/nuget/v/System.Data.JsonRpc.svg?style=flat-square)](https://www.nuget.org/packages/System.Data.JsonRpc)

Generating a random UUID using RANDOM.ORG service.

```cs
public class UuidsRandom
{
    [JsonProperty("data")]
    public Guid[] Data
    {
        get; set;
    }
}

public class UuidsResult
{
    [JsonProperty("random")]
    public UuidsRandom Random
    {
        get; set;
    }
}
```
```cs
var httpClient = new HttpClient();

var rpcUri = new Uri("https://api.random.org/json-rpc/2/invoke");
var rpcSerializer = new JsonRpcSerializer();

var rpcParameters = new Dictionary<string, object>
{
    ["apiKey"] = "00000000-0000-0000-0000-000000000000",
    ["n"] = 1
};

var rpcRequest = new JsonRpcRequest("generateUUIDs", 0, rpcParameters);

var rpcBindings = new Dictionary<JsonRpcId, JsonRpcMethodScheme>
{
    [0] = new JsonRpcMethodScheme(typeof(UuidsResult))
};

var jsonRequest = rpcSerializer.SerializeRequest(rpcRequest);
var httpRequestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
var httpResponse = await httpClient.PostAsync(rpcUri, httpRequestContent);
var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

var rpcData = rpcSerializer.DeserializeResponseData(jsonResponse, rpcBindings);
var rpcResult = (UuidsResult)rpcData.SingleItem.Message.Result;

Console.WriteLine("Random UUID: " + rpcResult.Random.Data[0]);
```