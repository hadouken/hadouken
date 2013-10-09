using Newtonsoft.Json;

namespace Hadouken.Framework.Rpc
{
    public class JsonRpcSuccessResponse : JsonRpcResponse
    {
        [JsonProperty("result", Required = Required.Always)]
        public object Result { get; set; }
    }
}
