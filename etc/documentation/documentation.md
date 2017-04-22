### Sample of communication with a server

Generating a few random numbers using [RANDOM.ORG](https://random.org) service.

1. Define types for request parameters and result objects:

```cs
[JsonObject(MemberSerialization.OptIn)]
class GenerateIntegersParams
{
    [JsonProperty("apiKey")]
    public string APIKey { get; set; }

    [JsonProperty("n")]
    public int Count { get; set; }

    [JsonProperty("min")]
    public int MinimumValue { get; set; }

    [JsonProperty("max")]
    public int MaximumValue { get; set; }
}

[JsonObject(MemberSerialization.OptIn)]
class GenerateIntegersRandomData
{
    [JsonProperty("data")]
    public int[] Data { get; set; }
}

[JsonObject(MemberSerialization.OptIn)]
class GenerateIntegersResult
{
    [JsonProperty("random")]
    public GenerateIntegersRandomData RandomData { get; set; }
}
```

2. Send a request to the server:

```cs
var jrSchema = new JsonRpcSerializerScheme();

jrSchema.Methods["generateIntegers"] = new JsonRpcMethodScheme(typeof(GenerateIntegersResult), typeof(object[]));

var jrSerializer = new JsonRpcSerializer(jrSchema);
var jrBindings = new Dictionary<JsonRpcId, string>();

var jrRequestParams = new GenerateIntegersParams()
{
    APIKey = "00000000-0000-0000-0000-000000000000",
    Count = 5,
    MinimumValue = 0,
    MaximumValue = 100
};

var jrRequest = new JsonRpcRequest("generateIntegers", Guid.NewGuid().ToString(), jrRequestParams);

jrBindings[jrRequest.Id] = jrRequest.Method;

var httpRequestString = jrSerializer.SerializeRequest(jrRequest);
var httpRequestContent = new StringContent(httpRequestString, Encoding.UTF8, "application/json-rpc");
var httpRequestUri = new Uri("https://api.random.org/json-rpc/1/invoke");
var httpResponse = await new HttpClient().PostAsync(httpRequestUri, httpRequestContent);
var httpResponseString = await httpResponse.Content.ReadAsStringAsync();

var jrResponsesData = jrSerializer.DeserializeResponsesData(httpResponseString, jrBindings);
var jrResponseResult = (GenerateIntegersResult)jrResponsesData.GetSingleItem().GetMessage().Result;

Console.WriteLine($"Random Numbers: {string.Join(", ", jrResponseResult.RandomData.Data)}");
```