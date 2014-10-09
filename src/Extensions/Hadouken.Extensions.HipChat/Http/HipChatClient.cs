using System;
using System.Collections.Generic;
using System.Net.Http;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Logging;
using Hadouken.Common.Net;
using Hadouken.Extensions.HipChat.Config;

namespace Hadouken.Extensions.HipChat.Http
{
    [Component]
    public sealed class HipChatClient : IHipChatClient
    {
        private static readonly Uri ApiUri = new Uri("https://api.hipchat.com/v1/rooms/message");
        private readonly ILogger<HipChatClient> _logger;
        private readonly IHttpClient _httpClient;

        public HipChatClient(ILogger<HipChatClient> logger,
            IHttpClient httpClient)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (httpClient == null) throw new ArgumentNullException("httpClient");
            _logger = logger;
            _httpClient = httpClient;
        }

        public void SendMessage(HipChatConfig config, string message)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (message == null) throw new ArgumentNullException("message");

            var data = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("room_id", config.RoomId),
                new KeyValuePair<string, string>("from", config.From),
                new KeyValuePair<string, string>("message", message),
                new KeyValuePair<string, string>("notify", config.Notify ? "1" : "0")
            };

            var uri = new Uri(ApiUri, "?auth_token=" + config.AuthenticationToken);
            var response = _httpClient.PostAsync(uri, new FormUrlEncodedContent(data)).Result;

            if (!response.IsSuccessStatusCode)
            {
                _logger.Error("Error sending message. Response status code: {StatusCode}.", response.StatusCode);
            }
        }
    }
}