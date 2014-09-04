using System;
using System.Linq;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Logging;
using Hadouken.Common.Timers;
using Hadouken.Extensions.Rss.Data;

namespace Hadouken.Extensions.Rss
{
    [Extension("plugin.rss",
        Name = "RSS",
        Description = "Monitors and downloads torrents from RSS feeds.",
        ResourceNamespace = "Hadouken.Extensions.Rss.Resources",
        Scripts = new[]
        {
            "js/app.js",
            "js/controllers/settingsController.js",
            "js/controllers/addFeedController.js",
            "js/controllers/upsertFilterController.js"
        }
    )]
    public sealed class RssPlugin : IPlugin
    {
        private readonly ILogger _logger;
        private readonly IRssRepository _rssRepository;
        private readonly ITimer _timer;
        private readonly IFeedChecker _feedChecker;

        public RssPlugin(ILogger logger, ITimerFactory timerFactory, IRssRepository rssRepository, IFeedChecker feedChecker)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (timerFactory == null) throw new ArgumentNullException("timerFactory");
            if (rssRepository == null) throw new ArgumentNullException("rssRepository");
            if (feedChecker == null) throw new ArgumentNullException("feedChecker");

            _logger = logger;
            _rssRepository = rssRepository;
            _timer = timerFactory.Create(60000, CheckFeeds);
            _feedChecker = feedChecker;
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
            var feeds = _rssRepository.GetFeeds().ToList();

            if (!feeds.Any()) return;

            foreach (var feed in feeds.Where(feed => (ticks%feed.PollInterval == 0) || ticks == 0))
            {
                try
                {
                    _feedChecker.Check(feed);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error when checking feed {Name}.", feed.Name);
                }
            }
        }
    }
}
