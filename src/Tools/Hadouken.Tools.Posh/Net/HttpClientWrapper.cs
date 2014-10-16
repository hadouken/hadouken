using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Hadouken.Tools.Posh.Net
{
    public sealed class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient _client;
        private readonly JavaScriptSerializer _serializer;

        public HttpClientWrapper()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("User-Agent", "Hadouken/Posh 1.0");

            _serializer = new JavaScriptSerializer();
        }

        public void SetAccessToken(string accessToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", accessToken);
        }

        public Task<HttpResponseMessage> PostAsJsonAsync(Uri uri, object data)
        {
            var json = _serializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return _client.PostAsync(uri, content);
        }
    }
}