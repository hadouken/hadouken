using System;
using System.Collections.Generic;
using System.Net.Http;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Logging;
using Hadouken.Common.Net;

namespace Hadouken.Extensions.Pushalot.Http {
    [Component]
    public class PushalotClient : IPushalotClient {
        private static readonly Uri ApiUri = new Uri("https://pushalot.com/api/sendmessage");
        private readonly IHttpClient _httpClient;
        private readonly ILogger<PushalotClient> _logger;

        public PushalotClient(ILogger<PushalotClient> logger, IHttpClient httpClient) {
            if (logger == null) {
                throw new ArgumentNullException("logger");
            }
            if (httpClient == null) {
                throw new ArgumentNullException("httpClient");
            }
            this._logger = logger;
            this._httpClient = httpClient;
        }

        public void Send(Message message) {
            if (message == null) {
                throw new ArgumentNullException("message");
            }

            var data = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("AuthorizationToken", message.AuthorizationToken),
                new KeyValuePair<string, string>("Body", message.Body),
                new KeyValuePair<string, string>("Title", message.Title),
                new KeyValuePair<string, string>("Source", message.Source)
            };

            var response = this._httpClient.PostAsync(ApiUri, new FormUrlEncodedContent(data)).Result;

            if (!response.IsSuccessStatusCode) {
                this._logger.Error("Error pushing message. Response status code: {StatusCode}.", response.StatusCode);
            }
        }
    }
}