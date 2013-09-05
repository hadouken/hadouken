using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc.Http
{
    public class HttpClientTransport : IClientTransport
    {
        private readonly string _requestUri;
        private readonly HttpClient _httpClient = new HttpClient();

        public HttpClientTransport(string requestUri)
        {
            _requestUri = requestUri;
        }

        public async Task<string> Send(string data)
        {
            var response = await _httpClient.PostAsync(_requestUri, new StringContent(data, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return String.Empty;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
