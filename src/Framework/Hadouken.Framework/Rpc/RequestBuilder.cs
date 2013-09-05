using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hadouken.Framework.Rpc
{
    public class RequestBuilder : IRequestBuilder
    {
        public IRequest Build(string json)
        {
            var requestObject = JObject.Parse(json);
            var request = JsonConvert.DeserializeObject<Request>(json);

            if (requestObject["params"] != null)
                request.ParameterAsJson = JsonConvert.SerializeObject(requestObject.Property("params").Value);

            return request;
        }
    }
}
