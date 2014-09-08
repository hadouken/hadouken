using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hadouken.Common.Net
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent httpContent);

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

        Task<Stream> GetStreamAsync(Uri uri);

        Task<Stream> GetStreamAsync(string url);

        Task<byte[]> GetByteArrayAsync(Uri uri);
    }
}
