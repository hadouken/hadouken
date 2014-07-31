using System;
using System.Linq;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Logging;
using Hadouken.Common.Net;
using Hadouken.Extensions.Rss.Config;
using Hadouken.Extensions.Rss.Http;

namespace Hadouken.Extensions.Rss
{
    [Component]
    public sealed class FeedService : IFeedService
    {
        private readonly ILogger _logger;
        private readonly ISyndicationFeedService _syndicationFeedService;
        private readonly IFilterMatcher _filterMatcher;
        private readonly IHttpClient _httpClient;

        public FeedService(ILogger logger,
            ISyndicationFeedService syndicationFeedService,
            IFilterMatcher filterMatcher,
            IHttpClient httpClient)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (syndicationFeedService == null) throw new ArgumentNullException("syndicationFeedService");
            if (filterMatcher == null) throw new ArgumentNullException("filterMatcher");
            if (httpClient == null) throw new ArgumentNullException("httpClient");

            _logger = logger;
            _syndicationFeedService = syndicationFeedService;
            _filterMatcher = filterMatcher;
            _httpClient = httpClient;
        }

        public void Check(Feed feed)
        {
            if (feed == null) throw new ArgumentNullException("feed");
            if (feed.Filters == null || !feed.Filters.Any()) return;

            var syndicationFeed = _syndicationFeedService.GetFeed(feed.Uri);
            var items = syndicationFeed.Items.Where(item => item.PublishDate.ToUniversalTime() > feed.LastUpdatedTime).ToList();

            foreach (var filter in feed.Filters)
            {
                var f = filter;

                foreach (var item in items.Where(item => _filterMatcher.IsMatch(item.Title.Text, f)))
                {
                    // Download
                }
            }

            feed.LastUpdatedTime = DateTimeOffset.UtcNow;
        }
    }
}