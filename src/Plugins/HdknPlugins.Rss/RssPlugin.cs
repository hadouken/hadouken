using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Common.Data;
using Hadouken.Common.Messaging;
using Hadouken.Common.Plugins;
using Migrator.Providers.SQLite;
using NLog;
using HdknPlugins.Rss.Timers;
using HdknPlugins.Rss.Data.Models;
using System.Xml;
using System.Text.RegularExpressions;
using HdknPlugins.Rss.Http;
using Hadouken.Common.BitTorrent;
using Hadouken.Common;

namespace HdknPlugins.Rss
{
    public class RssPlugin : Plugin
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IEnvironment _environment;
        private readonly IDataRepository _dataRepository;
        private readonly ITimer _timer;
        private readonly IWebClient _webClient;

        private int _ticks = -1;

        public RssPlugin(IMessageBus messageBus,
                         IEnvironment environment,
                         IDataRepository dataRepository,
                         ITimerFactory timerFactory,
                         IWebClient webClient) : base(messageBus)
        {
            _environment = environment;
            _dataRepository = dataRepository;
            _timer = timerFactory.CreateTimer();
            _webClient = webClient;
        }

        public override void Load()
        {
            var m =
                new Migrator.Migrator(
                    new SQLiteTransformationProvider(new SQLiteDialect(), _environment.ConnectionString),
                    this.GetType().Assembly, false);

            Logger.Debug("Updating all migrations in current assembly");

            m.MigrateToLastVersion();

            _timer.SetCallback(1000, CheckFeeds);
            _timer.Start();
        }

        internal void CheckFeeds()
        {
            _ticks++;
            var feeds = _dataRepository.List<Feed>(f => f.PollInterval % (_ticks * 60) == 0);

            if (feeds == null)
                return;

            foreach (var feed in feeds)
            {
                CheckFeed(feed);
            }
        }

        internal void CheckFeed(Feed feed)
        {
            using (var reader = XmlReader.Create(feed.Url))
            {
                var f = SyndicationFeed.Load(reader);

                if (f == null)
                {
                    Logger.Error("Could not load feed from {0}", feed.Url);
                    return;
                }

                foreach (var item in f.Items.Where(item => item.PublishDate > feed.LastUpdateTime))
                {
                    foreach (var filter in feed.Filters)
                    {
                        if (IsMatchingFilter(filter, item) && item.Links.Any())
                        {
                            Task.Factory.StartNew(() => DownloadItem(item, filter));
                            break;
                        }
                    }
                }
            }
        }

        internal bool IsMatchingFilter(Filter filter, SyndicationItem item)
        {
            if (!String.IsNullOrEmpty(filter.ExcludeFilter))
            {
                return Regex.IsMatch(item.Title.Text, filter.IncludeFilter) &&
                       !Regex.IsMatch(item.Title.Text, filter.ExcludeFilter);
            }

            return Regex.IsMatch(item.Title.Text, filter.IncludeFilter);
        }

        internal void DownloadItem(SyndicationItem item, Filter filter)
        {
            var link = item.Links.FirstOrDefault();

            if (link == null)
                return;

            var data = _webClient.DownloadData(link.Uri);

            if (data == null || data.Length <= 0)
                return;

            MessageBus.Publish(new AddTorrentMessage
                {
                    AutoStart = filter.AutoStart,
                    Data = data,
                    Label = filter.Label
                });

            // Update feed with the last updated time of the downloaded item.
            // This will make sure we only check items newer than the last one we downloaded.

            filter.Feed.LastUpdateTime = item.LastUpdatedTime.DateTime;
            _dataRepository.Update(filter.Feed);
        }

        public override void Unload()
        {
            _timer.Stop();
        }
    }
}
