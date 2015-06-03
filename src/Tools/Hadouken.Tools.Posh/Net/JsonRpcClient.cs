using System;
using Hadouken.Tools.Posh.Extensions;

namespace Hadouken.Tools.Posh.Net {
    public sealed class JsonRpcClient : IJsonRpcClient {
        private readonly IHttpClient _httpClient;
        private string _accessToken;

        public JsonRpcClient(IHttpClient httpClient) {
            if (httpClient == null) {
                throw new ArgumentNullException("httpClient");
            }
            this._httpClient = httpClient;
        }

        public string AccessToken {
            get { return this._accessToken; }
            set {
                this._accessToken = value;
                this._httpClient.SetAccessToken(value);
            }
        }

        public Uri Url { get; set; }

        public void Call(string methodName, params object[] parameters) {
            this.Call<object>(methodName, parameters);
        }

        public T Call<T>(string methodName, params object[] parameters) {
            var request = new {
                id = 1,
                jsonrpc = "2.0",
                method = methodName,
                @params = parameters
            };

            var result = this._httpClient.PostAsJsonAsync(new Uri(this.Url, "jsonrpc"), request).Result;
            return result.Content.ReadAsJsonRpcAsync<T>().Result;
        }
    }
}