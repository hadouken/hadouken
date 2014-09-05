using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Hadouken.Tools.Posh.Extensions
{
    public static class HttpContentExtensions
    {
        private class JsonRpcResponse<T>
        {
            public T result { get; set; }
        }

        public static async Task<T> ReadAsJsonRpcAsync<T>(this HttpContent httpContent)
        {
            var json = await httpContent.ReadAsStringAsync();
            var serializer = new JavaScriptSerializer();

            return serializer.Deserialize<JsonRpcResponse<T>>(json).result;
        }

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent httpContent)
        {
            var json = await httpContent.ReadAsStringAsync();
            var serializer = new JavaScriptSerializer();

            return serializer.Deserialize<T>(json);
        }

        public static async Task<object> ReadAsJsonAsync(this HttpContent httpContent)
        {
            var json = await httpContent.ReadAsStringAsync();
            var serializer = new JavaScriptSerializer();

            return serializer.DeserializeObject(json);
        }
    }
}
