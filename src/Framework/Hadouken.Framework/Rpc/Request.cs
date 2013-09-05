using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hadouken.Framework.Rpc
{
    public class Request : IRequest
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("method", Required = Required.Always)]
        public string Method { get; set; }

        [JsonProperty("jsonrpc", Required = Required.Always)]
        public string Protocol { get; set; }

        public string ParameterAsJson { get; set; }

        public object GetParameterObject(Type type)
        {
            if (String.IsNullOrEmpty(ParameterAsJson))
                return null;

            return JsonConvert.DeserializeObject(ParameterAsJson, type);
        }
    }
}
