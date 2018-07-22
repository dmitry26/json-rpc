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
using System.Data.JsonRpc.Resources;
using System.Globalization;
using System.Text;
using Utf8Json;

namespace System.Data.JsonRpc
{
    internal class Utf8JsonRpcMsgListFormatter<T> : IJsonFormatter<IReadOnlyList<JsonRpcItem<T>>>
        where T : JsonRpcMessage
    {
        public void Serialize(ref JsonWriter writer,IReadOnlyList<JsonRpcItem<T>> items,IJsonFormatterResolver formatterResolver)
        {
            throw new NotSupportedException();
        }

        public IReadOnlyList<JsonRpcItem<T>> Deserialize(ref JsonReader reader,IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
                return null;

            JsonRpcItem<T> item;

            if (reader.GetCurrentJsonToken() == JsonToken.BeginArray)
            {
                var list = new List<JsonRpcItem<T>>();
                var count = 0;

                while (reader.ReadIsInArray(ref count))
                {
                    try
                    {
                        item = new JsonRpcItem<T>(JsonSerializer.Deserialize<T>(ref reader,formatterResolver));
                    }
                    catch (JsonRpcException x)
                    {                       
                        item = new JsonRpcItem<T>(x);
                    }
                    catch (JsonParsingException x)
                    {
                        throw new JsonRpcException(JsonRpcErrorCodes.InvalidJson,Strings.GetString("core.deserialize.json_issue"),default,x);
                    }
                    catch (JsonRpcInnerException x)
                    {
                        item = new JsonRpcItem<T>(
                            new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,string.Format(CultureInfo.InvariantCulture,Strings.GetString("core.batch.invalid_item"),count - 1),default,x.InnerException));
                    }
                    catch (Exception x)
                    {
                        item = new JsonRpcItem<T>(
                            new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,string.Format(CultureInfo.InvariantCulture,Strings.GetString("core.batch.invalid_item"),count - 1),default,x));
                    }

                    list.Add(item);
                }

                if (list.Count == 0)
                    throw new JsonRpcException(JsonRpcErrorCodes.InvalidMessage,Strings.GetString("core.batch.empty"));

                return list;
            }

            try
            {
                item = new JsonRpcItem<T>(JsonSerializer.Deserialize<T>(ref reader,formatterResolver));
            }
            catch (JsonRpcException x)
            {
                item = new JsonRpcItem<T>(x);
            }
            catch (Exception x)
            {
                throw new JsonRpcException(JsonRpcErrorCodes.InvalidJson,Strings.GetString("core.deserialize.json_issue"),default,x);
            }

            return new JsonRpcItem<T>[] { item };
        }
    }
}
