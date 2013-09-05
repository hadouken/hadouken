using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hadouken.Framework.Rpc
{
    public abstract class Response : IResponse
    {
        [JsonProperty("id", Required = Required.AllowNull)]
        public int? Id { get; set; }

        [JsonProperty("jsonrpc", Required = Required.Always)]
        public string Protocol {
            get { return "2.0"; }
        }
    }

    public class SuccessResponse : Response
    {
        public SuccessResponse(object result)
        {
            Result = result;
        }

        [JsonProperty("result", Required = Required.Always)]
        public object Result { get; private set; }
    }
}
