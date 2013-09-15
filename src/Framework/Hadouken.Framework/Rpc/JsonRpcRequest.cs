using System;
using Newtonsoft.Json;

namespace Hadouken.Framework.Rpc
{
    public sealed class JsonRpcRequest
    {
        [JsonProperty("id", Required = Required.Always)]
        public object Id { get; set; }

        [JsonProperty("method", Required = Required.Always)]
        public string Method { get; set; }

        [JsonProperty("jsonrpc", Required = Required.Always)]
        public string Protocol { get; set; }

        [JsonProperty("params", Required = Required.Default)]
        public object Parameters { get; set; }

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
    }
}
