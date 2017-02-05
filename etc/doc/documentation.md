### Mapping deserialization exception to response error code

`JsonRpcExceptionType` | `JsonRpcErrorType`
--- | ---
`ParseError` | `ParseError`
`GenericError` | `InternalError`
`InvalidMethod` | `InvalidMethod`
`InvalidMessage` | `InvalidRequest`