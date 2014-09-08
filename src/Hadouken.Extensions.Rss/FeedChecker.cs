using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Logging;
using Hadouken.Common.Messaging;
using Hadouken.Common.Net;
using Hadouken.Extensions.Rss.Data;
using Hadouken.Extensions.Rss.Data.Models;
using Hadouken.Extensions.Rss.Http;

namespace Hadouken.Extensions.Rss
{
    [Component]
    public sealed class FeedChecker : IFeedChecker
    {
        private readonly ILogger<FeedChecker> _logger;
        private readonly IRssRepository _rssRepository;
        private readonly ISyndicationFeedService _syndicationFeedService;
        private readonly IFilterMatcher _filterMatcher;
        private readonly IHttpClient _httpClient;
        private readonly IMessageBus _messageBus;

        public FeedChecker(ILogger<FeedChecker> logger,
            IRssRepository rssRepository,
            ISyndicationFeedService syndicationFeedService,
            IFilterMatcher filterMatcher,
            IHttpClient httpClient,
            IMessageBus messageBus)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (rssRepository == null) throw new ArgumentNullException("rssRepository");
            if (syndicationFeedService == null) throw new ArgumentNullException("syndicationFeedService");
            if (filterMatcher == null) throw new ArgumentNullException("filterMatcher");
            if (httpClient == null) throw new ArgumentNullException("httpClient");
            if (messageBus == null) throw new ArgumentNullException("messageBus");

            _logger = logger;
            _rssRepository = rssRepository;
            _syndicationFeedService = syndicationFeedService;
            _filterMatcher = filterMatcher;
            _httpClient = httpClient;
            _messageBus = messageBus;
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
                    Download(filter, item);
                }
            }

            _rssRepository.UpdateFeedLastUpdatedTime(feed.Id, DateTime.UtcNow);
        }

        private void Download(Filter filter, SyndicationItem item)
        {
            var args = new TorrentArguments();
            var pattern = filter.IncludePattern;
            var input = item.Title.Text;
            var modifiers = _rssRepository.GetModifiersByFilterId(filter.Id);

            if (modifiers != null)
            {
                foreach (var modifier in modifiers)
                {
                    var regex = new Regex(pattern, RegexOptions.ExplicitCapture);
                    var match = regex.Match(input);
                    var value = modifier.Value;

                    foreach (var groupName in regex.GetGroupNames().Skip(1))
                    {
                        var group = match.Groups[groupName];
                        value = value.Replace(string.Format("${{{0}}}", groupName), group.Value);
                    }

                    switch (modifier.Target)
                    {
                        case ModifierTarget.Label:
                            args.Label = value;
                            break;

                        case ModifierTarget.SavePath:
                            args.SavePath = value;
                            break;
                    }
                }
            }

            var data = _httpClient.GetByteArrayAsync(item.Links.First().Uri).Result;
            _messageBus.Publish(new AddTorrentMessage(data) {Label = args.Label, SavePath = args.SavePath});
        }
    }
}