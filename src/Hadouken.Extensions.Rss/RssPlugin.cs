using System;
using System.Linq;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Logging;
using Hadouken.Common.Timers;
using Hadouken.Extensions.Rss.Config;

namespace Hadouken.Extensions.Rss
{
    [Extension("plugin.rss",
        Name = "RSS",
        Description = "Monitors and downloads torrents from RSS feeds.",
        ResourceNamespace = "Hadouken.Extensions.Rss.Resources",
        Scripts = new[] { "js/app.js", "js/controllers/settingsController.js" }
    )]
    public sealed class RssPlugin : IPlugin
    {
        private readonly ILogger _logger;
        private readonly ITimer _timer;
        private readonly IKeyValueStore _keyValueStore;
        private readonly IFeedService _feedService;

        public RssPlugin(ILogger logger, ITimerFactory timerFactory, IKeyValueStore keyValueStore, IFeedService feedService)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (timerFactory == null) throw new ArgumentNullException("timerFactory");
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            if (feedService == null) throw new ArgumentNullException("feedService");

            _logger = logger;
            _timer = timerFactory.Create(60000, CheckFeeds);
            _keyValueStore = keyValueStore;
            _feedService = feedService;
        }

        public void Load()
        {
            _timer.Start();
        }

        public void Unload()
        {
            _timer.Stop();
        }

        internal void CheckFeeds()
        {
            var ticks = _timer.Ticks;
            var feeds = _keyValueStore.Get<Feed[]>("rss.feeds");

            if (feeds == null || !feeds.Any()) return;

            foreach (var feed in feeds.Where(feed => (ticks%feed.PollInterval == 0) || ticks == 0))
            {
                try
                {
                    _feedService.Check(feed);
                }
                catch (Exception e)
                {
                    _logger.Error("Error when checking feed " + feed.Name, e);
                }
            }
        }
    }
}
