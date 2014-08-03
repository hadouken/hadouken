using System;
using System.ServiceModel.Syndication;
using System.Xml;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Net;

namespace Hadouken.Extensions.Rss.Http
{
    [Component]
    public sealed class SyndicationFeedService : ISyndicationFeedService
    {
        private readonly IHttpClient _httpClient;

        public SyndicationFeedService(IHttpClient httpClient)
        {
            if (httpClient == null) throw new ArgumentNullException("httpClient");
            _httpClient = httpClient;
        }

        public SyndicationFeed GetFeed(string url)
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse
            };

            using (var stream = _httpClient.GetStreamAsync(new Uri(url)).Result)
            using (var reader = XmlReader.Create(stream, settings))
            {
                return SyndicationFeed.Load(reader);
            }
        }
    }
}