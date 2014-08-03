using System;
using System.Linq;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Logging;
using Hadouken.Common.Net;
using Hadouken.Extensions.Rss.Data;
using Hadouken.Extensions.Rss.Data.Models;
using Hadouken.Extensions.Rss.Http;

namespace Hadouken.Extensions.Rss
{
    [Component]
    public sealed class FeedService : IFeedService
    {
        private readonly ILogger _logger;
        private readonly IRssRepository _rssRepository;
        private readonly ISyndicationFeedService _syndicationFeedService;
        private readonly IFilterMatcher _filterMatcher;
        private readonly IHttpClient _httpClient;

        public FeedService(ILogger logger,
            IRssRepository rssRepository,
            ISyndicationFeedService syndicationFeedService,
            IFilterMatcher filterMatcher,
            IHttpClient httpClient)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (rssRepository == null) throw new ArgumentNullException("rssRepository");
            if (syndicationFeedService == null) throw new ArgumentNullException("syndicationFeedService");
            if (filterMatcher == null) throw new ArgumentNullException("filterMatcher");
            if (httpClient == null) throw new ArgumentNullException("httpClient");

            _logger = logger;
            _rssRepository = rssRepository;
            _syndicationFeedService = syndicationFeedService;
            _filterMatcher = filterMatcher;
            _httpClient = httpClient;
        }

        public void Check(Feed feed)
        {
            if (feed == null) throw new ArgumentNullException("feed");

            var filters = _rssRepository.GetFiltersByFeedId(feed.Id).ToList();
            if (!filters.Any()) return;

            var syndicationFeed = _syndicationFeedService.GetFeed(feed.Url);
            var items = syndicationFeed.Items.Where(item => item.PublishDate.ToUniversalTime() > feed.LastUpdatedTime).ToList();

            foreach (var filter in filters)
            {
                var f = filter;

                foreach (var item in items.Where(item => _filterMatcher.IsMatch(item.Title.Text, f)))
                {
                    // Download
                }
            }

            feed.LastUpdatedTime = DateTime.UtcNow;
        }
    }
}