using System;
using System.IO;
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

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return _client.SendAsync(request);
        }

        public Task<Stream> GetStreamAsync(string url)
        {
            return _client.GetStreamAsync(url);
        }

        public Task<Stream> GetStreamAsync(Uri uri)
        {
            return _client.GetStreamAsync(uri);
        }

        public Task<byte[]> GetByteArrayAsync(Uri uri)
        {
            return _client.GetByteArrayAsync(uri);
        }
    }
}