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

        private JsonRpcRequest ConvertJsonTokenToRequest(JObject jsonToken)
        {
            if (_compatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                if (!jsonToken.TryGetValue("jsonrpc", out var protocolToken) || (protocolToken.Type != JTokenType.String) || ((string)protocolToken != "2.0"))
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.request.protocol.invalid_property"));
                }
            }

            var requestId = default(JsonRpcId);

            if (jsonToken.TryGetValue("id", out var requestIdToken))
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

            if (!jsonToken.TryGetValue("method", out var requestMethodToken) || (requestMethodToken.Type != JTokenType.String))
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
                        if (!jsonToken.TryGetValue("params", out var requestParametersToken) || ((requestParametersToken.Type != JTokenType.Array) && (requestParametersToken.Type != JTokenType.Object)))
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
                        if (!jsonToken.TryGetValue("params", out var requestParametersToken) || ((requestParametersToken.Type != JTokenType.Array) && (requestParametersToken.Type != JTokenType.Object)))
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

        private JObject ConvertRequestToJsonToken(JsonRpcRequest request)
        {
            var jsonToken = new JObject();

            if (_compatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                jsonToken["jsonrpc"] = "2.0";
            }

            jsonToken["method"] = request.Method;

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

                        jsonToken["params"] = requestParametersArrayToken;
                    }
                    break;
                case JsonRpcParametersType.ByName:
                    {
                        if (_compatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.request.params.unsupported_structure"), request.Id);
                        }

                        var jsonParametersObjectToken = new JObject();

                        try
                        {
                            foreach (var kvp in request.ParametersByName)
                            {
                                jsonParametersObjectToken.Add(kvp.Key, kvp.Value != null ? JToken.FromObject(kvp.Value) : _nullJsonToken);
                            }
                        }
                        catch (JsonException e)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), request.Id, e);
                        }

                        jsonToken["params"] = jsonParametersObjectToken;
                    }
                    break;
                case JsonRpcParametersType.None:
                    {
                        if (_compatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                        {
                            jsonToken["params"] = new JArray();
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
                            jsonToken["id"] = _nullJsonToken;
                        }
                    }
                    break;
                case JsonRpcIdType.String:
                    {
                        jsonToken["id"] = new JValue((string)request.Id);
                    }
                    break;
                case JsonRpcIdType.Integer:
                    {
                        jsonToken["id"] = new JValue((long)request.Id);
                    }
                    break;
                case JsonRpcIdType.Float:
                    {
                        jsonToken["id"] = new JValue((double)request.Id);
                    }
                    break;
            }

            return jsonToken;
        }

        private JArray ConvertRequestsToJsonToken(IReadOnlyList<JsonRpcRequest> requests)
        {
            if (requests.Count == 0)
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.batch.empty"));
            }

            var requestArrayToken = new JArray();

            for (var i = 0; i < requests.Count; i++)
            {
                if (requests[i] == null)
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.batch.invalid_item"), i));
                }

                requestArrayToken.Add(ConvertRequestToJsonToken(requests[i]));
            }

            return requestArrayToken;
        }

        private JsonRpcData<JsonRpcRequest> ConvertJsonTokenToRequestData(JToken jsonToken)
        {
            switch (jsonToken.Type)
            {
                case JTokenType.Object:
                    {
                        var requestToken = (JObject)jsonToken;
                        var requestItem = default(JsonRpcItem<JsonRpcRequest>);

                        try
                        {
                            requestItem = new JsonRpcItem<JsonRpcRequest>(ConvertJsonTokenToRequest(requestToken));
                        }
                        catch (JsonRpcException e)
                            when (e.ErrorCode != JsonRpcErrorCodes.InvalidOperation)
                        {
                            requestItem = new JsonRpcItem<JsonRpcRequest>(e);
                        }

                        return new JsonRpcData<JsonRpcRequest>(requestItem);
                    }
                case JTokenType.Array:
                    {
                        var requestArrayToken = (JArray)jsonToken;

                        if (requestArrayToken.Count == 0)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.batch.empty"));
                        }

                        var requestItems = new JsonRpcItem<JsonRpcRequest>[requestArrayToken.Count];

                        for (var i = 0; i < requestItems.Length; i++)
                        {
                            var requestToken = requestArrayToken[i];

                            if (requestToken.Type != JTokenType.Object)
                            {
                                var exception = new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.batch.invalid_item"), i));

                                requestItems[i] = new JsonRpcItem<JsonRpcRequest>(exception);

                                continue;
                            }

                            var request = default(JsonRpcRequest);

                            try
                            {
                                request = ConvertJsonTokenToRequest((JObject)requestToken);
                            }
                            catch (JsonRpcException e)
                                when (e.ErrorCode != JsonRpcErrorCodes.InvalidOperation)
                            {
                                requestItems[i] = new JsonRpcItem<JsonRpcRequest>(e);

                                continue;
                            }

                            requestItems[i] = new JsonRpcItem<JsonRpcRequest>(request);
                        }

                        return new JsonRpcData<JsonRpcRequest>(requestItems);
                    }
                default:
                    {
                        throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.input.invalid_structure"));
                    }
            }
        }

        private JsonRpcResponse ConvertJsonTokenToResponse(JObject jsonToken)
        {
            if (_compatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                if (!jsonToken.TryGetValue("jsonrpc", out var protocolToken) || (protocolToken.Type != JTokenType.String) || ((string)protocolToken != "2.0"))
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.request.protocol.invalid_property"));
                }
            }

            var responseId = default(JsonRpcId);

            if (jsonToken.TryGetValue("id", out var responseIdToken))
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

            var responseResultToken = jsonToken.GetValue("result");
            var responseErrorToken = jsonToken.GetValue("error");
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

        private JObject ConvertResponseToJsonToken(JsonRpcResponse response)
        {
            var jsonToken = new JObject();

            if (_compatibilityLevel == JsonRpcCompatibilityLevel.Level2)
            {
                jsonToken["jsonrpc"] = "2.0";
            }

            if (response.Success)
            {
                var jsonResultToken = default(JToken);

                try
                {
                    jsonResultToken = JToken.FromObject(response.Result);
                }
                catch (JsonException e)
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), response.Id, e);
                }

                jsonToken["result"] = jsonResultToken;

                if (_compatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                {
                    jsonToken["error"] = _nullJsonToken;
                }
            }
            else
            {
                var jsonErrorToken = new JObject
                {
                    ["code"] = response.Error.Code,
                    ["message"] = response.Error.Message
                };

                if (response.Error.HasData)
                {
                    var jsonErrorDataToken = default(JToken);

                    try
                    {
                        jsonErrorDataToken = response.Error.Data != null ? JToken.FromObject(response.Error.Data) : _nullJsonToken;
                    }
                    catch (JsonException e)
                    {
                        throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation, Strings.GetString("core.serialize.json_issue"), response.Id, e);
                    }

                    jsonErrorToken["data"] = jsonErrorDataToken;
                }

                if (_compatibilityLevel != JsonRpcCompatibilityLevel.Level2)
                {
                    jsonToken["result"] = _nullJsonToken;
                }

                jsonToken["error"] = jsonErrorToken;
            }

            switch (response.Id.Type)
            {
                case JsonRpcIdType.None:
                    {
                        jsonToken["id"] = _nullJsonToken;
                    }
                    break;
                case JsonRpcIdType.String:
                    {
                        jsonToken["id"] = new JValue((string)response.Id);
                    }
                    break;
                case JsonRpcIdType.Integer:
                    {
                        jsonToken["id"] = new JValue((long)response.Id);
                    }
                    break;
                case JsonRpcIdType.Float:
                    {
                        jsonToken["id"] = new JValue((double)response.Id);
                    }
                    break;
            }

            return jsonToken;
        }

        private JArray ConvertResponsesToJsonToken(IReadOnlyList<JsonRpcResponse> responses)
        {
            if (responses.Count == 0)
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.batch.empty"));
            }

            var responseArrayToken = new JArray();

            for (var i = 0; i < responses.Count; i++)
            {
                if (responses[i] == null)
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.batch.invalid_item"), i));
                }

                responseArrayToken.Add(ConvertResponseToJsonToken(responses[i]));
            }

            return responseArrayToken;
        }

        private JsonRpcData<JsonRpcResponse> ConvertJsonTokenToResponseData(JToken jsonToken)
        {
            switch (jsonToken.Type)
            {
                case JTokenType.Object:
                    {
                        var responseToken = (JObject)jsonToken;
                        var responseItem = default(JsonRpcItem<JsonRpcResponse>);

                        try
                        {
                            responseItem = new JsonRpcItem<JsonRpcResponse>(ConvertJsonTokenToResponse(responseToken));
                        }
                        catch (JsonRpcException e)
                            when (e.ErrorCode != JsonRpcErrorCodes.InvalidOperation)
                        {
                            responseItem = new JsonRpcItem<JsonRpcResponse>(e);
                        }

                        return new JsonRpcData<JsonRpcResponse>(responseItem);
                    }
                case JTokenType.Array:
                    {
                        var responseArrayToken = (JArray)jsonToken;

                        if (responseArrayToken.Count == 0)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.batch.empty"));
                        }

                        var responseItems = new JsonRpcItem<JsonRpcResponse>[responseArrayToken.Count];

                        for (var i = 0; i < responseItems.Length; i++)
                        {
                            var responseToken = responseArrayToken[i];

                            if (responseToken.Type != JTokenType.Object)
                            {
                                var exception = new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, string.Format(CultureInfo.InvariantCulture, Strings.GetString("core.batch.invalid_item"), i));

                                responseItems[i] = new JsonRpcItem<JsonRpcResponse>(exception);

                                continue;
                            }

                            var response = default(JsonRpcResponse);

                            try
                            {
                                response = ConvertJsonTokenToResponse((JObject)responseToken);
                            }
                            catch (JsonRpcException e)
                                when (e.ErrorCode != JsonRpcErrorCodes.InvalidOperation)
                            {
                                responseItems[i] = new JsonRpcItem<JsonRpcResponse>(e);

                                continue;
                            }

                            responseItems[i] = new JsonRpcItem<JsonRpcResponse>(response);
                        }

                        return new JsonRpcData<JsonRpcResponse>(responseItems);
                    }
                default:
                    {
                        throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage, Strings.GetString("core.deserialize.input.invalid_structure"));
                    }
            }
        }
    }
}