using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hadouken.Tools.Posh.Net
{
    public interface IHttpClient
    {
        void SetAccessToken(string accessToken);

        Task<HttpResponseMessage> PostAsJsonAsync(Uri uri, object data);
    }
}
