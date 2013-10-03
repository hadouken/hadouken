using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc
{
    public sealed class JsonRpcClient : IDisposable
    {
        private readonly Uri _host;
        private readonly HttpClient _httpClient = new HttpClient();
        private int _requestId = 0;

        public JsonRpcClient(Uri host)
        {
            _host = host;
        }

        public async Task<TResult> CallAsync<TResult>(string method, object parameters = null)
        {
            _requestId++;

            var request = new
                {
                    id = _requestId,
                    jsonrpc = "2.0",
                    method = "events.publish",
                    @params = new [] { method, parameters }
                };

            var json = JsonConvert.SerializeObject(request);
            var response = await _httpClient.PostAsync(_host, new StringContent(json, Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();

            var j = JToken.Parse(responseContent);
            return j["result"].Value<TResult>();
        }

        public void Dispose()
        {
        }
    }
}
