### Sample of communication with a server

Generating a few random numbers using [RANDOM.ORG](https://random.org) service.

1. Define types for request parameters and result objects:

```cs
class GenerateUUIDsParams
{
    [JsonProperty("apiKey")]
    public string APIKey { get; set; }

    [JsonProperty("n")]
    public int Count { get; set; }
}

class GenerateUUIDsRandomData
{
    [JsonProperty("data")]
    public string[] Data { get; set; }
}

class GenerateUUIDsResult
{
    [JsonProperty("random")]
    public GenerateUUIDsRandomData RandomData { get; set; }
}
```

2. Send a request to the server:

```cs
var jrScheme = new JsonRpcSerializerScheme();

jrScheme.GenericErrorDataType = typeof(object[]);

jrScheme.Methods["generateUUIDs"] = new JsonRpcMethodScheme(typeof(GenerateUUIDsResult), typeof(object[]));

var jrSerializer = new JsonRpcSerializer(jrScheme);
var jrBindings = new Dictionary<JsonRpcId, string>();

var jrRequestParams = new GenerateUUIDsParams
{
    APIKey = "00000000-0000-0000-0000-000000000000",
    Count = 1
};

var jrRequest = new JsonRpcRequest("generateUUIDs", Guid.NewGuid().ToString(), jrRequestParams);

jrBindings[jrRequest.Id] = jrRequest.Method;

var httpRequestString = jrSerializer.SerializeRequest(jrRequest);
var httpRequestContent = new StringContent(httpRequestString, Encoding.UTF8, "application/json");
var httpRequestUri = new Uri("https://api.random.org/json-rpc/1/invoke");
var httpResponse = await new HttpClient().PostAsync(httpRequestUri, httpRequestContent);
var httpResponseString = await httpResponse.Content.ReadAsStringAsync();

var jrResponsesData = jrSerializer.DeserializeResponsesData(httpResponseString, jrBindings);
var jrResponseResult = (GenerateUUIDsResult)jrResponsesData.GetSingleItem().GetMessage().Result;

Console.WriteLine($"Random UUID: {jrResponseResult.RandomData.Data[0]}");
```