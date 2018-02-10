## System.Data.JsonRpc

Provides support for serializing and deserializing [JSON-RPC 2.0](http://www.jsonrpc.org/specification) messages.

[![NuGet package](https://img.shields.io/nuget/v/System.Data.JsonRpc.svg?style=flat-square)](https://www.nuget.org/packages/System.Data.JsonRpc)

### Features

- The serializer supports transparent usage of number and string message identifiers.
- The serializer supports dynamic response type contracts when result data type depends on method parameters.

### Specifics

The serializer is a stateful serializer. Due to nuances of the JSON-RPC protocol, deserializing a response requires more than only defining a response contract. Before deserializing a caller must specify a request identifier mapping to the corresponding method name (static bindings), or specify a request identifier mapping to the corresponding response contract (dynamic bindings). The serializer implements `IDisposable` interface for clearing all active bindings during disposing.

### Samples

Sample of communication with a JSON-RPC server:

```cs
var serializer = new JsonRpcSerializer();
var request = new JsonRpcRequest("sum", 1L, new[] { 1, 2 });
var requestJson = serializer.SerializeRequest(request);

// [Sending an HTTP request and storing a response string in the "responseJson"]

serializer.ResponseContracts["sum"] = new JsonRpcResponseContract(typeof(int));
serializer.StaticResponseBindings[request.Id] = "sum";

var responseData = serializer.DeserializeResponseData(responseJson);
var result = (int)responseData.SingleItem.Message.Result;
```

- Example of client-side usage: [Community.RandomOrg](https://github.com/alexanderkozlenko/random-org)
- Example of server-side usage: [Community.AspNetCore.JsonRpc](https://github.com/alexanderkozlenko/aspnetcore-json-rpc)