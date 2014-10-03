using System;
using System.Text.RegularExpressions;
using Hadouken.Common;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Logging;
using Hadouken.Common.Timers;
using Hadouken.Extensions.UpdateChecker.Http;
using Hadouken.Extensions.UpdateChecker.Models;

namespace Hadouken.Extensions.UpdateChecker
{
    [Extension("plugin.updatechecker",
        Name = "UpdateChecker",
        Description = "Periodically checks for updates.",
        ResourceNamespace = "Hadouken.Extensions.UpdateChecker.Resources",
        Scripts = new[]
        {
            "js/app.js"
        }
    )]
    public class UpdateCheckerPlugin : IPlugin
    {
        private readonly ILogger<UpdateCheckerPlugin> _logger;
        private readonly IGitHubReleasesClient _gitHubReleasesClient;
        private readonly IKeyValueStore _keyValueStore;
        private readonly ITimer _updateTimer;
        private readonly Version _currentVersion;

        public UpdateCheckerPlugin(ILogger<UpdateCheckerPlugin> logger,
            ITimerFactory timerFactory,
            IGitHubReleasesClient gitHubReleasesClient,
            IKeyValueStore keyValueStore)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (timerFactory == null) throw new ArgumentNullException("timerFactory");
            if (gitHubReleasesClient == null) throw new ArgumentNullException("gitHubReleasesClient");
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");

            _logger = logger;
            _updateTimer = timerFactory.Create(1000 * 60, CheckForUpdates);
            _gitHubReleasesClient = gitHubReleasesClient;
            _keyValueStore = keyValueStore;
            _currentVersion = GetType().Assembly.GetName().Version;
        }

        public void Load()
        {
            _updateTimer.Start();
        }

        public void Unload()
        {
            _updateTimer.Stop();
        }

        private void CheckForUpdates()
        {
            var ticks = _updateTimer.Ticks;
            var interval = _keyValueStore.Get("updatechecker.interval", 180); // Default is to check every third hour

            if (ticks != 0 || ticks%interval != 0) return;

            try
            {
                _logger.Debug("Checking for update");

                var releases = _gitHubReleasesClient.ListReleases();
                var latestRelease = releases.MaxBy(r => r.Id);
                var latestVersion = ParseVersion(latestRelease.TagName);
                var latestStoredRelease = _keyValueStore.Get<Release>("updatechecker.release");

                if (latestStoredRelease != null
                    && latestRelease.Id == latestStoredRelease.Id)
                {
                    return;
                }

                if (latestVersion > _currentVersion)
                {
                    _logger.Info("New version of Hadouken is available.");

                    _keyValueStore.Set("updatechecker.release", latestRelease);
                    _keyValueStore.Set("updatechecker.muted", false);
                }
                else
                {
                    if (latestStoredRelease != null)
                    {
                        _keyValueStore.Set("updatechecker.release", null);
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Error when checking for updates.");
            }
        }

        private static Version ParseVersion(string tagName)
        {
            var match = Regex.Match(tagName, "v(?<major>[\\d+])\\.(?<minor>[\\d+])\\.(?<patch>[\\d+])");
            return Version.Parse(string.Format("{0}.{1}.{2}",
                match.Groups["major"].Value,
                match.Groups["minor"].Value,
                match.Groups["patch"].Value));
        }
    }
}
