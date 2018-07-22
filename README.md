## System.Data.JsonRpc

Provides support for serializing and deserializing [JSON-RPC 2.0](http://www.jsonrpc.org/specification) messages.

[![NuGet package](https://img.shields.io/nuget/v/System.Data.JsonRpc.svg?style=flat-square)](https://www.nuget.org/packages/System.Data.JsonRpc)

### Changes

- Ported to **Utf8Json** serializer
- Added IJsonRpcSerializer interface

### Benchmarks

`Utf8Json Serialize:` (~5X faster)

|                               Method |       Mean |     Error |    StdDev |  Gen 0 | Allocated |
|-------------------------------------:|-----------:|----------:|----------:|-------:|----------:|
|           ParamsNoneSerializeRequest |   554.3 ns | 10.908 ns | 13.396 ns | 0.0315 |     136 B |
|      BatchParamsNoneSerializeRequest |   638.4 ns | 12.849 ns | 13.195 ns | 0.0401 |     168 B |
|         ParamsByNameSerializeRequest | 1,094.3 ns | 21.353 ns | 29.934 ns | 0.0515 |     224 B |
|    BatchParamsByNameSerializeRequest | 1,282.9 ns | 24.419 ns | 25.077 ns | 0.0610 |     264 B |
|     ParamsByPositionSerializeRequest |   880.6 ns | 17.248 ns | 21.813 ns | 0.0448 |     192 B |
|BatchParamsByPositionSerializeRequest | 1,110.7 ns | 21.484 ns | 24.741 ns | 0.0534 |     232 B |
|             SuccessSerializeResponse |   645.4 ns | 12.894 ns | 18.076 ns | 0.0362 |     152 B |
|        BatchSuccessSerializeResponse |   795.2 ns | 15.328 ns | 16.401 ns | 0.0448 |     192 B |
|               ErrorSerializeResponse |   704.7 ns |  1.384 ns |  1.080 ns | 0.0467 |     200 B |
|          BatchErrorSerializeResponse |   794.9 ns | 15.883 ns | 21.741 ns | 0.0544 |     232 B |
|       ErrorWithDataSerializeResponse |   936.0 ns | 21.773 ns | 22.360 ns | 0.0515 |     216 B |
|  BatchErrorWithDataSerializeResponse | 1,080.6 ns | 21.422 ns | 27.091 ns | 0.0591 |     256 B |


`Newtonsoft Serialize:`


|                               Method |       Mean |     Error |    StdDev |  Gen 0 | Allocated |
|-------------------------------------:|-----------:|----------:|----------:|-------:|----------:|
|           ParamsNoneSerializeRequest | 2,296.3 ns |  43.86 ns |  50.51 ns | 0.3929 |    1656 B |
|      BatchParamsNoneSerializeRequest | 2,975.7 ns |  58.86 ns |  80.57 ns | 0.4768 |    2008 B |
|         ParamsByNameSerializeRequest | 4,775.9 ns |  50.23 ns |  41.94 ns | 0.8163 |    3432 B |
|    BatchParamsByNameSerializeRequest | 5,342.0 ns | 105.38 ns | 151.13 ns | 0.8621 |    3632 B |
|     ParamsByPositionSerializeRequest | 4,325.3 ns |  24.32 ns |  18.99 ns | 0.7019 |    2968 B |
|BatchParamsByPositionSerializeRequest | 4,895.5 ns |  97.46 ns | 119.69 ns | 0.7477 |    3168 B |
|             SuccessSerializeResponse | 2,997.0 ns |  58.19 ns |  77.68 ns | 0.5074 |    2144 B |
|        BatchSuccessSerializeResponse | 3,631.2 ns |  71.16 ns |  84.71 ns | 0.5951 |    2504 B |
|               ErrorSerializeResponse | 3,666.3 ns |  70.89 ns |  78.80 ns | 0.6332 |    2664 B |
|          BatchErrorSerializeResponse | 4,267.9 ns |  85.41 ns | 130.44 ns | 0.6790 |    2856 B |
|       ErrorWithDataSerializeResponse | 4,998.0 ns |  97.10 ns |  86.07 ns | 0.8469 |    3568 B |
|  BatchErrorWithDataSerializeResponse | 5,615.0 ns | 112.25 ns | 141.96 ns | 0.8926 |    3768 B |


`Utf8Json Deserialize:` (~3X faster)

|                                     Method |       Mean |    Error |   StdDev | Gen 0  | Allocated |
|-------------------------------------------:|-----------:|---------:|---------:|-------:|----------:|
|           ParamsNoneDeserializeRequestData | 1,457.6 ns | 28.49 ns | 39.00 ns | 0.1087 |     456 B |
|      BatchParamsNoneDeserializeRequestData | 1,548.0 ns | 30.75 ns | 37.77 ns | 0.1354 |     568 B |
|         ParamsByNameDeserializeRequestData | 2,690.6 ns | 52.88 ns | 80.76 ns | 0.1907 |     808 B |
|    BatchParamsByNameDeserializeRequestData | 2,954.1 ns | 57.25 ns | 83.92 ns | 0.2213 |     936 B |
|     ParamsByPositionDeserializeRequestData | 2,419.7 ns | 47.55 ns | 66.66 ns | 0.1526 |     648 B |
|BatchParamsByPositionDeserializeRequestData | 2,514.3 ns | 49.50 ns | 72.55 ns | 0.1831 |     776 B |
|               ErrorDeserializeResponseData | 2,357.9 ns | 46.63 ns | 71.21 ns | 0.1793 |     752 B |
|          BatchErrorDeserializeResponseData | 2,618.3 ns | 50.72 ns | 52.08 ns | 0.2060 |     880 B |
|       ErrorWithDataDeserializeResponseData | 3,038.2 ns | 59.69 ns | 91.15 ns | 0.1945 |     832 B |
|  BatchErrorWithDataDeserializeResponseData | 3,161.1 ns | 63.30 ns | 90.78 ns | 0.2289 |     968 B |
|             SuccessDeserializeResponseData | 1,661.2 ns | 32.44 ns | 39.84 ns | 0.1202 |     504 B |
|        BatchSuccessDeserializeResponseData | 1,769.5 ns | 35.23 ns | 48.22 ns | 0.1450 |     616 B |


`Newtonsoft Deserialize:`

|                                     Method |       Mean |     Error |    StdDev |  Gen 0 | Allocated |
|-------------------------------------------:|-----------:|----------:|----------:|-------:|----------:|
|           ParamsNoneDeserializeRequestData | 4,100.5 ns |  79.37 ns |  97.47 ns | 0.4120 |    1760 B |
|      BatchParamsNoneDeserializeRequestData | 4,692.4 ns |  93.13 ns | 114.37 ns | 0.5112 |    2176 B |
|         ParamsByNameDeserializeRequestData | 7,278.4 ns | 127.77 ns | 119.51 ns | 0.8163 |    3440 B |
|    BatchParamsByNameDeserializeRequestData | 7,740.2 ns | 149.08 ns | 165.70 ns | 0.8774 |    3696 B |
|     ParamsByPositionDeserializeRequestData | 6,465.1 ns | 122.55 ns | 120.36 ns | 0.6485 |    2744 B |
|BatchParamsByPositionDeserializeRequestData | 6,931.2 ns | 134.91 ns | 144.36 ns | 0.7095 |    3000 B |
|               ErrorDeserializeResponseData | 6,544.6 ns | 129.48 ns | 172.86 ns | 0.6943 |    2936 B |
|          BatchErrorDeserializeResponseData | 7,211.3 ns | 135.57 ns | 133.15 ns | 0.7553 |    3192 B |
|       ErrorWithDataDeserializeResponseData | 7,781.4 ns | 154.15 ns | 171.34 ns | 0.7629 |    3264 B |
|  BatchErrorWithDataDeserializeResponseData | 8,505.8 ns | 167.44 ns | 234.73 ns | 0.8240 |    3520 B |
|             SuccessDeserializeResponseData | 4,458.0 ns |  89.20 ns | 133.51 ns | 0.4120 |    1760 B |
|        BatchSuccessDeserializeResponseData | 4,987.0 ns |  53.93 ns |  45.03 ns | 0.5112 |    2176 B |

### Features

- The serializer supports transparent usage of number and string message identifiers.
- The serializer supports dynamic response type contracts when result data type depends on method parameters.
- The serializer supports serializing to and deserializing from streams.

### Specifics

The serializer is stateful. Due to the JSON-RPC protocol nuances, deserializing a response requires more than only defining a response contract. Before deserializing a caller must specify a request identifier mapping to the corresponding method name (static bindings), or specify a request identifier mapping to the corresponding response contract (dynamic bindings). The serializer supports disposing for clearing all active bindings.

### Compatibility

As recommended by the specification, the serializer provides backward compatibility for [JSON-RPC 1.0](http://www.jsonrpc.org/specification_v1) messages, limited to the intersection of JSON-RPC 1.0 and JSON-RPC 2.0 requirements and the API. As an example, the serializer can be used for serializing and deserializing Bitcoin protocol messages, according to the ["Bitcoin Core APIs - Remote Procedure Calls"](https://bitcoin.org/en/developer-reference#remote-procedure-calls-rpcs) documentation. To enable JSON-RPC 1.0 compatibility, a caller must change the compatibility level on a serializer instance.

### Examples

- Communication with a JSON-RPC 2.0 server:

```cs
var serializer = new JsonRpcSerializer();
var request = new JsonRpcRequest("sum", 1L, new[] { 1L, 2L });
var requestString = serializer.SerializeRequest(request);

// [Sending an HTTP request and storing a response string in the "responseString"]

serializer.ResponseContracts["sum"] = new JsonRpcResponseContract(typeof(int));
serializer.StaticResponseBindings[request.Id] = "sum";

var responseData = serializer.DeserializeResponseData(responseString);
var result = (int)responseData.Item.Message.Result;
```

- Example of client-side usage: https://github.com/alexanderkozlenko/json-rpc-client
- Example of server-side usage: https://github.com/alexanderkozlenko/aspnetcore-json-rpc