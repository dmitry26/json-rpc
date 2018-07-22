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

namespace System.Data.JsonRpc
{
    using static Utf8JsonRpcFormatterHelper;

    internal class Utf8JsonRpcResponseFormatterV2 : IJsonFormatter<JsonRpcResponse>
    {
        public void Serialize(ref JsonWriter writer,JsonRpcResponse rsp,IJsonFormatterResolver formatterResolver)
        {
            _ = rsp ?? throw new ArgumentNullException(nameof(rsp));

            writer.WriteBeginObject();

            writer.WritePropertyName("jsonrpc");
            writer.WriteString("2.0");

            if (rsp.Success)
            {
                writer.WriteValueSeparator();
                writer.WritePropertyName("result");
                JsonSerializer.Serialize(ref writer,rsp.Result);
            }
            else
            {
                writer.WriteValueSeparator();
                WriteError(ref writer,rsp);
            }

            writer.WriteValueSeparator();
            WriteRpcId(ref writer,rsp.Id);

            writer.WriteEndObject();
        }

        public JsonRpcResponse Deserialize(ref JsonReader reader,IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
                return null;

            var ver = "";
            var count = 0;
            JsonRpcId id = default;
            ArraySegment<byte>? resSeg = null;
            object res = null;
            ArraySegment<byte>? errSeg = null;
            JsonRpcError rpcErr = default;

            while (reader.ReadIsInObject(ref count))
            {
                var name = reader.ReadPropertyName();

                switch (name)
                {
                    case "jsonrpc":
                        ver = reader.ReadString();
                        break;
                    case "result":                        
                        resSeg = reader.ReadNextBlockSegment();
                        break;
                    case "error":
                        errSeg = reader.ReadNextBlockSegment();                                            
                        break;
                    case "id":
                        try
                        {
                            id = ReadRpcId(ref reader,formatterResolver);
                        }
                        catch (JsonRpcInnerException)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.response.id.invalid_property"));
                        }
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }

            if (resSeg == null && errSeg == null)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.response.invalid_properties"),id);

            var jsonRpcFormatterResolver = formatterResolver as JsonRpcResponseFormatterResolver;
            var contract = jsonRpcFormatterResolver.GetContract(id);
            JsonReader segReader;

            if (resSeg.HasValue && resSeg.Value.Count > 0)
            {
                segReader = new JsonReader(resSeg.Value.Array,resSeg.Value.Offset);                
                res = (contract?.ResultType != null)
                    ? JsonSerializer.NonGeneric.Deserialize(contract.ResultType,ref segReader,formatterResolver)
                    : throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.deserialize.response.method.contract.undefined"),id);                
            }

            if (errSeg.HasValue && errSeg.Value.Count > 0)
            {
                segReader = new JsonReader(errSeg.Value.Array,errSeg.Value.Offset);
                rpcErr = ReadRpcError(ref segReader,contract?.ErrorDataType ?? jsonRpcFormatterResolver.DefaultErrorDataType,
                    formatterResolver, x =>
                    {
                        switch (x)
                        {
                            case ArgumentOutOfRangeException ex:
                                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.response.error.code.invalid_range"),id,ex);
                            case ArgumentNullException ex:
                                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.response.error.message.invalid_property"),id,ex);
                            default:
                                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.response.error.invalid_type"),id,x);
                        }
                    });
            }            

            if (res == null && rpcErr == null || res != null && rpcErr != null)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.response.invalid_properties"),id);
                
            return (rpcErr == null)
                    ? new JsonRpcResponse(res,id)
                    : new JsonRpcResponse(rpcErr,id);
        }          
    }

    internal class Utf8JsonRpcResponseFormatterV1 : IJsonFormatter<JsonRpcResponse>
    {
        public void Serialize(ref JsonWriter writer,JsonRpcResponse rsp,IJsonFormatterResolver formatterResolver)
        {
            _ = rsp ?? throw new ArgumentNullException(nameof(rsp));

            writer.WriteBeginObject();

            if (rsp.Success)
            {
                writer.WritePropertyName("result");
                JsonSerializer.Serialize(ref writer,rsp.Result);

                writer.WriteValueSeparator();
                writer.WritePropertyName("error");
                writer.WriteNull();
            }
            else
            {
                writer.WritePropertyName("result");
                writer.WriteNull();

                writer.WriteValueSeparator();
                WriteError(ref writer,rsp);
            }

            writer.WriteValueSeparator();
            WriteRpcId(ref writer,rsp.Id);

            writer.WriteEndObject();
        }

