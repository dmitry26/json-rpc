### Sample of communication with a server

Generating a few random numbers using [RANDOM.ORG](https://random.org) service.

1. Define types for request parameters ans result objects:

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
var jrSchema = new JsonRpcSchema();

jrSchema.ResultTypeBindings["generateIntegers"] = typeof(GenerateIntegersResult);

var jrSerializer = new JsonRpcSerializer(jrSchema);
var jrBindingsProvider = new JsonRpcBindingsProvider();

var jrRequestParams = new GenerateIntegersParams()
{
    APIKey = Guid.Empty.ToString(),
    Count = 5,
    MinimumValue = 0,
    MaximumValue = 100
};

var jrRequest = new JsonRpcRequest("generateIntegers", Guid.NewGuid().ToString(), jrRequestParams);
var jrRequestContent = new StringContent(jrSerializer.SerializeRequest(jrRequest), Encoding.UTF8, "application/json-rpc");

jrBindingsProvider.SetBinding(jrRequest.GetIdAsString(), "generateIntegers");

var httpResponse = await new HttpClient().PostAsync("https://api.random.org/json-rpc/1/invoke", jrRequestContent);
var httpResponseContent = await httpResponse.Content.ReadAsStringAsync();
var jrResponsesData = jrSerializer.DeserializeResponsesData(httpResponseContent, jrBindingsProvider);
var jrResponseResult = (GenerateIntegersResult)jrResponsesData.GetSingleItem().GetMessage().Result;

Console.WriteLine($"Random Numbers: {string.Join(", ", jrResponseResult.RandomData.Data)}"); 
```

### Mapping exception type to response error type

`JsonRpcExceptionType` | `JsonRpcErrorType`
--- | ---
`ParseError` | `ParseError`
`GenericError` | `InternalError`
`InvalidMethod` | `InvalidMethod`
`InvalidMessage` | `InvalidRequest`