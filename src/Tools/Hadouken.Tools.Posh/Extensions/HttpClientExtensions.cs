using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Hadouken.Tools.Posh.Extensions {
    public static class HttpClientExtensions {
        public static Task<HttpResponseMessage> PostAsJsonAsync(this HttpClient httpClient, Uri uri, object data) {
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);

            var message = new HttpRequestMessage(HttpMethod.Post, uri) {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            return httpClient.SendAsync(message);
        }
    }
}