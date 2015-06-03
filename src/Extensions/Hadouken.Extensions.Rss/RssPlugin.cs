using System;
using System.Linq;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Logging;
using Hadouken.Common.Timers;
using Hadouken.Extensions.Rss.Data;

namespace Hadouken.Extensions.Rss {
    [Extension("plugin.rss",
        Name = "RSS",
        Description = "Monitors and downloads torrents from RSS feeds.",
        ResourceNamespace = "Hadouken.Extensions.Rss.Resources",
        Scripts = new[] {
            "js/app.js",
            "js/controllers/settingsController.js",
            "js/controllers/addFeedController.js",
            "js/controllers/upsertFilterController.js"
        }
        )]
    public sealed class RssPlugin : IPlugin {
        private readonly IFeedChecker _feedChecker;
        private readonly ILogger<RssPlugin> _logger;
        private readonly IRssRepository _rssRepository;
        private readonly ITimer _timer;

        public RssPlugin(ILogger<RssPlugin> logger, ITimerFactory timerFactory, IRssRepository rssRepository,
            IFeedChecker feedChecker) {
            if (logger == null) {
                throw new ArgumentNullException("logger");
            }
            if (timerFactory == null) {
                throw new ArgumentNullException("timerFactory");
            }
            if (rssRepository == null) {
                throw new ArgumentNullException("rssRepository");
            }
            if (feedChecker == null) {
                throw new ArgumentNullException("feedChecker");
            }

            this._logger = logger;
            this._rssRepository = rssRepository;
            this._timer = timerFactory.Create(60000, this.CheckFeeds);
            this._feedChecker = feedChecker;
        }

        public void Load() {
            this._timer.Start();
        }

        public void Unload() {
            this._timer.Stop();
        }

        internal void CheckFeeds() {
            var ticks = this._timer.Ticks;
            var feeds = this._rssRepository.GetFeeds().ToList();

            if (!feeds.Any()) {
                return;
            }

            foreach (var feed in feeds.Where(feed => (ticks%feed.PollInterval == 0) || ticks == 0)) {
                try {
                    this._logger.Info("Checking feed {Name}.", feed.Name);
                    this._feedChecker.Check(feed);
                }
                catch (Exception e) {
                    this._logger.Error(e, "Error when checking feed {Name}.", feed.Name);
                }
            }
        }
    }
}