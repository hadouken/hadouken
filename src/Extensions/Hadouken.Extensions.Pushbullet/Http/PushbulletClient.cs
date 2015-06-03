using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Logging;
using Hadouken.Common.Net;

namespace Hadouken.Extensions.Pushbullet.Http {
    [Component(ComponentLifestyle.Singleton)]
    public class PushbulletClient : IPushbulletClient {
        private static readonly Uri ApiUri = new Uri("https://api.pushbullet.com/v2/");
        private readonly IHttpClient _httpClient;
        private readonly ILogger<PushbulletClient> _logger;

        public PushbulletClient(ILogger<PushbulletClient> logger,
            IHttpClient httpClient) {
            if (logger == null) {
                throw new ArgumentNullException("logger");
            }
            if (httpClient == null) {
                throw new ArgumentNullException("httpClient");
            }

            this._logger = logger;
            this._httpClient = httpClient;
        }

        public void Send(string accessToken, Note note) {
            var data = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("title", note.Title),
                new KeyValuePair<string, string>("body", note.Body),
                new KeyValuePair<string, string>("type", "note")
            };

            var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(accessToken));
            var uri = new Uri(ApiUri, "pushes");

            var request = new HttpRequestMessage(HttpMethod.Post, uri) {
                Content = new FormUrlEncodedContent(data)
            };

            request.Headers.Add("Authorization", "Basic " + encodedToken);

            var response = this._httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode) {
                this._logger.Error("Error pushing message. Response status code: {StatusCode}.", response.StatusCode);
            }
        }
    }
}