using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hadouken.Common.Net
{
    public class HttpClientWrapper : IHttpClient
    {
        public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent httpContent)
        {
            using (var client = new HttpClient())
            {
                return client.PostAsync(uri, httpContent);
            }
        }
    }
}