        public JsonRpcResponse Deserialize(ref JsonReader reader,IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
                return null;

            var count = 0;
            JsonRpcId id = default;
            ArraySegment<byte>? resSeg = null;
            object res = null;
            ArraySegment<byte>? errSeg = null;
            JsonRpcError rpcErr = default;            

            while (reader.ReadIsInObject(ref count))
            {
                var name = reader.ReadPropertyName();

                switch (name)
                {
                    case "result":
                        resSeg = reader.ReadNextBlockSegment();                        
                        break;
                    case "error":
                        errSeg = reader.ReadNextBlockSegment();
                        break;
                    case "id":
                        try
                        {
                            id = ReadRpcId(ref reader,formatterResolver);
                        }
                        catch (JsonRpcInnerException)
                        {
                            throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.response.id.invalid_property"));
                        }
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }

            if (resSeg == null)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.response.invalid_properties"),id);

            var jsonRpcFormatterResolver = formatterResolver as JsonRpcResponseFormatterResolver;
            var contract = jsonRpcFormatterResolver.GetContract(id);
            JsonReader segReader;

            if (resSeg.HasValue && resSeg.Value.Count > 0)
            {
                segReader = new JsonReader(resSeg.Value.Array,resSeg.Value.Offset);
                res = (contract?.ResultType != null)
                    ? JsonSerializer.NonGeneric.Deserialize(contract.ResultType,ref segReader,formatterResolver)
                    : throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.deserialize.response.method.contract.undefined"),id);
            }

            if (errSeg.HasValue && errSeg.Value.Count > 0)
            {
                segReader = new JsonReader(errSeg.Value.Array,errSeg.Value.Offset);
                rpcErr = ReadRpcError(ref segReader,contract?.ErrorDataType ?? jsonRpcFormatterResolver.DefaultErrorDataType,
                    formatterResolver,x =>
                    {
                        switch (x)
                        {
                            case JsonRpcException exc:
                                throw new JsonRpcException(JsonRpcErrorCodes.InvalidOperation,Strings.GetString("core.deserialize.response.method.contract.undefined"),id,exc);
                            case ArgumentOutOfRangeException exc:
                                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.response.error.code.invalid_range"),id,exc);
                        }
                    });                
            }

            if (res == null && rpcErr == null)
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.deserialize.response.invalid_properties"),id);
           
            return (rpcErr == null)
                    ? new JsonRpcResponse(res,id)
                    : new JsonRpcResponse(rpcErr,id);
        }       
    }

    internal static partial class Utf8JsonRpcFormatterHelper
    {
        public static void WriteError(ref JsonWriter writer,JsonRpcResponse rsp)
        {
            writer.WritePropertyName("error");

            writer.WriteBeginObject();

            writer.WritePropertyName("code");
            writer.WriteInt64(rsp.Error.Code);

            writer.WriteValueSeparator();
            writer.WritePropertyName("message");
            writer.WriteString(rsp.Error.Message);

            if (rsp.Error.HasData)
            {
                writer.WriteValueSeparator();
                writer.WritePropertyName("data");
                JsonSerializer.Serialize(ref writer,rsp.Error.Data);
            }

            writer.WriteEndObject();
        }
       
        public static JsonRpcError ReadRpcError(ref JsonReader reader,Type errDataType,IJsonFormatterResolver formatterResolver,Action<Exception> excHandler)
        {           
            if (reader.ReadIsNull())
                return null;

            long code = 0;
            string msg = null;
            object data = null;
            var count = 0;
            var hasData = false;

            try
            {
                while (reader.ReadIsInObject(ref count))
                {
                    var name = reader.ReadPropertyName();

                    switch (name)
                    {
                        case "code":
                            code = reader.ReadInt64();
                            break;
                        case "message":
                            msg = reader.ReadString();
                            break;
                        case "data":
                            data = (errDataType != null)
                                ? JsonSerializer.NonGeneric.Deserialize(errDataType,ref reader,formatterResolver)
                                : null;
                            if (errDataType == null)                            
                                reader.ReadNextBlock();
                            else
                                hasData = true;
                            break;
                        default:
                            reader.ReadNextBlock();
                            break;
                    }
                }

                return hasData
                    ? new JsonRpcError(code,msg,data)
                    : new JsonRpcError(code,msg);
            }            
            catch (Exception x)
            {
                excHandler(x);
            }

            return new JsonRpcError(default,string.Empty);
        }
    }
}
