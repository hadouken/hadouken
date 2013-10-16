using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Hadouken.Framework.Rpc
{
    public sealed class JsonRpcRequest
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();

        static JsonRpcRequest()
        {
            SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            SerializerSettings.Converters.Add(new StringEnumConverter());
            SerializerSettings.Converters.Add(new VersionConverter());
        }

        [JsonProperty("id", Required = Required.Always)]
        public object Id { get; set; }

        [JsonProperty("method", Required = Required.Always)]
        public string Method { get; set; }

        [JsonProperty("jsonrpc", Required = Required.Always)]
        public string Protocol { get; set; }

        [JsonProperty("params", Required = Required.Default)]
        public JToken Parameters { get; set; }

        public static bool TryParse(string json, out JsonRpcRequest request, out Exception exception)
        {
            request = null;
            exception = null;

            try
            {
                request = JsonConvert.DeserializeObject<JsonRpcRequest>(json);
                return true;
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
