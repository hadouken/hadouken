using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hadouken.Common.Net
{
    public class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient _client = new HttpClient();

        public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent httpContent)
        {
            return _client.PostAsync(uri, httpContent);            
        }
    }
}