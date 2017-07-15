### Sample of communication with a server via JSON-RPC 2.0

Generating a few random numbers using [RANDOM.ORG](https://random.org) service.

1. Define types for request parameters and result objects:

```cs
class GenerateUuidsParams
{
    [JsonProperty("apiKey")]
    public string ApiKey { get; set; }

    [JsonProperty("n")]
    public int Count { get; set; }
}

class GenerateUuidsRandom
{
    [JsonProperty("data")]
    public Guid[] Data { get; set; }
}

class GenerateUuidsResult
{
    [JsonProperty("random")]
    public GenerateUuidsRandom Random { get; set; }
}
```

2. Send a request to the server:

```cs
var jrRequestParams = new GenerateUuidsParams
{
    ApiKey = "00000000-0000-0000-0000-000000000000", Count = 1
};

var jrRequest = new JsonRpcRequest("generateUUIDs", Guid.NewGuid().ToString(), jrRequestParams);

var jrBindings = new Dictionary<JsonRpcId, JsonRpcMethodScheme>
{
    [jrRequest.Id] = new JsonRpcMethodScheme(typeof(GenerateUuidsResult), typeof(object[]))
};

var jrSerializer = new JsonRpcSerializer();
var httpRequestString = jrSerializer.SerializeRequest(jrRequest);
var httpRequestContent = new StringContent(httpRequestString, Encoding.UTF8, "application/json");
var httpRequestUri = new Uri("https://api.random.org/json-rpc/1/invoke");
var httpResponse = await new HttpClient().PostAsync(httpRequestUri, httpRequestContent);
var httpResponseString = await httpResponse.Content.ReadAsStringAsync();
var jrResponsesData = jrSerializer.DeserializeResponsesData(httpResponseString, jrBindings);
var jrResponseResult = (GenerateUuidsResult)jrResponsesData.GetSingleItem().GetMessage().Result;

Console.WriteLine($"Random UUID: {jrResponseResult.Random.Data[0]}");
```