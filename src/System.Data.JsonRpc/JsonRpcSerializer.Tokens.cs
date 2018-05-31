using System.Collections.Generic;
using System.Data.JsonRpc.Resources;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace System.Data.JsonRpc
{
    public partial class JsonRpcSerializer
    {
        private static readonly JValue _nullJsonToken = JValue.CreateNull();

        private JsonRpcRequest ConvertTokenToRequest(JObject requestToken)
        {
            if (_compatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                if (!requestToken.TryGetValue("jsonrpc", out var protocolToken) || (protocolToken.Type != JTokenType.String) || ((string)protocolToken != "2.0"))
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.request.protocol.invalid_property"));
                }
            }

            var requestId = default(JsonRpcId);

            if (requestToken.TryGetValue("id", out var requestIdToken))
            {
                switch (requestIdToken.Type)
                {
                    case JTokenType.Null:
                        {
                        }
                        break;
                    case JTokenType.String:
                        {
                            requestId = (string)requestIdToken;
                        }
                        break;
                    case JTokenType.Integer:
                        {
                            try
                            {
                                requestId = (long)requestIdToken;
                            }
                            catch (OverflowException e)
                            {
                                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.message.id.large_number"), default, e);
                            }
                        }
                        break;
                    case JTokenType.Float:
                        {
                            try
                            {
                                requestId = (double)requestIdToken;
                            }
                            catch (OverflowException e)
                            {
                                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.message.id.large_number"), default, e);
                            }
                        }
                        break;
                    default:
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.request.id.invalid_property"));
                        }
                }
            }

            if (!requestToken.TryGetValue("method", out var requestMethodToken) || (requestMethodToken.Type != JTokenType.String))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.request.method.invalid_property"), requestId);
            }

            var requestMethod = (string)requestMethodToken;

            if (!_requestContracts.TryGetValue(requestMethod, out var contract))
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMethod, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.request.method.unsupported"), requestMethod), requestId);
            }
            if (contract == null)
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.request.method.contract.undefined"), requestMethod), requestId);
            }

            switch (contract.ParametersType)
            {
                case JsonRpcParametersType.ByPosition:
                    {
                        if (contract.ParametersByPosition.Count == 0)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("request.contract.params.invalid_count"), requestId);
                        }
                        if (!requestToken.TryGetValue("params", out var requestParametersToken) || ((requestParametersToken.Type != JTokenType.Array) && (requestParametersToken.Type != JTokenType.Object)))
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.request.params.invalid_property"), requestId);
                        }
                        if (requestParametersToken.Type != JTokenType.Array)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidParameters, Strings.GetString("core.deserialize.request.params.invalid_structure"), requestId);
                        }

                        var requestParametersArrayToken = (JArray)requestParametersToken;

                        if (requestParametersArrayToken.Count < contract.ParametersByPosition.Count)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidParameters, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.deserialize.request.params.invalid_count"), requestParametersArrayToken.Count), requestId);
                        }

                        var requestParameters = new object[contract.ParametersByPosition.Count];

                        try
                        {
                            for (var i = 0; i < requestParameters.Length; i++)
                            {
                                requestParameters[i] = requestParametersArrayToken[i].ToObject(contract.ParametersByPosition[i]);
                            }
                        }
                        catch (Exception e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.deserialize.json_issue"), requestId, e);
                        }

                        return new JsonRpcRequest(requestMethod, requestId, requestParameters);
                    }
                case JsonRpcParametersType.ByName:
                    {
                        if (contract.ParametersByName.Count == 0)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("request.contract.params.invalid_count"), requestId);
                        }
                        if (!requestToken.TryGetValue("params", out var requestParametersToken) || ((requestParametersToken.Type != JTokenType.Array) && (requestParametersToken.Type != JTokenType.Object)))
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.request.params.invalid_property"), requestId);
                        }
                        if (requestParametersToken.Type != JTokenType.Object)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidParameters, Strings.GetString("core.deserialize.request.params.invalid_structure"), requestId);
                        }

                        var requestParametersObjectToken = (JObject)requestParametersToken;
                        var requestParameters = new Dictionary<string, object>(contract.ParametersByName.Count, StringComparer.Ordinal);

                        try
                        {
                            foreach (var kvp in contract.ParametersByName)
                            {
                                if (!requestParametersObjectToken.TryGetValue(kvp.Key, StringComparison.Ordinal, out var requestParameterToken))
                                {
                                    continue;
                                }

                                requestParameters[kvp.Key] = requestParameterToken.ToObject(kvp.Value);
                            }
                        }
                        catch (Exception e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.deserialize.json_issue"), requestId, e);
                        }

                        return new JsonRpcRequest(requestMethod, requestId, requestParameters);
                    }
                default:
                    {
                        return new JsonRpcRequest(requestMethod, requestId);
                    }
            }
        }

        private JObject ConvertRequestToToken(JsonRpcRequest request)
        {
            var requestToken = new JObject();

            if (_compatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                requestToken["jsonrpc"] = "2.0";
            }

            requestToken["method"] = request.Method;

            switch (request.ParametersType)
            {
                case JsonRpcParametersType.ByPosition:
                    {
                        if (request.ParametersByPosition.Count == 0)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("request.params.invalid_count"), request.Id);
                        }

                        var requestParametersArrayToken = new JArray();

                        try
                        {
                            for (var i = 0; i < request.ParametersByPosition.Count; i++)
                            {
                                requestParametersArrayToken.Add(request.ParametersByPosition[i] != null ? JToken.FromObject(request.ParametersByPosition[i]) : _nullJsonToken);
                            }
                        }
                        catch (JsonException e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), request.Id, e);
                        }

                        requestToken["params"] = requestParametersArrayToken;
                    }
                    break;
                case JsonRpcParametersType.ByName:
                    {
                        if (_compatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.request.params.unsupported_structure"), request.Id);
                        }

                        var requestParametersObjectToken = new JObject();

                        try
                        {
                            foreach (var kvp in request.ParametersByName)
                            {
                                requestParametersObjectToken.Add(kvp.Key, kvp.Value != null ? JToken.FromObject(kvp.Value) : _nullJsonToken);
                            }
                        }
                        catch (JsonException e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), request.Id, e);
                        }

                        requestToken["params"] = requestParametersObjectToken;
                    }
                    break;
                case JsonRpcParametersType.None:
                    {
                        if (_compatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                        {
                            requestToken["params"] = new JArray();
                        }
                    }
                    break;
            }

            switch (request.Id.Type)
            {
                case JsonRpcIdType.None:
                    {
                        if (_compatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                        {
                            requestToken["id"] = _nullJsonToken;
                        }
                    }
                    break;
                case JsonRpcIdType.String:
                    {
                        requestToken["id"] = new JValue((string)request.Id);
                    }
                    break;
                case JsonRpcIdType.Integer:
                    {
                        requestToken["id"] = new JValue((long)request.Id);
                    }
                    break;
                case JsonRpcIdType.Float:
                    {
                        requestToken["id"] = new JValue((double)request.Id);
                    }
                    break;
            }

            return requestToken;
        }

        private JsonRpcResponse ConvertTokenToResponse(JObject responseObject)
        {
            if (_compatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                if (!responseObject.TryGetValue("jsonrpc", out var protocolToken) || (protocolToken.Type != JTokenType.String) || ((string)protocolToken != "2.0"))
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.request.protocol.invalid_property"));
                }
            }

            var responseId = default(JsonRpcId);

            if (responseObject.TryGetValue("id", out var responseIdToken))
            {
                switch (responseIdToken.Type)
                {
                    case JTokenType.Null:
                        {
                        }
                        break;
                    case JTokenType.String:
                        {
                            responseId = (string)responseIdToken;
                        }
                        break;
                    case JTokenType.Integer:
                        {
                            try
                            {
                                responseId = (long)responseIdToken;
                            }
                            catch (OverflowException e)
                            {
                                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.message.id.large_number"), default, e);
                            }
                        }
                        break;
                    case JTokenType.Float:
                        {
                            try
                            {
                                responseId = (double)responseIdToken;
                            }
                            catch (OverflowException e)
                            {
                                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.message.id.large_number"), default, e);
                            }
                        }
                        break;
                    default:
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.id.invalid_property"));
                        }
                }
            }

            var responseResultToken = responseObject.GetValue("result");
            var responseErrorToken = responseObject.GetValue("error");
            var responseSuccess = false;

            if (_compatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                if (((responseResultToken == null) && (responseErrorToken == null)) || ((responseResultToken != null) && (responseErrorToken != null)))
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.invalid_properties"), responseId);
                }

                responseSuccess = responseErrorToken == null;
            }
            else
            {
                if ((responseResultToken == null) || (responseErrorToken == null))
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.invalid_properties"), responseId);
                }

                responseSuccess = responseErrorToken.Type == JTokenType.Null;
            }

            if (responseSuccess)
            {
                if (responseId.Type == JsonRpcIdType.None)
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.invalid_properties"), responseId);
                }

                var contract = GetResponseContract(responseId);
                var responseResult = default(object);

                if (contract.ResultType != null)
                {
                    try
                    {
                        responseResult = responseResultToken.ToObject(contract.ResultType);
                    }
                    catch (Exception e)
                    {
                        throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.deserialize.json_issue"), responseId, e);
                    }
                }

                return new JsonRpcResponse(responseResult, responseId);
            }
            else
            {
                if (responseErrorToken.Type == JTokenType.Object)
                {
                    var responseErrorObjectToken = (JObject)responseErrorToken;
                    var responseErrorCode = default(long);

                    if (responseErrorObjectToken.TryGetValue("code", out var responseErrorCodeToken) && (responseErrorCodeToken.Type == JTokenType.Integer))
                    {
                        try
                        {
                            responseErrorCode = (long)responseErrorCodeToken;
                        }
                        catch (OverflowException e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.error.code.large_number"), responseId, e);
                        }
                    }
                    else
                    {
                        if (_compatibilityLevel == JsonRpcCompatibilityLevel.Level2)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.error.code.invalid_property"), responseId);
                        }
                    }

                    var responseErrorMessage = default(string);

                    if (responseErrorObjectToken.TryGetValue("message", out var responseErrorMessageToken) && (responseErrorMessageToken.Type == JTokenType.String))
                    {
                        responseErrorMessage = (string)responseErrorMessageToken;
                    }
                    else
                    {
                        if (_compatibilityLevel == JsonRpcCompatibilityLevel.Level2)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.error.message.invalid_property"), responseId);
                        }
                        else
                        {
                            responseErrorMessage = string.Empty;
                        }
                    }

                    var responseError = default(JsonRpcError);

                    if (responseErrorObjectToken.TryGetValue("data", out var responseErrorDataToken))
                    {
                        var responseErrorDataType = default(Type);

                        if (responseId.Type == JsonRpcIdType.None)
                        {
                            responseErrorDataType = _defaultErrorDataType;
                        }
                        else
                        {
                            var contract = GetResponseContract(responseId);

                            responseErrorDataType = contract.ErrorDataType;
                        }

                        var responseErrorData = default(object);

                        if (responseErrorDataType != null)
                        {
                            try
                            {
                                responseErrorData = responseErrorDataToken.ToObject(responseErrorDataType);
                            }
                            catch (Exception e)
                            {
                                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.deserialize.json_issue"), responseId, e);
                            }
                        }

                        try
                        {
                            responseError = new JsonRpcError(responseErrorCode, responseErrorMessage, responseErrorData);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.error.code.invalid_range"), responseId, e);
                        }
                    }
                    else
                    {
                        try
                        {
                            responseError = new JsonRpcError(responseErrorCode, responseErrorMessage);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.error.code.invalid_range"), responseId, e);
                        }
                    }

                    return new JsonRpcResponse(responseError, responseId);
                }
                else
                {
                    if (_compatibilityLevel == JsonRpcCompatibilityLevel.Level2)
                    {
                        throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.response.error.invalid_type"), responseId);
                    }
                    else
                    {
                        return new JsonRpcResponse(new JsonRpcError(default, string.Empty), responseId);
                    }
                }
            }
        }

        private JObject ConvertResponseToToken(JsonRpcResponse response)
        {
            var responseToken = new JObject();

            if (_compatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                responseToken["jsonrpc"] = "2.0";
            }

            if (response.Success)
            {
                var resultToken = default(JToken);

                try
                {
                    resultToken = JToken.FromObject(response.Result);
                }
                catch (JsonException e)
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), response.Id, e);
                }

                responseToken["result"] = resultToken;

                if (_compatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                {
                    responseToken["error"] = _nullJsonToken;
                }
            }
            else
            {
                var responseErrorToken = new JObject
                {
                    ["code"] = response.Error.Code,
                    ["message"] = response.Error.Message
                };

                if (response.Error.HasData)
                {
                    var responseErrorDataToken = default(JToken);

                    try
                    {
                        responseErrorDataToken = response.Error.Data != null ? JToken.FromObject(response.Error.Data) : _nullJsonToken;
                    }
                    catch (JsonException e)
                    {
                        throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), response.Id, e);
                    }

                    responseErrorToken["data"] = responseErrorDataToken;
                }

                if (_compatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                {
                    responseToken["result"] = _nullJsonToken;
                }

                responseToken["error"] = responseErrorToken;
            }

            switch (response.Id.Type)
            {
                case JsonRpcIdType.None:
                    {
                        responseToken["id"] = _nullJsonToken;
                    }
                    break;
                case JsonRpcIdType.String:
                    {
                        responseToken["id"] = new JValue((string)response.Id);
                    }
                    break;
                case JsonRpcIdType.Integer:
                    {
                        responseToken["id"] = new JValue((long)response.Id);
                    }
                    break;
                case JsonRpcIdType.Float:
                    {
                        responseToken["id"] = new JValue((double)response.Id);
                    }
                    break;
            }

            return responseToken;
        }
    }
}