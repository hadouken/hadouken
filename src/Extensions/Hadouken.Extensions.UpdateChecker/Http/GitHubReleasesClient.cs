using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Net;
using Hadouken.Common.Text;
using Hadouken.Extensions.UpdateChecker.Models;

namespace Hadouken.Extensions.UpdateChecker.Http
{
    [Component]
    public class GitHubReleasesClient : IGitHubReleasesClient
    {
        private static readonly string DefaultUrl = "https://api.github.com/repos/hadouken/hadouken/releases";

        private readonly IHttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IKeyValueStore _keyValueStore;
        private readonly Version _currentVersion;

        public GitHubReleasesClient(IHttpClient httpClient,
            IJsonSerializer jsonSerializer,
            IKeyValueStore keyValueStore)
        {
            if (httpClient == null) throw new ArgumentNullException("httpClient");
            if (jsonSerializer == null) throw new ArgumentNullException("jsonSerializer");
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");

            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
            _keyValueStore = keyValueStore;
            _currentVersion = GetType().Assembly.GetName().Version;
        }

        public IEnumerable<Release> ListReleases()
        {
            var url = _keyValueStore.Get("updatechecker.url", DefaultUrl);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Hadouken", _currentVersion.ToString()));

            var response = _httpClient.SendAsync(request).Result;
            var json = response.Content.ReadAsStringAsync().Result;

            return _jsonSerializer.DeserializeObject<Release[]>(json);
        }
    }
}