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
    APIKey = "00000000-0000-0000-0000-000000000000",
    Count = 5,
    MinimumValue = 0,
    MaximumValue = 100
};

var jrRequest = new JsonRpcRequest("generateIntegers", Guid.NewGuid().ToString(), jrRequestParams);

jrBindingsProvider.SetBinding(jrRequest.GetIdAsString(), "generateIntegers");

var httpRequestString = jrSerializer.SerializeRequest(jrRequest);
var httpRequestContent = new StringContent(httpRequestString, Encoding.UTF8, "application/json-rpc");
var httpRequestUri = new Uri("https://api.random.org/json-rpc/1/invoke");
var httpResponse = await new HttpClient().PostAsync(httpRequestUri, httpRequestContent);
var httpResponseString = await httpResponse.Content.ReadAsStringAsync();

var jrResponsesData = jrSerializer.DeserializeResponsesData(httpResponseString, jrBindingsProvider);
var jrResponseResult = (GenerateIntegersResult)jrResponsesData.GetSingleItem().GetMessage().Result;

Console.WriteLine($"Random Numbers: {string.Join(", ", jrResponseResult.RandomData.Data)}");
```

### Mapping exception type to response error type

Value of the `Type` property from the `JsonRpcException` exception throwed by `JsonRpcSerializer.DeserializeRequestsData` method can be successfully mapped to a `JsonRpcErrorType` value for an error to be sent back to a client:

`JsonRpcExceptionType` | `JsonRpcErrorType`
--- | ---
`ParseError` | `ParseError`
`GenericError` | `InternalError`
`InvalidMethod` | `InvalidMethod`
`InvalidMessage` | `InvalidRequest`

For example:

```cs
var jrError = new JsonRpcError((long)JsonRpcErrorType.ParseError, "...");
```

Standard messages for each response error can be found in the official specifications for the JSON-RPC.