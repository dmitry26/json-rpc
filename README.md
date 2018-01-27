## System.Data.JsonRpc

Provides support for serialization and deserialization of [JSON-RPC 2.0](http://www.jsonrpc.org/specification) messages.

[![NuGet package](https://img.shields.io/nuget/v/System.Data.JsonRpc.svg?style=flat-square)](https://www.nuget.org/packages/System.Data.JsonRpc)

Due to nuances of JSON-RPC protocol, deserialization of responses requires a bit more information than just specifying strongly-typed contracts for deserialization of result object. Before deserializing of responses a caller must specify bindings of used message identifiers to the corresponding methods (static bindings), or specify direct bindings of used message identifiers to the corresponding response contracts (dynamic bindings) if the type of result object depends on parameters. That's why the serializer implements `IDisposable` interface and must be considered as a steteful serializer.

```cs
var httpClient = new HttpClient();
var serviceUri = new Uri("https://...");
var serializer = new JsonRpcSerializer();

serializer.ResponseContracts["sum"] = new JsonRpcResponseContract(typeof(int));

var rpcParameters = new[] { 1, 2 };
var rpcRequest = new JsonRpcRequest("sum", 1L, rpcParameters);
var jsonRequest = serializer.SerializeRequest(rpcRequest);

var httpRequestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
var httpResponse = await httpClient.PostAsync(serviceUri, httpRequestContent);
var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

serializer.StaticResponseBindings[rpcRequest.Id] = "sum";

var rpcData = serializer.DeserializeResponseData(jsonResponse);
var rpcResult = (int)rpcData.SingleItem.Message.Result;

Console.WriteLine(rpcResult);
```

- Sample of JSON-RPC client: [Community.RandomOrg](https://github.com/alexanderkozlenko/random-org)
- Sample of JSON-RPC server: [Community.AspNetCore.JsonRpc](https://github.com/alexanderkozlenko/aspnetcore-json-rpc)