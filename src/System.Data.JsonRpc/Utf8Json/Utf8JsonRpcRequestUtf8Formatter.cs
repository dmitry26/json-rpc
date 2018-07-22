// Copyright (c) DMO Consulting LLC. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.JsonRpc.Resources;
using Utf8Json;
using System.Resources;
using System.Reflection;
using Utf8Json.Internal;
using System.Globalization;
using System.Linq;

namespace System.Data.JsonRpc
{
    using static Utf8JsonRpcFormatterHelper;

    internal class Utf8JsonRpcRequestFormatterV2 : IJsonFormatter<JsonRpcRequest>
    {
        public void Serialize(ref JsonWriter writer,JsonRpcRequest req,IJsonFormatterResolver formatterResolver)
        {
            _ = req ?? throw new ArgumentNullException(nameof(req));

            writer.WriteBeginObject();

            writer.WritePropertyName("jsonrpc");
            writer.WriteString("2.0");

            writer.WriteValueSeparator();
            writer.WritePropertyName("method");
            writer.WriteString(req.Method);

            if (req.ParametersType != JsonRpcParametersType.None)
            {
                writer.WriteValueSeparator();
                WriteRpcParams(ref writer,req);
            }

            if (req.Id.Type != JsonRpcIdType.None)
            {
                writer.WriteValueSeparator();
                WriteRpcId(ref writer,req.Id);
            }

            writer.WriteEndObject();
        }

        public JsonRpcRequest Deserialize(ref JsonReader reader,IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
                return null;

            if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
            {
                try { reader.ReadNextBlock(); } catch { }

                throw new JsonRpcInnerException(JsonRpcInnerErrorCode.InvalidMessage);
            }

            var count = 0;
            var method = "";
            object @params = null;
            ArraySegment<byte>? paramSeg = null;
            JsonRpcId id = default;
            Exception methodExc = null;

            while (reader.ReadIsInObject(ref count))
            {
                var name = reader.ReadPropertyName();

                switch (name)
                {
                    case "jsonrpc":
                        if (reader.ReadString() != "2.0")
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.request.protocol.invalid_property"));
                        break;
                    case "method":
                        try
                        {
                            method = reader.ReadString();
                        }
                        catch (Exception x)
                        {
                            methodExc = x;
                        }
                        break;
                    case "params":
                        paramSeg = reader.ReadNextBlockSegment();
                        break;
                    case "id":
                        try
                        {
                            id = ReadRpcId(ref reader,formatterResolver);
                        }
                        catch (JsonRpcInnerException)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.request.id.invalid_property"));
                        }
                        break;
                    default:
                        reader.ReadNext();
                        break;
                }
            }

            if (methodExc != null)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.request.method.invalid_property"),id);

            var jsonRpcFormatterResolver = formatterResolver as JsonRpcRequestFormatterResolver;

            if (!jsonRpcFormatterResolver.TryGetContract(method,out var contract))
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMethod,string.Format(CultureInfo.InvariantCulture,Strings.GetString("core.deserialize.request.method.unsupported"),method),id);

