using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace Hadouken.Framework.Rpc
{
    public abstract class JsonRpcResponse
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();

        static JsonRpcResponse()
        {
            SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            SerializerSettings.Converters.Add(new SemanticVersionConverter());
            SerializerSettings.Converters.Add(new StringEnumConverter());
            SerializerSettings.Converters.Add(new VersionConverter());
        }

        protected JsonRpcResponse()
        {
            Protocol = "2.0";
        }

        [JsonProperty("id", Required = Required.Always)]
        public object Id { get; set; }

        [JsonProperty("jsonrpc", Required = Required.Always)]
        public string Protocol { get; set; }

        public static bool TryParse(string json, out JsonRpcResponse response, out Exception exception)
        {
            response = null;
            exception = null;

            try
            {
                var obj = JToken.Parse(json) as IDictionary<string, JToken>;

                if (obj == null)
                    return false;

                if (obj.ContainsKey("result"))
                {
                    response = JsonConvert.DeserializeObject<JsonRpcSuccessResponse>(json);
                    return true;
                }
                
                if (obj.ContainsKey("error"))
                {
                    response = JsonConvert.DeserializeObject<JsonRpcErrorResponse>(json);
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, SerializerSettings);
        }
    }
}
