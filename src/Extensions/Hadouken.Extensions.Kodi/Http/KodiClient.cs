using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Logging;
using Hadouken.Common.Net;
using Hadouken.Common.Text;
using Hadouken.Extensions.Kodi.Config;

namespace Hadouken.Extensions.Kodi.Http {
    [Component]
    public sealed class KodiClient : IKodiClient {
        private const string MethodName = "GUI.ShowNotification";
        private readonly IHttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IKeyValueStore _keyValueStore;
        private readonly ILogger<KodiClient> _logger;

        public KodiClient(ILogger<KodiClient> logger,
            IKeyValueStore keyValueStore,
            IHttpClient httpClient,
            IJsonSerializer jsonSerializer) {
            if (logger == null) {
                throw new ArgumentNullException("logger");
            }
            if (keyValueStore == null) {
                throw new ArgumentNullException("keyValueStore");
            }
            if (httpClient == null) {
                throw new ArgumentNullException("httpClient");
            }
            if (jsonSerializer == null) {
                throw new ArgumentNullException("jsonSerializer");
            }
            this._logger = logger;
            this._keyValueStore = keyValueStore;
            this._httpClient = httpClient;
            this._jsonSerializer = jsonSerializer;
        }

        public void ShowNotification(string title, string message) {
            var config = this._keyValueStore.Get<KodiConfig>("kodi.config");
            if (config == null) {
                return;
            }

            var requestContent = new {
                id = 1,
                jsonrpc = "2.0",
                method = MethodName,
                @params = new[] {title, message}
            };

            var content = new StringContent(this._jsonSerializer.SerializeObject(requestContent));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, config.Url) {
                Content = content
            };

            if (config.EnableAuthentication) {
                var authValue = string.Concat(config.UserName, ":", config.Password);
                request.Headers.Add("Authorization",
                    "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authValue)));
            }

            var response = this._httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode) {
                this._logger.Error("Error sending notification. Response status code: {StatusCode}.",
                    response.StatusCode);
            }
        }
    }
}