            if (contract == null)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,string.Format(CultureInfo.InvariantCulture,Strings.GetString("core.deserialize.request.method.contract.undefined"),method),id);

            JsonReader segReader;

            if (paramSeg.HasValue && paramSeg.Value.Count > 0)
            {
                if (contract.ParametersType == JsonRpcParametersType.None)
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.request.params.invalid_property"),id);

                segReader = new JsonReader(paramSeg.Value.Array,paramSeg.Value.Offset);
                @params = ReadParams(ref segReader,contract,id,formatterResolver);
            }

            switch (@params)
            {
                case IReadOnlyList<object> l:
                    return new JsonRpcRequest(method,id,l);
                case IReadOnlyDictionary<string,object> d:
                    return new JsonRpcRequest(method,id,d);
                default:
                    return new JsonRpcRequest(method,id);
            }
        }
    }

    internal class Utf8JsonRpcRequestFormatterV1 : IJsonFormatter<JsonRpcRequest>
    {
        public void Serialize(ref JsonWriter writer,JsonRpcRequest req,IJsonFormatterResolver formatterResolver)
        {
            _ = req ?? throw new ArgumentNullException(nameof(req));

            writer.WriteBeginObject();

            writer.WritePropertyName("method");
            writer.WriteString(req.Method);

            if (req.ParametersType == JsonRpcParametersType.ByName)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.serialize.request.params.unsupported_structure"),req.Id);

            writer.WriteValueSeparator();
            WriteRpcParams(ref writer,req);

            writer.WriteValueSeparator();
            WriteRpcId(ref writer,req.Id);

            writer.WriteEndObject();
        }

        public JsonRpcRequest Deserialize(ref JsonReader reader,IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
                return null;

            var count = 0;
            var method = "";
            object @params = null;
            ArraySegment<byte>? paramSeg = null;
            JsonRpcId id = default;
            Exception methodExc = null;

            while (reader.ReadIsInObject(ref count))
            {
                var name = reader.ReadPropertyName();

                switch (name)
                {
                    case "method":
                        try
                        {
                            method = reader.ReadString();
                        }
                        catch (Exception x)
                        {
                            methodExc = x;
                        }
                        break;
                    case "params":
                        paramSeg = reader.ReadNextBlockSegment();
                        break;
                    case "id":
                        try
                        {
                            id = ReadRpcId(ref reader,formatterResolver);
                        }
                        catch (JsonRpcInnerException)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.request.id.invalid_property"));
                        }
                        break;
                    default:
                        reader.ReadNext();
                        break;
                }
            }

            if (methodExc != null)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.request.method.invalid_property"),id);

            var jsonRpcFormatterResolver = formatterResolver as JsonRpcRequestFormatterResolver;

            if (!jsonRpcFormatterResolver.TryGetContract(method,out var contract))
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMethod,string.Format(CultureInfo.InvariantCulture,Strings.GetString("core.deserialize.request.method.unsupported"),method),id);

            if (contract == null)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,string.Format(CultureInfo.InvariantCulture,Strings.GetString("core.deserialize.request.method.contract.undefined"),method),id);

            JsonReader segReader;

            if (paramSeg.HasValue && paramSeg.Value.Count > 0)
            {
                if (contract.ParametersType == JsonRpcParametersType.None)
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.request.params.invalid_property"),id);

                segReader = new JsonReader(paramSeg.Value.Array,paramSeg.Value.Offset);
                @params = ReadParams(ref segReader,contract,id,formatterResolver);
            }

            switch (@params)
            {
                case IReadOnlyList<object> l:
                    return new JsonRpcRequest(method,id,l);
                case IReadOnlyDictionary<string,object> d:
                    return new JsonRpcRequest(method,id,d);
                default:
                    return new JsonRpcRequest(method,id);
            }
        }
    }

    internal static partial class Utf8JsonRpcFormatterHelper
    {
        public static void WriteRpcParams(ref JsonWriter writer,JsonRpcRequest req)
        {
            writer.WritePropertyName("params");

            switch (req.ParametersType)
            {
                case JsonRpcParametersType.None:
                    JsonSerializer.Serialize(ref writer,new object[0]);
                    break;
                case JsonRpcParametersType.ByPosition:
                    if (req.ParametersByPosition.Count == 0)
                        throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("request.params.invalid_count"),req.Id);

                    JsonSerializer.Serialize(ref writer,req.ParametersByPosition);
                    break;
                case JsonRpcParametersType.ByName:
                    JsonSerializer.Serialize(ref writer,req.ParametersByName);
                    break;
            }
        }

        public static void WriteRpcId(ref JsonWriter writer,in JsonRpcId id)
        {
            writer.WritePropertyName("id");

            switch (id.Type)
            {
                case JsonRpcIdType.None:
                    writer.WriteNull();
                    break;
                case JsonRpcIdType.String:
                    writer.WriteString((string)id);
                    break;
                case JsonRpcIdType.Integer:
                    writer.WriteInt64((long)id);
                    break;
                case JsonRpcIdType.Float:
                    writer.WriteDoubleKeepZero((double)id);
                    break;
            }
        }

        public static JsonRpcId ReadRpcId(ref JsonReader reader,IJsonFormatterResolver formatterResolver)
        {
            var token = reader.GetCurrentJsonToken();

            switch (token)
            {
                case JsonToken.String:
                    return new JsonRpcId(reader.ReadString());
                case JsonToken.Number:
                    var segNum = reader.ReadNumberSegment();
                    var l = NumberConverter.ReadInt64(segNum.Array,segNum.Offset,out var readCount);
                    if (readCount == segNum.Count)
                        return new JsonRpcId(l);
                    var d = NumberConverter.ReadDouble(segNum.Array,segNum.Offset,out readCount);
                    return new JsonRpcId(d);
                case JsonToken.Null:
                    reader.ReadIsNull();
                    return default;
                default:
                    throw new JsonRpcInnerException(JsonRpcInnerErrorCode.InvalidIdProperty);
            }
        }

        public static object ReadParams(ref JsonReader reader,JsonRpcRequestContract contract,in JsonRpcId id,IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.request.params.invalid_property"),id);

            if (contract.ParametersType == JsonRpcParametersType.ByPosition)
            {
                if (reader.GetCurrentJsonToken() != JsonToken.BeginArray)
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.request.params.invalid_property"),id);

                IReadOnlyList<object> list;

                try
                {
                    list = ReadParamsByPosition(ref reader,contract.ParametersByPosition,formatterResolver);
                }
                catch (Exception x)
                {
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.request.params.invalid_structure"),id,x);
                }

                if (contract.ParametersByPosition != null && list.Count < contract.ParametersByPosition.Count)
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("request.contract.params.invalid_count"),id);

                return list;
            }

            if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.request.params.invalid_property"),id);

            var dict = ReadParamsByName(ref reader,contract.ParametersByName,formatterResolver);

            if (dict.Count < contract.ParametersByName.Count)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("request.contract.params.invalid_count"),id);

            return dict;
        }

        private static IReadOnlyList<object> ReadParamsByPosition(ref JsonReader reader,IReadOnlyList<Type> paramTypes,IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
                return null;
            
            if (paramTypes == null || paramTypes.Count == 0) return new object[0];

            var count = 0;
            var list = new List<object>();

            while (reader.ReadIsInArray(ref count))
            {                            
                var type = paramTypes[count - 1];

                if (type == null)
                {
                    list.Add(null);
                    reader.ReadNextBlock();
                }
                else
                {
                    var o = JsonSerializer.NonGeneric.Deserialize(type,ref reader,formatterResolver);
                    list.Add(o);
                }

                if (count >= paramTypes.Count)                    
                    break;
            }

            return list;
        }

        private static IReadOnlyDictionary<string,object> ReadParamsByName(ref JsonReader reader,IReadOnlyDictionary<string,Type> paramTypes,IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
                return null;

            if (paramTypes == null || paramTypes.Count == 0) return new Dictionary<string,object>(0);

            var count = 0;
            var dict = new Dictionary<string,object>();

            while (reader.ReadIsInObject(ref count))
            {
                var name = reader.ReadPropertyName();

                if (!paramTypes.TryGetValue(name,out var type))
                {
                    reader.ReadNextBlock();
                    continue;
                }

                if (type == null)
                {
                    dict.Add(name,null);
                    reader.ReadNextBlock();
                }
                else
                {
                    var o = JsonSerializer.NonGeneric.Deserialize(type,ref reader,formatterResolver);
                    dict.Add(name,o);
                }

                if (dict.Count >= paramTypes.Count) break;
            }

            return dict;
        }
    }
}
