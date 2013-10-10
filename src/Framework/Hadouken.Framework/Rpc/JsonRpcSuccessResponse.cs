using Newtonsoft.Json;

namespace Hadouken.Framework.Rpc
{
    public class JsonRpcSuccessResponse : JsonRpcResponse
    {
        [JsonProperty("result", Required = Required.AllowNull)]
        public object Result { get; set; }
    }
}
