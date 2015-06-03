using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Net;
using Hadouken.Common.Text;
using Hadouken.Extensions.UpdateChecker.Models;

namespace Hadouken.Extensions.UpdateChecker.Http {
    [Component]
    public class GitHubReleasesClient : IGitHubReleasesClient {
        private const string DefaultUrl = "https://api.github.com/repos/hadouken/hadouken/releases";
        private readonly Version _currentVersion;
        private readonly IHttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IKeyValueStore _keyValueStore;

        public GitHubReleasesClient(IHttpClient httpClient,
            IJsonSerializer jsonSerializer,
            IKeyValueStore keyValueStore) {
            if (httpClient == null) {
                throw new ArgumentNullException("httpClient");
            }
            if (jsonSerializer == null) {
                throw new ArgumentNullException("jsonSerializer");
            }
            if (keyValueStore == null) {
                throw new ArgumentNullException("keyValueStore");
            }

            this._httpClient = httpClient;
            this._jsonSerializer = jsonSerializer;
            this._keyValueStore = keyValueStore;
            this._currentVersion = this.GetType().Assembly.GetName().Version;
        }

        public IEnumerable<Release> ListReleases() {
            var url = this._keyValueStore.Get("updatechecker.url", DefaultUrl);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Hadouken", this._currentVersion.ToString()));

            var response = this._httpClient.SendAsync(request).Result;
            var json = response.Content.ReadAsStringAsync().Result;

            return this._jsonSerializer.DeserializeObject<Release[]>(json);
        }
    }
}