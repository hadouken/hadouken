using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hadouken.Framework.Rpc
{
    public class JsonRpcSuccessResponse : JsonRpcResponse
    {
        [JsonProperty("result", Required = Required.AllowNull)]
        public JToken Result { get; set; }
    }
}
