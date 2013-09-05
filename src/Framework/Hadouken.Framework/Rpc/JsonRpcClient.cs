using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hadouken.Framework.Rpc
{
    public class JsonRpcClient : IJsonRpcClient
    {
        private readonly IClientTransport _clientTransport;

        public JsonRpcClient(IClientTransport clientTransport)
        {
            _clientTransport = clientTransport;
        }

        public async Task<TResult> Call<TResult>(string method, object parameters)
        {
            var json = BuildRequest(method, parameters);
            var result = await _clientTransport.Send(json);

            if (String.IsNullOrEmpty(result))
            {
                return default(TResult);
            }

            var jsonObject = JObject.Parse(result);

            if (jsonObject["result"] != null)
            {
                return jsonObject["result"].ToObject<TResult>();
            }

            return default(TResult);
        }

        public void Dispose()
        {
            _clientTransport.Dispose();
        }

        private static string BuildRequest(string method, object parameters)
        {
            var rpcObject = new
            {
                jsonrpc = "2.0",
                method,
                @params = parameters
            };

            return JsonConvert.SerializeObject(rpcObject);
        }
    }
}
