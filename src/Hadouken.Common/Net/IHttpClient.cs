using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hadouken.Common.Net
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent httpContent);

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}
